namespace RResult.Test;

public static class TestHelper
{
    public static RResult<int, string> IntOk(int v) => RResult<int, string>.Ok(v);
    public static RResult<int, string> IntErr(string e) => RResult<int, string>.Err(e);
}
