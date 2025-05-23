namespace RResult.Api.DomainModels;

using RResult;

public record struct User(int Id, string Name, string? Meta)
{
    // shorthand
    public static RResult<User, ErrT> Ok(User user) =>
        RResult<User, ErrT>.Ok(user);
    public static RResult<User, ErrT> Err(ErrT et) =>
        RResult<User, ErrT>.Err(et);

    public static RResult<User, ErrT> Find(int id, bool success = true) =>
        success switch
        {
            true => Ok(new(id, "hoge", default)),
            _ => Err(ErrT.NotFound($"User {id} is not found"))
        };

    public static RResult<User, ErrT> Validate(User user) =>
        user switch
        {
            { Id: < 1 } => ErrT.Unknown($"Id: [{user.Id}] is out of range."),
            { Id: 6 } => ErrT.Hoge("hoge"),
            { Meta: null } => ErrT.None($"Meta: must be not null"),
            _ => Ok(user),
        };

    // TODO: Use ResultOr, RunCatching
    public static async Task<RResult<User, ErrT>> Update(User user)
    {
        var record = user with { Meta = "Updated" };
        try
        {
            await DB.CallDbUpdate(record, true);
            return RResult<User, ErrT>.Ok(record);
        }
        catch (Exception e)
        {
            return RResult<User, ErrT>.Err(ErrT.Unknown($"Update failed: {e.Message}"));
        }
    }

    public static User AppendMeta(User user) => user with { Meta = "lorem ipsum" };

    public static RResult<UserText, ErrT> WriteMail(User user) =>
        RResult<UserText, ErrT>.Ok(
            new UserText(user, $"Dear {user.Name}.\n\nLorem Ipsum.\n\n From XXX\n")
        );
    public static RResult<User, ErrT> SendMail(UserText userText) => Ok(userText.User);
};

public record struct UserText(User User, string Text) { };
