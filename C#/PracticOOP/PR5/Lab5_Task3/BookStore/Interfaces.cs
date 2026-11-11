using System.Collections.Generic;

namespace BookStore
{
    // =====================================================================
    // АРХИТЕКТУРА ПРИЛОЖЕНИЯ ОПИСАНА ИНТЕРФЕЙСАМИ.
    // Модули общаются только через эти "контракты" и ничего не знают
    // о конкретных классах друг друга (слабая связанность).
    // =====================================================================

    /// <summary>Любой товар магазина.</summary>
    public interface IProduct
    {
        string Title { get; }
        decimal Price { get; }
    }

    /// <summary>Книга — это товар, у которого есть автор.
    /// Наследование интерфейсов: IBook расширяет IProduct.</summary>
    public interface IBook : IProduct
    {
        string Author { get; }
    }

    /// <summary>Товар, который можно скачать (электронная книга).</summary>
    public interface IDownloadable
    {
        string Download();
    }

    /// <summary>Каталог товаров.</summary>
    public interface ICatalog
    {
        void Add(IBook book);
        IEnumerable<IBook> GetAll();
        IEnumerable<IBook> FindByAuthor(string author);
    }

    /// <summary>Корзина покупателя.</summary>
    public interface ICart
    {
        void Add(IProduct product);
        decimal Total();
        IReadOnlyList<IProduct> Items { get; }
    }

    /// <summary>Способ оплаты.</summary>
    public interface IPaymentMethod
    {
        string Name { get; }
        bool Pay(decimal amount);
    }
}
