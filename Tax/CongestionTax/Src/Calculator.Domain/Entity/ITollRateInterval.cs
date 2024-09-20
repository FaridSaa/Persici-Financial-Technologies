namespace Calculator.Domain.Entity
{
    public interface ITollRateInterval
    {
        TimeSpan Duration { get; set; }
        int Fee { get; set; }
    }
}
