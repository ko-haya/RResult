namespace myAPI;

public record struct User(int Id, string Name)
{
    public static User FindUser(int id) => new(1, "hoge");
};