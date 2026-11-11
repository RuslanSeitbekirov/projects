using System;
using System.Collections.Generic;

namespace DesignPatterns.Behavioral
{
    // =========================================================================
    //  НАБЛЮДАТЕЛЬ (Observer) — поведенческий паттерн.
    //
    //  Задача: когда в одном объекте что-то меняется, об этом должны
    //  автоматически узнать все, кому это интересно, — причём объект
    //  не должен знать заранее, кто именно на него "подписан".
    //
    //  В магазине: кнопка "сообщить о снижении цены". Покупатели подписываются
    //  на товар; когда цена падает, товар оповещает всех подписчиков.
    //
    //  Интересно: события C# (event) из практики 6 — это и есть паттерн
    //  "Наблюдатель", только встроенный в язык. Здесь он сделан вручную,
    //  чтобы было видно, как он устроен внутри.
    //
    //  Участники:
    //    ISubscriber  — интерфейс наблюдателя (подписчика);
    //    Customer     — конкретный наблюдатель;
    //    WatchedProduct — "издатель": хранит список подписчиков и оповещает их.
    // =========================================================================

    /// <summary>Наблюдатель: тот, кого надо оповещать.</summary>
    public interface ISubscriber
    {
        void Update(string productName, decimal newPrice);
    }

    /// <summary>Издатель: товар, за ценой которого следят.</summary>
    public class WatchedProduct
    {
        public string Name { get; }

        private decimal price;

        // Список подписчиков. Товар знает о них только то, что у них
        // есть метод Update, — больше ничего.
        private readonly List<ISubscriber> subscribers = new List<ISubscriber>();

        public WatchedProduct(string name, decimal price)
        {
            Name = name;
            this.price = price;
        }

        public void Subscribe(ISubscriber subscriber)
        {
            subscribers.Add(subscriber);
        }

        public void Unsubscribe(ISubscriber subscriber)
        {
            subscribers.Remove(subscriber);
        }

        public decimal Price
        {
            get { return price; }
            set
            {
                bool cheaper = value < price;
                price = value;

                // Оповещаем, только если цена снизилась
                if (cheaper)
                {
                    Notify();
                }
            }
        }

        /// <summary>Обходит всех подписчиков и сообщает им новость.</summary>
        private void Notify()
        {
            foreach (ISubscriber subscriber in subscribers)
            {
                subscriber.Update(Name, price);
            }
        }
    }

    /// <summary>Конкретный наблюдатель: покупатель.</summary>
    public class Customer : ISubscriber
    {
        private readonly string name;

        public Customer(string name)
        {
            this.name = name;
        }

        // Этот метод вызывает не покупатель, а товар — когда цена упала
        public void Update(string productName, decimal newPrice)
        {
            Console.WriteLine("  " + name + ", «" + productName + "» подешевел: теперь " + newPrice + " руб.");
        }
    }

    public static class ObserverDemo
    {
        public static void Run()
        {
            WatchedProduct phone = new WatchedProduct("Смартфон", 30000m);
            Customer anna = new Customer("Анна");
            Customer oleg = new Customer("Олег");

            phone.Subscribe(anna);
            phone.Subscribe(oleg);

            Console.WriteLine("Цена снижена до 27000:");
            phone.Price = 27000m;       // оба получат сообщение

            Console.WriteLine("Цена выросла до 29000:");
            phone.Price = 29000m;       // никто не оповещается
            Console.WriteLine("  (сообщений нет)");

            phone.Unsubscribe(oleg);    // Олег отписался

            Console.WriteLine("Цена снижена до 25000:");
            phone.Price = 25000m;       // сообщение получит только Анна
        }
    }
}
