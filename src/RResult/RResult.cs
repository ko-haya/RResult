using System.Security.Cryptography;

namespace RResult;

public record struct RResult<T, E> where T : notnull where E : notnull
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

    override public readonly string ToString() => this switch
    {
        { IsOk: true } => $"Ok({value})",
        _ => $"Err({error})",
    };

    public readonly T? Unwrap => this switch
    {
        { IsOk: true } => value,
        _ => throw new Exception("unwrap failed"),
    };

    public readonly E? UnwrapErr => this switch
    {
        { IsOk: true } => throw new Exception("unwrap failed"),
        _ => error,
    };

    // Returns `result` if the result is `Err`
    public readonly RResult<T, E> Or(RResult<T, E> result) => this switch
    {
        { IsOk: true } => this,
        _ => result,
    };

    // TODO: Add  `OrElse`

    // Returns result if the result is Ok, otherwise returns the Err value of self
    public readonly RResult<T, E> And(RResult<T, E> result) => this switch
    {
        { IsOk: true } => result,
        _ => this,
    };

    // Calls `fn` if the result is `Ok`
    // `fn` must always be of the same type as the receiver
    public readonly RResult<T, E> AndThenBy(Func<T?, RResult<T, E>> fn) => this switch
    {
        { IsOk: true } => fn(value),
        _ => this,
    };

    // Calls `fn` if the result is `Ok`
    // `fn` need not always be of the same type as the receiver
    //    public readonly R AndThen<R>(Func<T?, R> fn) => this switch
    //    {
    //        { IsOk: true } => fn(value),
    //        // TODO: return self as Error
    //        _ => throw new Exception("chain error"),
    //    };
    //
    //public readonly R AndThenOrSelf<R>(Func<T?, R> fn) => this switch
    //{
    //    { IsOk: true } => fn(value),
    //    // TODO: return self as Error
    //    _ => this,
    //};

    public readonly R AndThen<R>(Func<T?, R> fn, Func<E?, R> onFailure) => this switch
    {
        { IsOk: true } => fn(value),
        _ => onFailure(error),
        // _=> Err(error),
    };


    //public inline fun<V, E, F> Result<V, E>.asErr(): Result<Nothing, F> {
    //    return this as Result<Nothing, F>


    //public readonly R? Map<R>(Func<T?, R> fn) => this switch
    //{
    //    { IsOk: true } => fn(value),
    //    _ => default,
    //};

    // Omit `Match` that hard to use
    // [TestMethod]
    // public void TestMatch()
    // {
    //     var actual = RResult<int, string>.Ok(1).Match
    //                     (
    //                         n => n + 10, // 11
    //                         err => 0 // 0
    //                     );
    //     Assert.AreEqual(actual, 11);
    //     var actual2 = RResult<int, string>.Err("1").Match
    //                     (
    //                         v => v + 10, // 11
    //                         e => 0 // 0
    //                     );
    //     Assert.AreEqual(actual2, 0);
    //     var actual3 = GetBothStr_Res(true).Match // 1
    //                     (
    //                         value => value, // 11
    //                         err => err // 0
    //                     );
    //     Assert.AreEqual(actual3, "hoge");
    // }
    //public readonly R Match<R>(Func<T?, R> onSuccess, Func<E?, R> onFailure) => this switch
    //{
    //    { IsOk: true } => onSuccess(value),
    //    _ => onFailure(error),
    //};
}
