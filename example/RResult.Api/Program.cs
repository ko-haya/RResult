
using MyAPI;
using RResult;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", Get);

app.Run();

static async Task<IResult> Get() =>
    await User.Find(1)
        .Map(User.AppendMeta)
        .AndThen(User.Validate)
        .AndThenAsync(User.Update)
        .AndThenAsync(User.WriteMail)
        .InspectAsync(it => Console.WriteLine($"PutLog: {it.Text}"))
        .AndThenAsync(User.SendMail)
        .MapBothAsync<IResult, User, string>(
            Ok => TypedResults.Ok($"Hello! {Ok.Name}({Ok.Id})[{Ok.Meta}]"),
            Err => TypedResults.NotFound($"Error: {Err}")
        );
