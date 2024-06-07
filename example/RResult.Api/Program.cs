using MyAPI;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", Test);

static IResult Test()
{
    return User.Find(1)
               .Map(User.AppendMeta)
               .AndThen(User.Validate)
               // TODO:
               //.AndThenAsync(User.Update)
               .Inspect(user => PutLog(user.Name))
               .AndThen(User.WriteMail)
               .AndThen(User.SendMail) // TODO: Be async
               .MapBoth<IResult>
               (
                   Ok => TypedResults.Ok($"Hello! {Ok.Name}({Ok.Id})[{Ok.Meta}]"),
                   Err => TypedResults.NotFound($"Error: {Err}")
               );
}

static void PutLog(string str)
{
    Console.WriteLine($"PutLog: {str}");
}

app.Run();
