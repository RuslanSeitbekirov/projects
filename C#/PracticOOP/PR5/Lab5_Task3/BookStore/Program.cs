using System;
using System.Text;

namespace BookStore
{
    internal class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            // Собираем магазин из реализаций интерфейсов
            ICatalog catalog = new Catalog();
            catalog.Add(new Book("Война и мир", "Лев Толстой", 900m));
            catalog.Add(new Book("Анна Каренина", "Лев Толстой", 750m));
            catalog.Add(new EBook("Мастер и Маргарита", "Михаил Булгаков", 300m));
            catalog.Add(new EBook("Преступление и наказание", "Фёдор Достоевский", 250m));

            Store store = new Store(catalog, new CardPayment());

            Console.WriteLine("=== Каталог ===");
            foreach (IBook book in store.Catalog.GetAll())
            {
                Console.WriteLine(book);
            }

            Console.WriteLine("\n=== Поиск по автору \"Толстой\" ===");
            foreach (IBook book in store.Catalog.FindByAuthor("Толстой"))
            {
                Console.WriteLine(book);
            }

            Console.WriteLine("\n=== Корзина ===");
            ICart cart = new Cart();
            foreach (IBook book in store.Catalog.GetAll())
            {
                if (book.Price < 800m) cart.Add(book);
            }
            foreach (IProduct p in cart.Items)
            {
                Console.WriteLine(p);
            }
            Console.WriteLine($"Итого: {cart.Total()} руб.");

            Console.WriteLine("\n=== Оформление заказа ===");
            store.Checkout(cart);

            // Тот же магазин с другой оплатой — код Store не меняется
            Console.WriteLine("\n=== Тот же заказ, оплата наличными ===");
            new Store(catalog, new CashPayment()).Checkout(cart);

            if (!Console.IsInputRedirected)
            {
                Console.WriteLine("\nНажмите любую клавишу...");
                Console.ReadKey();
            }
        }
    }
}
