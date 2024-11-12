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

        Assert.IsType<Ok<Todo>>(result.Result);
        var okResult = (Ok<Todo>)result.Result;
        Assert.NotNull(okResult.Value);
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
        var newTodo = new TodoDto("Test title", false);
        var result = await TodoController.CreateTodo(newTodo, context);

        //Assert
        Assert.IsType<Created<Todo>>(result);

        Assert.NotNull(result);
        Assert.NotNull(result.Location);

        Assert.NotEmpty(context.Todos);
        Assert.Collection(context.Todos, todo =>
        {
            Assert.Equal("Test title", todo.Name);
            Assert.False(todo.IsComplete);
        });
    }

    //[Fact]
    //public async Task UpdateTodoUpdatesTodoInDatabase()
    //{
    //    //Arrange
    //    await using var context = new MockDb().CreateDbContext();

    //    context.Todos.Add(new Todo
    //    {
    //        Id = 1,
    //        Title = "Exiting test title",
    //        IsDone = false
    //    });

    //    await context.SaveChangesAsync();

    //    var updatedTodo = new Todo
    //    {
    //        Id = 1,
    //        Title = "Updated test title",
    //        IsDone = true
    //    };

    //    //Act
    //    var result = await TodoEndpointsV1.UpdateTodo(updatedTodo, context);

    //    //Assert
    //    Assert.IsType<Results<Created<Todo>, NotFound>>(result);

    //    var createdResult = (Created<Todo>)result.Result;

    //    Assert.NotNull(createdResult);
    //    Assert.NotNull(createdResult.Location);

    //    var todoInDb = await context.Todos.FindAsync(1);

    //    Assert.NotNull(todoInDb);
    //    Assert.Equal("Updated test title", todoInDb!.Title);
    //    Assert.True(todoInDb.IsDone);
    //}

    //[Fact]
    //public async Task DeleteTodoDeletesTodoInDatabase()
    //{
    //    //Arrange
    //    await using var context = new MockDb().CreateDbContext();

    //    var existingTodo = new Todo
    //    {
    //        Id = 1,
    //        Title = "Exiting test title",
    //        IsDone = false
    //    };

    //    context.Todos.Add(existingTodo);

    //    await context.SaveChangesAsync();

    //    //Act
    //    var result = await TodoEndpointsV1.DeleteTodo(existingTodo.Id, context);

    //    //Assert
    //    Assert.IsType<Results<NoContent, NotFound>>(result);

    //    var noContentResult = (NoContent)result.Result;

    //    Assert.NotNull(noContentResult);
    //    Assert.Empty(context.Todos);
    //}
}
