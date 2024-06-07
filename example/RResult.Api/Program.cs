using myAPI;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", Test);

static IResult Test()
{
    return User.Find(1)
               .Map(User.AppendMeta)
               .AndThen(User.Validate)
               //.AndThen(User.UpdateDb)
               //.Inspect(user => PutLog(""))
               //.AndThen(User.WriteMail)
               //.AndThen(User.SendMail)
               .MapBoth<IResult>
               (
                   Ok => TypedResults.Ok($"Hello! {Ok.Name}({Ok.Id})[{Ok.Meta}]"),
                   Err => TypedResults.NotFound($"Error: {Err}")
               );
}
app.Run();
