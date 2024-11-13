namespace RResult.Api;

void Main()
{
    Customer john = new Customer.Eligible("John");
    Customer mary = new Customer.Eligible("Mary");
    Customer richard = new Customer.Registered("Richard");
    Customer sarah = new Customer.Guest("Sarah");

    (CalculateTotal(john, 100m) == 90m).Dump("john");
    (CalculateTotal(mary, 99m) == 99m).Dump("mary");
    (CalculateTotal(richard, 100m) == 100m).Dump("richard");
    (CalculateTotal(sarah, 100m) == 100m).Dump("sarah");
}

// You can define other methods, fields, classes and namespaces here
public abstract record Customer
{
    private Customer() { }

    public sealed record Eligible(string Id) : Customer;
    public sealed record Registered(string Id) : Customer;
    public sealed record Guest(string Id) : Customer;
}

public decimal CalculateTotal(Customer customer, decimal spend)
    {
        var discount = customer switch
        {
            Customer.Eligible c when spend >= 100m => spend * 0.1m,
            _ => 0m
        };
        return spend - discount;
    }
