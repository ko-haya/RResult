namespace RResult.Test;

[TestClass]
public partial class OrTest
{
    [TestMethod]
    public void TestOr()
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

    [TestMethod]
    public void TestOrElse()
    {
        static RResult<int, int> LSq(int x) => RResult<int, int>.Ok(x * x);
        static RResult<int, int> LErr(int x) => RResult<int, int>.Err(x);
        Assert.AreEqual(LSq(2).OrElse(LSq).OrElse(LSq).Unwrap, 4);
        Assert.AreEqual(LSq(2).OrElse(LErr).OrElse(LSq).Unwrap, 4);
        Assert.AreEqual(LErr(3).OrElse(LSq).OrElse(LErr).Unwrap, 9);
        Assert.AreEqual(LErr(3).OrElse(LErr).OrElse(LErr).UnwrapErr, 3);
    }
}
