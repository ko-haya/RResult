using myAPI;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", Test);

static IResult Test()
{
    return User.FindUser(1)
               .AndThen(User.ValidateUser)
               .MapBoth<IResult>
               (
                   Ok => TypedResults.Ok($"Hello! {Ok.Name}"),
                   Err => TypedResults.NotFound($"Error: {Err}")
               );
}
app.Run();
