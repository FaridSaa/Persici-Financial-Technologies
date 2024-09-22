namespace Calculator.Infrastructure.Database.Entity
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table(nameof(CycTaxFreeVehicleType))]
    public class CycTaxFreeVehicleType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CityYearCurrencyId { get; set; }
        public int VehicleTypeId { get; set; }
        [ForeignKey(nameof(CityYearCurrencyId))] public virtual CityYearCurrency? CityYearCurrency { get; set; }
        [ForeignKey(nameof(VehicleTypeId))] public virtual VehicleType? VehicleType { get; set; }
    }
}
