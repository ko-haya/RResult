namespace RResult.Test;

[TestClass]
public partial class OrTest
{
    [TestMethod]
    public void TestOr()
    {
        Assert.AreEqual(IntOk(1).Or(IntOk(2)).Or(IntErr("failed")), 1);
        Assert.AreEqual(IntErr("failed").Or(IntOk(3)).Or(IntOk(4)), 3);
        Assert.AreEqual(IntErr("failed").Or(IntErr("failed2")), "failed2");
    }

    [TestMethod]
    public void TestOrElse()
    {
        static RResult<int, int> LSq(int x) => RResult<int, int>.Ok(x * x);
        static RResult<int, int> LErr(int x) => RResult<int, int>.Err(x - 1);
        Assert.AreEqual(LSq(2).OrElse(LSq).OrElse(LSq).ToString(), "Ok(4)");
        Assert.AreEqual(LSq(2).OrElse(LErr).OrElse(LSq).ToString(), "Ok(4)");
        Assert.AreEqual(LErr(3).OrElse(LSq).OrElse(LErr).ToString(), "Ok(4)");
        Assert.AreEqual(LErr(3).OrElse(LErr).OrElse(LErr).ToString(), "Ok(0)");
    }
}
