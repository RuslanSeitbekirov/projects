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
            
            
            Killer killer = new Killer("killer", 1000, new byte[] {2, 2, 4}, 100);
            // killer.setValues("killer", 1000, new byte[] {2, 2, 4});
            // killer.printValues();
            killer.Health = 100;
            killer.Lazer();

            
            Robot bot1 = new Robot("bot");
            bot1.Width = -100;
            System.Console.WriteLine(bot1.Width);
            

            Robot.Print();


        }


    }
}