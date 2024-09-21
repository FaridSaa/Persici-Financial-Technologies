using Calculator.Domain.Entity;
using Calculator.Infrastructure.Database;
using Moq;

namespace Test.Calculator.Infrastructure.Database
{
    public class TestDataBaseRepository
    {
        [Fact]
        public async Task GetRuleSheetAsync()
        {
            //Arrage
            using var db = new InMemorySqlite(InMemorySqlite.CreateNewContextOptions());
            var swedenHolidays = new PublicHoliday.SwedenPublicHoliday();
            var publicHolidays = swedenHolidays.PublicHolidays(2013);
            var dbRepo = new DataBaseRepository(db.DbContext, swedenHolidays);

            var mockCity = new Mock<ICity>();
            mockCity.SetupGet(x => x.Name).Returns("Gothenburg");

            var years = new List<int>() { 2013 };

            //Act
            var ruleSheet = await dbRepo.GetRuleSheetAsync(mockCity.Object, years, CancellationToken.None);

            //Assert
            _ = Assert.IsAssignableFrom<IDictionary<int, IRuleSheet>>(ruleSheet);
            Assert.NotNull(ruleSheet);
            _ = Assert.Single(ruleSheet);

            var yearRuleSheet = ruleSheet[2013];

            Assert.Equal("Gothenburg", yearRuleSheet.City.Name);
            Assert.Equal(2013, yearRuleSheet.Year);
            Assert.Equal(CurrencyUnitEnum.SEK, yearRuleSheet.CurrencyUnit);
            Assert.Equal(10, yearRuleSheet.TollRateIntervals.Count());
            Assert.Equal(16, yearRuleSheet.PublicHolidays.Count());

            Assert.NotNull(yearRuleSheet.TaxFreePeriods);
            _ = Assert.Single(yearRuleSheet.TaxFreePeriods);
            Assert.Equal(new DateTime(2013, 07, 01), yearRuleSheet.TaxFreePeriods.First().From);
            Assert.Equal(new DateTime(2013, 07, 31), yearRuleSheet.TaxFreePeriods.First().To);

            Assert.NotNull(yearRuleSheet.TaxFreeVehicleTypes);
            Assert.Equal(6,yearRuleSheet.TaxFreeVehicleTypes.Count());

            Assert.NotNull(yearRuleSheet.HolidayTaxFreePeriod);
            Assert.Equal(1, yearRuleSheet.HolidayTaxFreePeriod.DayAfter);
            Assert.Equal(1, yearRuleSheet.HolidayTaxFreePeriod.DayBefore);

            Assert.NotNull(yearRuleSheet.MaxTollFeePerDay);
            Assert.Equal(60,yearRuleSheet.MaxTollFeePerDay);

            Assert.NotNull(yearRuleSheet.SingleChargeDurationPerMinute);
            Assert.Equal(60, yearRuleSheet.SingleChargeDurationPerMinute);

            Assert.True(yearRuleSheet.IsHolidayTollFreeRuleApplied);
            Assert.True(yearRuleSheet.IsWeekendTollFreeRuleApplied);
        }

        //diffrent test senarios will append here , like multipe year fetching
    }
}
