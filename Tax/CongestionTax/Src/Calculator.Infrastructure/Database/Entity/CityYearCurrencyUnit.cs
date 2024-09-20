namespace Calculator.Infrastructure.Database.Entity
{
    using Calculator.Domain.Entity;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table(nameof(CityYearCurrencyUnit))]
    public class CityYearCurrencyUnit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public CurrencyUnitEnum Unit { get; set; }
        public int CityYearId { get; set; }
        [ForeignKey(nameof(CityYearId))] public virtual CityYear? CityYear { get; set; }
    }
}
