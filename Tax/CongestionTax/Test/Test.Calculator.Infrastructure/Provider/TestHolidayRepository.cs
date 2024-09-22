using Calculator.Domain.Entity.Interface;
using Calculator.Infrastructure.Provider;
using Moq;
using PublicHoliday;

namespace Test.Calculator.Infrastructure.Provider
{
    public class TestHolidayRepository
    {
        private readonly IDictionary<string, PublicHolidayBase> container;
        public TestHolidayRepository()
        {
            container = new Dictionary<string, PublicHolidayBase>()
            {
                {"Gothenburg", new SwedenPublicHoliday() },
                {"New York", new USAPublicHoliday() },
                {"Paris", new FrancePublicHoliday() }
            };
        }

        [Fact]
        public void GetPublicHolidays()
        {
            //Arrage
            var mockCity = new Mock<ICity>();
            mockCity.SetupGet(x => x.Name).Returns("Gothenburg");

            //Act
            var holidayProvider = new HolidayRepository(container);
            var holidays = holidayProvider.GetPublicHolidays(mockCity.Object, 2013);

            //Assert
            _ = Assert.IsAssignableFrom<IEnumerable<DateTime>>(holidays);
            Assert.NotEmpty(holidays);
            Assert.Equal(16, holidays.Count());
        }
        [Fact]
        public void DifferentCityAndYear()
        {
            //Arrage
            var mockCity = new Mock<ICity>();
            mockCity.SetupGet(x => x.Name).Returns("Paris");

            //Act
            var holidayProvider = new HolidayRepository(container);
            var holidays = holidayProvider.GetPublicHolidays(mockCity.Object, 2022);

            //Assert
            _ = Assert.IsAssignableFrom<IEnumerable<DateTime>>(holidays);
            Assert.NotEmpty(holidays);
        }
        [Fact]
        public void UnSupportedCity()
        {
            //Arrage
            var mockCity = new Mock<ICity>();
            mockCity.SetupGet(x => x.Name).Returns("Tehran");

            //Act
            var holidayProvider = new HolidayRepository(container);

            //Assert
            _ = Assert.Throws<KeyNotFoundException>(() => holidayProvider.GetPublicHolidays(mockCity.Object, 2013));
        }
        [Fact]
        public void InvalidYear()
        {
            //Arrage
            var mockCity = new Mock<ICity>();
            mockCity.SetupGet(x => x.Name).Returns("Paris");

            //Act
            var holidayProvider = new HolidayRepository(container);

            //Assert
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => holidayProvider.GetPublicHolidays(mockCity.Object, 0));
        }
    }
}
