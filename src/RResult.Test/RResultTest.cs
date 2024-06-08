namespace RResult.Test;

[TestClass]
public class RResultTest
{
    [TestMethod]
    public void TestRResult()
    {
        Assert.AreEqual(IntOk(1), 1);
        Assert.AreEqual(IntErr("error"), "error");
    }

    [TestMethod]
    public void TestErrDefaultAsOk()
    {
        // If nullable error type, default error can set 
        var resultStr = RResult<string, string>.Err(default).ToString();
        var resultStr2 = RResult<string, string>.Err("error").ToString();
        Assert.AreEqual(resultStr, "Err()");
        Assert.AreEqual(resultStr2, "Err(error)");

        // If not nullable error type, so default value will OK
        var resultInt = RResult<int, int>.Err(default).ToString();
        var resultInt2 = RResult<int, int>.Err(1).ToString();
        Assert.AreEqual(resultInt, "Ok(0)");
        Assert.AreEqual(resultInt2, "Err(1)");

        var resultBool = RResult<bool, bool>.Err(default).ToString();
        var resultBool2 = RResult<bool, bool>.Err(true).ToString();
        Assert.AreEqual(resultBool, "Ok(False)");
        Assert.AreEqual(resultBool2, "Err(True)");
    }

    [TestMethod]
    public void TestRResultUnitType()
    {
        var resultOk = RResult<RUnit, string>.Ok(default).ToString();
        Assert.AreEqual(resultOk, "Ok(())");
        // default treat as Ok
        var resultErr = RResult<RUnit, int>.Err(1).ToString();
        Assert.AreEqual(resultErr, "Err(1)");
    }

    [TestMethod]
    public void TestRResultBothSameTypes()
    {
        static RResult<string, string> GetBothTypeStr(bool success) => success switch
        {
            // This needs type declarationbecause it cannot use implicit constructor when same types.
            true => RResult<string, string>.Ok("hoge"),
            _ => RResult<string, string>.Err("fail"),
        };
        // This needs Unwrap because it cannot use implicit constructor when same name.
        Assert.AreEqual(GetBothTypeStr(true).Unwrap, "hoge");
        Assert.AreEqual(GetBothTypeStr(false).UnwrapErr, "fail");
        Assert.AreEqual(RResult<int, int>.Ok(2).Unwrap, 2);
    }

    // Can use implicit constructor when deffenet types.
    // Can use Exception for Error type
    public static RResult<int, Exception> GetEx(bool success) => success switch
    {
        true => 1,
        _ => new Exception("fail"),
    };

    [TestMethod]
    public void TestRResultException()
    {
        Assert.AreEqual(GetEx(true), 1);
        Assert.AreEqual(GetEx(false).UnwrapErr?.Message, "fail");
    }

    [TestMethod]
    public void TestUnwrapOr()
    {
        static RResult<string, string> SayOk(string x) => RResult<string, string>.Ok(x);
        static RResult<string, string> SayErr(string x) => RResult<string, string>.Err(x);

        Assert.AreEqual(SayOk("yes").UnwrapOr("no"), "yes");
        Assert.AreEqual(SayErr("yes").UnwrapOr("no"), "no");
        Assert.AreEqual(SayOk("yes").UnwrapOrElse(_ => "no"), "yes");
        Assert.AreEqual(SayErr("yes").UnwrapOrElse(v => $"expected {v}, but no"), "expected yes, but no");
    }

    static RResult<int, int> LSq(int x) => RResult<int, int>.Ok(x * x);
    static RResult<int, int> LErr(int x) => RResult<int, int>.Err(x);
    static Task<RResult<int, int>> LSqTask(int x) =>
        Task.FromResult(LSq(x));
    static Task<RResult<int, int>> LErrTask(int x) =>
        Task.FromResult(LErr(x));


    [TestMethod]
    public void TestInspectOnOk()
    {
        int counter = 0;
        var actual = LSq(2).Inspect(x => counter += x + 1);
        Assert.AreEqual(actual.ToString(), "Ok(4)");
        Assert.AreEqual(counter, 5);
    }

    [TestMethod]
    public async Task TestAsyncInspectOnOk()
    {
        int counter = 0;
        var actual = await LSqTask(2).InspectAsync(x => counter += x + 1);
        Assert.AreEqual(actual.ToString(), "Ok(4)");
        Assert.AreEqual(counter, 5);
    }

    [TestMethod]
    public void TestInspectOnErr()
    {
        int counter = 0;
        var actual = LErr(2).Inspect(x => counter += x + 1);
        Assert.AreEqual(actual.ToString(), "Err(2)");
        Assert.AreEqual(counter, 0);
    }

    [TestMethod]
    public async Task TestAsyncInspectOnErr()
    {
        int counter = 0;
        var actual = await LErrTask(2).InspectAsync(x => counter += x + 1);
        Assert.AreEqual(actual.ToString(), "Err(2)");
        Assert.AreEqual(counter, 0);
    }

    [TestMethod]
    public void TestInspectErrOnOk()
    {
        int counter = 0;
        var actual = LSq(2).InspectErr(x => counter += x + 1);
        Assert.AreEqual(actual.ToString(), "Ok(4)");
        Assert.AreEqual(counter, 0);
    }

    [TestMethod]
    public async Task TestAsyncInspectErrOnOk()
    {
        int counter = 0;
        var actual = await LSqTask(2).InspectErrAsync(x => counter += x + 1);
        Assert.AreEqual(actual.ToString(), "Ok(4)");
        Assert.AreEqual(counter, 0);
    }

    [TestMethod]
    public void TestInspectErrOnErr()
    {
        int counter = 0;
        var actual = LErr(2).InspectErr(x => counter += x + 1);
        Assert.AreEqual(actual.ToString(), "Err(2)");
        Assert.AreEqual(counter, 3);
    }

    [TestMethod]
    public async Task TestAsyncInspectErrOnErr()
    {
        int counter = 0;
        var actual = await LErrTask(2).InspectErrAsync(x => counter += x + 1);
        Assert.AreEqual(actual.ToString(), "Err(2)");
        Assert.AreEqual(counter, 3);
    }
}
