namespace Calculator.Domain.Entity
{
    public interface IHolidayTaxFreePeriod
    {
        int DayBefore { get; set; }
        int DayAfter { get; set; }
    }
}
