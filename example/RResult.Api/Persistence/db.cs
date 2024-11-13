namespace RResult.Api;

using Microsoft.EntityFrameworkCore;
using RResult.Api.DomainModels;

public record struct DB
{
    public static async Task<int> CallDbUpdate<V>(V record, bool success = true)
    {
        await Task.Delay(1);
        if (!success)
            throw new Exception("DB Update exception");
        return 1;
    }
}

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();
    public DbSet<Tag> Tags => Set<Tag>();
}
