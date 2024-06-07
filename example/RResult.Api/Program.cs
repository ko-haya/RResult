
using MyAPI;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", Get);

app.Run();

static IResult Get() =>
    User.Find(1)
        .Map(User.AppendMeta)
        .AndThen(User.Validate)
        // TODO:
        //.AndThenAsync(User.Update)
        .AndThen(User.WriteMail)
        .Inspect(it => PutLog(it.Text))
        .AndThen(User.SendMail) // TODO: Be async
        .MapBoth<IResult>
        (
            Ok => TypedResults.Ok($"Hello! {Ok.Name}({Ok.Id})[{Ok.Meta}]"),
            Err => TypedResults.NotFound($"Error: {Err}")
        );

static void PutLog(string str) =>
    Console.WriteLine($"PutLog: {str}");

