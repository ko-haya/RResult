namespace RResult;

public record struct RResult<T, E> where T : notnull where E : notnull
{
    private readonly T? value;
    private readonly E? error;

    private RResult(T? v, E? e) => (value, error) = (v, e);

    public static RResult<T, E> Ok(T v) => new(v, default);

    public static RResult<T, E> Err(E e) => new(default, e);

    public static implicit operator RResult<T, E>(T v) => new(v, default);
    public static implicit operator RResult<T, E>(E e) => new(default, e);

    public readonly bool IsOk => value is not null && error is null;
    public readonly bool IsErr => !IsOk;

    public readonly T? Unwrap => this switch
    {
        { IsOk: true } => value,
        _ => throw new NotSupportedException(),
    };

    public readonly E? UnwrapErr => this switch
    {
        { IsOk: true } => throw new Exception("unwrap failed"),
        _ => error,
    };

    public readonly RResult<T, E> AndThen(Func<T?, RResult<T, E>> fn) => this switch
    {
        { IsOk: true } => fn(value),
        _ => this,
    };

    public readonly R Match<R>(Func<T?, R> onSuccess, Func<E?, R> onFailure) => this switch
    {
        { IsOk: true } => onSuccess(value),
        _ => onFailure(error),
    };
}
