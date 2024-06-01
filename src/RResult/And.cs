namespace RResult;

public readonly partial record struct RResult<T, E>
{
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
}