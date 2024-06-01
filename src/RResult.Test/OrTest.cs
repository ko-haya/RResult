namespace RResult.Test;

using static RResult.Test.TestHelper;

[TestClass]
public partial class OrTest
{
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
