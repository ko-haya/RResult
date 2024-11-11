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

app.MapGet("/", () => SampleGet(1));
app.MapGet("/{id}", SampleGet);

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

static async Task<Ok<List<Todo>>> GetAllTodos(TodoDb db) =>
    TypedResults.Ok(await db.Todos.ToListAsync());

static async Task<Ok<List<Todo>>> GetCompleteTodos(TodoDb db) =>
    TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).ToListAsync());

// TODO: To be switched
static async Task<Results<Ok<Todo>, NotFound>> GetTodo(int id, TodoDb db) =>
    await db.Todos.FindAsync(id)
        is Todo todo
            ? TypedResults.Ok(todo)
            : TypedResults.NotFound();

// TODO: To be pipelined
static async Task<Created<Todo>> CreateTodo(Todo todo, TodoDb db)
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return TypedResults.Created($"/todoitems/{todo.Id}", todo);
}

// TODO: To be pipelined
static async Task<Results<NoContent, NotFound>> UpdateTodo(int id, Todo inputTodo, TodoDb db)
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return TypedResults.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
};


// TODO: To be pipelined
static async Task<Results<NoContent, NotFound>> DeleteTodo(int id, TodoDb db)
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
};


static async Task<Results<Ok<User>, NotFound<string>, UnprocessableEntity<string>, BadRequest<string>>> SampleGet(int id = 1) =>
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
