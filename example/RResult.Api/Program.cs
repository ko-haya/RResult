using MyApi;
using RResult;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", Get);
app.MapGet("/{id}", Get);

app.Run();

static async Task<IResult> Get(int id = 1) =>
    await User.Find(id)
          .Map(User.AppendMeta)
          .AndThen(User.Validate)
          .AndThenAsync(User.Update)
          .AndThenAsync(User.WriteMail)
          .InspectAsync(it => Console.WriteLine($"PutLog: {it.Text}"))
          .AndThenAsync(User.SendMail)
          .MapBothAsync<User, ErrT, IResult>(
              Ok => TypedResults.Ok($"Hello! {Ok.Name}({Ok.Id})[{Ok.Meta}]"),
              Err => Err switch
              {
                  { apiErr: ApiErr.NotFound } => TypedResults.NotFound(Err.desc),
                  { apiErr: ApiErr.Unknown } => TypedResults.UnprocessableEntity(Err.desc),
                  { apiErr: ApiErr.Hoge } => TypedResults.BadRequest(Err.desc),
                  _ => TypedResults.NotFound($"Error: {Err.desc}")
              }
          );
