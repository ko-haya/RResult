namespace RResult;

public readonly record struct RResult<T, E>
// where E : notnull
{
    private readonly T? value;
    private readonly E? error;

    private RResult(T? v, E? e) => (value, error) = (v, e);

    public static RResult<T, E> Ok(T v) => new(v, default);

    public static RResult<T, E> Err(E? e) => new(default, e);

    public static implicit operator RResult<T, E>(T v) => new(v, default);
    public static implicit operator RResult<T, E>(E e) => new(default, e);

    public readonly bool IsOk =>
        value is not null && EqualityComparer<E>.Default.Equals(error, default);
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


    // Maps a `Result<T, E>` to `Result<U, E>` by applying a function to a
    // contained [`Ok`] value, leaving an [`Err`] value untouched.
    public readonly RResult<U, E> Map<U>(Func<T?, U> transform) =>
        this switch
        {
            { IsOk: true } => RResult<U, E>.Ok(transform(value)),
            _ => RResult<U, E>.Err(error),
        };

    // Maps a `Result<T, E>` to `Result<T, F>` by applying a function to a
    // contained [`Err`] value, leaving an [`Ok`] value untouched.
    // This function can be used to pass through a successful result while handling
    public readonly RResult<T, F> MapErr<F>(Func<E?, F> transform) =>
        this switch
        {
            { IsOk: true } => RResult<T, F>.Ok(value!),
            _ => RResult<T, F>.Err(transform(error)),
        };

    // Returns result if the result is Ok, otherwise returns the Err value of self
    public readonly RResult<T, E> And(RResult<T, E> result) =>
        this switch
        {
            { IsOk: true } => result,
            _ => this,
        };

    // Calls function if the result is `Ok`
    // Can transform  T => U, but E must be same.
    public readonly RResult<U, E> AndThen<U>(Func<T?, RResult<U, E>> transform) =>
        this switch
        {
            { IsOk: true } => transform(value),
            _ => RResult<U, E>.Err(error),
        };

    // Returns params `result` if the result is `Err`
    public readonly RResult<T, E> Or(RResult<T, E> result) =>
        this switch
        {
            { IsOk: true } => this,
            _ => result,
        };

    // TODO: Add  `OrElse`

    // TODO: Add  `OrElseBy`
}
