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

    // TODO: Use enum error
    public static async Task<RResult<Todo, ErrT>> Upsert(Todo newTodo, AppDbContext db) =>
        newTodo.Id is not 0 ?
            await Update(newTodo, db)
            : await Create(newTodo, db);

    private static async Task<RResult<Todo, ErrT>> Create(Todo newTodo, AppDbContext db)
    {
        // FIX: MemoryDb cannot use this
        //using var transaction = db.Database.BeginTransaction();
        try
        {
            await db.Todos.AddAsync(newTodo);
            await db.SaveChangesAsync();
            //transaction.Commit();
            return Todo.Ok(newTodo);
        }
        catch (Exception e)
        {
            return Todo.Err(ErrT.Unknown($"Record create failed: {e.Message}"));
        }
    }

    private static async Task<RResult<Todo, ErrT>> Update(Todo newTodo, AppDbContext db)
    {
        var oldTodo = await db.Todos.FirstOrDefaultAsync(t => t.Id == newTodo.Id);
        if (oldTodo == null)
        {
            return Todo.Err(ErrT.NotFound($"Record not found"));
        }
        // FIX: MemoryDb cannot use this
        //using var transaction = db.Database.BeginTransaction();
        try
        {
            db.Todos.Update(newTodo);
            await db.SaveChangesAsync(); // Unprocessable entity?
            //transaction.Commit();
            return Todo.Ok(newTodo);
        }
        catch (Exception e)
        {
            return Todo.Err(ErrT.Unknown($"Record update failed: {e.Message}"));
        }
    }
}

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();
    public DbSet<Tag> Tags => Set<Tag>();
}
