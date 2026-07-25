using System;
using System.Collections.Generic;

namespace pr_1{
    
    class Program3{
        
        static void Main()
        {
            // for(float i = 200; i > 10; i /= 2)
            // {
            //     System.Console.WriteLine("Element: {0}", i);
            // }
            // bool i = true;
            // while (i)
            // {
            //     int n = 0;
            //     System.Console.WriteLine("Element: {0}", i);
            //     n++;
            //     string end = Console.ReadLine();
            //     if (end == "end")
            //     {
            //         i = false;
            //     }

            // }
            // float i = 200;
            // do
            // {
            //     System.Console.WriteLine("Elem:" + i);
            // }while(i<10);

            // short[] nums = new short[10];
            // // string[] words = new string[] {"John", "bob", "Alex"};
            // Random random = new Random();
            // short sum = 0;
            // for(byte i = 0; i < nums.Length; i++)
            // {
            //     nums[i] = Convert.ToInt16(random.Next(-5, 15));
            //     System.Console.WriteLine("el:{0}", nums[i]);

            //     sum += nums[i];
            // }
            // System.Console.WriteLine(sum);

            // char[,] sym = new char[2,3]; // количесиво , равно глубине матрицы матрица с матрицами и тд.
            // sym[0,0] = 'H';
            // int[,] nums =
            // {
            //     {4, 6, 7},
            //     {4, 6, 7},
            //     {4, 6, 7},
            // };

            // nums[1, 2] = 56;

            // short[,] nums = {
            //     {4, 5, 65, 8915},
            //     {234, 564, 6544, 45},
            //     {4, 5, 622, 4544},
            //     {4121, 512, 625, 5468},
            //     {4161, 5154, 6531, 4586},
            //     };

            // foreach (short el in nums)
            // {
            //     System.Console.WriteLine(el);
            // }

            List<int> numb = new List<int> () {4, 5, 563, 2, 24,24};

            numb.Add(0);
            numb.Add(100);
            numb.Add(200);
            numb.Add(300);

            numb.Remove(100); // указываем не индекс, а элемент

            numb.Sort(); // по возрастанию
            numb.Reverse(); // Sort по убыванию
            foreach (int item in numb)

            {
                System.Console.WriteLine(item);
            }
        }
    }
}