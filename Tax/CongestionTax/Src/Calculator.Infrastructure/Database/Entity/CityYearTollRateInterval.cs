namespace Calculator.Infrastructure.Database.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table(nameof(CityYearTollRateInterval))]
    public class CityYearTollRateInterval
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "time"), Required] public TimeSpan Duration { get; set; }
        public int Fee { get; set; }
        public int CityYearCurrencyUnitId { get; set; }
        [ForeignKey(nameof(CityYearCurrencyUnitId))] public virtual CityYearCurrencyUnit? CityYearCurrencyUnit { get; set; }
    }
}
