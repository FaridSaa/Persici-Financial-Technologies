namespace Calculator.Domain.Entity.DbModel
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Calculator.Domain.Entity;

    [Table(nameof(VehicleType))]
    public class VehicleType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "tinyint"), Required] public VehicleTypeEnum Category { get; set; }

    }
}
