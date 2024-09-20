namespace Calculator.Domain.Process
{
    using Calculator.Domain.Entity;
    using ErrorOr;
    public interface ICalculationService
    {
        Task<ErrorOr<IEnumerable<DateTollFee>>> CalculateAsync(ICity city, IVehicle vehicle, IEnumerable<DateTime> dateTimes, CancellationToken cancellationToken);
    }
}
