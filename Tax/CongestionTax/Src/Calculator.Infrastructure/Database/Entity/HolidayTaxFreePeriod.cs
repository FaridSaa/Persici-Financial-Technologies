using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calculator.Infrastructure.Database.Entity
{
    [Table(nameof(HolidayTaxFreePeriod))]
    public class HolidayTaxFreePeriod
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int DayBefore { get; set; }
        public int DayAfter { get; set; }
        public int CityYearCurrencyRuleSheetId { get; set; }
        [ForeignKey(nameof(CityYearCurrencyRuleSheetId))] public virtual CityYearCurrencyRuleSheet? CityYearCurrencyRuleSheet { get; set; }
    }
}
