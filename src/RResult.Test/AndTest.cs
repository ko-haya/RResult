namespace RResult.Test;

[TestClass]
public class AndTest
{
    [TestMethod]
    public void TestAnd()
    {
        var actual = Ok(1).And(Ok(2));
        Assert.AreEqual(actual, 2);

        var actual2 = Ok(1).And(Err("failed"));
        Assert.AreEqual(actual2, "failed");

        var actual3 = Err("failed").And(Ok(1));
        Assert.AreEqual(actual3, "failed");
    }

    [TestMethod]
    public void TestAndThen()
    {
        // Happy path
        var actual_ok = Ok(1) // 1
                        .AndThen(v => Ok(v + 10)) // 11
                        .AndThen(v => Ok($"{v}")); // "11"
        Assert.AreEqual(actual_ok, Ok("11"));

        // UnHappy path
        //  - Use Exception type error
        //  - Forces first error
        var actual_err = RResult<int, Exception>.Err(new Exception("failed at 1"))
                         .AndThen(RResult<int, Exception>.Ok) // pass!
                         .AndThen(_ => RResult<int, Exception>.Err(new Exception("failed at 2"))); // pass!
        Assert.AreEqual(actual_err.UnwrapErr!.Message, "failed at 1");

        // UnHappy path
        //  - Use string type error
        var actual_err2 = Err("faild at 1")
                         .AndThen(Ok) // pass!
                         .AndThen(_ => RResult<string, string>.Err("failed at 2")); // pass!
        Assert.AreEqual(actual_err.UnwrapErr!.Message, "failed at 1");

        // UnHappy path
        //  - Thru when error occured
        var actual_err3 = Ok(1) // Ok(1)
                         .AndThen(_ => Err("failed at 2"))
                         .AndThen(Ok); // pass!
        Assert.AreEqual(actual_err3, "failed at 2");
    }

    [TestMethod]
    public void TestMap()
    {
        var actual = Ok(3) // Ok(300)
                     .Map(a => a * a) // Ok(9))
                     .Map(n => n + 10);// Ok(19))
        Assert.AreEqual(actual, 19);

        var actual2 = Err("error") // Err(error)
                     .Map(n => n + 10) // pass
                     .Map(a => a + 1); // pass
        Assert.AreEqual(actual2, "error");

        // Convert type
        var actual3 = Ok(300) // Ok(300)
                     .Map(n => $"{n}")
                     .Unwrap;
        Assert.AreEqual(actual3, "300");
    }

    public static string Stringify(int x) => $"error code: {x}";

    [TestMethod]
    public void TestMapErr()
    {
        var actual = RResult<int, int>.Ok(2) // Ok(2)
                                              .MapErr(Stringify); // pass
        Assert.AreEqual(actual.ToString(), "Ok(2)");

        var actual2 = RResult<int, int>.Err(3) // Err(3)
                     .MapErr(Stringify);
        Assert.AreEqual(actual2, "error code: 3");

        var actual3 = RResult<string, int>.Ok("ok")
                     .MapErr(n => n + 10) // pass 
                     .MapErr(a => a + 1); // pass 
        Assert.AreEqual(actual3, "ok");

        // Convert type
        var actual4 = RResult<int, int>.Err(300) // Error(300)
                     .MapErr(n => $"{n}");
        Assert.AreEqual(actual4, "300");
    }
}
