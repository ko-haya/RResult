using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

using RResult.Api;
using RResult;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/favicon.ico", () => TypedResults.NotFound());

app.MapGet("/", () => Get(1));
app.MapGet("/{id}", Get);

string[] Summaries =
["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/weather", Results<Ok<WeatherForecast>, NotFound> () =>
    {
        return TypedResults.Ok(new WeatherForecast(
            DateTime.Now.AddDays(2),
            Random.Shared.Next(-20, 55),
            Summaries[Random.Shared.Next(Summaries.Length)]
        ));
    });

// Todos
app.MapGet("/todoitems", GetAllTodos);
app.MapGet("/todoitems/complete", GetCompleteTodos);
app.MapGet("/todoitems/{id}", GetTodo);
app.MapPost("/todoitems", CreateTodo);
app.MapPut("/todoitems/{id}", UpdateTodo);
app.MapDelete("/todoitems/{id}", DeleteTodo);

app.Run();


static async Task<IResult> GetAllTodos(TodoDb db)
{
    return TypedResults.Ok(await db.Todos.ToArrayAsync());
}

static async Task<IResult> GetCompleteTodos(TodoDb db)
{
    return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).ToListAsync());
}

static async Task<IResult> GetTodo(int id, TodoDb db)
{
    return await db.Todos.FindAsync(id)
        is Todo todo
            ? TypedResults.Ok(todo)
            : TypedResults.NotFound();
}

static async Task<IResult> UpdateTodo(int id, Todo inputTodo, TodoDb db)
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return TypedResults.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
};

static async Task<IResult> CreateTodo(Todo todo, TodoDb db)
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/todoitems/{todo.Id}", todo);
}

static async Task<IResult> DeleteTodo(int id, TodoDb db)
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
};


static async Task<Results<Ok<User>, NotFound<string>, UnprocessableEntity<string>, BadRequest<string>>> Get(int id = 1) =>
    await User.Find(id)
          .Map(User.AppendMeta)
          .AndThen(User.Validate)
          .AndThenAsync(User.Update)
          .AndThenAsync(User.WriteMail)
          .InspectAsync(it => Console.WriteLine($"PutLog: {it.Text}"))
          .AndThenAsync(User.SendMail) switch
    {
        { IsOk: true, Unwrap: var ok } => TypedResults.Ok(ok),
        { IsOk: false, UnwrapErr: var err }
            when err.apiErr is ApiErr.NotFound => TypedResults.NotFound(err.desc),
        { IsOk: false, UnwrapErr: var err }
            when err.apiErr is ApiErr.Unknown => TypedResults.UnprocessableEntity(err.desc),
        { IsOk: false, UnwrapErr: var err } => TypedResults.BadRequest(err.desc),
    };

record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
