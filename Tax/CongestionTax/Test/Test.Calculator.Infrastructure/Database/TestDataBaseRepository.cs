namespace Test.Calculator.Infrastructure.Database
{
    using global::Calculator.Infrastructure.Database.Entity;

    public class TestDataBaseRepository
    {
        [Fact]
        public async Task GetRuleSheetAsync()
        {
            using var db = new InMemorySqlite(InMemorySqlite.CreateNewContextOptions());
            

            var c = db.DbContext.City.ToList();
            Assert.NotNull(c);
        }
    }
}
