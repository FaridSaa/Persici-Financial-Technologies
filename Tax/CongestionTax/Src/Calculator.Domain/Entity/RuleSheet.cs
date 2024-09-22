namespace Calculator.Domain.Entity
{
    using System.Collections.Generic;
    using Calculator.Domain.Entity.Enum;
    using Calculator.Domain.Entity.Interface;

    public class RuleSheet : IRuleSheet
    {
        public required ICity City { get; set; }
        public required int Year { get; set; }
        public required CurrencyUnitEnum CurrencyUnit { get; set; }
        public required IEnumerable<ITaxRateInterval> TaxRateIntervals { get; set; }
        public required IEnumerable<DateTime> PublicHolidays { get; set; }
        public IEnumerable<ITaxFreePeriod>? TaxFreePeriods { get; set; }
        public IEnumerable<VehicleTypeEnum>? TaxFreeVehicleTypes { get; set; }
        public IHolidayTaxFreePeriod? HolidayTaxFreePeriod { get; set; }
        public int? MaxTaxFeePerDay { get; set; }
        public int? SingleChargeDurationPerMinute { get; set; }
        public bool IsWeekendTaxFreeRuleApplied { get; set; }
        public bool IsHolidayTaxFreeRuleApplied { get; set; }
    }
}
