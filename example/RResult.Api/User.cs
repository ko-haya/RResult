namespace myAPI;

using RResult;

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

    public static User AppendMeta(User user) => user with { Meta = "lorem ipsum" };
};