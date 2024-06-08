namespace RResult;

public static class RResultAsyncExtensions
{
    // Output: Task<RResult>
    // Input: RResult
    // Transform returns: Task<RResult>
    public static async Task<RResult<U, E>> AndThenAsync<U, T, E>(
        this RResult<T, E> input,
        Func<T?, Task<RResult<U, E>>> transform
        ) =>
        input switch
        {
            { IsOk: true } => await transform(input.Unwrap),
            _ => RResult<U, E>.Err(input.UnwrapErr),
        };

    // Output: Task<RResult>
    // Input: Task<RResult>
    // Transform returns: Task<RResult>
    public static async Task<RResult<U, E>> AndThenAsync<U, T, E>(
        this Task<RResult<T, E>> input,
        Func<T?, Task<RResult<U, E>>> transform
        ) =>
        await input switch
        {
            { IsOk: true } => await transform((await input).Unwrap),
            _ => RResult<U, E>.Err((await input).UnwrapErr),
        };

    // Output: Task<RResult>
    // Input: Task<RResult>
    // Transform returns: RResult
    public static async Task<RResult<U, E>> AndThenAsync<U, T, E>(
        this Task<RResult<T, E>> input,
        Func<T?, RResult<U, E>> transform
        ) =>
        await input switch
        {
            { IsOk: true } => await Task.FromResult(transform((await input).Unwrap)),
            _ => RResult<U, E>.Err((await input).UnwrapErr),
        };
}
