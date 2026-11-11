using System;

namespace DesignPatterns.Structural
{
    // =========================================================================
    //  ФАСАД (Facade) — структурный паттерн.
    //
    //  Задача: спрятать сложную систему из многих классов за одним простым
    //  классом с одним-двумя методами.
    //
    //  В магазине: чтобы оформить заказ, надо проверить склад, снять деньги,
    //  зарезервировать товар и вызвать доставку — и всё в правильном порядке.
    //  Без фасада каждый, кто оформляет заказ, должен был бы знать все эти
    //  классы и порядок вызовов. С фасадом достаточно вызвать PlaceOrder.
    //
    //  Участники:
    //    Warehouse, PaymentService, DeliveryDepartment — подсистема;
    //    OrderFacade — фасад.
    // =========================================================================

    // ---------------- Подсистема (три независимых класса) ----------------

    public class Warehouse
    {
        public bool IsInStock(string product)
        {
            Console.WriteLine("  склад: «" + product + "» есть в наличии");
            return true;
        }

        public void Reserve(string product)
        {
            Console.WriteLine("  склад: «" + product + "» отложен для покупателя");
        }
    }

    public class PaymentService
    {
        public bool Charge(decimal amount)
        {
            Console.WriteLine("  оплата: списано " + amount + " руб.");
            return true;
        }
    }

    public class DeliveryDepartment
    {
        public void Schedule(string product, string address)
        {
            Console.WriteLine("  доставка: «" + product + "» поедет по адресу " + address);
        }
    }

    // ---------------- Фасад ----------------

    /// <summary>Простой "вход" в сложную систему.</summary>
    public class OrderFacade
    {
        // Фасад сам создаёт и хранит части подсистемы
        private readonly Warehouse warehouse = new Warehouse();
        private readonly PaymentService payment = new PaymentService();
        private readonly DeliveryDepartment delivery = new DeliveryDepartment();

        /// <summary>Один метод вместо четырёх вызовов в правильном порядке.</summary>
        public bool PlaceOrder(string product, decimal price, string address)
        {
            if (!warehouse.IsInStock(product))
            {
                return false;
            }
            if (!payment.Charge(price))
            {
                return false;
            }
            warehouse.Reserve(product);
            delivery.Schedule(product, address);
            return true;
        }
    }

    public static class FacadeDemo
    {
        public static void Run()
        {
            OrderFacade shop = new OrderFacade();

            // Покупателю (вызывающему коду) не нужно знать про склад,
            // оплату и доставку — только про фасад.
            Console.WriteLine("Оформляем заказ:");
            bool ok = shop.PlaceOrder("Монитор", 15000m, "Симферополь, ул. Ленина, 1");
            Console.WriteLine("Заказ оформлен: " + ok);
        }
    }
}
