namespace Calculator.Infrastructure.Database
{
    using Calculator.Domain.Entity;
    using Calculator.Domain.Repository;
    using Calculator.Infrastructure.Database.Context;
    using Microsoft.EntityFrameworkCore;
    using PublicHoliday;
    using System;
    using System.Linq;

    public class DataBaseRepository(AppDbContext appDbContext, SwedenPublicHoliday swedenPublicHoliday) : IRepository
    {
        private readonly AppDbContext appDbContext = appDbContext;
        private readonly SwedenPublicHoliday swedenPublicHoliday = swedenPublicHoliday;

        public async Task<IDictionary<int, IRuleSheet>> GetRuleSheetAsync(ICity city, IEnumerable<int> years, CancellationToken cancellationToken)
        {
            var cityYearBaseQuerable = appDbContext.City
                .AsNoTracking()
                .Where(x => x.Name == city.Name)
                .Join(appDbContext.CityYearCurrency, c => c.Id, cyc => cyc.CityId, (c, cyc) => new { c, cyc })
                .Where(x => years.Contains(x.cyc.Year))
                .AsSplitQuery();

            var tollRateIntervalCombinedData = await cityYearBaseQuerable
                .Join(appDbContext.CityYearCurrencyTollRateInterval, s => s.cyc.Id, tri => tri.CityYearCurrencyId, (s, tri) => new { cycId = s.cyc.Id, tri })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var tollRateByYear = tollRateIntervalCombinedData
                .GroupBy(x => x.cycId)
                .ToDictionary(k => k.Key, v => v.Select(i => i.tri));

            var taxFreeVehicleCombinedData = await cityYearBaseQuerable
                .Join(appDbContext.CityYearCurrencyTaxFreeVehicleType, s => s.cyc.Id, tfvt => tfvt.CityYearCurrencyId, (s, tfvt) => new { cycId = s.cyc.Id, tfvt.VehicleType!.Type })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var taxFreeVehicleByYear = taxFreeVehicleCombinedData?
                .GroupBy(x => x.cycId)
                .ToDictionary(k => k.Key, v => v.Select(i => i.Type));

            var taxFreeDatePeriodCombinedData = await cityYearBaseQuerable
               .Join(appDbContext.CityYearCurrencyTaxFreeDatePeriod, s => s.cyc.Id, tfdp => tfdp.CityYearCurrencyId, (s, tfdp) => new { cycId = s.cyc.Id, tfdp })
               .ToListAsync(cancellationToken)
               .ConfigureAwait(false);

            var taxFreeDatePeriodByYear = taxFreeDatePeriodCombinedData?
                .GroupBy(x => x.cycId)
                .ToDictionary(k => k.Key, v => v.Select(i => i.tfdp));

            return await cityYearBaseQuerable
            .Join(appDbContext.CityYearCurrencyRuleSheet, s => s.cyc.Id, cycr => cycr.CityYearCurrencyId, (s, cycr) => new { s.c, s.cyc, cycr })

            .GroupJoin(appDbContext.HolidayTaxFreePeriod, s => s.cycr.Id, htfp => htfp.CityYearCurrencyRuleSheetId, (s, htfp) => new { s.c, s.cyc, s.cycr, htfp })
            .SelectMany(g => g.htfp.DefaultIfEmpty(), (s, htfp) => new { s.c, s.cyc, s.cycr, htfp })

            .Select(x => new RuleSheet()
            {
                City = x.c,
                Year = x.cyc.Year,
                CurrencyUnit = x.cyc.CurrencyUnit,
                TollRateIntervals = tollRateByYear[x.cyc.Id],
                PublicHolidays = swedenPublicHoliday.PublicHolidays(x.cyc.Year),
                TaxFreePeriods = taxFreeDatePeriodByYear != null ? taxFreeDatePeriodByYear[x.cyc.Id] : null,
                TaxFreeVehicleTypes = taxFreeVehicleByYear != null ? taxFreeVehicleByYear[x.cyc.Id] : null,
                HolidayTaxFreePeriod = x.htfp,
                MaxTollFeePerDay = x.cycr.MaxTollFeePerDay,
                SingleChargeDurationPerMinute = x.cycr.SingleChargeDurationPerMinute,
                IsWeekendTollFreeRuleApplied = x.cycr.IsWeekendTollFreeRuleApplied,
                IsHolidayTollFreeRuleApplied = x.cycr.IsHolidayTollFreeRuleApplied,
            })
            .ToDictionaryAsync(key => key.Year, value => value as IRuleSheet, cancellationToken)
            .ConfigureAwait(false);
        }
    }
}
