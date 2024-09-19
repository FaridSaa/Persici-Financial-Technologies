namespace Calculator.Infrastructure.Database.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table(nameof(CityYearTaxFreeDatePeriod))]
    public class CityYearTaxFreeDatePeriod
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "datetime2(0)"), Required] public DateTime From { get; set; }
        [Column(TypeName = "datetime2(0)"), Required] public DateTime To { get; set; }
        public int CityYearId { get; set; }
        [ForeignKey(nameof(CityYearId))] public virtual CityYear? CityYear { get; set; }
    }
}
