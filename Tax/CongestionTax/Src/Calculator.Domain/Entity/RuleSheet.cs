namespace Calculator.Domain.Entity
{
    using System.Collections.Generic;

    public class RuleSheet : IRuleSheet
    {
        public required ICity City { get; set; }
        public required int Year { get; set; }
        public required CurrencyUnitEnum CurrencyUnit { get; set; }
        public required IEnumerable<ITollRateInterval> TollRateIntervals { get; set; }
        public required IEnumerable<DateTime> PublicHolidays { get; set; }
        public IEnumerable<ITaxFreePeriod>? TaxFreePeriods { get; set; }
        public IEnumerable<VehicleTypeEnum>? TaxFreeVehicleTypes { get; set; }
        public IHolidayTaxFreePeriod? HolidayTaxFreePeriod { get; set; }
        public int? MaxTollFeePerDay { get; set; }
        public int? SingleChargeDurationPerMinute { get; set; }
        public bool IsWeekendTollFreeRuleApplied { get; set; }
        public bool IsHolidayTollFreeRuleApplied { get; set; }
    }
}
