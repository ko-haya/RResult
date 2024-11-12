using Microsoft.AspNetCore.Http.HttpResults;
using RResult.Api;
using RResult.Api.DomainModels;
using RResult.Api.Controllers;
using RResult.Api.Test.Helpers;

namespace RResult.Api.Test;

public class TodoInMemoryTests
{
    private readonly TodoDb context = new MockDb().CreateDbContext();

    [Fact]
    public async Task GetTodoReturnsNotFoundIfNotExists()
    {
        var result = await TodoController.GetTodo(1, context);

        Assert.IsType<NotFound>(result.Result);
        var notFoundResult = (NotFound)result.Result;
        Assert.NotNull(notFoundResult);
    }

    [Fact]
    public async Task GetTodoReturnsOk()
    {
        context.Todos.Add(new Todo(1, "Test title 2", false));
        await context.SaveChangesAsync();

        var result = await TodoController.GetTodo(1, context);

        Assert.IsType<Ok<TodoDto>>(result.Result);
        var okResult = (Ok<TodoDto>)result.Result;
        Assert.False(okResult.Value.IsComplete);
    }

    [Fact]
    public async Task GetAllReturnsTodos()
    {
        context.Todos.Add(new Todo(1, "Test title 1", false));
        context.Todos.Add(new Todo(2, "Test title 2", true));
        await context.SaveChangesAsync();

        var result = await TodoController.GetAllTodos(context);

        Assert.IsType<Ok<List<Todo>>>(result);
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
            context
        );

        Assert.IsType<Created<Todo>>(result);
        Assert.Collection(context.Todos, todo =>
        {
            Assert.Equal("Test title", todo.Name);
            Assert.False(todo.IsComplete);
        });
    }

    [Fact]
    public async Task UpdateTodo()
    {
        int targetId = 1;
        context.Todos.Add(new Todo(targetId, "Exiting test title", false));
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var updatedTodo = new TodoDto("Updated test title", true);
        var result = await TodoController.UpdateTodo(
            targetId,
            updatedTodo,
            context
        );

        Assert.IsType<NoContent>(result.Result);

        var todoInDb = await context.Todos.FindAsync(targetId);
        Assert.NotNull(todoInDb);
        Assert.Equal("Updated test title", todoInDb!.Name);
        Assert.True(todoInDb.IsComplete);
    }

    [Fact]
    public async Task DeleteTodoDeletesTodoInDatabase()
    {
        var existingTodo = new Todo(1, "Exiting test title", false);
        context.Todos.Add(existingTodo);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await TodoController.DeleteTodo(existingTodo.Id, context);

        Assert.IsType<NoContent>(result.Result);
        Assert.Empty(context.Todos);
    }
}
