namespace RResult.Test;
using RF = RResultFactory;

[TestClass]
public class FactoryTest
{
    [TestMethod]
    public void TestToRResultOr()
    {
        Assert.AreEqual(RF.Ok(2), 2);
        Assert.AreEqual(RF.Err("error"), "error");
    }
}
