namespace Calculator.Domain.Entity.Interface
{
    public interface ITaxFreePeriod
    {
        DateTime From { get; set; }
        DateTime To { get; set; }
    }
}
