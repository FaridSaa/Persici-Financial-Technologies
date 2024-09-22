namespace Calculator.Domain.Repository
{
    using Calculator.Domain.Entity.Interface;

    public interface IRepository
    {
        Task<IDictionary<int,IRuleSheet>> GetRuleSheetAsync(ICity city, IEnumerable<int> Years, CancellationToken cancellationToken);
    }
}
