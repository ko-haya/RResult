namespace RResult.Api;

using Microsoft.EntityFrameworkCore;

public struct DB
{
    public static async Task<int> CallDbUpdate<V>(V record, bool success = true)
    {
        await Task.Delay(1);
        if (!success)
            throw new Exception("DB Update exception");
        return 1;
    }
}

public class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options)
        : base(options) { }

    public DbSet<Todo> Todos => Set<Todo>();
}
