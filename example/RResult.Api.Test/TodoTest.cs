using Microsoft.AspNetCore.Http.HttpResults;
using RResult.Api.Controllers;
using RResult.Api.DomainModels;
using RResult.Api.Test.Helpers;

namespace RResult.Api.Test;

public class TodoInMemoryTests
{
    private readonly AppDbContext db = new MockDb().CreateDbContext();

    [Fact]
    public async Task GetTodoReturnsNotFoundIfNotExists()
    {
        var result = await TodoController.GetTodo(1, db);

        Assert.IsType<NotFound>(result.Result);
        var notFoundResult = (NotFound)result.Result;
        Assert.NotNull(notFoundResult);
    }

    [Fact]
    public async Task GetTodoReturnsOk()
    {
        db.Todos.Add(new Todo(1, "Test title 2", false));
        await db.SaveChangesAsync();

        var result = await TodoController.GetTodo(1, db);

        Assert.IsType<Ok<TodoDto>>(result.Result);
        var okResult = (Ok<TodoDto>)result.Result;
        Assert.False(okResult.Value.IsComplete);
    }

    [Fact]
    public async Task GetAllReturnsTodos()
    {
        db.Todos.Add(new Todo(1, "Test title 1", false));
        db.Todos.Add(new Todo(2, "Test title 2", true));
        await db.SaveChangesAsync();

        var result = await TodoController.GetAllTodos(db);

        Assert.IsType<Ok<List<TodoDto>>>(result);
        Assert.NotNull(result.Value);
        Assert.Collection(result.Value,
            todo1 =>
                {
                    Assert.False(todo1.IsComplete);
                },
            todo2 =>
                {
                    Assert.True(todo2.IsComplete);
                });
    }

    [Fact]
    public async Task CreateTodo()
    {
        var result = await TodoController.CreateTodo(
            new TodoDto("Test title", false),
            db
        );

        Assert.IsType<Created<Todo>>(result.Result);
        Assert.Collection(db.Todos, todo =>
        {
            Assert.Equal("Test title", todo.Name);
            Assert.False(todo.IsComplete);
        });
    }

    [Fact]
    public async Task UpdateTodo()
    {
        int targetId = 1;
        db.Todos.Add(new Todo(targetId, "Exiting test title", false));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var updatedTodo = new TodoDto("Updated test title", true);
        var result = await TodoController.UpdateTodo(
            targetId,
            updatedTodo,
            db
        );

        Assert.IsType<NoContent>(result.Result);

        var todoInDb = await db.Todos.FindAsync(targetId);
        Assert.NotNull(todoInDb);
        Assert.Equal("Updated test title", todoInDb!.Name);
        Assert.True(todoInDb.IsComplete);
    }

    [Fact]
    public async Task DeleteTodoDeletesTodoInDatabase()
    {
        var existingTodo = new Todo(1, "Exiting test title", false);
        db.Todos.Add(existingTodo);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var result = await TodoController.DeleteTodo(existingTodo.Id, db);

        Assert.IsType<NoContent>(result.Result);
        Assert.Empty(db.Todos);
    }
}
