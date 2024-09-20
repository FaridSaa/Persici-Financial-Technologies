namespace Calculator.Infrastructure.Database.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table(nameof(CityYearCurrencyRuleSheet))]
    public class CityYearCurrencyRuleSheet
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "bit")] bool IsWeekendTollFreeRuleApplied { get; set; }
        [Column(TypeName = "bit")] bool IsHolidayTollFreeRuleApplied { get; set; }
        public int? MaxTollFeePerDay { get; set; }
        public int? SingleChargeDurationPerMinute { get; set; }
        public int CityYearCurrencyUnitId { get; set; }
        [ForeignKey(nameof(CityYearCurrencyUnitId))] public virtual CityYearCurrencyUnit? CityYearCurrencyUnit { get; set; }
    }
}
