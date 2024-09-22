namespace Calculator.Infrastructure.Database.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table(nameof(CycRuleSheet))]
    public class CycRuleSheet
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "bit")] public bool IsWeekendTollFreeRuleApplied { get; set; }
        [Column(TypeName = "bit")] public bool IsHolidayTollFreeRuleApplied { get; set; }
        public int? MaxTollFeePerDay { get; set; }
        public int? SingleChargeDurationPerMinute { get; set; }
        public int CityYearCurrencyId { get; set; }
        [ForeignKey(nameof(CityYearCurrencyId))] public virtual CityYearCurrency? CityYearCurrency { get; set; }
    }
}
