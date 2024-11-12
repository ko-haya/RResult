namespace RResult.Api.Controllers;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RResult.Api.DomainModels;

public readonly record struct TodoController
{
    public static async Task<Ok<List<Todo>>> GetAllTodos(TodoDb db) =>
        TypedResults.Ok(await db.Todos.ToListAsync());

    public static async Task<Ok<List<Todo>>> GetCompleteTodos(TodoDb db) =>
        TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).ToListAsync());

    public static async Task<Results<Ok<TodoDto>, NotFound>> GetTodo(int id, TodoDb db) =>
        await db.Todos.FirstOrDefaultAsync(t => t.Id == id)
            is Todo todo
                ? TypedResults.Ok(new TodoDto(todo.Name, todo.IsComplete))
                : TypedResults.NotFound();

    // TODO: To be pipelined
    public static async Task<Created<Todo>> CreateTodo(TodoDto todo, TodoDb db)
    {
        var newTodo = new Todo(null, todo.Name, todo.IsComplete);
        await db.Todos.AddAsync(newTodo);
        await db.SaveChangesAsync(); // Unprocessable entity
        return TypedResults.Created($"/todoitems/{newTodo.Id}", newTodo);
    }

    // TODO: To be pipelined
    public static async Task<Results<NoContent, NotFound>> UpdateTodo(int id, TodoDto inputTodo, TodoDb db)
    {
        Todo? todo = await db.Todos.FirstOrDefaultAsync(t => t.Id == id);
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
    public static async Task<Results<NoContent, NotFound>> DeleteTodo(int id, TodoDb db)
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
