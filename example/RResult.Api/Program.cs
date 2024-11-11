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
app.MapGet("/", () => SampleController.SampleGet(1));
app.MapGet("/{id}", SampleController.SampleGet);

// Todos
app.MapGet("/todos", TodoController.GetAllTodos);
app.MapGet("/todos/complete", TodoController.GetCompleteTodos);
app.MapGet("/todos/{id}", TodoController.GetTodo);
app.MapPost("/todos", TodoController.CreateTodo);
app.MapPut("/todos/{id}", TodoController.UpdateTodo);
app.MapDelete("/todos/{id}", TodoController.DeleteTodo);

app.MapGet("/weather", WeatherController.CalcWeather);

app.Run();
