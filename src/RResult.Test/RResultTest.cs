namespace RResult.Test;

[TestClass]
public class RResultTest
{
    [TestMethod]
    public void TestRResult()
    {
        var resultOk = RResult<int, string>.Ok(1);
        Assert.AreEqual(resultOk, 1);
        var resultErr = RResult<int, string>.Err("error!");
        Assert.AreEqual(resultErr, "error!");
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
        // This needs Unwrap because it cannot use implicit constructor when same name.
        var actual = GetBothTypeStr(true).Unwrap;
        Assert.AreEqual(actual, "hoge");
        var actual2 = GetBothTypeStr(false).UnwrapErr;
        Assert.AreEqual(actual2, "fail");

        var actual3 = RResult<int, int>.Ok(2); // Ok(3)
        Assert.AreEqual(actual3.Unwrap, 2);
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
        var actual = GetEx(true).Unwrap;
        Assert.AreEqual(actual, 1);
        var actual2 = GetEx(false).UnwrapErr;
        Assert.AreEqual(actual2?.Message, "fail");
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

    [TestMethod]
    public void TestInspectOnOk()
    {
        int counter = 0;
        var actual = LSq(2).Inspect(x => counter += x + 1);
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
    public void TestInspectErrOnOk()
    {
        int counter = 0;
        var actual = LSq(2).InspectErr(x => counter += x + 1);
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
}
