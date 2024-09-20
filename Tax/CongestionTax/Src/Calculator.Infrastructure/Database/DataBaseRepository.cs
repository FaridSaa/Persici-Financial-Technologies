namespace Calculator.Infrastructure.Database
{
    using Calculator.Domain.Entity;
    using Calculator.Domain.Repository;
    using Calculator.Infrastructure.Database.Context;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;

    public class DataBaseRepository(AppDbContext appDbContext) : IRepository
    {
        private readonly AppDbContext appDbContext = appDbContext;
        public async Task<IDictionary<int, IRuleSheet>> GetRuleSheetAsync(ICity city, IEnumerable<int> years, CancellationToken cancellationToken)
        {
            return await appDbContext.City
                .AsNoTracking()

                .Where(x => x.Name == city.Name)

                .Join(appDbContext.CityYearCurrency, c => c.Id, cyc => cyc.CityId, (c, cyc) => new { c, cyc })

                .Where(x => years.Contains(x.cyc.Year))

                .Join(appDbContext.CityYearCurrencyRuleSheet, s => s.cyc.Id, cycr => cycr.CityYearCurrencyId, (s, cycr) => new { s.c, s.cyc, cycr })

                .Join(appDbContext.CityYearCurrencyTollRateInterval, s => s.cyc.Id, tri => tri.CityYearCurrencyId, (s, tri) => new { s.c, s.cyc, s.cycr, tri })

                .GroupJoin(appDbContext.CityYearCurrencyTaxFreeVehicleType, s => s.cyc.Id, tfvt => tfvt.CityYearCurrencyId, (s, tfvt) => new { s.c, s.cyc, s.cycr, s.tri, tfvt })
                .SelectMany(g => g.tfvt.DefaultIfEmpty(), (s, tfvt) => new { s.c, s.cyc, s.cycr, s.tri, tfvt })

                .GroupJoin(appDbContext.CityYearCurrencyTaxFreeDatePeriod, s => s.cyc.Id, tfdp => tfdp.CityYearCurrencyId, (s, tfdp) => new { s.c, s.cyc, s.cycr, s.tri, s.tfvt, tfdp })
                .SelectMany(g => g.tfdp.DefaultIfEmpty(), (s, tfdp) => new { s.c, s.cyc, s.cycr, s.tri, s.tfvt, tfdp })

                .GroupJoin(appDbContext.HolidayTaxFreePeriod, s => s.cycr.Id, htfp => htfp.CityYearCurrencyRuleSheetId, (s, htfp) => new { s.c, s.cyc, s.cycr, s.tri, s.tfvt, s.tfdp, htfp })
                .SelectMany(g => g.htfp.DefaultIfEmpty(), (s, htfp) => new { s.c, s.cyc, s.cycr, s.tri, s.tfvt, s.tfdp, htfp })

                .GroupBy(x => x.cyc.Id)
                .Select(x => new
                {
                    SingleObjects = x.Select(s => new { City = s.c, CityYearCurrency = s.cyc, RuleSheet = s.cycr, HolidayTaxFreePeriod = s.htfp }).First(),
                    TollRateIntervals = x.Select(s => s.tri),
                    TaxFreeVehicleTypes = x.Where(s => s.tfvt != null).Select(s => s.tfvt!.VehicleType!.Type),
                    TaxFreePeriods = x.Where(s => s.tfdp != null).Select(s => s.tfdp!),
                })
                .Select(x => new RuleSheet()
                {
                    City = x.SingleObjects.City,
                    Year = x.SingleObjects.CityYearCurrency.Year,
                    CurrencyUnit = x.SingleObjects.CityYearCurrency.CurrencyUnit,
                    TollRateIntervals = x.TollRateIntervals,
                    TaxFreePeriods = x.TaxFreePeriods,
                    TaxFreeVehicleTypes = x.TaxFreeVehicleTypes,
                    HolidayTaxFreePeriod = x.SingleObjects.HolidayTaxFreePeriod,
                    MaxTollFeePerDay = x.SingleObjects.RuleSheet.MaxTollFeePerDay,
                    SingleChargeDurationPerMinute = x.SingleObjects.RuleSheet.SingleChargeDurationPerMinute,
                    IsWeekendTollFreeRuleApplied = x.SingleObjects.RuleSheet.IsWeekendTollFreeRuleApplied,
                    IsHolidayTollFreeRuleApplied = x.SingleObjects.RuleSheet.IsHolidayTollFreeRuleApplied
                })
                .AsSplitQuery()
                .ToDictionaryAsync(key => key.Year, value => value as IRuleSheet, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
