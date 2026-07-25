using System;

namespace pr_1{
    
    class Program{
        
        static void Main() {

        /*   int num = Convert.ToInt32(Console.ReadLine());
           if (num == 67)
            {
                for (int i=0; i < 67; i++)
                {
                    Console.WriteLine("🤷‍♂️" + "SIIIIXXXXXSSSSEEEEEEEVEN!!!!!!!!!!!!!!" + "🤷‍♂️");
                }
            }
            else
            {
             Console.Write(num);   
             Console.Write(num);   
             Console.Write(num);   
             Console.Write(num);   
            }
           

            // uint M = 2222222222; //только положительные int
            // byte nm = 255; //только от 0 до 255
            // short s = -32768; //от -32768 до 32767
            // long l = 900000000000000000; //big
            // float f = 10000.4f;
            // double d = 3434; // для пиздецки больших чисел с точкой
            // string word = "sk;vhdsvs;lhv";
            // char c = 'f';
            // bool b = true; // как обычно
            Console.WriteLine("Нажмите чтобы закрыть теминал");
            Console.ReadKey();
        */
        /* float user_input = float.Parse(Console.ReadLine());

        // float result;
        // result = user_input + 10.2123f;
        // // result = user_input - 10.2123f;
        // // result = user_input * 10.2123f;
        // // result = user_input / 10.2123f; // просто деление
        // // result = user_input % 10.2123f; // остаток от деления

        // // result += 1f;
        // // result -= 1f;
        // // result *= 1f;
        // // result /= 1f;
        // // result %= 1f;

        // result--; // работает также как и result -= 1f;, но если записывается в функуию:Console.WriteLine("Вывод: " + result--); то вычитание произойдет после вывода
        // --result; // обратно result--;  

        // Console.WriteLine("Вывод: ");
        */ 
        

            // const int number = 23;
            // number =  232;
            // Console.WriteLine(Math.PI);
            // Console.WriteLine(Math.Abs(-20));
            // Console.WriteLine(Math.Ceiling(4.56f)); // в большую
            // Console.WriteLine(Math.Floor(4.99f)); // в меньшую
            // Console.WriteLine(Math.Round(4.99f)); // округление
            // Console.WriteLine(Math.Round(4.49f));
            // Console.WriteLine(Math.Max(4.49f, 4));
            // Console.WriteLine(Math.Min(4.49f, 4));
            // Console.WriteLine(Math.Pow(4.49f, 4)); // число, степень
            // Console.WriteLine(Math.Sqrt(4));

            System.Console.WriteLine("Введите радиус круга: ");
            double radius = Convert.ToDouble(Console.ReadLine());
            double area = Math.PI * Math.Pow(radius, 2);
            System.Console.WriteLine("Плащадь круга с радиусом {0} равна {1}", radius, area);
        } 
    }
}