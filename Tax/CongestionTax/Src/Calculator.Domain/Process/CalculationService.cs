namespace Calculator.Domain.Process
{
    using Calculator.Domain.Entity;
    using Calculator.Domain.Repository;
    using ErrorOr;
    using System.Globalization;

    public class CalculationService(IRepository repository) : ICalculationService
    {
        private readonly IRepository repository = repository;
        public async Task<ErrorOr<IEnumerable<DateTollFee>>> CalculateAsync(ICity city, IVehicle vehicle, IEnumerable<DateTime> dateTimes, CancellationToken cancellationToken)
        {
            if (city is null)
            {
                return Error.Validation("Undefined city");
            }
            if (vehicle is null)
            {
                return Error.Validation("Undefined vehicle");
            }
            if (dateTimes is null || dateTimes.Count() is 0)
            {
                return Error.Validation("Undefined datetimes");
            }

            var years = dateTimes
                .Select(s => s.Date.Year)
                .Distinct();

            var ruleSheetByYear = await repository
                .GetRuleSheetAsync(city, years, cancellationToken)
                .ConfigureAwait(false);

            var dateTimesPerDay = dateTimes
                .GroupBy(x => x.Date)
                .Select(x => new { x.Key.Date, Time = x.Select(i => i.TimeOfDay) });

            var tollFees = new List<DateTollFee>();
            foreach (var dayIntervals in dateTimesPerDay)
            {
                if (!ruleSheetByYear.TryGetValue(dayIntervals.Date.Year, out var ruleSheet))
                {
                    continue;
                }
                if (ruleSheet.IsWeekendTollFreeRuleApplied)
                {
                    if (dayIntervals.Date is { DayOfWeek: DayOfWeek.Saturday or DayOfWeek.Sunday })
                    {
                        tollFees.Add(new DateTollFee() { Date = dayIntervals.Date, Fee = 0, Unit = ruleSheet.CurrencyUnit });
                        continue;
                    }
                }
                if (ruleSheet.TaxFreePeriods is not null)
                {
                    var isTaxFreeDate = ruleSheet.TaxFreePeriods.Any(x => x.From <= dayIntervals.Date && dayIntervals.Date <= x.To);
                    if (isTaxFreeDate)
                    {
                        tollFees.Add(new DateTollFee() { Date = dayIntervals.Date, Fee = 0, Unit = ruleSheet.CurrencyUnit });
                        continue;
                    }
                }
                if (ruleSheet.IsHolidayTollFreeRuleApplied)
                {
                    if (ruleSheet.PublicHolidays.Contains(dayIntervals.Date))
                    {
                        tollFees.Add(new DateTollFee() { Date = dayIntervals.Date, Fee = 0, Unit = ruleSheet.CurrencyUnit });
                        continue;
                    }
                    if (ruleSheet.HolidayTaxFreePeriod is not null)
                    {
                        var isDateInAfterHolidayPeriod = ruleSheet.PublicHolidays
                            .Any(x => x.Date <= dayIntervals.Date && dayIntervals.Date <= x.Date.AddDays(ruleSheet.HolidayTaxFreePeriod.DayAfter));

                        var isDateInBeforeHolidayPeriod = ruleSheet.PublicHolidays
                            .Any(x => x.Date >= dayIntervals.Date && dayIntervals.Date >= x.Date.AddDays(ruleSheet.HolidayTaxFreePeriod.DayBefore * -1));

                        if (isDateInAfterHolidayPeriod || isDateInBeforeHolidayPeriod)
                        {
                            tollFees.Add(new DateTollFee() { Date = dayIntervals.Date, Fee = 0, Unit = ruleSheet.CurrencyUnit });
                            continue;
                        }
                    }
                }
            }
            return tollFees;
        }
    }
}
