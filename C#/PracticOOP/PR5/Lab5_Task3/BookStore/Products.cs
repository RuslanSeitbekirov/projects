namespace BookStore
{
    /// <summary>
    /// Абстрактный класс: общая реализация для всех товаров.
    /// Объект абстрактного класса создать нельзя — только наследников.
    /// </summary>
    public abstract class Product : IProduct
    {
        public string Title { get; }
        public decimal Price { get; }

        protected Product(string title, decimal price)
        {
            Title = title;
            Price = price;
        }

        /// <summary>Каждый наследник обязан описать себя сам.</summary>
        public abstract string Describe();

        public override string ToString() => Describe();
    }

    /// <summary>Бумажная книга.</summary>
    public class Book : Product, IBook
    {
        public string Author { get; }

        public Book(string title, string author, decimal price) : base(title, price)
        {
            Author = author;
        }

        public override string Describe() => $"{Author} — «{Title}», {Price} руб.";
    }

    /// <summary>
    /// Электронная книга: наследует класс Book и дополнительно реализует
    /// второй интерфейс IDownloadable (множественное наследование интерфейсов).
    /// </summary>
    public class EBook : Book, IDownloadable
    {
        public EBook(string title, string author, decimal price) : base(title, author, price) { }

        // Явная реализация интерфейса: метод доступен только
        // через ссылку типа IDownloadable, а не через EBook.
        string IDownloadable.Download() => $"Файл «{Title}.pdf» скачан";

        public override string Describe() => base.Describe() + " [электронная]";
    }
}
