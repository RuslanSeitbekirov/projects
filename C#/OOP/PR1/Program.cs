using System;
using System.Dynamic;

namespace project
{
    
    class Program
    {
        static void Main()
        {
            // Robot bot = new Robot("bot", 800, new byte[] {0, 0, 0});
            // bot.printValues();
            
            
            // Killer killer = new Killer("killer", 1000, new byte[] {2, 2, 4}, 100);
            // killer.setValues("killer", 1000, new byte[] {2, 2, 4});
            // killer.printValues();
            // killer.Health = 100;
            // killer.Lazer();

            
            // Robot bot1 = new Robot("bot");
            // bot1.Width = -100;
            // System.Console.WriteLine(bot1.Width);
            

            // Robot.Print();

            // List<Robot> robots = new List<Robot> ();
            // robots.Add(new Robot("Alex", 400, new byte[] {0, 0, 10}));
            // robots.Add(new Robot("Bov", 400, new byte[] {0, 10, 10}));
            // robots.Add(new Robot("Lox", 400, new byte[] {10, 0, 10}));
            // robots.Add(new Robot("Chmo", 400, new byte[] {10, 10, 0}));

            List<Killer> robots = new List<Killer> ();
            robots.Add(new Killer("Alex", 400, new byte[] {0, 0, 10}, 100));
            robots.Add(new Killer("Bov", 400, new byte[] {0, 10, 10}, 100));
            robots.Add(new Killer("Lox", 400, new byte[] {10, 0, 10}, 100));
            robots.Add(new Killer("Chmo", 400, new byte[] {10, 10, 0}, 100));

            // foreach (Robot obj in robots)
            // {
            //     System.Console.WriteLine(obj.Name);
            // }
            
            Robot newRobot = null;

            foreach (Killer obj in robots)
            {
                if (obj.Name == "Lox")
                {
                    newRobot = obj as Robot; // as преобразует обьект Killer в Robot
                }
                // System.Console.WriteLine(obj.Name);
                System.Console.WriteLine(obj is Robot); // is проверяет создан ли обьект на основе класса/типа данных
            }


        }


    }
}