namespace MyAPI;

public enum ApiErr : ushort
{
    None = 0,
    Unknown = 1,
    NotFound = 2,
    Hoge = 3,
}

public record struct ErrT(ApiErr apiErr, string desc)
{
    public static ErrT Unkown(string desc) => new(ApiErr.Unknown, desc);
    public static ErrT NotFound(string desc) => new(ApiErr.NotFound, desc);
    public static ErrT Hoge(string desc) => new(ApiErr.Hoge, desc);
    public static ErrT None(string desc) => new(ApiErr.None, desc);
};
