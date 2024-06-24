using Microsoft.VisualBasic;

namespace RResult.Test;

[TestClass]
public class AsyncTest
{
    static int Multiply(int i) => i * i;
    static Task<RResult<int, string>> TaskMultiply(int n) =>
        Task.FromResult(
            RResult<int, string>.Ok(Multiply(n))
        );

    static Task<RResult<int, string>> TaskSum(int n) =>
        Task.FromResult(
            RResult<int, string>.Ok(n + n)
        );

    static async Task<RResult<int, string>> MultiplyAsync(int n) =>
        RResult<int, string>.Ok(
            await Task.FromResult(Multiply(n))
        );

    static Task<int> TaskSumIt(int input) => Task.FromResult(input + input);
    static Task<int> TaskSquareIt(int input) => Task.FromResult(input * input);

    [TestMethod]
    public async Task TestAsyncBasics()
    {
        Assert.AreEqual(await Task.FromResult(Multiply(4)), 16);
        Assert.AreEqual(await TaskMultiply(2), await MultiplyAsync(2));
    }

    [TestMethod]
    public async Task TestAsyncAndThenRecieverIsNotTask()
    {
        Assert.AreEqual(await IntOk(3).AndThenAsync(TaskMultiply), 9);
        Assert.AreEqual(await IntErr("error").AndThenAsync(TaskMultiply), "error");
    }

    //[TestMethod]
    //public async Task TestOrElseAsync()
    //{
    //    static Task<RResult<int, int>> LSq(int x) =>
    //        Task.FromResult(
    //            RResult<int, int>.Ok(x * x)
    //        );

    //    static Task<RResult<int, int>> TaskLErr(int x) =>
    //        Task.FromResult(
    //            RResult<int, int>.Err(x)
    //        );

    //    //Assert.AreEqual(await LSq(2).OrElsenAsync(LSq).OrElseAsync(LSq).Unwrap, 4);
    //    //Assert.AreEqual(await TaskLErr(3).OrElseAsync(TaskMultiply), "error");
    //}

    [TestMethod]
    public async Task TestAsyncAndThenRecieverIsTask()
    {
        Assert.AreEqual(await Task.FromResult(IntOk(4)).AndThenAsync(TaskMultiply), 16);
        Assert.AreEqual(await Task.FromResult(IntErr("error")).AndThenAsync(TaskMultiply), "error");
    }

    [TestMethod]
    public async Task TestAsyncChain()
    {
        // Async only
        var x = await Task.FromResult(IntOk(3)) // 3
                          .AndThenAsync(TaskMultiply) // 9
                          .AndThenAsync(TaskSum); // 18
        Assert.AreEqual(x, 18);

        // Mix
        var y = await IntOk(3) // 3
                      .AndThen(x => IntOk(x + x)) // 6
                      .AndThenAsync(TaskMultiply) // 36
                      .AndThenAsync(x => IntOk(x + x)); // 72
        Assert.AreEqual(y, 72);
    }

    [TestMethod]
    public async Task TestDAsyncChain()
    {
        // Async
        var x = await Task.FromResult(3)
                          .DAndThenAsync(TaskSumIt)
                          .DAndThenAsync(TaskSquareIt);
        Assert.AreEqual(x, 36);

        // Sync
        var y = 3
                .DAndThen(x => x + x)
                .DAndThen(x => x * x);
        Assert.AreEqual(y, 36);

        // Mix
        var z = await 3
                      .DAndThen(x => x + x)
                      .DAndThenAsync(TaskSquareIt)
                      .DAndThenAsync(x => x + x);
        Assert.AreEqual(z, 72);
    }
}
public static class DummyExtensions
{
    // Reciever: `Task`, Returns `Task`
    public static async Task<TO> DAndThenAsync<TI, TO>(
        this Task<TI> input,
        Func<TI, Task<TO>> transform
    ) =>
        await transform(await input);

    // Reciever: `Task`, Returns `Task`, not Taskable func
    public static async Task<TO> DAndThenAsync<TI, TO>(
        this Task<TI> input,
        Func<TI, TO> transform
    ) =>
        await Task.FromResult(transform(await input));

    // Reciever: not `Task`, Returns `Task`
    public static async Task<TO> DAndThenAsync<TI, TO>(
        this TI input,
        Func<TI, Task<TO>> transform
     ) =>
            await transform(input);

    // Reciever: not `Task`, Returns not `Task`
    public static TO DAndThen<TI, TO>(
        this TI input,
        Func<TI, TO> transform
    ) =>
        transform(input);
}
