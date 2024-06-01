using System.Security.Cryptography;

namespace RResult;

public readonly record struct RResult<T, E>
// where T : notnull where E : notnull
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

    override public readonly string ToString() =>
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

    // Returns params `result` if the result is `Err`
    public readonly RResult<T, E> Or(RResult<T, E> result) =>
        this switch
        {
            { IsOk: true } => this,
            _ => result,
        };

    // TODO: Add  `OrElse`

    // Returns result if the result is Ok, otherwise returns the Err value of self
    public readonly RResult<T, E> And(RResult<T, E> result) =>
        this switch
        {
            { IsOk: true } => result,
            _ => this,
        };

    // Calls `onSuccess` function if the result is `Ok`
    // `onSuccess` must always be of the same type as the receiver
    public readonly RResult<T, E> AndThenBy(Func<T?, RResult<T, E>> onSuccess) =>
        this switch
        {
            { IsOk: true } => onSuccess(value),
            _ => this,
        };

    // Calls `onSuccess` function if the result is `Ok`
    // Can transform  T => T2, but E must be same.
    public readonly RResult<T2, E> AndThen<T2>(Func<T?, RResult<T2, E>> onSuccess) =>
        this switch
        {
            { IsOk: true } => onSuccess(value),
            _ => RResult<T2, E>.Err(error),
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

    //public readonly R? Map<R>(Func<T?, R> fn) => this switch
    //{
    //    { IsOk: true } => fn(value),
    //    _ => default,
    //};
}
