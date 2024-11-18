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

    public static async Task<Results<Created<Todo>, UnprocessableEntity>> CreateTodo(TodoDto todo, AppDbContext db) =>
        // Fix: Unusable method chainings by null possiblity.
        await DB.Upsert(new Todo(default, todo.Name, todo.IsComplete), db) switch
        {
            { IsOk: true, Unwrap: var newTodo }
                when newTodo is not null => TypedResults.Created($"/todoitems/{newTodo.Id}", newTodo),
            _ => TypedResults.UnprocessableEntity()
        };

    public static async Task<Results<NoContent, NotFound, UnprocessableEntity>> UpdateTodo(int id, TodoDto inputTodo, AppDbContext db) =>
        await DB.Upsert(new Todo(id, inputTodo.Name, inputTodo.IsComplete), db) switch
        {
            { IsOk: true } => TypedResults.NoContent(),
            { IsOk: false, UnwrapErr: var err}
                when err.apiErr is ApiErr.NotFound => TypedResults.NotFound(),
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
