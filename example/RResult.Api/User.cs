using RResult;
namespace myAPI;

public record struct User(int Id, string Name, string? Meta)
{
    // shorthand
    public static RResult<User, string> Ok(User user) =>
        RResult<User, string>.Ok(user);
    public static RResult<User, string> Err(string message) =>
        RResult<User, string>.Err(message);

    public static RResult<User, string> FindUser(int id, bool success = true) =>
        success switch
        {
            true => Ok(new(1, "hoge", default)),
            _ => Err($"Not found: {id}") // TODO: Enum
        };

    public static RResult<User, string> ValidateUser(User user) =>
        user switch
        {
            { Id: 1 } => Ok(user),
            _ => Err($"Not valid: {user.Id}") // TODO: Enum
        };


    //public static User FindUser(int id) => new(1, "hoge");
};