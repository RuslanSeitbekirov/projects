using System;
using System.Dynamic;

namespace project
{
    
    class Program
    {
        static void Main()
        {
            Robot bot = new Robot("bot", 800, new byte[] {0, 0, 0});
            bot.printValues();
            
            
            Robot killer = new Robot();
            killer.setValues("killer", 1000, new byte[] {2, 2, 4});
            killer.printValues();

            Robot bot1 = new Robot("bot");
            bot1.Width = -100;
            System.Console.WriteLine(bot1.Width);

            Robot.Print();


        }


    }
}