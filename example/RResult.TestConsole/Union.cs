namespace RResult.TestConsole;

public abstract record Customer
{
    private Customer() { }

    public sealed record Eligible(string Id) : Customer;
    public sealed record Registered(string Id, int age) : Customer;
    public sealed record Guest(string Id) : Customer;
};

public readonly struct God
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
}
