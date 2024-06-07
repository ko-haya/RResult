using myAPI;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", Test);

static IResult Test()
{
    return User.FindUser(1)
               .AndThen(User.ValidateUser)
               //.Map(User.AppendMeta)
               //.AndThen(User.UpdateDb)
               //.Inspect(user => PutLog(""))
               //.AndThen(User.WriteMail)
               //.AndThen(User.SendMail)
               .MapBoth<IResult>
               (
                   Ok => TypedResults.Ok($"Hello! {Ok.Name}({Ok.Id})"),
                   Err => TypedResults.NotFound($"Error: {Err}")
               );
}
app.Run();
