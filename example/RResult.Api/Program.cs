using myAPI;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () =>
{
    var m = "Hello World!";
    var user = User.FindUser(1);
    return TypedResults.Ok($"{m} {user.Name}");
});



//static string SetM(string str)
//{
//    return $"message is: {str}";
//}


//R SendMailUser(int id) =>  // Input
//	User.find(id)
//	.AndThen(user => Validate(user))
//	.Map(user => AppendMeta(user))
//	.AndThen(user => UpdateDb(user))
//	.Inspect(user => PutLog($"saved: user is {user.id}")
//	.AndThen(user => WriteMail(user))
//	.AndThen((user, text) => SendMail(user, text))
//	.MapBoth(
//		Ok => Ok_200("Success")
//		Err => Err_400($"Failure: ${Err}")
//	) // Output

app.Run();
