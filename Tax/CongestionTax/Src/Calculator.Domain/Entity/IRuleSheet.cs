namespace Calculator.Domain.Entity
{
    public interface IRuleSheet
    {
        ICity City { get; set; }
        int Year { get; set; }
        CurrencyUnitEnum CurrencyUnit { get; set; }
        IEnumerable<ITollRateInterval> TollRateIntervals { get; set; }
        IEnumerable<ITaxFreePeriod>? TaxFreePeriods { get; set; }
        IEnumerable<VehicleTypeEnum>? TaxFreeVehicleTypes { get; set; }
        IHolidayTaxFreePeriod? HolidayTaxFreePeriod { get; set; }
        public int? MaxTollFeePerDay { get; set; }
        public int? SingleChargeDurationPerMinute { get; set; }
        bool IsWeekendTollFreeRuleApplied { get; set; }
        bool IsHolidayTollFreeRuleApplied { get; set; }
    }
}
