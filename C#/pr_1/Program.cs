using System;

namespace pr_1{
    
    class Program{
        
        static void Main() {

           int num = Convert.ToInt32(Console.ReadLine());
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
        } 

    }
}