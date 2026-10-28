using System;

int number = 2147483647;
System.Console.WriteLine(number*2);
// Важно: checked влияет только на арифметику, записанную прямо внутри 
// блока, а не на код вызываемых из него методов.
try
{ 
    checked
    {
        System.Console.WriteLine(number*2);
    }
}
catch (System.OverflowException ex)
{
    number = 0;
    System.Console.WriteLine("number = " + number);
    System.Console.WriteLine(ex.Message);     // "Arithmetic operation resulted in an overflow."
    System.Console.WriteLine(ex.StackTrace); 
}

for (int i = 0; i < 20; i++)
{
    System.Console.WriteLine("\n");
    System.Console.WriteLine("Hello world!");
}
