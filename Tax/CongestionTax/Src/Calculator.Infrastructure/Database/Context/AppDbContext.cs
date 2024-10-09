namespace Calculator.Infrastructure.Database.Context
{
    using Calculator.Infrastructure.Database.Entity;
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext(DbContextOptions option) : DbContext(option)
    {
        public DbSet<City> City { get; set; }
        public DbSet<VehicleType> VehicleType { get; set; }
        public DbSet<CityYearCurrency> CityYearCurrency { get; set; }
        public DbSet<CycTaxFreeVehicleType> CycTaxFreeVehicleType { get; set; }
        public DbSet<CycTaxFreeDatePeriod> CycTaxFreeDatePeriod { get; set; }
        public DbSet<CycTaxRateInterval> CycTaxRateInterval { get; set; }
        public DbSet<CycRuleSheet> CycRuleSheet { get; set; }
        public DbSet<HolidayTaxFreePeriod> HolidayTaxFreePeriod { get; set; }

#warning Remove repeated indexes after base call
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<City>().HasIndex(i => i.Name).IsUnique();

            _ = modelBuilder.Entity<VehicleType>().HasIndex(i => i.Type).IsUnique();

            _ = modelBuilder.Entity<CityYearCurrency>().HasIndex(i => new { i.CityId, i.Year }).IsUnique();
            _ = modelBuilder.Entity<CityYearCurrency>().HasOne(r => r.City).WithMany().OnDelete(DeleteBehavior.Cascade);

            _ = modelBuilder.Entity<CycRuleSheet>().HasOne(r => r.CityYearCurrency).WithOne().OnDelete(DeleteBehavior.Cascade);

            _ = modelBuilder.Entity<CycTaxFreeVehicleType>().HasIndex(i => new { i.CityYearCurrencyId, i.VehicleTypeId }).IsUnique();
            _ = modelBuilder.Entity<CycTaxFreeVehicleType>().HasOne(r => r.CityYearCurrency).WithMany().OnDelete(DeleteBehavior.Cascade);
            _ = modelBuilder.Entity<CycTaxFreeVehicleType>().HasOne(r => r.VehicleType).WithMany().OnDelete(DeleteBehavior.Cascade);

            _ = modelBuilder.Entity<CycTaxFreeDatePeriod>().HasIndex(i => new { i.CityYearCurrencyId, i.From , i.To }).IsUnique();
            _ = modelBuilder.Entity<CycTaxFreeDatePeriod>().HasOne(r => r.CityYearCurrency).WithMany().OnDelete(DeleteBehavior.Cascade);

            _ = modelBuilder.Entity<CycTaxRateInterval>().HasIndex(i => new { i.CityYearCurrencyId, i.From , i.To }).IsUnique();
            _ = modelBuilder.Entity<CycTaxRateInterval>().HasOne(r => r.CityYearCurrency).WithMany().OnDelete(DeleteBehavior.Cascade);

            _ = modelBuilder.Entity<HolidayTaxFreePeriod>().HasOne(r => r.CycRuleSheet).WithOne().OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
