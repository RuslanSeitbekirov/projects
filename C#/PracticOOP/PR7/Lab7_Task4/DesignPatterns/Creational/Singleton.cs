using System;

namespace DesignPatterns.Creational
{
    // =========================================================================
    //  ОДИНОЧКА (Singleton) — порождающий паттерн.
    //
    //  Задача: у класса должен быть ТОЛЬКО ОДИН объект на всю программу,
    //  и к нему нужен доступ из любого места.
    //
    //  В магазине: настройки (название, валюта) должны быть одни на всех.
    //  Если бы каждый модуль создал свои настройки, они могли бы разойтись.
    //
    //  Как сделано:
    //    1) конструктор закрытый (private) — снаружи написать new нельзя;
    //    2) единственный объект хранится в статическом поле самого класса;
    //    3) получить его можно только через статическое свойство Instance.
    // =========================================================================

    /// <summary>Настройки магазина — существуют в одном экземпляре.</summary>
    public sealed class ShopSettings   // sealed — от класса нельзя наследоваться
    {
        // Здесь лежит тот самый единственный объект (пока null — ещё не создан)
        private static ShopSettings instance;

        // "Замок" на случай, если Instance одновременно спросят два потока
        // (про lock — практика 6): без него могли бы создаться два объекта.
        private static readonly object locker = new object();

        public string ShopName { get; set; }
        public string Currency { get; set; }

        // Закрытый конструктор: new ShopSettings() снаружи не скомпилируется
        private ShopSettings()
        {
            ShopName = "КомпМаркет";
            Currency = "руб.";
            Console.WriteLine("  (конструктор ShopSettings вызван)");
        }

        /// <summary>Единственная "дверь" к объекту.</summary>
        public static ShopSettings Instance
        {
            get
            {
                lock (locker)
                {
                    // Объект создаётся только при ПЕРВОМ обращении
                    // (это называют "ленивой" инициализацией)
                    if (instance == null)
                    {
                        instance = new ShopSettings();
                    }
                    return instance;
                }
            }
        }
    }

    public static class SingletonDemo
    {
        public static void Run()
        {
            // Два раза обращаемся к Instance из "разных мест программы"
            ShopSettings first = ShopSettings.Instance;
            ShopSettings second = ShopSettings.Instance;

            // Конструктор вызвался только один раз, и обе переменные
            // указывают на один и тот же объект.
            Console.WriteLine("Это один и тот же объект: " + ReferenceEquals(first, second));

            // Меняем через одну переменную — видно через другую
            first.ShopName = "ТехноМир";
            Console.WriteLine("Название магазина через вторую переменную: " + second.ShopName);

            // Минус паттерна, про который много пишут: это по сути глобальная
            // переменная, из-за неё классы трудно тестировать по отдельности.
        }
    }
}
