using System.ComponentModel;

namespace RResult;

public record RResult<T, E> where T : notnull where E : notnull
{
    private readonly T? value = default;
    private readonly E? error = default;

    // Constructer
    internal RResult(T? t, E? e) => (t, e) = (value, error);
    public static implicit operator RResult<T, E>(T v) => new(v, default);
    public static implicit operator RResult<T, E>(E e) => new(default, e);
    public static RResult<T, E> Ok(T? value) => new(value, default);
    public static RResult<T, E> Err(E? e) => new(default, e);
    public bool IsOk => error is null && value is not null;
    public bool IsError => !IsOk;

    public RResult<T, E> TestMatch<RResult>(Func<T?, RResult<T, E>> onSuccess, Func<E?, RResult<T, E>> onFailure) =>
        this switch
        {
            { IsOk: true } => onSuccess(value),
            _ => onFailure(error),
        };

    public RResult<T, E> AsOk<RResult>() => Ok(value);
    public RResult<T, E> AsError<RResult>() => Err(error);

    public RResult<T, E> AndThen<RResult>(Func<T?, RResult<T, E>> fn) =>
        this switch
        {
            { IsOk: true } => fn(value),
            _ => AsError<RResult>(),
        };
}
