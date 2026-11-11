using System;

namespace DesignPatterns.Structural
{
    // =========================================================================
    //  ДЕКОРАТОР (Decorator) — структурный паттерн.
    //
    //  Задача: добавлять объекту новые возможности "на ходу", не меняя его
    //  класс и не создавая кучу наследников.
    //
    //  В магазине: к товару можно добавить подарочную упаковку, расширенную
    //  гарантию, страховку — в любом сочетании. Через наследование пришлось бы
    //  писать классы LaptopWithWrap, LaptopWithWarranty,
    //  LaptopWithWrapAndWarranty... А декораторы просто надеваются друг на друга,
    //  как матрёшки.
    //
    //  Главная хитрость: декоратор САМ реализует тот же интерфейс, что и товар,
    //  поэтому завёрнутый товар можно использовать везде, где обычный,
    //  и заворачивать ещё раз.
    //
    //  Участники:
    //    IProduct          — общий интерфейс;
    //    Laptop            — обычный товар;
    //    ProductDecorator  — базовый декоратор (хранит товар внутри);
    //    GiftWrap, ExtendedWarranty — конкретные декораторы.
    // =========================================================================

    public interface IProduct
    {
        string Name { get; }
        decimal Price { get; }
    }

    /// <summary>Обычный товар.</summary>
    public class Laptop : IProduct
    {
        public string Name { get { return "Ноутбук"; } }
        public decimal Price { get { return 60000m; } }
    }

    /// <summary>Базовый декоратор: по умолчанию ничего не меняет.</summary>
    public abstract class ProductDecorator : IProduct
    {
        // Товар, который мы "заворачиваем". Это может быть и другой декоратор!
        protected readonly IProduct product;

        protected ProductDecorator(IProduct product)
        {
            this.product = product;
        }

        // virtual: наследники переопределят и добавят своё
        public virtual string Name { get { return product.Name; } }
        public virtual decimal Price { get { return product.Price; } }
    }

    /// <summary>Подарочная упаковка: +300 руб.</summary>
    public class GiftWrap : ProductDecorator
    {
        public GiftWrap(IProduct product) : base(product) { }

        // Берём то, что вернул "внутренний" товар, и добавляем своё
        public override string Name { get { return product.Name + " + подарочная упаковка"; } }
        public override decimal Price { get { return product.Price + 300m; } }
    }

    /// <summary>Расширенная гарантия: +10% от цены.</summary>
    public class ExtendedWarranty : ProductDecorator
    {
        public ExtendedWarranty(IProduct product) : base(product) { }

        public override string Name { get { return product.Name + " + гарантия 3 года"; } }
        public override decimal Price { get { return product.Price * 1.10m; } }
    }

    public static class DecoratorDemo
    {
        private static void Print(IProduct product)
        {
            Console.WriteLine(product.Name + " = " + product.Price + " руб.");
        }

        public static void Run()
        {
            IProduct item = new Laptop();
            Print(item);

            // Заворачиваем товар в декоратор. Переменная того же типа IProduct!
            item = new ExtendedWarranty(item);
            Print(item);

            // Заворачиваем уже завёрнутый товар ещё раз
            item = new GiftWrap(item);
            Print(item);

            // Можно и в одну строку, и в любом порядке
            Print(new GiftWrap(new GiftWrap(new Laptop())));
        }
    }
}
