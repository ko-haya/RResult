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

    public static RResult<string, string> GetBothTypeStr(bool success) => success switch
    {
        // This needs type declarationbecause it cannot use implicit constructor when same types.
        true => RResult<string, string>.Ok("hoge"),
        _ => RResult<string, string>.Err("fail"),
    };

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

    // Shorthands
    public static RResult<int, string> Ok(int v) => RResult<int, string>.Ok(v);
    public static RResult<string, string> Ok(string v) => RResult<string, string>.Ok(v);
    public static RResult<int, string> Err(string e) => RResult<int, string>.Err(e);

    [TestMethod]
    public void TestRResultAnd()
    {
        var actual = Ok(1)
                     .And(Ok(2));
        Assert.AreEqual(actual, 2);

        var actual2 = Ok(1)
                      .And(Err("failed"));
        Assert.AreEqual(actual2, "failed");

        var actual3 = Err("failed")
                      .And(Ok(1));
        Assert.AreEqual(actual3, "failed");
    }

    [TestMethod]
    public void TestRResultAndThen()
    {
        // Can transform return type.
        var actual = Ok(1) // Ok(1)
                     .AndThen(n => Ok(n.ToString())) // Ok("1")
                     .AndThen(a => Ok(int.Parse(a!))); //  Ok(1)
        Assert.AreEqual(actual, 1);
    }

    [TestMethod]
    public void TestRResultAndThenBy()
    {
        // Can only use return type at first one.
        var actual = Ok(300) // Ok(300)
                     .AndThenBy(n => n + 10)// Ok(310))
                     .AndThenBy(a => a + 1); // Ok(311))
        Assert.AreEqual(actual, 311);
    }

    [TestMethod]
    public void TestRResultOr()
    {
        var actual = Ok(1) // 1
                     .Or(Ok(2)) // pass
                     .Or(Err("failed"));  // pass
        Assert.AreEqual(actual, 1);

        var actual2 = Err("failed")
                      .Or(Ok(3))
                      .Or(Ok(4));  // pass
        Assert.AreEqual(actual2, 3);

        var actual3 = Err("failed")
                      .Or(Err("failed2"));
        Assert.AreEqual(actual3, "failed2");
    }
}