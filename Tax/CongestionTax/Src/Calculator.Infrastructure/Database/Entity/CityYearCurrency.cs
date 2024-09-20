namespace Calculator.Infrastructure.Database.Entity
{
    using Calculator.Domain.Entity;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table(nameof(CityYearCurrency))]
    public class CityYearCurrency
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Year { get; set; }
        public CurrencyUnitEnum CurrencyUnit { get; set; }
        public int CityId { get; set; }
        [ForeignKey(nameof(CityId))] public virtual City? City { get; set; }
    }
}
