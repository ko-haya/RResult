namespace RResult;

public readonly partial record struct RResult<T, E>
{
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