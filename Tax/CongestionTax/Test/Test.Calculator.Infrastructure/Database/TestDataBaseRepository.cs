using Calculator.Domain.Entity;
using Calculator.Infrastructure.Database;

namespace Test.Calculator.Infrastructure.Database
{
    public class TestDataBaseRepository
    {
        [Fact]
        public async Task GetRuleSheetAsync()
        {
            using var db = new InMemorySqlite(InMemorySqlite.CreateNewContextOptions());
            var swedenHolidays = new PublicHoliday.SwedenPublicHoliday();
            var dbRepo = new DataBaseRepository(db.DbContext,swedenHolidays);

            var mockCity = new Mock<ICity>();

            var ruleSheet = await dbRepo.GetRuleSheetAsync()
        }
    }
}
