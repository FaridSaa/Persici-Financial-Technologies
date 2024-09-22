using Calculator.Infrastructure.Database.Context;
using Calculator.Infrastructure.Database.Entity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using Calculator.Domain.Entity.Enum;

namespace Test.Calculator.Infrastructure.Database
{
    public class InMemorySqlite : IDisposable
    {
        private readonly DbConnection? _connection;
        public AppDbContext DbContext { get; }

        public InMemorySqlite(DbContextOptions<AppDbContext> dbContextOptions)
        {
            _connection = RelationalOptionsExtension.Extract(dbContextOptions).Connection;
            DbContext = new AppDbContext(dbContextOptions);
            DbContext.Database.EnsureDeleted();
            DbContext.Database.EnsureCreated();

            //Data Of Test Senario

            var city = DbContext.City.Add(new City() { Id = 1, Name = "Gothenburg" });

            var cityYearCurrency = DbContext.CityYearCurrency.Add(new CityYearCurrency()
            {
                Id = 1,
                CityId = city.Entity.Id,
                Year = 2013,
                CurrencyUnit = CurrencyUnitEnum.SEK
            });

            _ = DbContext.CycTaxRateInterval.Add(new CycTaxRateInterval()
            {
                From = new TimeSpan(06, 00, 00),
                To = new TimeSpan(06, 29, 00),
                Fee = 8,
                CityYearCurrencyId = cityYearCurrency.Entity.Id,
            });
            _ = DbContext.CycTaxRateInterval.Add(new CycTaxRateInterval()
            {
                From = new TimeSpan(06, 30, 00),
                To = new TimeSpan(06, 59, 00),
                Fee = 13,
                CityYearCurrencyId = cityYearCurrency.Entity.Id,
            });
            _ = DbContext.CycTaxRateInterval.Add(new CycTaxRateInterval()
            {
                From = new TimeSpan(07, 00, 00),
                To = new TimeSpan(07, 59, 00),
                Fee = 18,
                CityYearCurrencyId = cityYearCurrency.Entity.Id,
            });
            _ = DbContext.CycTaxRateInterval.Add(new CycTaxRateInterval()
            {
                From = new TimeSpan(08, 00, 00),
                To = new TimeSpan(08, 29, 00),
                Fee = 13,
                CityYearCurrencyId = cityYearCurrency.Entity.Id,
            });
            _ = DbContext.CycTaxRateInterval.Add(new CycTaxRateInterval()
            {
                From = new TimeSpan(08, 30, 00),
                To = new TimeSpan(14, 59, 00),
                Fee = 8,
                CityYearCurrencyId = cityYearCurrency.Entity.Id,
            });
            _ = DbContext.CycTaxRateInterval.Add(new CycTaxRateInterval()
            {
                From = new TimeSpan(15, 00, 00),
                To = new TimeSpan(15, 29, 00),
                Fee = 13,
                CityYearCurrencyId = cityYearCurrency.Entity.Id,
            });
            _ = DbContext.CycTaxRateInterval.Add(new CycTaxRateInterval()
            {
                From = new TimeSpan(15, 30, 00),
                To = new TimeSpan(16, 59, 00),
                Fee = 18,
                CityYearCurrencyId = cityYearCurrency.Entity.Id,
            });
            _ = DbContext.CycTaxRateInterval.Add(new CycTaxRateInterval()
            {
                From = new TimeSpan(17, 00, 00),
                To = new TimeSpan(17, 59, 00),
                Fee = 13,
                CityYearCurrencyId = cityYearCurrency.Entity.Id,
            });
            _ = DbContext.CycTaxRateInterval.Add(new CycTaxRateInterval()
            {
                From = new TimeSpan(18, 00, 00),
                To = new TimeSpan(18, 29, 00),
                Fee = 8,
                CityYearCurrencyId = cityYearCurrency.Entity.Id,
            });
            _ = DbContext.CycTaxRateInterval.Add(new CycTaxRateInterval()
            {
                From = new TimeSpan(18, 30, 00),
                To = new TimeSpan(05, 59, 00),
                Fee = 0,
                CityYearCurrencyId = cityYearCurrency.Entity.Id,
            });

            //During the month of July Also Any Other Periods
            _ = DbContext.CycTaxFreeDatePeriod.Add(new CycTaxFreeDatePeriod()
            {
                From = new DateTime(2013, 07, 01),
                To = new DateTime(2013, 07, 31),
                CityYearCurrencyId = cityYearCurrency.Entity.Id
            });

            _ = DbContext.VehicleType.Add(new VehicleType() { Id = 1, Type = VehicleTypeEnum.Car });
            var motor = DbContext.VehicleType.Add(new VehicleType() { Id = 2, Type = VehicleTypeEnum.Motorcycles });
            var bus = DbContext.VehicleType.Add(new VehicleType() { Id = 3, Type = VehicleTypeEnum.Bus });
            var millitry = DbContext.VehicleType.Add(new VehicleType() { Id = 4, Type = VehicleTypeEnum.Millitry });
            var emergency = DbContext.VehicleType.Add(new VehicleType() { Id = 5, Type = VehicleTypeEnum.Emergency });
            var diplomat = DbContext.VehicleType.Add(new VehicleType() { Id = 6, Type = VehicleTypeEnum.Diplomat });
            var foreign = DbContext.VehicleType.Add(new VehicleType() { Id = 7, Type = VehicleTypeEnum.Foreign });

            _ = DbContext.CycTaxFreeVehicleType.Add(new CycTaxFreeVehicleType()
            {
                CityYearCurrencyId = cityYearCurrency.Entity.Id,
                VehicleTypeId = motor.Entity.Id
            });
            _ = DbContext.CycTaxFreeVehicleType.Add(new CycTaxFreeVehicleType()
            {
                CityYearCurrencyId = cityYearCurrency.Entity.Id,
                VehicleTypeId = bus.Entity.Id
            });
            _ = DbContext.CycTaxFreeVehicleType.Add(new CycTaxFreeVehicleType()
            {
                CityYearCurrencyId = cityYearCurrency.Entity.Id,
                VehicleTypeId = millitry.Entity.Id
            });
            _ = DbContext.CycTaxFreeVehicleType.Add(new CycTaxFreeVehicleType()
            {
                CityYearCurrencyId = cityYearCurrency.Entity.Id,
                VehicleTypeId = emergency.Entity.Id
            });
            _ = DbContext.CycTaxFreeVehicleType.Add(new CycTaxFreeVehicleType()
            {
                CityYearCurrencyId = cityYearCurrency.Entity.Id,
                VehicleTypeId = diplomat.Entity.Id
            });
            _ = DbContext.CycTaxFreeVehicleType.Add(new CycTaxFreeVehicleType()
            {
                CityYearCurrencyId = cityYearCurrency.Entity.Id,
                VehicleTypeId = foreign.Entity.Id
            });

            var ruleSheet = DbContext.CycRuleSheet.Add(new CycRuleSheet()
            {
                Id = 1,
                CityYearCurrencyId = cityYearCurrency.Entity.Id,
                IsHolidayTollFreeRuleApplied = true,
                IsWeekendTollFreeRuleApplied = true,
                MaxTollFeePerDay = 60,
                SingleChargeDurationPerMinute = 60,
            });

            _ = DbContext.HolidayTaxFreePeriod.Add(new HolidayTaxFreePeriod()
            {
                DayAfter = 1,
                DayBefore = 1,
                CityYearCurrencyRuleSheetId = ruleSheet.Entity.Id
            });

            DbContext.SaveChanges();
        }
        public static DbContextOptions<AppDbContext> CreateNewContextOptions()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlite()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseSqlite(CreateInMemoryDatabase());
            builder.UseInternalServiceProvider(serviceProvider);
            builder.EnableSensitiveDataLogging();

            return builder.Options;
        }

        private static SqliteConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            return connection;
        }

        void IDisposable.Dispose()
        {
            DbContext.Dispose();
            _connection?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
