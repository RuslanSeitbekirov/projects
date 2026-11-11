using System;

namespace DesignPatterns.Creational
{
    // =========================================================================
    //  ФАБРИЧНЫЙ МЕТОД (Factory Method) — порождающий паттерн.
    //
    //  Задача: базовый класс знает, ЧТО делать с объектом, но не знает,
    //  объект КАКОГО именно класса нужно создать. Создание он поручает
    //  наследникам через специальный (фабричный) метод.
    //
    //  В магазине: оформление доставки всегда одинаковое (создать доставку,
    //  посчитать цену, напечатать), а вид доставки бывает разный.
    //  Чтобы добавить новый вид, пишем два новых класса и не трогаем старый код.
    //
    //  Участники:
    //    IDelivery        — "продукт" (что создаём);
    //    Courier..., Post... — конкретные продукты;
    //    DeliveryService  — "создатель" с абстрактным фабричным методом;
    //    CourierService... — конкретные создатели.
    // =========================================================================

    /// <summary>Продукт: способ доставки.</summary>
    public interface IDelivery
    {
        string Name { get; }
        decimal Cost { get; }
        int Days { get; }
    }

    public class CourierDelivery : IDelivery
    {
        public string Name { get { return "Курьер"; } }
        public decimal Cost { get { return 500m; } }
        public int Days { get { return 1; } }
    }

    public class PostDelivery : IDelivery
    {
        public string Name { get { return "Почта"; } }
        public decimal Cost { get { return 250m; } }
        public int Days { get { return 7; } }
    }

    public class PickupDelivery : IDelivery
    {
        public string Name { get { return "Самовывоз"; } }
        public decimal Cost { get { return 0m; } }
        public int Days { get { return 2; } }
    }

    /// <summary>Создатель: общий алгоритм оформления доставки.</summary>
    public abstract class DeliveryService
    {
        /// <summary>
        /// ФАБРИЧНЫЙ МЕТОД. Тела нет: какой объект создать, решает наследник.
        /// </summary>
        protected abstract IDelivery CreateDelivery();

        /// <summary>
        /// Общая логика. Здесь нет ни одного "new Courier..." — метод
        /// работает с любым видом доставки через интерфейс IDelivery.
        /// </summary>
        public void SendOrder(string product)
        {
            IDelivery delivery = CreateDelivery();   // вызов фабричного метода
            Console.WriteLine("«" + product + "» → " + delivery.Name + ": " +
                              delivery.Cost + " руб., дней: " + delivery.Days);
        }
    }

    // Конкретные создатели: каждый переопределяет только фабричный метод

    public class CourierService : DeliveryService
    {
        protected override IDelivery CreateDelivery() { return new CourierDelivery(); }
    }

    public class PostService : DeliveryService
    {
        protected override IDelivery CreateDelivery() { return new PostDelivery(); }
    }

    public class PickupService : DeliveryService
    {
        protected override IDelivery CreateDelivery() { return new PickupDelivery(); }
    }

    public static class FactoryMethodDemo
    {
        public static void Run()
        {
            // Переменная типа базового класса, а внутри — разные наследники
            DeliveryService[] services = { new CourierService(), new PostService(), new PickupService() };

            foreach (DeliveryService service in services)
            {
                // Вызов одинаковый, а доставка создаётся разная
                service.SendOrder("Ноутбук");
            }
        }
    }
}
