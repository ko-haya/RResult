using RResult;
namespace myAPI;

public record struct User(int Id, string Name)
{
    public static RResult<User, string> FindUser(int id) =>
        RResult<User, string>.Ok(new(1, "hoge"));

    public static RResult<User, string> FindErr(int id) =>
        RResult<User, string>.Err($"Not found: {id}");

    public static RResult<User, string> ValidateUser(User user) =>
        user switch
        {
            { Id: 1 } => RResult<User, string>.Ok(user),
            _ => RResult<User, string>.Err($"Not valid: {user.Id}") // TODO: Enum
        };


    //public static User FindUser(int id) => new(1, "hoge");
};