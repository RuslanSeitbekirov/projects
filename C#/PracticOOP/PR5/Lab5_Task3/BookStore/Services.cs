using System;
using System.Collections.Generic;
using System.Linq;

namespace BookStore
{
    /// <summary>Каталог, хранящий книги в памяти.</summary>
    public class Catalog : ICatalog
    {
        private readonly List<IBook> books = new List<IBook>();

        public void Add(IBook book) => books.Add(book);

        public IEnumerable<IBook> GetAll() => books;

        public IEnumerable<IBook> FindByAuthor(string author) =>
            books.Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Корзина покупателя.</summary>
    public class Cart : ICart
    {
        private readonly List<IProduct> items = new List<IProduct>();

        public IReadOnlyList<IProduct> Items => items;

        public void Add(IProduct product) => items.Add(product);

        public decimal Total() => items.Sum(p => p.Price);
    }

    /// <summary>Оплата картой.</summary>
    public class CardPayment : IPaymentMethod
    {
        public string Name => "Банковская карта";
        public bool Pay(decimal amount) => amount > 0;
    }

    /// <summary>Оплата наличными (частичная реализация: сдача не считается).</summary>
    public class CashPayment : IPaymentMethod
    {
        public string Name => "Наличные";
        public bool Pay(decimal amount) => true;
    }

    /// <summary>
    /// Магазин работает ТОЛЬКО с интерфейсами. Конкретный каталог и способ
    /// оплаты передаются снаружи (внедрение зависимостей через конструктор),
    /// поэтому их легко заменить, не меняя код магазина.
    /// </summary>
    public class Store
    {
        private readonly ICatalog catalog;
        private readonly IPaymentMethod payment;

        public Store(ICatalog catalog, IPaymentMethod payment)
        {
            this.catalog = catalog;
            this.payment = payment;
        }

        public ICatalog Catalog => catalog;

        public void Checkout(ICart cart)
        {
            decimal sum = cart.Total();
            bool ok = payment.Pay(sum);
            Console.WriteLine($"Оплата {sum} руб. ({payment.Name}): {(ok ? "успешно" : "отказ")}");

            // Электронные книги после оплаты можно скачать
            foreach (IProduct item in cart.Items)
            {
                if (ok && item is IDownloadable file)
                {
                    Console.WriteLine("  " + file.Download());
                }
            }
        }
    }
}
