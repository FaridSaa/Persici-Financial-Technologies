namespace Calculator.Infrastructure.Database.Entity
{
    using Calculator.Domain.Entity;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table(nameof(CityYearCurrencyTollRateInterval))]
    public class CityYearCurrencyTollRateInterval : ITollRateInterval
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "time"), Required] public TimeSpan Duration { get; set; }
        public int Fee { get; set; }
        public int CityYearCurrencyId { get; set; }
        [ForeignKey(nameof(CityYearCurrencyId))] public virtual CityYearCurrency? CityYearCurrency { get; set; }
    }
}
