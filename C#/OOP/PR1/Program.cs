using System;

namespace project
{
    
    class Program
    {
        static void Main()
        {
            Robot bot = new Robot();
            bot.setValues("bot", 800, new byte[] {0, 0, 0});
            bot.printValues();
            
            
            Robot killer = new Robot();
            killer.setValues("killer", 1000, new byte[] {2, 2, 4});
            killer.printValues();

        }
    }
}