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
app.MapGet("/todos", TodoHandler.GetAllTodos);
app.MapGet("/todos/complete", TodoHandler.GetCompleteTodos);
app.MapGet("/todos/{id}", TodoHandler.GetTodo);
app.MapPost("/todos", TodoHandler.CreateTodo);
app.MapPut("/todos/{id}", TodoHandler.UpdateTodo);
app.MapDelete("/todos/{id}", TodoHandler.DeleteTodo);

app.Run();

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
