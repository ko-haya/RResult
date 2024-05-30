using System.ComponentModel;

namespace RResult;

public record RResult<T> where T : notnull
{
    private readonly T? value;
    private readonly Exception? error;

    private RResult(T? v, Exception? e) => (value, error) = (v, e);
    public static RResult<T> Ok(T v) => new(v, null);
    public static RResult<T> Err(Exception e) => new(default, e);
    public static implicit operator RResult<T>(T v) => new(v, null);
    public static implicit operator RResult<T>(Exception e) => new(default, e);

    public bool IsOk => value is not null && error is null;
    public T? GetValue => IsOk ? value : default;
    public Exception? GetError => !IsOk ? error : default;
    public RResult<T> AsOk<RResult>() => Ok(value!);
    public RResult<T> AsError<RResult>() => Err(error!);
    public RResult<T> AndThen<RResult>(Func<T?, RResult<T>> fn) =>
        this switch
        {
            { IsOk: true } => fn(value),
            _ => Err(error!)!,
        };

    public R Match<R>(Func<T?, R> onSuccess, Func<Exception?, R> onFailure) =>
            this switch
            {
                { IsOk: true } => onSuccess(value),
                _ => onFailure(error),
            };
}
