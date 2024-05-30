namespace RResult.Test;

[TestClass]
public class RResultTest
{
    public static RResult<int, string> GetNum_Res(bool success) =>
         success switch
         {
             true => 1,
             _ => "fail",
         };

    public static RResult<string, string> GetBothStr_Res(bool success) =>
        success switch
        {
            true => RResult<string, string>.Ok("hoge"),
            _ => RResult<string, string>.Err("fail"),
            // Error
            //true => "hoge",
            //_ => "fail",
        };

    public static RResult<bool, string> GetVoid_Res(bool success) =>
        success switch
        {
            true => true,
            _ => "fail",
        };

    [TestMethod]
    public void TestResult()
    {
        var resultOk = RResult<string, string>.Ok("foo").GetValue;
        Assert.AreEqual(resultOk, "foo");
        var resultErr = RResult<string, string>.Err("error!").GetError;
        Assert.AreEqual(resultErr, "error!");
    }

    [TestMethod]
    public void TestResultSameParams()
    {
        // Test (T, E) = (string, string)
        var actual = GetBothStr_Res(true).GetValue;
        Assert.AreEqual(actual, "hoge");
        var actual2 = GetBothStr_Res(false).GetError;
        Assert.AreEqual(actual2, "fail");
    }

    [TestMethod]
    public void TestResultVoidParams()
    {
        var actual = GetVoid_Res(true).GetValue;
        Assert.AreEqual(actual, true);
        var actual3 = GetVoid_Res(false).GetError;
        Assert.AreEqual(actual3, "fail");
    }

    [TestMethod]
    public void TestAndThen()
    {
        var actual = GetNum_Res(true) // 1
                        .AndThen(n => n + 10) // 11
                        .AndThen(s => s + 10) // 21
                        .GetValue;
        Assert.AreEqual(actual, 21);
    }

    [TestMethod]
    public void TestMatch()
    {
        int actual = GetNum_Res(true).Match // 1
                        (
                            n => n + 10, // 11
                            err => 0 // 0
                        );
        Assert.AreEqual(actual, 11);

        int actual2 = GetNum_Res(false).Match // 1
                        (
                            n => n + 10, // 11
                            err => 0 // 0
                        );
        Assert.AreEqual(actual2, 0);
    }
}