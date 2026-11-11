using System;
using System.Collections.Generic;
using System.IO;

namespace SolidPrinciples.SingleResponsibility
{
    // =========================================================================
    //  S — Single Responsibility Principle
    //      ПРИНЦИП ЕДИНСТВЕННОЙ ОТВЕТСТВЕННОСТИ
    //
    //  "У класса должна быть только одна причина для изменения".
    //  Проще: один класс — одно дело.
    //
    //  ПЛОХО (так писать не надо — один класс делает три разных дела):
    //
    //      class Order
    //      {
    //          public void AddItem(...)  { ... }   // хранит товары, считает сумму
    //          public void Print()       { ... }   // печатает на экран
    //          public void SaveToFile()  { ... }   // сохраняет в файл
    //      }
    //
    //  Почему плохо: захотели поменять вид чека — правим Order; решили
    //  хранить заказы в базе данных — опять правим Order. Причин для
    //  изменения три, и каждая правка может случайно сломать остальное.
    //
    //  ХОРОШО: разделить на три класса (см. ниже).
    // =========================================================================

    /// <summary>Отвечает ТОЛЬКО за данные заказа и подсчёт суммы.</summary>
    public class Order
    {
        private readonly List<string> names = new List<string>();
        private readonly List<decimal> prices = new List<decimal>();

        public void AddItem(string name, decimal price)
        {
            names.Add(name);
            prices.Add(price);
        }

        public int Count { get { return names.Count; } }

        public string GetName(int index) { return names[index]; }

        public decimal GetPrice(int index) { return prices[index]; }

        public decimal Total()
        {
            decimal sum = 0;
            foreach (decimal price in prices)
            {
                sum += price;
            }
            return sum;
        }
    }

    /// <summary>Отвечает ТОЛЬКО за то, как заказ выглядит на экране.</summary>
    public class OrderPrinter
    {
        public void Print(Order order)
        {
            for (int i = 0; i < order.Count; i++)
            {
                Console.WriteLine("  " + order.GetName(i) + " — " + order.GetPrice(i) + " руб.");
            }
            Console.WriteLine("  ИТОГО: " + order.Total() + " руб.");
        }
    }

    /// <summary>Отвечает ТОЛЬКО за сохранение заказа в файл.</summary>
    public class OrderFileSaver
    {
        public void Save(Order order, string path)
        {
            // StreamWriter — практика 7, задание 1
            using (StreamWriter writer = new StreamWriter(path))
            {
                for (int i = 0; i < order.Count; i++)
                {
                    writer.WriteLine(order.GetName(i) + ";" + order.GetPrice(i));
                }
            }
        }
    }

    public static class Demo
    {
        public static void Run()
        {
            Order order = new Order();
            order.AddItem("Мышь", 900m);
            order.AddItem("Коврик", 300m);

            // Каждый класс делает своё дело
            new OrderPrinter().Print(order);

            string path = Path.Combine(AppContext.BaseDirectory, "order.txt");
            new OrderFileSaver().Save(order, path);
            Console.WriteLine("  Заказ сохранён в файл order.txt (рядом с программой)");

            // Теперь, чтобы поменять вид чека, правим только OrderPrinter,
            // а Order и OrderFileSaver остаются нетронутыми.
        }
    }
}
