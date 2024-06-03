namespace RResult.Test;

public static class TestHelper
{
    public static RResult<int, string> Ok(int v) => RResult<int, string>.Ok(v);
    public static RResult<string, string> Ok(string v) => RResult<string, string>.Ok(v);
    public static RResult<int, string> Err(string e) => RResult<int, string>.Err(e);

    public static RResult<string, string> GetBothTypeStr(bool success) => success switch
    {
        // This needs type declarationbecause it cannot use implicit constructor when same types.
        true => RResult<string, string>.Ok("hoge"),
        _ => RResult<string, string>.Err("fail"),
    };

    public static string Stringify(int x) => $"error code: {x}";
    public static string PrintToConsole(string x) => x;

}
