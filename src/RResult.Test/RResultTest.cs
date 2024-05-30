namespace RResult.Test;

[TestClass]
public class RResultTest
{
    public RResult<string, string> Pain(string str)
    {
        return RResult<string, string>.Err(str)!;
    }

    [TestMethod]
    public void TestResult()
    {
        var resultOk = RResult<string, string>.Ok("foo");
        Assert.AreEqual("foo", resultOk.GetValue);

        var resultErr = RResult<string, string>.Err("error!");
        Assert.AreEqual("error!", resultErr.GetError);
        //new RResult.Ok("foo").AndThen(Pain);
    }
}