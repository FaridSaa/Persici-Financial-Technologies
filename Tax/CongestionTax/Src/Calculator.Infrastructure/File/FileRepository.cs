namespace Calculator.Infrastructure.File
{
    using Calculator.Domain.Entity.Interface;
    using Calculator.Domain.Repository;
    public class FileRepository : IRepository
    {
        public Task<IDictionary<int, IRuleSheet>> GetRuleSheetAsync(ICity city, IEnumerable<int> Years, CancellationToken cancellationToken)
           => throw new NotImplementedException();
    }
}
