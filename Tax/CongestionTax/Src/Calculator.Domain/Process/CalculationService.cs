namespace Calculator.Domain.Process
{
    using Calculator.Domain.Entity;
    using Calculator.Domain.Repository;

    public class CalculationService(IRepository repository) : ICalculationService
    {
        private readonly IRepository repository = repository;
        public async Task<IEnumerable<DateTax>> CalculateAsync(ICity city, IVehicle vehicle, IEnumerable<DateTime> dateTimes, CancellationToken cancellationToken)
        {
            var years = dateTimes
                .Select(s => s.Date.Year)
                .Distinct();

            var ruleSheetByYear = await repository
                .GetRuleSheetAsync(city, years, cancellationToken)
                .ConfigureAwait(false);

            var dateTimesPerDay = dateTimes
                .GroupBy(x => x.Date)
                .Select(x => new
                {
                    x.Key.Date,
                    Time = x
                    .Where(i => i.TimeOfDay >= ruleSheetByYear[x.Key.Year].TollRateIntervals.Min(m => m.From))
                    .Where(i => i.TimeOfDay <= ruleSheetByYear[x.Key.Year].TollRateIntervals.Max(m => m.To))
                    .Select(i => i.TimeOfDay)
                    .Distinct()
                    .OrderBy(o => o)
                    .ToList()

                }).OrderBy(o => o.Date);

            var tollFees = new List<DateTax>();
            foreach (var datePeriods in dateTimesPerDay)
            {
                if (!ruleSheetByYear.TryGetValue(datePeriods.Date.Year, out var ruleSheet))
                {
                    continue;
                }

                if (ruleSheet.TaxFreeVehicleTypes != null && ruleSheet.TaxFreeVehicleTypes.Contains(vehicle.GetVehicleType()))
                {
                    tollFees.Add(new DateTax() { Date = datePeriods.Date, Fee = 0, Unit = ruleSheet.CurrencyUnit });
                    continue;
                }

                if (ruleSheet.IsWeekendTollFreeRuleApplied)
                {
                    if (datePeriods.Date is { DayOfWeek: DayOfWeek.Saturday or DayOfWeek.Sunday })
                    {
                        tollFees.Add(new DateTax() { Date = datePeriods.Date, Fee = 0, Unit = ruleSheet.CurrencyUnit });
                        continue;
                    }
                }

                if (ruleSheet.TaxFreePeriods is not null)
                {
                    var isTaxFreeDate = ruleSheet.TaxFreePeriods.Any(x => x.From <= datePeriods.Date && datePeriods.Date <= x.To);
                    if (isTaxFreeDate)
                    {
                        tollFees.Add(new DateTax() { Date = datePeriods.Date, Fee = 0, Unit = ruleSheet.CurrencyUnit });
                        continue;
                    }
                }

                if (ruleSheet.IsHolidayTollFreeRuleApplied)
                {
                    if (ruleSheet.PublicHolidays.Contains(datePeriods.Date))
                    {
                        tollFees.Add(new DateTax() { Date = datePeriods.Date, Fee = 0, Unit = ruleSheet.CurrencyUnit });
                        continue;
                    }
                    if (ruleSheet.HolidayTaxFreePeriod is not null)
                    {
                        var isDateInAfterHolidayPeriod = ruleSheet.PublicHolidays
                            .Any(x => x.Date <= datePeriods.Date && datePeriods.Date <= x.Date.AddDays(ruleSheet.HolidayTaxFreePeriod.DayAfter));

                        var isDateInBeforeHolidayPeriod = ruleSheet.PublicHolidays
                            .Any(x => x.Date >= datePeriods.Date && datePeriods.Date >= x.Date.AddDays(ruleSheet.HolidayTaxFreePeriod.DayBefore * -1));

                        if (isDateInAfterHolidayPeriod || isDateInBeforeHolidayPeriod)
                        {
                            tollFees.Add(new DateTax() { Date = datePeriods.Date, Fee = 0, Unit = ruleSheet.CurrencyUnit });
                            continue;
                        }
                    }
                }

                var dayFee = 0;
                if (ruleSheet.SingleChargeDurationPerMinute is not null)
                {
                    var filteredTimeSpans = new List<TimeSpan>();
                    var sortedTimeSpans = datePeriods.Time;
                    for (int i = 0; i < sortedTimeSpans.Count;)
                    {
                        var sequence = new List<TimeSpan>();
                        for (int j = i + 1; j < sortedTimeSpans.Count; j++)
                        {
                            if (Math.Abs((sortedTimeSpans[j] - sortedTimeSpans[i]).TotalMinutes) <= ruleSheet.SingleChargeDurationPerMinute)
                            {
                                filteredTimeSpans.Add(sortedTimeSpans[i]);
                                sequence.Add(sortedTimeSpans[i]);
                                filteredTimeSpans.Add(sortedTimeSpans[j]);
                                sequence.Add(sortedTimeSpans[j]);
                            }
                            else
                            {
                                i = j;
                                if (sequence.Count >= 2)
                                {
                                    dayFee += ruleSheet.TollRateIntervals.Max(s => s.Fee);
                                }
                                break;
                            }
                        }
                    }
                    filteredTimeSpans.ForEach(i => datePeriods.Time.Remove(i));
                }

                foreach (var each in datePeriods.Time)
                {
                    var rate = ruleSheet.TollRateIntervals.First(x => each >= x.From && each <= x.To);
                    dayFee += rate.Fee;
                }

                if (ruleSheet.MaxTollFeePerDay is not null && dayFee > ruleSheet.MaxTollFeePerDay)
                {
                    dayFee = ruleSheet.MaxTollFeePerDay.Value;
                }
                tollFees.Add(new DateTax() { Date = datePeriods.Date, Fee = dayFee, Unit = ruleSheet.CurrencyUnit });
            }
            return tollFees;
        }
    }
}
