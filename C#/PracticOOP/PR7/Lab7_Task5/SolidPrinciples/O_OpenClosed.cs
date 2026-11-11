using System;

namespace SolidPrinciples.OpenClosed
{
    // =========================================================================
    //  O — Open/Closed Principle
    //      ПРИНЦИП ОТКРЫТОСТИ / ЗАКРЫТОСТИ
    //
    //  "Классы должны быть открыты для расширения, но закрыты для изменения".
    //  Проще: новую возможность добавляем НОВЫМ кодом, а не правкой старого,
    //  который уже написан, проверен и работает.
    //
    //  ПЛОХО:
    //
    //      decimal GetPrice(decimal price, string customerType)
    //      {
    //          if (customerType == "обычный") return price;
    //          if (customerType == "vip")     return price * 0.8m;
    //          // появился новый вид покупателя -> лезем сюда и дописываем if
    //      }
    //
    //  Каждый новый вид скидки заставляет менять этот метод, и при этом
    //  легко сломать то, что уже работало.
    //
    //  ХОРОШО: правило скидки — отдельный класс с общим интерфейсом.
    //  Новая скидка = новый класс. PriceCalculator не меняется никогда.
    // =========================================================================

    /// <summary>Правило расчёта цены для покупателя.</summary>
    public interface ICustomerDiscount
    {
        string Title { get; }
        decimal Apply(decimal price);
    }

    public class RegularCustomer : ICustomerDiscount
    {
        public string Title { get { return "обычный покупатель"; } }
        public decimal Apply(decimal price) { return price; }
    }

    public class VipCustomer : ICustomerDiscount
    {
        public string Title { get { return "VIP (−20%)"; } }
        public decimal Apply(decimal price) { return price * 0.80m; }
    }

    /// <summary>
    /// Этот класс я добавил ПОЗЖЕ остальных — и ни одной строчки
    /// в других классах менять не пришлось. Это и есть "расширение".
    /// </summary>
    public class StudentCustomer : ICustomerDiscount
    {
        public string Title { get { return "студент (−10%)"; } }
        public decimal Apply(decimal price) { return price * 0.90m; }
    }

    /// <summary>
    /// "Закрыт для изменения": в нём нет ни одного if по видам покупателей,
    /// поэтому править его ради новой скидки не нужно.
    /// </summary>
    public class PriceCalculator
    {
        public decimal GetPrice(decimal price, ICustomerDiscount discount)
        {
            return discount.Apply(price);
        }
    }

    public static class Demo
    {
        public static void Run()
        {
            PriceCalculator calculator = new PriceCalculator();
            ICustomerDiscount[] customers = { new RegularCustomer(), new VipCustomer(), new StudentCustomer() };

            foreach (ICustomerDiscount customer in customers)
            {
                Console.WriteLine("  " + customer.Title + ": " + calculator.GetPrice(1000m, customer) + " руб.");
            }
        }
    }
}
