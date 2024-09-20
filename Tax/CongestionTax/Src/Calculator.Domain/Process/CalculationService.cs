namespace Calculator.Domain.Process
{
    using Calculator.Domain.Entity;
    using Calculator.Domain.Repository;
    using ErrorOr;
    public class CalculationService(IRepository repository) : ICalculationService
    {
        private readonly IRepository repository = repository;
        public async Task<ErrorOr<IEnumerable<DateTollFee>>> CalculateAsync(ICity city, IVehicle vehicle, IEnumerable<DateTime> dateTimes , CancellationToken cancellationToken)
        {
            if (city is null)
            {
                return Error.Validation("UnDefined City");
            }
            if (vehicle is null)
            {
                return Error.Validation("UnDefined Vehicle");
            }
            if (dateTimes is null || dateTimes.Count() is 0)
            {
                return Error.Validation("UnDefined DateTimes");
            }
            var years = dateTimes.Select(s => s.Date.Year).Distinct();
            var ruleSheetByYear = await repository.GetRuleSheetAsync(city, years, cancellationToken).ConfigureAwait(false);

            var dateTimesPerDay = dateTimes.GroupBy(x => x.Date);
            var TollFees = new List<DateTollFee>();
            foreach (var item in dateTimesPerDay)
            {
                //logic
            }
            return TollFees;
        }
    }
}
