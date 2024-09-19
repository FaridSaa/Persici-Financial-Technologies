namespace Calculator.DataAccess.Database.Context
{
    using Calculator.DataAccess.Database.Entity;
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext(DbContextOptions option) : DbContext(option)
    {
        public DbSet<VehicleType> Vehicle { get; set; }
        public DbSet<City> City { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<City>().HasIndex(i => i.Name).IsUnique();

            _ = modelBuilder.Entity<VehicleType>().HasIndex(i => i.Category).IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
