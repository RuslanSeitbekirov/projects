using System;

namespace DesignPatterns.Behavioral
{
    // =========================================================================
    //  СТРАТЕГИЯ (Strategy) — поведенческий паттерн.
    //
    //  Задача: есть несколько способов сделать одно и то же (несколько
    //  алгоритмов), и нужно легко переключаться между ними.
    //
    //  В магазине: скидку можно считать по-разному — без скидки, процентом,
    //  по промокоду. Без паттерна в методе расчёта был бы длинный
    //  if / else if / else, который пришлось бы править ради каждой новой скидки.
    //
    //  Каждый алгоритм выносится в отдельный класс с общим интерфейсом.
    //
    //  Участники:
    //    IDiscountStrategy — интерфейс стратегии;
    //    NoDiscount, PercentDiscount, PromoCodeDiscount — конкретные стратегии;
    //    PriceCalculator   — "контекст": пользуется стратегией, не зная, какая она.
    // =========================================================================

    public interface IDiscountStrategy
    {
        string Title { get; }

        /// <summary>Получает цену, возвращает цену со скидкой.</summary>
        decimal Apply(decimal price);
    }

    public class NoDiscount : IDiscountStrategy
    {
        public string Title { get { return "без скидки"; } }

        public decimal Apply(decimal price)
        {
            return price;
        }
    }

    public class PercentDiscount : IDiscountStrategy
    {
        private readonly int percent;

        public PercentDiscount(int percent)
        {
            this.percent = percent;
        }

        public string Title { get { return "скидка " + percent + "%"; } }

        public decimal Apply(decimal price)
        {
            return price - price * percent / 100;
        }
    }

    public class PromoCodeDiscount : IDiscountStrategy
    {
        public string Title { get { return "промокод на 500 руб."; } }

        public decimal Apply(decimal price)
        {
            // Цена не может стать отрицательной
            return Math.Max(0, price - 500m);
        }
    }

    /// <summary>Контекст: считает итог, используя текущую стратегию.</summary>
    public class PriceCalculator
    {
        // Стратегию можно менять в любой момент (set)
        public IDiscountStrategy Discount { get; set; }

        public PriceCalculator()
        {
            Discount = new NoDiscount();   // стратегия по умолчанию
        }

        public void PrintTotal(decimal price)
        {
            // Здесь нет ни одного if: какая бы стратегия ни стояла,
            // вызов один и тот же.
            Console.WriteLine(price + " руб., " + Discount.Title + " → " + Discount.Apply(price) + " руб.");
        }
    }

    public static class StrategyDemo
    {
        public static void Run()
        {
            PriceCalculator calculator = new PriceCalculator();
            calculator.PrintTotal(3000m);

            calculator.Discount = new PercentDiscount(15);   // подменили алгоритм
            calculator.PrintTotal(3000m);

            calculator.Discount = new PromoCodeDiscount();
            calculator.PrintTotal(3000m);
        }
    }
}
