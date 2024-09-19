﻿namespace Calculator.Infrastructure.Database.Context
{
    using Calculator.Infrastructure.Database.Entity;
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext(DbContextOptions option) : DbContext(option)
    {
        public DbSet<City> City { get; set; }
        public DbSet<VehicleType> Vehicle { get; set; }
        public DbSet<CityYear> CityYear { get; set; }
        public DbSet<CityYearTaxFreeVehicleType> CityYearTaxFreeVehicleType { get; set; }
        public DbSet<CityYearTaxFreeDatePeriod> CityYearTaxFreeDatePeriod { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<City>().HasIndex(i => i.Name).IsUnique();

            _ = modelBuilder.Entity<VehicleType>().HasIndex(i => i.Category).IsUnique();

            _ = modelBuilder.Entity<CityYear>().HasIndex(i => new { i.CityId, i.Year }).IsUnique();
            _ = modelBuilder.Entity<CityYear>().HasOne(r => r.City).WithMany().OnDelete(DeleteBehavior.Cascade);

            _ = modelBuilder.Entity<CityYearTaxFreeVehicleType>().HasIndex(i => new { i.CityYearId, i.VehicleTypeId }).IsUnique();
            _ = modelBuilder.Entity<CityYearTaxFreeVehicleType>().HasOne(r => r.CityYear).WithMany().OnDelete(DeleteBehavior.Cascade);
            _ = modelBuilder.Entity<CityYearTaxFreeVehicleType>().HasOne(r => r.VehicleType).WithMany().OnDelete(DeleteBehavior.Cascade);

            _ = modelBuilder.Entity<CityYearTaxFreeDatePeriod>().HasIndex(i => new { i.CityYearId, i.From , i.To }).IsUnique();
            _ = modelBuilder.Entity<CityYearTaxFreeDatePeriod>().HasOne(r => r.CityYear).WithMany().OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);

            //removing extra unnessesery index , this should call after base
            var cityIndexMetaData = modelBuilder.Entity<CityYear>().HasIndex(i => i.CityId).Metadata;
            modelBuilder.Entity<CityYear>().Metadata.RemoveIndex(cityIndexMetaData);

            var cityYearIndexMetaData = modelBuilder.Entity<CityYearTaxFreeVehicleType>().HasIndex(i => i.CityYearId).Metadata;
            modelBuilder.Entity<CityYearTaxFreeVehicleType>().Metadata.RemoveIndex(cityYearIndexMetaData);

            var cityYearPeriodIndexMetaData = modelBuilder.Entity<CityYearTaxFreeDatePeriod>().HasIndex(i => i.CityYearId).Metadata;
            modelBuilder.Entity<CityYearTaxFreeDatePeriod>().Metadata.RemoveIndex(cityYearPeriodIndexMetaData);
        }
    }
}
