using Microsoft.EntityFrameworkCore;

using RResult.Api;
using RResult.Api.Controllers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    opt.UseInMemoryDatabase("TodoList");
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
/// Routes
app.MapGet("/favicon.ico", () => TypedResults.NotFound());
app.MapGet("/", () => SampleController.SampleGet(1));
app.MapGet("/{id}", SampleController.SampleGet);

// Todos
RouteGroupBuilder todoGroup = app.MapGroup("/todos");
todoGroup.MapGet("/", TodoController.GetAllTodos);
todoGroup.MapGet("/complete", TodoController.GetCompleteTodos);
todoGroup.MapGet("/{id}", TodoController.GetTodo);
todoGroup.MapPost("/", TodoController.CreateTodo);
todoGroup.MapPut("/{id}", TodoController.UpdateTodo);
todoGroup.MapDelete("/{id}", TodoController.DeleteTodo);

// Weather
RouteGroupBuilder weatherGroup = app.MapGroup("/weather");
weatherGroup.MapGet("/", WeatherController.CalcWeather);

app.Run();
