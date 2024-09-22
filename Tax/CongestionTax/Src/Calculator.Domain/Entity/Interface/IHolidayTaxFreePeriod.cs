namespace Calculator.Domain.Entity.Interface
{
    public interface IHolidayTaxFreePeriod
    {
        int DayBefore { get; set; }
        int DayAfter { get; set; }
    }
}
