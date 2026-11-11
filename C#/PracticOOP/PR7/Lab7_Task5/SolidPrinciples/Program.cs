using System;
using System.Text;

namespace SolidPrinciples
{
    /// <summary>
    /// Практика 7, второе "Задание 4" в методичке (у меня — Lab7_Task5):
    /// принципы проектирования SOLID.
    ///
    /// SOLID — это пять советов, как писать классы, чтобы программу было
    /// легко менять и дополнять. Название сложено из первых букв:
    ///
    ///   S — Single Responsibility  — один класс делает одно дело
    ///   O — Open/Closed            — добавляй новое новым кодом, не правя старый
    ///   L — Liskov Substitution    — наследник не должен ломать поведение предка
    ///   I — Interface Segregation  — много маленьких интерфейсов лучше одного большого
    ///   D — Dependency Inversion   — зависеть от интерфейсов, а не от конкретных классов
    ///
    /// Каждый принцип — в своём файле. В начале файла в комментарии показан
    /// "плохой" вариант и объяснено, чем он плох, ниже — исправленный код.
    ///
    /// Ещё есть принцип KISS (Keep It Simple, Stupid — "делай проще"):
    /// не усложнять код без необходимости. Поэтому примеры здесь маленькие.
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("===== S — принцип единственной ответственности =====");
            SingleResponsibility.Demo.Run();

            Console.WriteLine();
            Console.WriteLine("===== O — принцип открытости/закрытости =====");
            OpenClosed.Demo.Run();

            Console.WriteLine();
            Console.WriteLine("===== L — принцип подстановки Лисков =====");
            Liskov.Demo.Run();

            Console.WriteLine();
            Console.WriteLine("===== I — принцип разделения интерфейсов =====");
            InterfaceSegregation.Demo.Run();

            Console.WriteLine();
            Console.WriteLine("===== D — принцип инверсии зависимостей =====");
            DependencyInversion.Demo.Run();

            if (!Console.IsInputRedirected)
            {
                Console.WriteLine();
                Console.WriteLine("Нажмите любую клавишу...");
                Console.ReadKey();
            }
        }
    }
}
