namespace Calculator.DataAccess.Database.Context
{
    using Calculator.Domain.Entity.DbModel;
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext(DbContextOptions option) : DbContext(option)
    {
        public DbSet<VehicleType> Vehicle { get; set; }
        public DbSet<City> City { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<City>().HasIndex(i => i.Name).IsUnique();

            _ = modelBuilder.Entity<VehicleType>().HasIndex(i => i.Category).IsUnique();

            _ = modelBuilder.Entity<CityYear>().HasIndex(i => new { i.CityId, i.Year }).IsUnique();
            _ = modelBuilder.Entity<CityYear>().HasOne(r => r.City).WithMany();


            base.OnModelCreating(modelBuilder);

            //removing extra unnessesery index , this should call after base
            var indexMetaData = modelBuilder.Entity<CityYear>().HasIndex(i=>i.CityId).Metadata;
            modelBuilder.Entity<CityYear>().Metadata.RemoveIndex(indexMetaData);
        }
    }
}
