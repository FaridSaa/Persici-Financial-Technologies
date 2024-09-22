namespace Calculator.Infrastructure.Database.Entity
{
    using Calculator.Domain.Entity.Interface;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table(nameof(City))]
    public class City : ICity
    {
        public City() => Name = string.Empty;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(128)"), Required] public string Name { get; set; }

    }
}
