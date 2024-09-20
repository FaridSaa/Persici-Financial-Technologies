namespace Calculator.Domain.Entity
{
    public interface ITollRateInterval
    {
        TimeSpan From { get; set; }
        TimeSpan To { get; set; }
        int Fee { get; set; }
    }
}
