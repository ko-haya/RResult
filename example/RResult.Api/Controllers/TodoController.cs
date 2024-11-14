namespace RResult.Api.Controllers;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RResult.Api.DomainModels;

public readonly record struct TodoController
{
    public static async Task<Ok<List<TodoDto>>> GetAllTodos(AppDbContext db)
    {
        var todoDtos = await db.Todos.Select(todo =>
            new TodoDto(todo.Name, todo.IsComplete)
        ).ToListAsync();
        return TypedResults.Ok(todoDtos);
    }

    public static async Task<Ok<List<TodoDto>>> GetCompleteTodos(AppDbContext db)
    {
        var todoDtos = await db.Todos.Where(t => t.IsComplete).Select(todo =>
            new TodoDto(todo.Name, todo.IsComplete)
        ).ToListAsync();
        return TypedResults.Ok(todoDtos);
    }

    public static async Task<Results<Ok<TodoDto>, NotFound>> GetTodo(int id, AppDbContext db) =>
        await db.Todos.FirstOrDefaultAsync(t => t.Id == id)
            is Todo todo
                ? TypedResults.Ok(new TodoDto(todo.Name, todo.IsComplete))
                : TypedResults.NotFound();

    // TODO: To be pipelined
    public static async Task<Created<TodoDto>> CreateTodo(TodoDto todo, AppDbContext db)
    {
        var newTodo = new Todo(0, todo.Name, todo.IsComplete);
        await db.Todos.AddAsync(newTodo);
        await db.SaveChangesAsync(); // Unprocessable entity
        return TypedResults.Created($"/todoitems/{newTodo.Id}", todo);
    }

    // TODO: To be pipelined
    public static async Task<Results<NoContent, NotFound>> UpdateTodo(int id, TodoDto inputTodo, AppDbContext db)
    {
        var todo = await db.Todos.FirstOrDefaultAsync(t => t.Id == id);
        if (todo is null) return TypedResults.NotFound();

        Todo newTodo = todo with
        {
            Name = inputTodo.Name,
            IsComplete = inputTodo.IsComplete
        };

        db.Todos.Update(newTodo);
        await db.SaveChangesAsync(); // Unprocessable entity

        return TypedResults.NoContent();
    }

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
