namespace RResult.Api;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

public readonly record struct TodoController
{
    public static async Task<Ok<List<Todo>>> GetAllTodos(TodoDb db) =>
        TypedResults.Ok(await db.Todos.ToListAsync());

    public static async Task<Ok<List<Todo>>> GetCompleteTodos(TodoDb db) =>
        TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).ToListAsync());

    // TODO: To be switched
    public static async Task<Results<Ok<Todo>, NotFound>> GetTodo(int id, TodoDb db) =>
        await db.Todos.FindAsync(id)
            is Todo todo
                ? TypedResults.Ok(todo)
                : TypedResults.NotFound();

    // TODO: To be pipelined
    public static async Task<Created<Todo>> CreateTodo(Todo todo, TodoDb db)
    {
        db.Todos.Add(todo);
        await db.SaveChangesAsync();
        return TypedResults.Created($"/todoitems/{todo.Id}", todo);
    }

    // TODO: To be pipelined
    public static async Task<Results<NoContent, NotFound>> UpdateTodo(int id, Todo inputTodo, TodoDb db)
    {
        var todo = await db.Todos.FindAsync(id);

        if (todo is null) return TypedResults.NotFound();

        todo.Name = inputTodo.Name;
        todo.IsComplete = inputTodo.IsComplete;

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }


    // TODO: To be pipelined
    public static async Task<Results<NoContent, NotFound>> DeleteTodo(int id, TodoDb db)
    {
        if (await db.Todos.FindAsync(id) is Todo todo)
        {
            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound();
    }
};
