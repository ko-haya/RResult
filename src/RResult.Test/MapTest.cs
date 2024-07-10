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
    public async Task TestMapAsync()
    {
        var actual = await IntOk(3) // Ok(300)
                           .MapAsync(a => a * a) // Ok(9))
                           .MapAsync(n => n + 10);// Ok(19))
        Assert.AreEqual(actual, 19);

        var actual2 = await IntErr("error") // Err(error)
                            .MapAsync(n => n + 10) // pass
                            .MapAsync(a => a + 1); // pass
        Assert.AreEqual(actual2, "error");

        // Convert type
        var actual3 = await IntOk(300) // Ok(300)
                            .MapAsync(n => $"{n}");
        Assert.AreEqual(actual3.Unwrap, "300");

        var actual4 = Task.FromResult(IntOk(300)) // Ok(300)
                      .MapAsync(n => $"{n}");
        Assert.AreEqual((await actual4).Unwrap, "300");
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
    public async Task TestMapErrAsync()
    {
        static string Stringify(int x) => $"error code: {x}";

        var actual = await RResult<int, int>.Ok(2) // Ok(2)
                           .MapErrAsync(Stringify); // pass
        Assert.AreEqual(actual.ToString(), "Ok(2)");

        var actual2 = await RResult<int, int>.Err(3) // Err(3)
                            .MapErrAsync(Stringify);
        Assert.AreEqual(actual2, "error code: 3");

        var actual3 = await RResult<string, int>.Ok("ok")
                            .MapErrAsync(n => n + 10) // pass
                            .MapErrAsync(a => a + 1); // pass
        Assert.AreEqual(actual3, "ok");

        // Convert type
        var actual4 = await Task.FromResult(RResult<int, int>.Err(300)) // Error(300)
                            .MapErrAsync(n => $"{n}");
        Assert.AreEqual(actual4, "300");

        var actual5 = await Task.FromResult(RResult<int, int>.Err(300)) // Error(300)
                            .MapErrAsync(n => $"{n}");
        Assert.AreEqual(actual5, "300");
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

    [TestMethod]
    public async Task TestRResultMapBothAsync()
    {
        // Happy path
        var actual_ok = await Task.FromResult(RResult<int, string>.Ok(1))
                              .MapBothAsync(
                                  success => $"returned: {success}", // use this
                                  failure => $"failed: {failure}"
                              );
        Assert.AreEqual(actual_ok, "returned: 1");

        // Un Happy path
        var actual_er = await Task.FromResult(RResult<int, int>.Err(1))
                                  .MapBothAsync(
                                      success => $"returned: {success}",
                                      failure => $"failed: {failure}" // use this
                                  );
        Assert.AreEqual(actual_er, "failed: 1");

        // Can even convert Err message
        var actual_err = await Task.FromResult(IntErr("failed at 1"))
                         .MapBothAsync(
                            IntOk, // pass
                            _ => RResult<int, string>.Err("failed at 3")
                         );
        Assert.AreEqual(actual_err, "failed at 3");

        var actual_err2 = await Task.FromResult(IntOk(1)) // Ok(1)
                         .MapBothAsync(
                            _ => IntErr("failed at 2"),
                            _ => 0 // pass
                         )
                         .MapBothAsync(
                            success => $"returned: {success}",
                            failure => $"failed: {failure}" // use this
                         );
        Assert.AreEqual(actual_err2, "failed: failed at 2");
    }
}
