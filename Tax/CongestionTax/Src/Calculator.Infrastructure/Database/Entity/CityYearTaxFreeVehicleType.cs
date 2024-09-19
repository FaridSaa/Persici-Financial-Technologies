namespace Calculator.Infrastructure.Database.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table(nameof(CityYearTaxFreeVehicleType))]
    public class CityYearTaxFreeVehicleType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CityYearId { get; set; }
        public int VehicleTypeId { get; set; }
        [ForeignKey(nameof(CityYearId))] public virtual CityYear? CityYear { get; set; }
        [ForeignKey(nameof(VehicleTypeId))] public virtual VehicleType? VehicleType { get; set; }
    }
}
