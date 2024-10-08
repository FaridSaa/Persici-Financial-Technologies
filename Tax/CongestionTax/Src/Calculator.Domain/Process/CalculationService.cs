﻿namespace Calculator.Domain.Process
{
    using Calculator.Domain.Entity;
    using Calculator.Domain.Entity.Interface;
    using Calculator.Domain.Repository;

    public class CalculationService(IRepository repository) : ICalculationService
    {
        private readonly IRepository repository = repository;

#warning Use fluent validation on input data and implement result pattern for ouput
#warning Some refactor needed for clean code
        public async Task<IEnumerable<DateTax>> CalculateAsync(ICity city, IVehicle vehicle, IEnumerable<DateTime> dateTimes, CancellationToken cancellationToken)
        {
            var years = dateTimes.Select(s => s.Date.Year).Distinct();
            var ruleSheetByYear = await repository.GetRuleSheetAsync(city, years, cancellationToken).ConfigureAwait(false);

            var dateTimesPerDay = dateTimes
                .GroupBy(x => x.Date)
                .Select(x => new
                {
                    x.Key.Date,
                    Time = x.Select(i => i.TimeOfDay).Distinct().OrderBy(o => o).ToList()

                }).OrderBy(o => o.Date);

            var tollFees = new List<DateTax>();
            foreach (var dateTimePerDay in dateTimesPerDay)
            {
                if (!ruleSheetByYear.TryGetValue(dateTimePerDay.Date.Year, out var ruleSheet))
                {
                    continue;
                }

                if (ruleSheet.TaxFreeVehicleTypes != null && ruleSheet.TaxFreeVehicleTypes.Contains(vehicle.GetVehicleType()))
                {
                    tollFees.Add(new() { Date = dateTimePerDay.Date, Fee = 0, Unit = ruleSheet.CurrencyUnit });
                    continue;
                }

                if (ruleSheet.IsWeekendTaxFreeRuleApplied)
                {
                    if (dateTimePerDay.Date is { DayOfWeek: DayOfWeek.Saturday or DayOfWeek.Sunday })
                    {
                        tollFees.Add(new() { Date = dateTimePerDay.Date, Fee = 0, Unit = ruleSheet.CurrencyUnit });
                        continue;
                    }
                }

                if (ruleSheet.TaxFreePeriods is not null)
                {
                    var isTaxFreeDate = ruleSheet.TaxFreePeriods.Any(x => x.From <= dateTimePerDay.Date && dateTimePerDay.Date <= x.To);
                    if (isTaxFreeDate)
                    {
                        tollFees.Add(new() { Date = dateTimePerDay.Date, Fee = 0, Unit = ruleSheet.CurrencyUnit });
                        continue;
                    }
                }

                if (ruleSheet.IsHolidayTaxFreeRuleApplied)
                {
                    if (ruleSheet.PublicHolidays.Contains(dateTimePerDay.Date))
                    {
                        tollFees.Add(new() { Date = dateTimePerDay.Date, Fee = 0, Unit = ruleSheet.CurrencyUnit });
                        continue;
                    }
                    if (ruleSheet.HolidayTaxFreePeriod is not null)
                    {
                        var isDateInBeforeHolidayPeriod = ruleSheet.PublicHolidays.Any(x => dateTimePerDay.Date.AddDays(ruleSheet.HolidayTaxFreePeriod.DayAfter) == x.Date);
                        var isDateInAfterHolidayPeriod = ruleSheet.PublicHolidays.Any(x => dateTimePerDay.Date.AddDays(ruleSheet.HolidayTaxFreePeriod.DayBefore * -1) == x.Date);
                        if (isDateInAfterHolidayPeriod || isDateInBeforeHolidayPeriod)
                        {
                            tollFees.Add(new() { Date = dateTimePerDay.Date, Fee = 0, Unit = ruleSheet.CurrencyUnit });
                            continue;
                        }
                    }
                }

                var dayFee = 0;
                if (ruleSheet.SingleChargeDurationPerMinute is not null)
                {
                    var filteredTimeSpans = new List<TimeSpan>();
                    var sortedTimeSpans = dateTimePerDay.Time;
                    for (int i = 0; i < sortedTimeSpans.Count; i++)
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
                                    dayFee += ruleSheet.TaxRateIntervals.Max(s => s.Fee);
                                }
                                break;
                            }
                        }
                    }
                    filteredTimeSpans.ForEach(i => dateTimePerDay.Time.Remove(i));
                }

                foreach (var each in dateTimePerDay.Time)
                {
                    var rate = ruleSheet.TaxRateIntervals.FirstOrDefault(x => each >= x.From && each <= x.To)
                        ?? ruleSheet.TaxRateIntervals
                        .Where(x => (x.To - x.From).TotalMinutes < 0)
                        .Where(x => each >= x.From).LastOrDefault();

                    if (rate is not null)
                    {
                        dayFee += rate.Fee;
                    }
                }

                if (ruleSheet.MaxTaxFeePerDay is not null && dayFee > ruleSheet.MaxTaxFeePerDay)
                {
                    dayFee = ruleSheet.MaxTaxFeePerDay.Value;
                }
                tollFees.Add(new() { Date = dateTimePerDay.Date, Fee = dayFee, Unit = ruleSheet.CurrencyUnit });
            }
            return tollFees;
        }
    }
}
