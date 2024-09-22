namespace Calculator.Infrastructure.Database.Entity
{
    using Calculator.Domain.Entity.Interface;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table(nameof(CycTaxRateInterval))]
    public class CycTaxRateInterval : ITaxRateInterval
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "time"), Required] public TimeSpan From { get; set; }
        [Column(TypeName = "time"), Required] public TimeSpan To { get; set; }
        public int Fee { get; set; }
        public int CityYearCurrencyId { get; set; }
        [ForeignKey(nameof(CityYearCurrencyId))] public virtual CityYearCurrency? CityYearCurrency { get; set; }
    }
}
