namespace RResult.Test;

[TestClass]
public class RResultTest
{
    // Can use implicit constructor
    public static RResult<int, string> GetNum_Res(bool success) =>
         success switch
         {
             true => 1,
             _ => "fail",
         };

    // Can not use implicit constructor when same name

    public static RResult<int, Exception> GetEx_Res(bool success) =>
         success switch
         {
             true => 1,
             _ => new Exception("fail"),
         };

    [TestMethod]
    public void TestResult()
    {
        var resultOk = RResult<string, string>.Ok("foo").Unwrap;
        Assert.AreEqual(resultOk, "foo");
        var resultErr = RResult<string, string>.Err("error!").UnwrapErr;
        Assert.AreEqual(resultErr, "error!");
    }

    public static RResult<string, string> GetBothStr_Res(bool success) =>
        success switch
        {
            true => RResult<string, string>.Ok("hoge"),
            _ => RResult<string, string>.Err("fail"),
        };

    [TestMethod]
    public void TestResultSameParams()
    {
        var actual = GetBothStr_Res(true).Unwrap;
        Assert.AreEqual(actual, "hoge");
        var actual2 = GetBothStr_Res(false).UnwrapErr;
        Assert.AreEqual(actual2, "fail");
    }

    public static RResult<bool, string> GetVoid_Res() => true;

    [TestMethod]
    public void TestResultVoidParams()
    {
        var actual = GetVoid_Res();
        Assert.AreEqual(actual, true);
    }

    [TestMethod]
    public void TestResultException()
    {
        var actual = GetEx_Res(true).Unwrap;
        Assert.AreEqual(actual, 1);
        var actual2 = GetEx_Res(false).UnwrapErr;
        Assert.AreEqual(actual2?.Message, "fail");
    }

    [TestMethod]
    public void TestAnd()
    {
        var actual = RResult<int, string>.Ok(1)
                                            .And(RResult<int, string>.Ok(1));
        Assert.AreEqual(actual, 1);

        var actual2 = RResult<int, string>.Ok(1)
                                            .And(RResult<int, string>.Err("failed"));
        Assert.AreEqual(actual2, "failed");

        var actual3 = RResult<int, string>.Err("failed")
                                            .And(RResult<int, string>.Ok(1));
        Assert.AreEqual(actual2, "failed");
    }

    [TestMethod]
    public void TestAndThen()
    {
        var actual = RResult<int, string>.Ok(1) // Ok(1)
                       .AndThen(n => RResult<string, string>.Ok(n.ToString())) // Ok("1")
                       .AndThen(a => RResult<int, string>.Ok(int.Parse(a!))); //  Ok(1)
        Assert.AreEqual(actual, 1);
    }

    [TestMethod]
    public void TestAndThenBy()
    {
        var actual = RResult<int, string>.Ok(300) // Ok(300)
                            .AndThenBy(n => n + 10)// Ok(310))
                            .AndThenBy(a => a + 1); // Ok(311))
        Assert.AreEqual(actual, 311);
    }

    [TestMethod]
    public void TestOr()
    {
        var actual = RResult<int, string>.Ok(1) // 1
                                            .Or(RResult<int, string>.Ok(2))
                                            .Or(RResult<int, string>.Err("failed"));
        Assert.AreEqual(actual, 1);

        var actual2 = RResult<int, string>.Err("failed")
                                            .Or(RResult<int, string>.Ok(3))
                                            .Or(RResult<int, string>.Ok(4));
        Assert.AreEqual(actual2, 3);

        var actual3 = RResult<int, string>.Err("failed")
                                            .Or(RResult<int, string>.Err("failed2"));
        Assert.AreEqual(actual3, "failed2");
    }
}