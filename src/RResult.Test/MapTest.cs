namespace RResult.Test;

[TestClass]
public class MapTest
{
    [TestMethod]
    public void TestMap()
    {
        var actual = IntOk(3) // Ok(300)
                     .Map(a => a * a) // Ok(9))
                     .Map(n => n + 10);// Ok(19))
        Assert.AreEqual(actual, 19);

        var actual2 = IntErr("error") // Err(error)
                     .Map(n => n + 10) // pass
                     .Map(a => a + 1); // pass
        Assert.AreEqual(actual2, "error");

        // Convert type
        var actual3 = IntOk(300) // Ok(300)
                     .Map(n => $"{n}")
                     .Unwrap;
        Assert.AreEqual(actual3, "300");
    }

    [TestMethod]
    public void TestMapErr()
    {
        static string Stringify(int x) => $"error code: {x}";
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

    [TestMethod]
    public void TestRResultMapBoth()
    {
        // Happy path
        var actual_ok = RResult<int, string>.Ok(1)
                        .MapBoth(
                            success => $"returned: {success}", // use this
                            failure => $"failed: {failure}"
                        );
        Assert.AreEqual(actual_ok, "returned: 1");

        // Un Happy path
        var actual_er = RResult<int, int>.Err(1)
                        .MapBoth(
                            success => $"returned: {success}",
                            failure => $"failed: {failure}" // use this
                        );
        Assert.AreEqual(actual_er, "failed: 1");

        // Can even convert Err message
        var actual_err = IntErr("failed at 1")
                         .MapBoth(
                            IntOk, // pass
                            _ => RResult<int, string>.Err("failed at 3")
                         );
        Assert.AreEqual(actual_err, "failed at 3");

        var actual_err2 = IntOk(1) // Ok(1)
                         .MapBoth(
                            _ => IntErr("failed at 2"),
                            _ => 0 // pass
                         )
                         .MapBoth(
                            success => $"returned: {success}",
                            failure => $"failed: {failure}" // use this
                         );
        Assert.AreEqual(actual_err2, "failed: failed at 2");
    }
}