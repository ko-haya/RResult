namespace RResult.Test;

[TestClass]
public class AndTest
{
    [TestMethod]
    public void TestAnd()
    {
        Assert.AreEqual(IntOk(1).And(IntOk(2)), 2);
        Assert.AreEqual(IntOk(1).And(IntErr("failed")), "failed");
        Assert.AreEqual(IntErr("failed").And(IntOk(1)), "failed");
    }

    [TestMethod]
    public void TestAndThen()
    {
        static RResult<string, string> StrOk(string v) => RResult<string, string>.Ok(v);
        // Happy path
        var actual_ok = IntOk(1) // 1
                        .AndThen(v => IntOk(v + 10)) // 11
                        .AndThen(v => StrOk($"{v}")); // "11"
        Assert.AreEqual(actual_ok.Unwrap, "11");

        // UnHappy path
        //  - Use Exception type error
        //  - Forces first error
        var actual_err = RResult<int, Exception>.Err(new Exception("failed at 1"))
                         .AndThen(RResult<int, Exception>.Ok) // pass!
                         .AndThen(_ => RResult<int, Exception>.Err(new Exception("failed at 2"))); // pass!
        Assert.AreEqual(actual_err.UnwrapErr!.Message, "failed at 1");

        // UnHappy path
        //  - Use string type error
        var actual_err2 = IntErr("faild at 1")
                         .AndThen(IntOk) // pass!
                         .AndThen(_ => RResult<string, string>.Err("failed at 2")); // pass!
        Assert.AreEqual(actual_err.UnwrapErr!.Message, "failed at 1");

        // UnHappy path
        //  - Thru when error occured
        var actual_err3 = IntOk(1) // Ok(1)
                         .AndThen(_ => IntErr("failed at 2"))
                         .AndThen(IntOk); // pass!
        Assert.AreEqual(actual_err3, "failed at 2");
    }
}
