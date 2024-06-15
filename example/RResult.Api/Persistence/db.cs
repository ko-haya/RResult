namespace Persistence;

// Just simple library code that not use RResult
public struct DB
{
    public static async Task<int> CallDbUpdate<V>(V record, bool success = true)
    {
        await Task.Delay(1);
        if (!success)
            throw new Exception("DB Update exception");
        return 1;
    }
}
