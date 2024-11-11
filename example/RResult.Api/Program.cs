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
app.MapGet("/", () => SampleHandler.SampleGet(1));
app.MapGet("/{id}", SampleHandler.SampleGet);

// Todos
app.MapGet("/todos", TodoHandler.GetAllTodos);
app.MapGet("/todos/complete", TodoHandler.GetCompleteTodos);
app.MapGet("/todos/{id}", TodoHandler.GetTodo);
app.MapPost("/todos", TodoHandler.CreateTodo);
app.MapPut("/todos/{id}", TodoHandler.UpdateTodo);
app.MapDelete("/todos/{id}", TodoHandler.DeleteTodo);

app.MapGet("/weather", WeatherHandler.CalcWeather);

app.Run();
