namespace Calculator.Infrastructure.Provider
{
    using Calculator.Domain.Entity.Interface;
    using Calculator.Domain.Repository;
    using PublicHoliday;
    public sealed class HolidayRepository(IDictionary<string, PublicHolidayBase> publicHolidayContainer) : IHolidayRepository
    {
        private readonly IDictionary<string, PublicHolidayBase> publicHolidayContainer = publicHolidayContainer;
        public IEnumerable<DateTime> GetPublicHolidays(ICity city, int year) => publicHolidayContainer[city.Name].PublicHolidays(year);
    }
}
