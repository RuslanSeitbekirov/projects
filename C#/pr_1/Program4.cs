using System;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic; // Библиотека для работы с файлами

namespace pr_1{
    
    class Program4{
        
        static void Main()
        {
            /* Print("Hello");
            // Print("CUM");
            // Print("ON");

            // int sum = Summa(55233333, 1212312312);
            // Print(sum.ToString());

            // byte[] nums = {5, 7, 8, 3, 5};
            // byte res1 = Summa(nums);
             Print(res1.ToString())*/;

            /* string word = "Hello";
            // word += "!";

            // System.Console.WriteLine(word.Length);
            // word = String.Concat(word, "!!");
            // System.Console.WriteLine(word);
            // System.Console.WriteLine(String.Compare(word, "Hello!!!!!!")); // сравнение строк если нехватка символов 1 если одинаковы 0 если больше -1 
            
            // string people = "Alex,Bob,John";
            // string[] names = people.Split(new char[]{','}); // разбиение по элементам
            // people = String.Join(" ", names); // соеденили элементы массива в строку через пробел 
            // System.Console.WriteLine(people);



            // System.Console.WriteLine(word.Trim()); // убирает пробелы в начале строки и в конце
            // Console.WriteLine(word.Substring(0, word.Length - 1)); // c каккого элемента начали и каким закнчили обрезать
            // Console.WriteLine(word.ToLower()); // Всё к нижнему регистру
            // Console.WriteLine(word.ToUpper()); // Всё капсом */

            // System.Console.WriteLine("Ведите текст: ");
            // string text = Console.ReadLine();
            // using (FileStream stream = new FileStream("info.txt", FileMode.OpenOrCreate)) // изоляция работы с файлами
            // {
            //     byte[] array = System.Text.Encoding.Default.GetBytes(text);

            //     stream.Write(array, 0, array.Length);
            // }

            using(FileStream stream1 = File.OpenRead("info.txt"))
            {
                byte[] array = new byte[stream1.Length];
                // stream1.Read(array, 0, array.Length);

                string textFromFile = System.Text.Encoding.Default.GetString(array);
                System.Console.WriteLine(textFromFile);
            }
        }

        /* public static void Print(string word)
        // {
        //     System.Console.WriteLine(word);
        // }

        // public static byte Summa(byte[] digits)
        // {
        //     byte summ = 0;
        //     foreach(byte el in digits) summ += el;
        //     return summ;
            
           }*/
    }
}