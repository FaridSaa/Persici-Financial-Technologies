namespace Calculator.Infrastructure.Database.Entity
{
    using Calculator.Domain.Entity;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table(nameof(CityYearCurrencyTaxFreeDatePeriod))]
    public class CityYearCurrencyTaxFreeDatePeriod : ITaxFreePeriod
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "datetime2(0)"), Required] public DateTime From { get; set; }
        [Column(TypeName = "datetime2(0)"), Required] public DateTime To { get; set; }
        public int CityYearCurrencyId { get; set; }
        [ForeignKey(nameof(CityYearCurrencyId))] public virtual CityYearCurrency? CityYearCurrency { get; set; }
    }
}
