namespace Calculator.Domain.Process
{
    using Calculator.Domain.Entity;
    using Calculator.Domain.Entity.Interface;

    public interface ICalculationService
    {
        Task<IEnumerable<DateTax>> CalculateAsync(ICity city, IVehicle vehicle, IEnumerable<DateTime> dateTimes, CancellationToken cancellationToken);
    }
}
