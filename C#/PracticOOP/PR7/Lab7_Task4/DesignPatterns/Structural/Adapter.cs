using System;

namespace DesignPatterns.Structural
{
    // =========================================================================
    //  АДАПТЕР (Adapter) — структурный паттерн.
    //
    //  Задача: есть готовый класс с "неудобным" набором методов, а нашей
    //  программе нужен другой интерфейс. Менять чужой класс нельзя
    //  (например, это библиотека банка). Адаптер — "переходник" между ними,
    //  как переходник для розетки.
    //
    //  В магазине: наш код умеет платить через IPaymentSystem (сумма в рублях),
    //  а у старой банковской библиотеки метод принимает копейки, код валюты
    //  и возвращает число-код ошибки.
    //
    //  Участники:
    //    IPaymentSystem — интерфейс, которого ждёт наш магазин;
    //    OldBankApi     — чужой класс с другим интерфейсом ("адаптируемый");
    //    BankAdapter    — адаптер: реализует наш интерфейс, а внутри вызывает банк.
    // =========================================================================

    /// <summary>Интерфейс, с которым работает магазин.</summary>
    public interface IPaymentSystem
    {
        /// <summary>Оплатить сумму в рублях. true — успешно.</summary>
        bool Pay(decimal rubles);
    }

    /// <summary>
    /// "Чужая" библиотека банка. Представляем, что менять её нельзя.
    /// </summary>
    public class OldBankApi
    {
        /// <summary>Возвращает 0, если всё хорошо, иначе код ошибки.</summary>
        public int SendMoney(string currencyCode, long kopecks)
        {
            Console.WriteLine("  [банк] перевод " + kopecks + " коп., валюта " + currencyCode);
            if (kopecks <= 0)
            {
                return 13;   // ошибка: неверная сумма
            }
            return 0;
        }
    }

    /// <summary>Адаптер: снаружи выглядит как IPaymentSystem, внутри — банк.</summary>
    public class BankAdapter : IPaymentSystem
    {
        // Объект, который мы "оборачиваем"
        private readonly OldBankApi bank;

        public BankAdapter(OldBankApi bank)
        {
            this.bank = bank;
        }

        public bool Pay(decimal rubles)
        {
            // Вся работа адаптера — перевести "наш язык" на "язык банка":
            long kopecks = (long)(rubles * 100);          // рубли -> копейки
            int errorCode = bank.SendMoney("RUB", kopecks);
            return errorCode == 0;                        // код ошибки -> true/false
        }
    }

    public static class AdapterDemo
    {
        public static void Run()
        {
            // Магазин видит только IPaymentSystem и не знает про OldBankApi
            IPaymentSystem payment = new BankAdapter(new OldBankApi());

            Console.WriteLine("Оплата 1999,90 руб.: " + (payment.Pay(1999.90m) ? "успешно" : "ошибка"));
            Console.WriteLine("Оплата 0 руб.: " + (payment.Pay(0m) ? "успешно" : "ошибка"));
        }
    }
}
