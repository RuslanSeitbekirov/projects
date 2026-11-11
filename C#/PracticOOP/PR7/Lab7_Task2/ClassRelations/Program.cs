using System;
using System.Text;

namespace ClassRelations
{
    /// <summary>
    /// Практика 7, задание 2: отношения между классами.
    ///
    /// Шпаргалка, которую я составил, пока разбирался (от слабой связи к сильной):
    ///
    ///   Ассоциация — объекты знают друг о друге.        Студент — Преподаватель
    ///   Агрегация  — целое хранит части, созданные       Команда — Игроки
    ///                снаружи; части живут и без целого.
    ///   Композиция — целое само создаёт части;           Автомобиль — Двигатель
    ///                без целого частей нет.
    ///
    /// Главный вопрос, чтобы отличить агрегацию от композиции:
    /// "Кто создаёт часть и что будет с частью, если удалить целое?"
    ///
    /// На UML-диаграммах: ассоциация — простая линия, агрегация — линия
    /// с пустым ромбом у целого, композиция — с закрашенным ромбом.
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("========== 1. Ассоциация ==========");
            AssociationDemo.Run();

            Console.WriteLine();
            Console.WriteLine("========== 2. Агрегация ==========");
            AggregationDemo.Run();

            Console.WriteLine();
            Console.WriteLine("========== 3. Композиция ==========");
            CompositionDemo.Run();

            Console.WriteLine();
            Console.WriteLine("========== 4. Наследование и композиция ==========");
            InheritanceVsCompositionDemo.Run();

            if (!Console.IsInputRedirected)
            {
                Console.WriteLine();
                Console.WriteLine("Нажмите любую клавишу...");
                Console.ReadKey();
            }
        }
    }
}
