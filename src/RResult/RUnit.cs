namespace RResult;

// UnitType
// https://learn.microsoft.com/ja-jp/dotnet/fsharp/language-reference/unit-type
public readonly struct RUnit
{
    // Solution 1: assign some value in the constructor before "really" assigning through the property setter.
    private static readonly RUnit _value = new();
    public static ref readonly RUnit Value => ref _value;
    public static Task<RUnit> Task { get; } = System.Threading.Tasks.Task.FromResult(_value);

    public static int CompareTo(RUnit other) => 0;
    public override int GetHashCode() => 0;
    public static bool Equals(RUnit other) => true;
    public override bool Equals(object? obj) => obj is RUnit;

    public static bool operator ==(RUnit first, RUnit second) => true;
    public static bool operator !=(RUnit first, RUnit second) => false;

    public override string ToString() => "()";
}
