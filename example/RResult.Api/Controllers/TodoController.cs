namespace RResult.Api.Controllers;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RResult.Api.DomainModels;

public readonly record struct TodoController
{
    public static async Task<Ok<List<TodoDto>>> GetAllTodos(AppDbContext db) =>
        await db.Todos
            .Select(todo => new TodoDto(todo.Name, todo.IsComplete)
        ).ToListAsync() switch
        { List<TodoDto> todoDtos => TypedResults.Ok(todoDtos) };

    public static async Task<Ok<List<TodoDto>>> GetCompleteTodos(AppDbContext db) =>
        await db.Todos.Where(t => t.IsComplete)
            .Select(todo => new TodoDto(todo.Name, todo.IsComplete)
        ).ToListAsync() switch
        { List<TodoDto> todoDtos => TypedResults.Ok(todoDtos) };

    public static async Task<Results<Ok<TodoDto>, NotFound>> GetTodo(int id, AppDbContext db) =>
        await db.Todos.FirstOrDefaultAsync(t => t.Id == id) switch
        {
            Todo todo => TypedResults.Ok(new TodoDto(todo.Name, todo.IsComplete)),
            _ => TypedResults.NotFound()
        };

    public static async Task<RResult<Todo, string>> Create(Todo todo, AppDbContext db)
    {
        // FIX: MemoryDb cannot use this
        //using var transaction = db.Database.BeginTransaction();
        var newTodo = new Todo(default, todo.Name, todo.IsComplete);
        try
        {
            await db.Todos.AddAsync(newTodo);
            await db.SaveChangesAsync();
            //transaction.Commit();
            return RResult<Todo, string>.Ok(newTodo);
        }
        catch (Exception e)
        {
            return RResult<Todo, string>.Err($"Update failed: {e.Message}");
        }
    }

    public static async Task<RResult<Todo, string>> Update(int id, TodoDto todo, AppDbContext db)
    {
        var oldTodo = await db.Todos.FirstOrDefaultAsync(t => t.Id == id);
        if (oldTodo == null) {
            return RResult<Todo,string>.Err("Todo not found");
        }
        var newTodo = new Todo(id, todo.Name, todo.IsComplete);
        // FIX: MemoryDb cannot use this
        //using var transaction = db.Database.BeginTransaction();
        try
        {
            db.Todos.Update(newTodo);
            await db.SaveChangesAsync(); // Unprocessable entity?
            //transaction.Commit();
            return RResult<Todo, string>.Ok(newTodo);
        }
        catch (Exception e)
        {
            return RResult<Todo, string>.Err($"Update failed: {e.Message}");
        }
    }

    public static async Task<Results<Created<Todo>, UnprocessableEntity>> CreateTodo(TodoDto todo, AppDbContext db) =>
        // Fix: Unusable method chainings by null possiblity.
        await Create(new Todo(default, todo.Name, todo.IsComplete), db) switch
        {
            { IsOk: true, Unwrap: var newTodo }
                when newTodo is not null => TypedResults.Created($"/todoitems/{newTodo.Id}", newTodo),
            _ => TypedResults.UnprocessableEntity()
        };

    public static async Task<Results<NoContent, NotFound, UnprocessableEntity>> UpdateTodo(int id, TodoDto inputTodo, AppDbContext db) =>
        await Update(id, inputTodo , db) switch
        {
            { IsOk: true } => TypedResults.NoContent(),
            { IsOk: false, UnwrapErr: var err}
                // TODO: Use enum
                when err == "Todo not found" => TypedResults.NotFound(),
            _ => TypedResults.UnprocessableEntity()
        };

    // TODO: To be pipelined
    public static async Task<Results<NoContent, NotFound>> DeleteTodo(int id, AppDbContext db)
    {
        if (await db.Todos.FirstOrDefaultAsync(t => t.Id == id) is Todo todo)
        {
            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        return TypedResults.NotFound();
    }
};
