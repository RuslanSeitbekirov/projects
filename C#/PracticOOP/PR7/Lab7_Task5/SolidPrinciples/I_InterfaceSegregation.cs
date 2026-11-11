using System;

namespace SolidPrinciples.InterfaceSegregation
{
    // =========================================================================
    //  I — Interface Segregation Principle
    //      ПРИНЦИП РАЗДЕЛЕНИЯ ИНТЕРФЕЙСОВ
    //
    //  "Класс не должен зависеть от методов, которыми он не пользуется".
    //  Проще: много маленьких интерфейсов лучше одного огромного.
    //
    //  ПЛОХО — один "толстый" интерфейс на все товары:
    //
    //      interface IShopItem
    //      {
    //          void ShipByCourier();   // отправить курьером
    //          void Download();        // скачать
    //      }
    //
    //      class PaperBook : IShopItem
    //      {
    //          public void ShipByCourier() { ... }
    //          public void Download()
    //          {
    //              // бумажную книгу скачать нельзя, но метод написать ОБЯЗАНЫ
    //              throw new NotSupportedException();
    //          }
    //      }
    //
    //  Класс вынужден реализовывать метод, который ему не нужен, а тот,
    //  кто его вызовет, получит исключение прямо во время работы программы.
    //
    //  ХОРОШО: разбить интерфейс на маленькие "роли". Класс берёт только те,
    //  которые ему подходят.
    // =========================================================================

    /// <summary>Товар можно отправить курьером.</summary>
    public interface IShippable
    {
        void ShipByCourier();
    }

    /// <summary>Товар можно скачать.</summary>
    public interface IDownloadable
    {
        void Download();
    }

    /// <summary>Бумажная книга: только доставка.</summary>
    public class PaperBook : IShippable
    {
        public void ShipByCourier()
        {
            Console.WriteLine("  бумажная книга передана курьеру");
        }
    }

    /// <summary>Электронная книга: только скачивание.</summary>
    public class EBook : IDownloadable
    {
        public void Download()
        {
            Console.WriteLine("  электронная книга скачана");
        }
    }

    /// <summary>
    /// Игра продаётся и на диске, и в цифровом виде — класс реализует
    /// ОБА интерфейса (интерфейсов у класса может быть сколько угодно).
    /// </summary>
    public class VideoGame : IShippable, IDownloadable
    {
        public void ShipByCourier()
        {
            Console.WriteLine("  диск с игрой передан курьеру");
        }

        public void Download()
        {
            Console.WriteLine("  игра скачана");
        }
    }

    public static class Demo
    {
        public static void Run()
        {
            // Курьерской службе нужны только те товары, которые можно везти
            IShippable[] forCourier = { new PaperBook(), new VideoGame() };
            foreach (IShippable item in forCourier)
            {
                item.ShipByCourier();
            }

            // Серверу загрузок — только те, что можно скачать
            IDownloadable[] forDownload = { new EBook(), new VideoGame() };
            foreach (IDownloadable item in forDownload)
            {
                item.Download();
            }

            // Положить бумажную книгу в массив forDownload не даст сам
            // компилятор — ошибка видна сразу, а не во время работы программы.
        }
    }
}
