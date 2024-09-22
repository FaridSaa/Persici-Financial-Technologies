namespace Calculator.Infrastructure.Database
{
    using Calculator.Domain.Entity;
    using Calculator.Domain.Entity.Interface;
    using Calculator.Domain.Repository;
    using Calculator.Infrastructure.Database.Context;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;

    public class DataBaseRepository(AppDbContext appDbContext, IHolidayRepository holidayRepository) : IRepository
    {
        private readonly AppDbContext appDbContext = appDbContext;
        private readonly IHolidayRepository holidayRepository = holidayRepository;

        public async Task<IDictionary<int, IRuleSheet>> GetRuleSheetAsync(ICity city, IEnumerable<int> years, CancellationToken cancellationToken)
        {
            //perhaps we can reduce query requests , maybe later

            var cityYearBaseQuerable = appDbContext.City
                .AsNoTracking()
                .Where(x => x.Name == city.Name)
                .Join(appDbContext.CityYearCurrency, c => c.Id, cyc => cyc.CityId, (c, cyc) => new { c, cyc })
                .Where(x => years.Contains(x.cyc.Year))
                .AsSplitQuery();

            var taxRateIntervalCombinedData = await cityYearBaseQuerable
                .Join(appDbContext.CycTaxRateInterval, s => s.cyc.Id, tri => tri.CityYearCurrencyId, (s, tri) => new { cycId = s.cyc.Id, tri })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var taxFreeVehicleCombinedData = await cityYearBaseQuerable
                .Join(appDbContext.CycTaxFreeVehicleType, s => s.cyc.Id, tfvt => tfvt.CityYearCurrencyId, (s, tfvt) => new { cycId = s.cyc.Id, tfvt.VehicleType!.Type })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var taxFreeDatePeriodCombinedData = await cityYearBaseQuerable
                .Join(appDbContext.CycTaxFreeDatePeriod, s => s.cyc.Id, tfdp => tfdp.CityYearCurrencyId, (s, tfdp) => new { cycId = s.cyc.Id, tfdp })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var taxRateByYear = taxRateIntervalCombinedData
                .GroupBy(x => x.cycId)
                .ToDictionary(k => k.Key, v => v.Select(i => i.tri)
                .OrderBy(o => o.From).AsEnumerable());

            var taxFreeVehicleByYear = taxFreeVehicleCombinedData?
                .GroupBy(x => x.cycId)
                .ToDictionary(k => k.Key, v => v.Select(i => i.Type));

            var taxFreeDatePeriodByYear = taxFreeDatePeriodCombinedData?
                .GroupBy(x => x.cycId)
                .ToDictionary(k => k.Key, v => v.Select(i => i.tfdp));

            return await cityYearBaseQuerable
            .Join(appDbContext.CycRuleSheet, s => s.cyc.Id, cycr => cycr.CityYearCurrencyId, (s, cycr) => new { s.c, s.cyc, cycr })
            .GroupJoin(appDbContext.HolidayTaxFreePeriod, s => s.cycr.Id, htfp => htfp.CityYearCurrencyRuleSheetId, (s, htfp) => new { s.c, s.cyc, s.cycr, htfp })
            .SelectMany(g => g.htfp.DefaultIfEmpty(), (s, htfp) => new { s.c, s.cyc, s.cycr, htfp })
            .Select(x => new RuleSheet()
            {
                City = x.c,
                Year = x.cyc.Year,
                CurrencyUnit = x.cyc.CurrencyUnit,
                TaxRateIntervals = taxRateByYear[x.cyc.Id],
                PublicHolidays = holidayRepository.GetPublicHolidays(x.c, x.cyc.Year),
                TaxFreePeriods = taxFreeDatePeriodByYear != null ? taxFreeDatePeriodByYear[x.cyc.Id] : null,
                TaxFreeVehicleTypes = taxFreeVehicleByYear != null ? taxFreeVehicleByYear[x.cyc.Id] : null,
                HolidayTaxFreePeriod = x.htfp,
                MaxTaxFeePerDay = x.cycr.MaxTaxFeePerDay,
                SingleChargeDurationPerMinute = x.cycr.SingleChargeDurationPerMinute,
                IsWeekendTaxFreeRuleApplied = x.cycr.IsWeekendTaxFreeRuleApplied,
                IsHolidayTaxFreeRuleApplied = x.cycr.IsHolidayTaxFreeRuleApplied,
            })
            .ToDictionaryAsync(key => key.Year, value => value as IRuleSheet, cancellationToken)
            .ConfigureAwait(false);
        }
    }
}
