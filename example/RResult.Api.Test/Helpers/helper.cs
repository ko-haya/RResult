using Microsoft.EntityFrameworkCore;

namespace RResult.Api.Test.Helpers;

public class MockDb : IDbContextFactory<TodoDb>
{
    public TodoDb CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TodoDb>()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseInMemoryDatabase($"InMemoryTestDb-{DateTime.Now.ToFileTimeUtc()}")
            .Options;

        return new TodoDb(options);
    }
}
