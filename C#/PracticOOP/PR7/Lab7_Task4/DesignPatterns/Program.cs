using System;
using System.Text;
using DesignPatterns.Behavioral;
using DesignPatterns.Creational;
using DesignPatterns.Structural;

namespace DesignPatterns
{
    /// <summary>
    /// Практика 7, задание 4: 9 паттернов проектирования трёх типов.
    /// Все примеры — про интернет-магазин компьютерной техники.
    ///
    /// Паттерн — это не готовый код, а известное удачное РЕШЕНИЕ типовой
    /// задачи: "если у тебя такая проблема — расположи классы вот так".
    ///
    ///   Порождающие   — КАК СОЗДАВАТЬ объекты:
    ///       Singleton (Одиночка), Factory Method (Фабричный метод), Builder (Строитель)
    ///   Структурные   — КАК СОЕДИНЯТЬ классы и объекты в более крупные:
    ///       Adapter (Адаптер), Decorator (Декоратор), Facade (Фасад)
    ///   Поведенческие — КАК объекты ОБЩАЮТСЯ и делят обязанности:
    ///       Strategy (Стратегия), Observer (Наблюдатель), Command (Команда)
    ///
    /// Каждый паттерн лежит в своём файле; в начале файла написано,
    /// какую задачу он решает и кто в нём участвует.
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("################ ПОРОЖДАЮЩИЕ ################");
            Title("1. Singleton (Одиночка)");
            SingletonDemo.Run();
            Title("2. Factory Method (Фабричный метод)");
            FactoryMethodDemo.Run();
            Title("3. Builder (Строитель)");
            BuilderDemo.Run();

            Console.WriteLine();
            Console.WriteLine("################ СТРУКТУРНЫЕ ################");
            Title("4. Adapter (Адаптер)");
            AdapterDemo.Run();
            Title("5. Decorator (Декоратор)");
            DecoratorDemo.Run();
            Title("6. Facade (Фасад)");
            FacadeDemo.Run();

            Console.WriteLine();
            Console.WriteLine("################ ПОВЕДЕНЧЕСКИЕ ################");
            Title("7. Strategy (Стратегия)");
            StrategyDemo.Run();
            Title("8. Observer (Наблюдатель)");
            ObserverDemo.Run();
            Title("9. Command (Команда)");
            CommandDemo.Run();

            if (!Console.IsInputRedirected)
            {
                Console.WriteLine();
                Console.WriteLine("Нажмите любую клавишу...");
                Console.ReadKey();
            }
        }

        private static void Title(string text)
        {
            Console.WriteLine();
            Console.WriteLine("===== " + text + " =====");
        }
    }
}
