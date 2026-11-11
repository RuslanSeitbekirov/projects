using System;
using System.Text;

namespace CollectionsDemo
{
    /// <summary>
    /// Практика 8, задание 1: коллекции.
    ///
    /// Коллекция — объект, который хранит много других объектов и умеет
    /// сам менять свой размер (в отличие от массива).
    ///
    ///   HeterogeneousDemo.cs — пункт 1: ArrayList с элементами разных типов
    ///                          и его методы (Add, Insert, Remove, Sort...);
    ///   EnumerableDemo.cs    — пункт 2: свои IEnumerable и IEnumerator
    ///                          (чтобы мой класс работал в foreach), и yield;
    ///   StackQueueDemo.cs    — пункт 3: стек и очередь.
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("========== 1. Разнородная коллекция ArrayList ==========");
            HeterogeneousDemo.Run();

            Console.WriteLine();
            Console.WriteLine("========== 2. IEnumerable и IEnumerator ==========");
            EnumerableDemo.Run();

            Console.WriteLine();
            Console.WriteLine("========== 3. Стек и очередь ==========");
            StackQueueDemo.Run();

            // Ждём клавишу, только если программа запущена в обычной консоли
            if (!Console.IsInputRedirected)
            {
                Console.WriteLine();
                Console.WriteLine("Нажмите любую клавишу...");
                Console.ReadKey();
            }
        }
    }
}
