namespace Calculator.Domain.Entity.Interface
{
    public interface ITaxRateInterval
    {
        TimeSpan From { get; set; }
        TimeSpan To { get; set; }
        int Fee { get; set; }
    }
}
