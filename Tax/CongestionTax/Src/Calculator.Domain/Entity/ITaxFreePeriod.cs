namespace Calculator.Domain.Entity
{
    public interface ITaxFreePeriod
    {
        DateTime From { get; set; }
        DateTime To { get; set; }
    }
}
