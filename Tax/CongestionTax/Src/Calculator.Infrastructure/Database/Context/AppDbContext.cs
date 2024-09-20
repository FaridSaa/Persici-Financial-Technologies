namespace Calculator.Infrastructure.Database.Context
{
    using Calculator.Infrastructure.Database.Entity;
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext(DbContextOptions option) : DbContext(option)
    {
        public DbSet<City> City { get; set; }
        public DbSet<VehicleType> VehicleType { get; set; }
        public DbSet<CityYearCurrency> CityYearCurrency { get; set; }
        public DbSet<CityYearCurrencyTaxFreeVehicleType> CityYearCurrencyTaxFreeVehicleType { get; set; }
        public DbSet<CityYearCurrencyTaxFreeDatePeriod> CityYearCurrencyTaxFreeDatePeriod { get; set; }
        public DbSet<CityYearCurrencyTollRateInterval> CityYearCurrencyTollRateInterval { get; set; }
        public DbSet<CityYearCurrencyRuleSheet> CityYearCurrencyRuleSheet { get; set; }
        public DbSet<HolidayTaxFreePeriod> HolidayTaxFreePeriod { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<City>().HasIndex(i => i.Name).IsUnique();

            _ = modelBuilder.Entity<VehicleType>().HasIndex(i => i.Type).IsUnique();

            _ = modelBuilder.Entity<CityYearCurrency>().HasIndex(i => new { i.CityId, i.Year }).IsUnique();
            _ = modelBuilder.Entity<CityYearCurrency>().HasOne(r => r.City).WithMany().OnDelete(DeleteBehavior.Cascade);

            _ = modelBuilder.Entity<CityYearCurrencyRuleSheet>().HasOne(r => r.CityYearCurrency).WithOne().OnDelete(DeleteBehavior.Cascade);


            _ = modelBuilder.Entity<CityYearCurrencyTaxFreeVehicleType>().HasIndex(i => new { i.CityYearCurrencyId, i.VehicleTypeId }).IsUnique();
            _ = modelBuilder.Entity<CityYearCurrencyTaxFreeVehicleType>().HasOne(r => r.CityYearCurrency).WithMany().OnDelete(DeleteBehavior.Cascade);
            _ = modelBuilder.Entity<CityYearCurrencyTaxFreeVehicleType>().HasOne(r => r.VehicleType).WithMany().OnDelete(DeleteBehavior.Cascade);

            _ = modelBuilder.Entity<CityYearCurrencyTaxFreeDatePeriod>().HasIndex(i => new { i.CityYearCurrencyId, i.From , i.To }).IsUnique();
            _ = modelBuilder.Entity<CityYearCurrencyTaxFreeDatePeriod>().HasOne(r => r.CityYearCurrency).WithMany().OnDelete(DeleteBehavior.Cascade);

            _ = modelBuilder.Entity<CityYearCurrencyTollRateInterval>().HasIndex(i => new { i.CityYearCurrencyId, i.Duration }).IsUnique();
            _ = modelBuilder.Entity<CityYearCurrencyTollRateInterval>().HasOne(r => r.CityYearCurrency).WithMany().OnDelete(DeleteBehavior.Cascade);


            _ = modelBuilder.Entity<HolidayTaxFreePeriod>().HasOne(r => r.CityYearCurrencyRuleSheet).WithOne().OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
