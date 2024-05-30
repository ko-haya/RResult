using System.ComponentModel;

namespace RResult;

public record RResult<T, E>
{
    private readonly T? value;
    private readonly E? error;

    private RResult(T v, E e) => (value, error) = (v, e);
    public static RResult<T, E?> Ok(T v) => new(v, default);
    public static RResult<T?, E> Err(E e) => new(default, e);
    public static implicit operator RResult<T, E?>(T v) => new(v, default);
    public static implicit operator RResult<T?, E>(E e) => new(default, e);

    public bool IsOk => value is not null && error is null;
    public T? GetValue => IsOk ? value : default;
    public E? GetError => !IsOk ? error : default;
    public RResult<T, E?> AsOk<RResult>() => Ok(value!);
    public RResult<T?, E> AsError<RResult>() => Err(error!);
    public RResult<T, E> AndThen<RResult>(Func<T?, RResult<T, E>> fn) =>
        this switch
        {
            { IsOk: true } => fn(value),
            _ => Err(error!)!,
        };

    public R Match<R>(Func<T?, R> onSuccess, Func<E?, R> onFailure) =>
            this switch
            {
                { IsOk: true } => onSuccess(value),
                _ => onFailure(error),
            };
}
