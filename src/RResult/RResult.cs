namespace RResult;

public readonly partial record struct RResult<T, E> where E : notnull
{
    private readonly T? value;
    private readonly E? error;

    private RResult(T? v, E? e) => (value, error) = (v, e);

    public static RResult<T, E> Ok(T v) => new(v, default);

    public static RResult<T, E> Err(E? e) => new(default, e);

    public static implicit operator RResult<T, E>(T v) => new(v, default);
    public static implicit operator RResult<T, E>(E e) => new(default, e);

    public readonly bool IsOk => value is not null && error is null;
    public readonly bool IsErr => !IsOk;

    public override string ToString() =>
        this switch
        {
            { IsOk: true } => $"Ok({value})",
            _ => $"Err({error})",
        };

    public readonly T? Unwrap =>
        this switch
        {
            { IsOk: true } => value,
            _ => throw new Exception("RResult: unwrap failed"),
        };

    public readonly E? UnwrapErr =>
        this switch
        {
            { IsOk: true } => throw new Exception("RResult: unwrap failed"),
            _ => error,
        };

    // TODO: Add test
    public readonly T? UnwrapTry =>
        this switch
        {
            { IsOk: true } => value,
            _ => default,
        };

    // TODO: Add test
    public readonly E? UnwrapErrTry =>
        this switch
        {
            { IsOk: true } => default,
            _ => error,
        };

    // Calls `onSuccess` if the result is `Ok`
    // Calls `onFailure` if the result is `Ok`
    // Each callback function needs must be same type as the receiver `R`
    public readonly R Match<R>(Func<T?, R> onSuccess, Func<E?, R> onFailure) =>
        this switch
        {
            { IsOk: true } => onSuccess(value),
            _ => onFailure(error),
        };
}
