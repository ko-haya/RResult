namespace RResult;

public readonly record struct RResult<T, E>
{
    private readonly T? value;
    private readonly E? error;

    private RResult(T? v, E? e) => (value, error) = (v, e);

    public static RResult<T, E> Ok(T v) => new(v, default);

    public static RResult<T, E> Err(E? e) => new(default, e);

    public static implicit operator RResult<T, E>(T v) => new(v, default);
    public static implicit operator RResult<T, E>(E e) => new(default, e);

    public readonly bool IsOk => value is not null && IsErrDefault;
    public readonly bool IsErr => !IsOk;
    private readonly bool IsErrDefault => EqualityComparer<E>.Default.Equals(error, default);

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

    // Returns the contained[`Ok`] value or a provided option.
    public readonly T? UnwrapOr(T option) =>
        this switch
        {
            { IsOk: true } => value,
            _ => option,
        };

    // Returns the contained [`Ok`] value or computes it from a closure.
    public readonly T? UnwrapOrElse(Func<E?, T> op) =>
        this switch
        {
            { IsOk: true } => value,
            _ => op(error),
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
            { IsErr: true } => RResult<T, F>.Err(transform(error)),
            _ => RResult<T, F>.Ok(value!),
        };

    // Maps this [Result<T, E>][Result] to [U] by applying either the [success] function if this
    // result [is ok][Result.isOk], or the [failure] function if this result 
    public readonly U MapBoth<U>(Func<T?, U> success, Func<E?, U> failure) =>
        this switch
        {
            { IsOk: true } => success(value),
            _ => failure(error),
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

    // Calls `transform` if the result is [`Err`], otherwise returns the [`Ok`] value of `self`.
    public readonly RResult<T, F> OrElse<F>(Func<E?, RResult<T, F>> transform) =>
        this switch
        {
            { IsOk: true } => RResult<T, F>.Ok(value!),
            _ => transform(error),
        };

    // Calls a function with a reference to the contained value if [`Ok`].
    public readonly RResult<T, E> Inspect(Action<T?> action)
    {
        if (IsOk)
            action(value);
        return this;
    }

    // Calls a function with a reference to the contained value if [`Err`].
    public readonly RResult<T, E> InspectErr(Action<E?> action)
    {
        if (IsErr)
            action(error);
        return this;
    }
}

// Experimental
public readonly struct RResultFactory
{
    public static RResult<T, RUnit> Ok<T>(T v) => RResult<T, RUnit>.Ok(v);
    public static RResult<RUnit, E> Err<E>(E e) => RResult<RUnit, E>.Err(e);
}
