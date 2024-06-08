using System.Runtime.CompilerServices;

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

    public static async Task<RResult<T, E>> InspectAsync<T, E>(
        this Task<RResult<T, E>> input,
        Action<T?> action
    )
    {
        var result = await input;
        if (result.IsOk)
            action(result.Unwrap);
        return result;
    }

    public static async Task<RResult<T, E>> InspectErrAsync<T, E>(
        this Task<RResult<T, E>> input,
        Action<E?> action
    )
    {
        var result = await input;
        if (result.IsErr)
            action(result.UnwrapErr);
        return result;
    }

    public static async Task<RResult<U, E>> MapAsync<U, T, E>(
        this Task<RResult<T, E>> input,
        Func<T?, U> transform
    ) =>
        await input switch
        {
            { IsOk: true } => RResult<U, E>.Ok(transform((await input).Unwrap)),
            _ => RResult<U, E>.Err((await input).UnwrapErr),
        };

    public static async Task<RResult<T, F>> MapErrAsync<T, E, F>(
        this Task<RResult<T?, E>> input,
        Func<E?, F> transform
    ) =>
        await input switch
        {
            { IsErr: true } => RResult<T, F>.Err(transform((await input).UnwrapErr)),
            _ => RResult<T, F>.Ok((await input).Unwrap!),
        };

    public static async Task<U> MapBothAsync<U, T, E>(
        this Task<RResult<T?, E>> input,
        Func<T?, U> success,
        Func<E?, U> failure
    ) =>
        await input switch
        {
            { IsOk: true } => success((await input).Unwrap),
            _ => failure((await input).UnwrapErr),
        };
}
