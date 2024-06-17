namespace RResult;

// UnitType
// https://learn.microsoft.com/ja-jp/dotnet/fsharp/language-reference/unit-type
public readonly record struct RUnit
{
    private static readonly RUnit _value = new();
    public static ref readonly RUnit Value => ref _value;
    public static Task<RUnit> Task { get; } = System.Threading.Tasks.Task.FromResult(_value);
    public override string ToString() => "()";
}
