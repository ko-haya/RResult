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
    public void TestAndThenBy()
    {
        // Can only use return type at first one.
        var actual = Ok(300) // Ok(300)
                     .AndThenBy(n => n + 10)// Ok(310))
                     .AndThenBy(a => a + 1); // Ok(311))
        Assert.AreEqual(actual, 311);

        var actual2 = Ok(300) // Ok(300)
                     .AndThenBy(_ => Err("error")) // Err(error))
                     .AndThenBy(n => n + 10); // pass
        Assert.AreEqual(actual2, "error");

        var actual3 = Ok(300) // Ok(300)
                     .AndThenBy(_ => Err("error")) // Err(error))
                     .Or(Ok(1)); // effective.
        Assert.AreEqual(actual3, 1);
    }
}
