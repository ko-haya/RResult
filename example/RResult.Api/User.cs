namespace MyAPI;

using RResult;
using Persistence;

public record struct UserText(User User, string Text) { };
public record struct User(int Id, string Name, string? Meta)
{
    // shorthand
    public static RResult<User, string> Ok(User user) =>
        RResult<User, string>.Ok(user);
    public static RResult<User, string> Err(string message) =>
        RResult<User, string>.Err(message);

    public static RResult<User, string> Find(int id, bool success = true) =>
        success switch
        {
            true => Ok(new(id, "hoge", default)),
            _ => Err($"Not found: {id}") // TODO: Enum
        };

    public static RResult<User, string> Validate(User user) =>
        user switch
        {
            { Id: 666 } => Err("Id must be not 666"),
            { Meta: null } => Err("Meta must be not null"),
            _ => Ok(user),
        };

    public static async Task<RResult<User, string>> Update(User user) =>
        await Persistence.UpdateDb(user with { Meta = "Updated" });

    public static User AppendMeta(User user) => user with { Meta = "lorem ipsum" };

    public static RResult<UserText, string> WriteMail(User user)
    {
        var userText = new UserText { User = user, Text = $"Dear {user.Name}.\n\nLorem Ipsum.\n\n From XXX" };
        return RResult<UserText, string>.Ok(userText);
    }

    public static RResult<User, string> SendMail(UserText userText) => Ok(userText.User);
};
