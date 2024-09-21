using Calculator.Domain.Entity;
using Calculator.Domain.Process;
using Calculator.Domain.Repository;
using Moq;
using PublicHoliday;

namespace Test.Calculator.Domain.Process
{
    public class TestCalculationService
    {
        [Fact]
        public async Task CalculateAsyncSimpleScenario()
        {
            //Arrage
            var mockCity = new Mock<ICity>();
            mockCity.SetupGet(x => x.Name).Returns("Gothenburg");

            var mockVehicle = new Mock<IVehicle>();
            mockVehicle.Setup(x => x.GetVehicleType()).Returns(VehicleTypeEnum.Car);

            var mockRuleSheet = new Mock<IRuleSheet>();
            mockRuleSheet.SetupGet(x => x.City).Returns(mockCity.Object);
            mockRuleSheet.SetupGet(x => x.Year).Returns(2013);
            mockRuleSheet.SetupGet(x => x.CurrencyUnit).Returns(CurrencyUnitEnum.SEK);
            mockRuleSheet.SetupGet(x => x.TollRateIntervals).Returns(new List<TollRateInterval>()
            {
                new() { From = new TimeSpan(06,00,00), To = new TimeSpan(06,29,00), Fee = 8 },
                new() { From = new TimeSpan(06,30,00), To = new TimeSpan(06,59,00), Fee = 13 },
                new() { From = new TimeSpan(07,00,00), To = new TimeSpan(07,59,00), Fee = 18 },
                new() { From = new TimeSpan(08,00,00), To = new TimeSpan(08,29,00), Fee = 13 },
                new() { From = new TimeSpan(08,30,00), To = new TimeSpan(14,59,00), Fee = 8 },
                new() { From = new TimeSpan(15,00,00), To = new TimeSpan(15,29,00), Fee = 13 },
                new() { From = new TimeSpan(15,30,00), To = new TimeSpan(16,59,00), Fee = 18 },
                new() { From = new TimeSpan(17,00,00), To = new TimeSpan(17,59,00), Fee = 13 },
                new() { From = new TimeSpan(18,00,00), To = new TimeSpan(18,29,00), Fee = 8 },
                new() { From = new TimeSpan(18,30,00), To = new TimeSpan(05,59,00), Fee = 0 },
            });
            mockRuleSheet.SetupGet(x => x.PublicHolidays).Returns(new SwedenPublicHoliday().PublicHolidays(2013));

            var taxFreePeriod = new Mock<ITaxFreePeriod>();
            taxFreePeriod.Object.From = new DateTime(2013, 07, 01);
            taxFreePeriod.Object.To = new DateTime(2013, 07, 31);

            mockRuleSheet.SetupGet(x => x.TaxFreePeriods).Returns(new List<ITaxFreePeriod>() { taxFreePeriod.Object });
            mockRuleSheet.SetupGet(x => x.TaxFreeVehicleTypes).Returns(new List<VehicleTypeEnum>()
            {
                VehicleTypeEnum.Emergency,
                VehicleTypeEnum.Bus,
                VehicleTypeEnum.Millitry,
                VehicleTypeEnum.Diplomat,
                VehicleTypeEnum.Motorcycles,
                VehicleTypeEnum.Foreign
            });
            var holidayPeriod = new Mock<IHolidayTaxFreePeriod>();
            holidayPeriod.Object.DayAfter = 1;
            holidayPeriod.Object.DayBefore = 1;

            mockRuleSheet.SetupGet(x => x.HolidayTaxFreePeriod).Returns(holidayPeriod.Object);
            mockRuleSheet.SetupGet(x => x.MaxTollFeePerDay).Returns(60);
            mockRuleSheet.SetupGet(x => x.SingleChargeDurationPerMinute).Returns(60);
            mockRuleSheet.SetupGet(x => x.IsHolidayTollFreeRuleApplied).Returns(true);
            mockRuleSheet.SetupGet(x => x.IsWeekendTollFreeRuleApplied).Returns(true);

            var ruleSheetDic = new Dictionary<int, IRuleSheet>() { { 2013, mockRuleSheet.Object } };

            var mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetRuleSheetAsync(It.IsAny<ICity>(), It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>())).ReturnsAsync(ruleSheetDic);

            var dateTimes = new List<DateTime>()
            {
                new(2013,01,14,21,00,00),
                new(2013,01,15,21,00,00),

            };

            //Act
            var caculationService = new CalculationService(mockRepository.Object);
            var dateTax = await caculationService.CalculateAsync(mockCity.Object, mockVehicle.Object, dateTimes, CancellationToken.None);

            //Assert
            _ = Assert.IsAssignableFrom<IEnumerable<DateTax>>(dateTax);
            Assert.NotNull(dateTax);
            Assert.Equal(0, dateTax.Sum(s => s.Fee));
        }

        [Fact]
        public async Task CalculateAsyncCourseExactScenario()
        {
            //Arrage
            var mockCity = new Mock<ICity>();
            mockCity.SetupGet(x => x.Name).Returns("Gothenburg");

            var mockVehicle = new Mock<IVehicle>();
            mockVehicle.Setup(x => x.GetVehicleType()).Returns(VehicleTypeEnum.Car);

            var mockRuleSheet = new Mock<IRuleSheet>();
            mockRuleSheet.SetupGet(x => x.City).Returns(mockCity.Object);
            mockRuleSheet.SetupGet(x => x.Year).Returns(2013);
            mockRuleSheet.SetupGet(x => x.CurrencyUnit).Returns(CurrencyUnitEnum.SEK);
            mockRuleSheet.SetupGet(x => x.TollRateIntervals).Returns(new List<TollRateInterval>()
            {
                new() { From = new TimeSpan(06,00,00), To = new TimeSpan(06,29,00), Fee = 8 },
                new() { From = new TimeSpan(06,30,00), To = new TimeSpan(06,59,00), Fee = 13 },
                new() { From = new TimeSpan(07,00,00), To = new TimeSpan(07,59,00), Fee = 18 },
                new() { From = new TimeSpan(08,00,00), To = new TimeSpan(08,29,00), Fee = 13 },
                new() { From = new TimeSpan(08,30,00), To = new TimeSpan(14,59,00), Fee = 8 },
                new() { From = new TimeSpan(15,00,00), To = new TimeSpan(15,29,00), Fee = 13 },
                new() { From = new TimeSpan(15,30,00), To = new TimeSpan(16,59,00), Fee = 18 },
                new() { From = new TimeSpan(17,00,00), To = new TimeSpan(17,59,00), Fee = 13 },
                new() { From = new TimeSpan(18,00,00), To = new TimeSpan(18,29,00), Fee = 8 },
                new() { From = new TimeSpan(18,30,00), To = new TimeSpan(05,59,00), Fee = 0 },
            });
            mockRuleSheet.SetupGet(x => x.PublicHolidays).Returns(new SwedenPublicHoliday().PublicHolidays(2013));

            var taxFreePeriod = new Mock<ITaxFreePeriod>();
            taxFreePeriod.Object.From = new DateTime(2013, 07, 01);
            taxFreePeriod.Object.To = new DateTime(2013, 07, 31);

            mockRuleSheet.SetupGet(x => x.TaxFreePeriods).Returns(new List<ITaxFreePeriod>() { taxFreePeriod.Object });
            mockRuleSheet.SetupGet(x => x.TaxFreeVehicleTypes).Returns(new List<VehicleTypeEnum>()
            {
                VehicleTypeEnum.Emergency,
                VehicleTypeEnum.Bus,
                VehicleTypeEnum.Millitry,
                VehicleTypeEnum.Diplomat,
                VehicleTypeEnum.Motorcycles,
                VehicleTypeEnum.Foreign
            });
            var holidayPeriod = new Mock<IHolidayTaxFreePeriod>();
            holidayPeriod.Object.DayAfter = 1;
            holidayPeriod.Object.DayBefore = 1;

            mockRuleSheet.SetupGet(x => x.HolidayTaxFreePeriod).Returns(holidayPeriod.Object);
            mockRuleSheet.SetupGet(x => x.MaxTollFeePerDay).Returns(60);
            mockRuleSheet.SetupGet(x => x.SingleChargeDurationPerMinute).Returns(60);
            mockRuleSheet.SetupGet(x => x.IsHolidayTollFreeRuleApplied).Returns(true);
            mockRuleSheet.SetupGet(x => x.IsWeekendTollFreeRuleApplied).Returns(true);

            var ruleSheetDic = new Dictionary<int, IRuleSheet>() { { 2013, mockRuleSheet.Object } };

            var mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetRuleSheetAsync(It.IsAny<ICity>(), It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>())).ReturnsAsync(ruleSheetDic);

            var dateTimes = new List<DateTime>()
            {
                new(2013,01,14,21,00,00),
                new(2013,01,15,21,00,00),
                new(2013,02,07,06,23,27),
                new(2013,02,07,15,27,00),
                new(2013,02,08,06,27,00),
                new(2013,02,08,06,20,27),
                new(2013,02,08,14,35,00),
                new(2013,02,08,15,29,00),
                new(2013,02,08,15,47,00),
                new(2013,02,08,16,01,00),
                new(2013,02,08,16,48,00),
                new(2013,02,08,17,49,00),
                new(2013,02,08,18,29,00),
                new(2013,02,08,18,35,00),
                new(2013,03,26,14,25,00),
                new(2013,03,28,14,07,27)
            };

            //Act
            var caculationService = new CalculationService(mockRepository.Object);
            var dateTax = await caculationService.CalculateAsync(mockCity.Object, mockVehicle.Object, dateTimes, CancellationToken.None);

            var taxableFeePerDay = dateTax.ToList();
            //Assert
            _ = Assert.IsAssignableFrom<IEnumerable<DateTax>>(dateTax);
            Assert.NotNull(dateTax);

            Assert.Equal(0, taxableFeePerDay[0].Fee);
            Assert.Equal(new DateTime(2013, 01, 14), taxableFeePerDay[0].Date);

            Assert.Equal(0, taxableFeePerDay[1].Fee);
            Assert.Equal(new DateTime(2013, 01, 15), taxableFeePerDay[1].Date);

            Assert.Equal(21, taxableFeePerDay[2].Fee);
            Assert.Equal(new DateTime(2013, 02, 07), taxableFeePerDay[2].Date);

            Assert.Equal(60, taxableFeePerDay[3].Fee);
            Assert.Equal(new DateTime(2013, 02, 08), taxableFeePerDay[3].Date);

            Assert.Equal(8, taxableFeePerDay[4].Fee);
            Assert.Equal(new DateTime(2013, 03, 26), taxableFeePerDay[4].Date);

            Assert.Equal(8, taxableFeePerDay[5].Fee);
            Assert.Equal(new DateTime(2013, 03, 28), taxableFeePerDay[5].Date);
        }

        [Fact]
        public async Task CourseExactScenarioAddedWeekendAndHolidays()
        {
            //Arrage
            var mockCity = new Mock<ICity>();
            mockCity.SetupGet(x => x.Name).Returns("Gothenburg");

            var mockVehicle = new Mock<IVehicle>();
            mockVehicle.Setup(x => x.GetVehicleType()).Returns(VehicleTypeEnum.Car);

            var mockRuleSheet = new Mock<IRuleSheet>();
            mockRuleSheet.SetupGet(x => x.City).Returns(mockCity.Object);
            mockRuleSheet.SetupGet(x => x.Year).Returns(2013);
            mockRuleSheet.SetupGet(x => x.CurrencyUnit).Returns(CurrencyUnitEnum.SEK);
            mockRuleSheet.SetupGet(x => x.TollRateIntervals).Returns(new List<TollRateInterval>()
            {
                new() { From = new TimeSpan(06,00,00), To = new TimeSpan(06,29,00), Fee = 8 },
                new() { From = new TimeSpan(06,30,00), To = new TimeSpan(06,59,00), Fee = 13 },
                new() { From = new TimeSpan(07,00,00), To = new TimeSpan(07,59,00), Fee = 18 },
                new() { From = new TimeSpan(08,00,00), To = new TimeSpan(08,29,00), Fee = 13 },
                new() { From = new TimeSpan(08,30,00), To = new TimeSpan(14,59,00), Fee = 8 },
                new() { From = new TimeSpan(15,00,00), To = new TimeSpan(15,29,00), Fee = 13 },
                new() { From = new TimeSpan(15,30,00), To = new TimeSpan(16,59,00), Fee = 18 },
                new() { From = new TimeSpan(17,00,00), To = new TimeSpan(17,59,00), Fee = 13 },
                new() { From = new TimeSpan(18,00,00), To = new TimeSpan(18,29,00), Fee = 8 },
                new() { From = new TimeSpan(18,30,00), To = new TimeSpan(05,59,00), Fee = 0 },
            });
            var publicHolidays = new SwedenPublicHoliday().PublicHolidays(2013);
            mockRuleSheet.SetupGet(x => x.PublicHolidays).Returns(publicHolidays);

            var taxFreePeriod = new Mock<ITaxFreePeriod>();
            taxFreePeriod.Object.From = new DateTime(2013, 07, 01);
            taxFreePeriod.Object.To = new DateTime(2013, 07, 31);

            mockRuleSheet.SetupGet(x => x.TaxFreePeriods).Returns(new List<ITaxFreePeriod>() { taxFreePeriod.Object });
            mockRuleSheet.SetupGet(x => x.TaxFreeVehicleTypes).Returns(new List<VehicleTypeEnum>()
            {
                VehicleTypeEnum.Emergency,
                VehicleTypeEnum.Bus,
                VehicleTypeEnum.Millitry,
                VehicleTypeEnum.Diplomat,
                VehicleTypeEnum.Motorcycles,
                VehicleTypeEnum.Foreign
            });
            var holidayPeriod = new Mock<IHolidayTaxFreePeriod>();
            holidayPeriod.Object.DayAfter = 1;
            holidayPeriod.Object.DayBefore = 1;

            mockRuleSheet.SetupGet(x => x.HolidayTaxFreePeriod).Returns(holidayPeriod.Object);
            mockRuleSheet.SetupGet(x => x.MaxTollFeePerDay).Returns(60);
            mockRuleSheet.SetupGet(x => x.SingleChargeDurationPerMinute).Returns(60);
            mockRuleSheet.SetupGet(x => x.IsHolidayTollFreeRuleApplied).Returns(true);
            mockRuleSheet.SetupGet(x => x.IsWeekendTollFreeRuleApplied).Returns(true);

            var ruleSheetDic = new Dictionary<int, IRuleSheet>() { { 2013, mockRuleSheet.Object } };

            var mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetRuleSheetAsync(It.IsAny<ICity>(), It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>())).ReturnsAsync(ruleSheetDic);

            var dateTimes = new List<DateTime>
            {
                new(2013, 01, 14, 21, 00, 00),
                new(2013, 01, 15, 21, 00, 00),
                new(2013, 02, 07, 06, 23, 27),
                new(2013, 02, 07, 15, 27, 00),
                new(2013, 02, 08, 06, 27, 00),
                new(2013, 02, 08, 06, 20, 27),
                new(2013, 02, 08, 14, 35, 00),
                new(2013, 02, 08, 15, 29, 00),
                new(2013, 02, 08, 15, 47, 00),
                new(2013, 02, 08, 16, 01, 00),
                new(2013, 02, 08, 16, 48, 00),
                new(2013, 02, 08, 17, 49, 00),
                new(2013, 02, 08, 18, 29, 00),
                new(2013, 02, 08, 18, 35, 00),
                new(2013, 03, 26, 14, 25, 00),
                new(2013, 03, 28, 14, 07, 27),

                //july
                new(2013, 07, 28, 14, 07, 27),
               
                //wrong order
                publicHolidays.Where(x => x.Month is 4).First(),
                //holiday
                publicHolidays.Where(x => x.Month is 1).Last(),

                //weekend
                new(2013, 10, 19, 14, 07, 27),
            };



            //Act
            var caculationService = new CalculationService(mockRepository.Object);
            var dateTax = await caculationService.CalculateAsync(mockCity.Object, mockVehicle.Object, dateTimes, CancellationToken.None);

            var taxableFeePerDay = dateTax.ToList();
            //Assert
            _ = Assert.IsAssignableFrom<IEnumerable<DateTax>>(dateTax);
            Assert.NotNull(dateTax);

            Assert.Equal(0, taxableFeePerDay[0].Fee);
            Assert.Equal(1, taxableFeePerDay[0].Date.Month);

            Assert.Equal(0, taxableFeePerDay[1].Fee);
            Assert.Equal(new DateTime(2013, 01, 14), taxableFeePerDay[1].Date);

            Assert.Equal(0, taxableFeePerDay[2].Fee);
            Assert.Equal(new DateTime(2013, 01, 15), taxableFeePerDay[2].Date);

            Assert.Equal(21, taxableFeePerDay[3].Fee);
            Assert.Equal(new DateTime(2013, 02, 07), taxableFeePerDay[3].Date);

            Assert.Equal(60, taxableFeePerDay[4].Fee);
            Assert.Equal(new DateTime(2013, 02, 08), taxableFeePerDay[4].Date);

            Assert.Equal(8, taxableFeePerDay[5].Fee);
            Assert.Equal(new DateTime(2013, 03, 26), taxableFeePerDay[5].Date);

            Assert.Equal(8, taxableFeePerDay[6].Fee);
            Assert.Equal(new DateTime(2013, 03, 28), taxableFeePerDay[6].Date);

            Assert.Equal(0, taxableFeePerDay[7].Fee);
            Assert.Equal(4, taxableFeePerDay[7].Date.Month);

            Assert.Equal(0, taxableFeePerDay[8].Fee);
            Assert.Equal(7, taxableFeePerDay[8].Date.Month);

            Assert.Equal(0, taxableFeePerDay[9].Fee);
            Assert.Equal(new DateTime(2013, 10, 19), taxableFeePerDay[9].Date);
        }

        [Fact]
        public async Task CourseExactScenarioAddedWeekendAndHolidaysTaxFreeVehicle()
        {
            //Arrage
            var mockCity = new Mock<ICity>();
            mockCity.SetupGet(x => x.Name).Returns("Gothenburg");

            var mockVehicle = new Mock<IVehicle>();
            mockVehicle.Setup(x => x.GetVehicleType()).Returns(VehicleTypeEnum.Motorcycles);

            var mockRuleSheet = new Mock<IRuleSheet>();
            mockRuleSheet.SetupGet(x => x.City).Returns(mockCity.Object);
            mockRuleSheet.SetupGet(x => x.Year).Returns(2013);
            mockRuleSheet.SetupGet(x => x.CurrencyUnit).Returns(CurrencyUnitEnum.SEK);
            mockRuleSheet.SetupGet(x => x.TollRateIntervals).Returns(new List<TollRateInterval>()
            {
                new() { From = new TimeSpan(06,00,00), To = new TimeSpan(06,29,00), Fee = 8 },
                new() { From = new TimeSpan(06,30,00), To = new TimeSpan(06,59,00), Fee = 13 },
                new() { From = new TimeSpan(07,00,00), To = new TimeSpan(07,59,00), Fee = 18 },
                new() { From = new TimeSpan(08,00,00), To = new TimeSpan(08,29,00), Fee = 13 },
                new() { From = new TimeSpan(08,30,00), To = new TimeSpan(14,59,00), Fee = 8 },
                new() { From = new TimeSpan(15,00,00), To = new TimeSpan(15,29,00), Fee = 13 },
                new() { From = new TimeSpan(15,30,00), To = new TimeSpan(16,59,00), Fee = 18 },
                new() { From = new TimeSpan(17,00,00), To = new TimeSpan(17,59,00), Fee = 13 },
                new() { From = new TimeSpan(18,00,00), To = new TimeSpan(18,29,00), Fee = 8 },
                new() { From = new TimeSpan(18,30,00), To = new TimeSpan(05,59,00), Fee = 0 },
            });
            var publicHolidays = new SwedenPublicHoliday().PublicHolidays(2013);
            mockRuleSheet.SetupGet(x => x.PublicHolidays).Returns(publicHolidays);

            var taxFreePeriod = new Mock<ITaxFreePeriod>();
            taxFreePeriod.Object.From = new DateTime(2013, 07, 01);
            taxFreePeriod.Object.To = new DateTime(2013, 07, 31);

            mockRuleSheet.SetupGet(x => x.TaxFreePeriods).Returns(new List<ITaxFreePeriod>() { taxFreePeriod.Object });
            mockRuleSheet.SetupGet(x => x.TaxFreeVehicleTypes).Returns(new List<VehicleTypeEnum>()
            {
                VehicleTypeEnum.Emergency,
                VehicleTypeEnum.Bus,
                VehicleTypeEnum.Millitry,
                VehicleTypeEnum.Diplomat,
                VehicleTypeEnum.Motorcycles,
                VehicleTypeEnum.Foreign
            });
            var holidayPeriod = new Mock<IHolidayTaxFreePeriod>();
            holidayPeriod.Object.DayAfter = 1;
            holidayPeriod.Object.DayBefore = 1;

            mockRuleSheet.SetupGet(x => x.HolidayTaxFreePeriod).Returns(holidayPeriod.Object);
            mockRuleSheet.SetupGet(x => x.MaxTollFeePerDay).Returns(60);
            mockRuleSheet.SetupGet(x => x.SingleChargeDurationPerMinute).Returns(60);
            mockRuleSheet.SetupGet(x => x.IsHolidayTollFreeRuleApplied).Returns(true);
            mockRuleSheet.SetupGet(x => x.IsWeekendTollFreeRuleApplied).Returns(true);

            var ruleSheetDic = new Dictionary<int, IRuleSheet>() { { 2013, mockRuleSheet.Object } };

            var mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetRuleSheetAsync(It.IsAny<ICity>(), It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>())).ReturnsAsync(ruleSheetDic);

            var dateTimes = new List<DateTime>
            {
                new(2013, 01, 14, 21, 00, 00),
                new(2013, 01, 15, 21, 00, 00),
                new(2013, 02, 07, 06, 23, 27),
                new(2013, 02, 07, 15, 27, 00),
                new(2013, 02, 08, 06, 27, 00),
                new(2013, 02, 08, 06, 20, 27),
                new(2013, 02, 08, 14, 35, 00),
                new(2013, 02, 08, 15, 29, 00),
                new(2013, 02, 08, 15, 47, 00),
                new(2013, 02, 08, 16, 01, 00),
                new(2013, 02, 08, 16, 48, 00),
                new(2013, 02, 08, 17, 49, 00),
                new(2013, 02, 08, 18, 29, 00),
                new(2013, 02, 08, 18, 35, 00),
                new(2013, 03, 26, 14, 25, 00),
                new(2013, 03, 28, 14, 07, 27),

                //july
                new(2013, 07, 28, 14, 07, 27),
               
                //wrong order
                publicHolidays.Where(x => x.Month is 4).First(),
                //holiday
                publicHolidays.Where(x => x.Month is 1).Last(),

                //weekend
                new(2013, 10, 19, 14, 07, 27),
            };



            //Act
            var caculationService = new CalculationService(mockRepository.Object);
            var dateTax = await caculationService.CalculateAsync(mockCity.Object, mockVehicle.Object, dateTimes, CancellationToken.None);

            var taxableFeePerDay = dateTax.ToList();
            //Assert
            _ = Assert.IsAssignableFrom<IEnumerable<DateTax>>(dateTax);
            Assert.NotNull(dateTax);

            Assert.Equal(10, taxableFeePerDay.Count);
            Assert.Equal(0, taxableFeePerDay.Sum(i=>i.Fee));
        }

        //test whit diffrent rule sheet diffrent city diffrent year
    }

    public class TollRateInterval : ITollRateInterval
    {
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
        public int Fee { get; set; }
    }
}
