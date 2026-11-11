using System;
using System.Collections.Generic;

namespace SolidPrinciples.DependencyInversion
{
    // =========================================================================
    //  D — Dependency Inversion Principle
    //      ПРИНЦИП ИНВЕРСИИ ЗАВИСИМОСТЕЙ
    //
    //  "Модули верхнего уровня не должны зависеть от модулей нижнего уровня.
    //  И те и другие должны зависеть от абстракций".
    //  Проще: важный класс (логика магазина) не должен сам создавать
    //  конкретные "рабочие" классы через new — пусть знает только интерфейс,
    //  а готовый объект ему передадут снаружи.
    //
    //  ПЛОХО:
    //
    //      class OrderService
    //      {
    //          private EmailSender sender = new EmailSender();   // жёсткая связь
    //
    //          public void Confirm(string order)
    //          {
    //              sender.Send("Заказ принят: " + order);
    //          }
    //      }
    //
    //  OrderService намертво привязан к почте. Захотели слать SMS — надо
    //  переписывать OrderService. Проверить его без настоящей отправки
    //  писем тоже нельзя.
    //
    //  ХОРОШО: OrderService зависит от интерфейса IMessageSender, а какой
    //  именно отправитель будет работать, решает тот, кто создаёт сервис.
    //  Передача готового объекта через конструктор называется
    //  "внедрение зависимости" (Dependency Injection).
    // =========================================================================

    /// <summary>Абстракция: "что-то, что умеет отправлять сообщения".</summary>
    public interface IMessageSender
    {
        void Send(string text);
    }

    // ---------------- Модули нижнего уровня (детали) ----------------

    public class EmailSender : IMessageSender
    {
        public void Send(string text)
        {
            Console.WriteLine("  [e-mail] " + text);
        }
    }

    public class SmsSender : IMessageSender
    {
        public void Send(string text)
        {
            Console.WriteLine("  [SMS] " + text);
        }
    }

    /// <summary>
    /// "Ненастоящий" отправитель для проверки: никуда ничего не шлёт,
    /// а только запоминает сообщения. С ним можно проверить OrderService,
    /// не отправляя писем.
    /// </summary>
    public class FakeSender : IMessageSender
    {
        public List<string> Messages { get; } = new List<string>();

        public void Send(string text)
        {
            Messages.Add(text);
        }
    }

    // ---------------- Модуль верхнего уровня (логика) ----------------

    public class OrderService
    {
        // Поле типа ИНТЕРФЕЙСА. Слова new в этом классе нет вообще.
        private readonly IMessageSender sender;

        /// <summary>Отправителя передают снаружи через конструктор.</summary>
        public OrderService(IMessageSender sender)
        {
            this.sender = sender;
        }

        public void Confirm(string order)
        {
            sender.Send("Ваш заказ «" + order + "» принят");
        }
    }

    public static class Demo
    {
        public static void Run()
        {
            // Один и тот же OrderService работает с разными отправителями,
            // и его код для этого менять не нужно.
            new OrderService(new EmailSender()).Confirm("Клавиатура");
            new OrderService(new SmsSender()).Confirm("Клавиатура");

            // Проверка логики без настоящей отправки
            FakeSender fake = new FakeSender();
            new OrderService(fake).Confirm("Клавиатура");
            Console.WriteLine("  [проверка] отправлено сообщений: " + fake.Messages.Count +
                              ", текст: " + fake.Messages[0]);
        }
    }
}
