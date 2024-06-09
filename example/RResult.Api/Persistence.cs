namespace Persistence;

using MyAPI; // TODO: Remove dependency
using RResult;

public struct Persistence
{
    public static async Task<int> CallDbUpdate(bool success = true)
    {
        await Task.Delay(1);
        if (!success)
            throw new Exception("DB Update exception");
        return 1;
    }

    public static async Task<RResult<V, ErrT>> UpdateDb<V>(V record)
    {
        try
        {
            await CallDbUpdate(true);
            return RResult<V, ErrT>.Ok(record);
        }
        catch (Exception e)
        {
            return RResult<V, ErrT>.Err(ErrT.Unkown($"Update failed: {e.Message}"));
        }
    }
}
