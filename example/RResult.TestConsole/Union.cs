namespace RResult.TestConsole;

public abstract record Customer
{
    private Customer() { }

    public sealed record Eligible(string Id) : Customer;
    public sealed record Registered(string Id, int Age) : Customer;
    public sealed record Guest(string Id) : Customer;
};

public readonly record struct God
{
    public static decimal CalculateTotal(Customer customer, decimal spend)
    {
        var discount = customer switch
        {
            Customer.Eligible c when spend >= 100m => spend * 0.1m,
            _ => 0m
        };
        return spend - discount;
    }

    public static string PrintProfile(Customer customer)
    {
        var result = customer switch
        {
            Customer.Eligible c =>
                $"{c.Id}, {c.GetType().Name}, {God.CalculateTotal(c, 100m)}",
            Customer.Registered c =>
                $"{c.Age}, {c.Id}, {c.GetType().Name}, {God.CalculateTotal(c, 100m)}",
            Customer.Guest c =>
                $"{c.Id}, {c.GetType().Name}, {God.CalculateTotal(c, 100m)}",
            _ => ""
        };
        return result;
    }
}
