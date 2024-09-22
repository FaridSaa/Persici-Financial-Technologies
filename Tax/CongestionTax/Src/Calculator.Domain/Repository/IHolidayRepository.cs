namespace Calculator.Domain.Repository
{
    using Calculator.Domain.Entity.Interface;
    public interface IHolidayRepository
    {
        IEnumerable<DateTime> GetPublicHolidays(ICity city, int year);
    }
}
