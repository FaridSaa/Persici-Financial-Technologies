using Calculator.Domain.Entity.Enum;

namespace Calculator.Domain.Entity.Interface
{
    public interface IRuleSheet
    {
        ICity City { get; set; }
        int Year { get; set; }
        CurrencyUnitEnum CurrencyUnit { get; set; }
        IEnumerable<ITaxRateInterval> TaxRateIntervals { get; set; }
        IEnumerable<DateTime> PublicHolidays { get; set; }
        IEnumerable<ITaxFreePeriod>? TaxFreePeriods { get; set; }
        IEnumerable<VehicleTypeEnum>? TaxFreeVehicleTypes { get; set; }
        IHolidayTaxFreePeriod? HolidayTaxFreePeriod { get; set; }
        public int? MaxTaxFeePerDay { get; set; }
        public int? SingleChargeDurationPerMinute { get; set; }
        bool IsWeekendTaxFreeRuleApplied { get; set; }
        bool IsHolidayTaxFreeRuleApplied { get; set; }
    }
}
