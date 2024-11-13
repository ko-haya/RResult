namespace RResult.TestConsole;

using RResult;

class Program
{
    public static RResult<int, Exception> GetEx_Res(bool success) =>
         success switch
         {
             true => 1,
             // _ => new Exception("panic!")
             _ => new Exception("fail"),
         };
    static void Main(string[] args)
    {
        int row = 0;

        do
        {
            if (row == 0 || row >= 25)
                ResetConsole();
            //var hoge = ErrT.Unknown("hoge");
            //Console.WriteLine($"Input: {hoge}");
            var john = new Customer.Eligible("John");
            var total = God.CalculateTotal(john, 100m);
            Console.WriteLine($"He is : {john.Id}, {john.GetType().Name}");
            Console.WriteLine($"total suspects 90m: {total}");

            var john2 = new Customer.Registered("Ally", 100);
            var total2 = God.CalculateTotal(john2, 100m);
            Console.WriteLine($"He is : {john2.Id}, {john2.age}, {john2.GetType().Name}");
            Console.WriteLine($"total suspects 90m: {total2}");


            //string? input = Console.ReadLine();

            //if (string.IsNullOrEmpty(input)) break;
            //var resultOk = RResult<string, string>.Ok("hoge");
            //var resultErr = RResult<string, Exception>.Err(new Exception("error!"));
            //RResult<RUnit, string> resultUnit = default;

            ////var actual3 = RResult<int, int>.Ok(2); // Ok(2)
            //Console.WriteLine($"value is: {resultUnit}");
            //Console.WriteLine($"value is: {resultOk.Unwrap}");
            //Console.WriteLine($"value is: {resultErr.UnwrapErr?.Message}");
            //Console.WriteLine($"Input: {input}");
            //Console.WriteLine("Begins with uppercase? " +
            //     $"{(StringLibrary.StartsWithUpper(input) ? "Yes" : "No")}");
            Console.WriteLine();
            row += 4;
        } while (true);
        //return;

        // Declare a ResetConsole local method
        void ResetConsole()
        {
            if (row > 0)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            Console.Clear();
            Console.WriteLine($"{Environment.NewLine}Press <Enter> only to exit; otherwise, enter a string and press <Enter>:{Environment.NewLine}");
            row = 3;
        }
    }
}
