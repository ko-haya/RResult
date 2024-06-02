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
    public void TestRResultUnitType()
    {
        var resultOk = RResult<RUnit, string>.Ok(default).ToString();
        Assert.AreEqual(resultOk, "Ok(())");
        var resultErr = RResult<RUnit, RUnit>.Err(default).ToString();
        Assert.AreEqual(resultErr.ToString(), "Err(())");
    }

    [TestMethod]
    public void TestRResultBothSameTypes()
    {
        // This needs Unwrap because it cannot use implicit constructor when same name.
        var actual = GetBothTypeStr(true).Unwrap;
        Assert.AreEqual(actual, "hoge");
        var actual2 = GetBothTypeStr(false).UnwrapErr;
        Assert.AreEqual(actual2, "fail");
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
    public void TestRResultMatch()
    {
        // Happy path
        var actual_ok = Ok(1)
                        .Match(
                            v => TestHelper.Ok(v + 10), // 11
                            RResult<int, string>.Err
                        )
                        .Match(
                            v => Ok($"{v}"), // "11"
                            RResult<string, string>.Err
                        );
        Assert.AreEqual(actual_ok, Ok("11"));

        // Can convert Err message
        var actual_err = Err("failed at 1")
                         .Match(
                            Ok,
                            RResult<int, string>.Err
                         )
                         .Match(
                            Ok,
                            _ => RResult<int, string>.Err("failed at 3")
                         );
        Assert.AreEqual(actual_err, "failed at 3");

        var actual_err2 = Ok(1) // Ok(1)
                         .Match(
                            _ => Err("failed at 2"),
                            RResult<int, string>.Err
                         )
                         .Match(
                            Ok,
                            RResult<int, string>.Err
                         );
        Assert.AreEqual(actual_err2, "failed at 2");
    }
}
