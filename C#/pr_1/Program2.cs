using System;
using System.Diagnostics;

namespace pr_1{
    
    class Program2{
        
        static void Main() {
            /*int a = 5;
            string word = "hello";
            if (a == 5)
            {
                System.Console.WriteLine("Number is 5");
            }
            if (word != "hello")
            {
                System.Console.WriteLine("Пошел нахуй, грубиян");
            }
            else
            {
                System.Console.WriteLine("hello!!!");
            }


            bool isHasCar = false;
            if(a>7)
                System.Console.WriteLine("Number > 7");
            else if(a < 5)
                Console.WriteLine("Number is < 5");
            else if(a == 4)
                Console.WriteLine("Number is 4");
            else if((a == 5 && isHasCar) || word == "hello") {
                Console.WriteLine("Number is 5");
                    if(isHasCar)
                        Console.WriteLine("User has car!");
            } else   
                Console.WriteLine("Number is unrecognized!");
            */

            /*System.Console.WriteLine("Enter name:");
            string role = System.Console.ReadLine();

            if(role == "Admin") {
                Console.Write("Enter user name: ");
                string user_name = Console.ReadLine();
                Console.Write("Enter {0} age: ", user_name);
                short age = Convert.ToInt16(Console.ReadLine());

                if(age <= 0 || age >99) {
                    Console.Write("Enter {0} age: ", user_name);
                    age = Convert.ToInt16(Console.ReadLine());
                }

                if(age <= 0 || age >99)
                {
                    System.Console.WriteLine("Error");
                } else System.Console.WriteLine("User's age is {0}", age);
                

            }
            */
        


            short user_input = Convert.ToInt16(Console.ReadLine());

            switch(user_input)
            {
                case 5:
                    System.Console.WriteLine("Number is 5");
                    break;
                case 15:
                    System.Console.WriteLine("Number is 15");
                    break;
                case 20:
                    System.Console.WriteLine("Number is 20");
                    break;
                case 25:
                    System.Console.WriteLine("Number is 25");
                    break;
                case 30:
                    System.Console.WriteLine("Number is 30");
                    break;
                default:
                    System.Console.WriteLine("Nubber is non!");
                    break;
            }

        } 
    }
}