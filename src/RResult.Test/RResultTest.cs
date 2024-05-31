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

    [TestMethod]
    public void TestRResultAnd()
    {
        var actual = RResult<int, string>.Ok(1)
                                         .And(RResult<int, string>.Ok(2));
        Assert.AreEqual(actual, 2);

        var actual2 = RResult<int, string>.Ok(1)
                                          .And(RResult<int, string>.Err("failed"));
        Assert.AreEqual(actual2, "failed");

        var actual3 = RResult<int, string>.Err("failed")
                                          .And(RResult<int, string>.Ok(1));
        Assert.AreEqual(actual2, "failed");
    }

    [TestMethod]
    public void TestRResultAndThen()
    {
        // Can transform return type.
        var actual = RResult<int, string>.Ok(1) // Ok(1)
                                         .AndThen(n => RResult<string, string>.Ok(n.ToString())) // Ok("1")
                                         .AndThen(a => RResult<int, string>.Ok(int.Parse(a!))); //  Ok(1)
        Assert.AreEqual(actual, 1);
    }

    [TestMethod]
    public void TestRResultAndThenBy()
    {
        // Can only use return type at first one.
        var actual = RResult<int, string>.Ok(300) // Ok(300)
                                         .AndThenBy(n => n + 10)// Ok(310))
                                         .AndThenBy(a => a + 1); // Ok(311))
        Assert.AreEqual(actual, 311);
    }

    [TestMethod]
    public void TestRResultOr()
    {
        var actual = RResult<int, string>.Ok(1) // 1
                                         .Or(RResult<int, string>.Ok(2)) // pass
                                         .Or(RResult<int, string>.Err("failed"));  // pass
        Assert.AreEqual(actual, 1);

        var actual2 = RResult<int, string>.Err("failed")
                                          .Or(RResult<int, string>.Ok(3))
                                          .Or(RResult<int, string>.Ok(4));  // pass
        Assert.AreEqual(actual2, 3);

        var actual3 = RResult<int, string>.Err("failed")
                                          .Or(RResult<int, string>.Err("failed2"));
        Assert.AreEqual(actual3, "failed2");
    }
}