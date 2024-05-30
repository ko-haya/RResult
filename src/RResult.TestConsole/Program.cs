﻿using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using RResult;
class Program
{
    static RResult<int, string> GetNum_Res(bool success)
    {
        if (!success)
        {
            return "fail";

        }
        return 1;
    }
    static void Main(string[] args)
    {
        int row = 0;

        do
        {
            if (row == 0 || row >= 25)
                ResetConsole();
            //string? input = Console.ReadLine();

            //if (string.IsNullOrEmpty(input)) break;
            var resultOk = RResult<string, string>.Ok("hoge");
            var resultErr = RResult<string, Exception>.Err(new Exception("error!"));

            int hoge = 1;
            hoge = GetNum_Res(false).Match<int>(
                     n => n + 10,
                     err => 0);
            Console.WriteLine($"value is: {resultOk.GetValue}");
            Console.WriteLine($"value is: {resultErr.GetError}");
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
