using System;

namespace pr_1{
    
    class Program5{
        
        static void Main()
        {
            // try
            // {
            //     int num = Convert.ToInt32(Console.ReadLine());
            //     System.Console.WriteLine(num);
            // }
            // catch (FormatException)
            // {
            //     System.Console.WriteLine("Uncorrect format!");
            // }
            
            // int a = 5, b = 0, res;
            // res = a/b;

            try
            {
                int a = 5, b = 0, res;
                Console.Write("Write first num:");
                a = Convert.ToInt32(Console.ReadLine());
                System.Console.Write("Write second num:");
                b = Convert.ToInt32(Console.ReadLine());
                res = a/b;
                System.Console.WriteLine("Result: " + res);
            }
            catch (System.DivideByZeroException)
            {
                
                System.Console.WriteLine("You can't divide num on zero!!");
            }
            catch (FormatException)
            {
                System.Console.WriteLine("Uncorrect format!");
            }
            finally
            {
                System.Console.WriteLine("I go kakat!");
            }


        }
    }
}