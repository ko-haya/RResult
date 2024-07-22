namespace RResult.Test;


[TestClass]
public class FactoryTest
{
    [TestMethod]
    public void TestToRResultOr()
    {

        Assert.AreEqual(RResult.Ok(2), 2);
        Assert.AreEqual(RResult.Err("error"), "error");
    }
}
