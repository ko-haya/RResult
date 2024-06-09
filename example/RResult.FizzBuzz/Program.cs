namespace RResult.FizzBuzz;

using RResult;

class Program
{
    static void Main(string[] args) =>
        Console.WriteLine(
            string.Join(",", Enumerable.Range(1, 30).Select(Pipeline))
        );

    static string Pipeline(int i) =>
        DoDivide(i, 15, "FizzBuzz")
            .OrElse(it => DoDivide(it, 3, "Fizz"))
            .OrElse(it => DoDivide(it, 5, "Buzz"))
            .MapBoth(
                Ok => $"{Ok}",
                Err => $"{Err}"
            );

    static RResult<string, int> DoDivide(int dividend, int divisor, string sig) =>
        (dividend % divisor) switch
        {
            0 => sig,
            _ => dividend,
        };
}
