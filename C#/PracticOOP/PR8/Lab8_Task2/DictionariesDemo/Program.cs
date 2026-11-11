using System;
using System.Collections.Generic;
using System.Diagnostics;   // Stopwatch — секундомер
using System.Text;

namespace DictionariesDemo
{
    /// <summary>
    /// Практика 8, задание 2: словари и индексаторы.
    ///
    ///   MyDictionary.cs — свой словарь (хеш-таблица) + индексатор по ключу;
    ///   SmartArray.cs   — "умный массив" с индексатором и проверками;
    ///   Program.cs      — проверка обоих классов и замер скорости поиска.
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("========== 1. Свой словарь MyDictionary ==========");
            DictionaryBasics();

            Console.WriteLine();
            Console.WriteLine("========== Скорость выборки по ключу ==========");
            SpeedTest();

            Console.WriteLine();
            Console.WriteLine("========== 2. Умный массив (индексатор) ==========");
            SmartArrayDemo();

            if (!Console.IsInputRedirected)
            {
                Console.WriteLine();
                Console.WriteLine("Нажмите любую клавишу...");
                Console.ReadKey();
            }
        }

        // =====================================================================
        //  Как пользоваться моим словарём
        // =====================================================================

        private static void DictionaryBasics()
        {
            // Ключ — название города (string), значение — телефонный код (int)
            MyDictionary<string, int> codes = new MyDictionary<string, int>();

            // Добавление методом Add
            codes.Add("Симферополь", 3652);
            codes.Add("Москва", 495);

            // Добавление через индексатор: ключа ещё нет — он будет создан
            codes["Казань"] = 843;

            Console.WriteLine("Пар в словаре: " + codes.Count);

            // Чтение через индексатор: в скобках ключ, а не номер
            Console.WriteLine("Код Симферополя: " + codes["Симферополь"]);

            // Запись через индексатор для существующего ключа меняет значение
            codes["Москва"] = 499;
            Console.WriteLine("Новый код Москвы: " + codes["Москва"]);

            // ContainsKey — проверить, есть ли ключ
            Console.WriteLine("Есть «Казань»: " + codes.ContainsKey("Казань") +
                              ", есть «Париж»: " + codes.ContainsKey("Париж"));

            // TryGetValue — безопасное чтение, когда не уверены, что ключ есть
            int code;
            if (codes.TryGetValue("Париж", out code))
            {
                Console.WriteLine("Код Парижа: " + code);
            }
            else
            {
                Console.WriteLine("TryGetValue(«Париж») вернул false — такого ключа нет");
            }

            // Чтение несуществующего ключа через [ ] — исключение
            try
            {
                Console.WriteLine(codes["Париж"]);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine("codes[«Париж»] → исключение: " + ex.Message);
            }

            // Повторный Add того же ключа — тоже исключение
            try
            {
                codes.Add("Казань", 1);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Повторный Add → исключение: " + ex.Message);
            }

            // Remove — удаление
            Console.WriteLine("Remove(«Москва»): " + codes.Remove("Москва") + ", осталось пар: " + codes.Count);
        }

        // =====================================================================
        //  Замер скорости выборки по ключу
        // =====================================================================

        /// <summary>
        /// Сравниваю три способа найти значение по ключу:
        ///   1) обычный список пар — перебор всех элементов подряд;
        ///   2) мой MyDictionary;
        ///   3) стандартный Dictionary из .NET.
        /// </summary>
        private static void SpeedTest()
        {
            const int Size = 100000;     // сколько пар хранится
            const int Searches = 1000;   // сколько раз ищем

            // Заполняем все три хранилища одинаковыми данными:
            // ключ "user0", "user1", ...; значение — номер.
            List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
            MyDictionary<string, int> mine = new MyDictionary<string, int>();
            Dictionary<string, int> standard = new Dictionary<string, int>();

            for (int i = 0; i < Size; i++)
            {
                string key = "user" + i;
                list.Add(new KeyValuePair<string, int>(key, i));
                mine.Add(key, i);
                standard.Add(key, i);
            }

            // Ключи, которые будем искать: равномерно по всему диапазону
            string[] keys = new string[Searches];
            for (int i = 0; i < Searches; i++)
            {
                keys[i] = "user" + (i * (Size / Searches));
            }

            Console.WriteLine("В каждом хранилище " + Size + " пар, выполняем " + Searches + " поисков.");

            // sum нужен, чтобы убедиться, что все три способа нашли одно и то же
            long sum;

            // ---- 1) Список: линейный поиск ----
            Stopwatch watch = Stopwatch.StartNew();
            sum = 0;
            foreach (string key in keys)
            {
                // Идём по списку с начала, пока не встретим нужный ключ
                foreach (KeyValuePair<string, int> pair in list)
                {
                    if (pair.Key == key)
                    {
                        sum += pair.Value;
                        break;
                    }
                }
            }
            watch.Stop();
            double listTime = watch.Elapsed.TotalMilliseconds;
            Console.WriteLine("  список (перебор):      " + listTime.ToString("0.000") + " мс, контрольная сумма " + sum);

            // ---- 2) Мой словарь ----
            watch.Restart();
            sum = 0;
            foreach (string key in keys)
            {
                sum += mine[key];
            }
            watch.Stop();
            double mineTime = watch.Elapsed.TotalMilliseconds;
            Console.WriteLine("  MyDictionary:          " + mineTime.ToString("0.000") + " мс, контрольная сумма " + sum);

            // ---- 3) Стандартный словарь ----
            watch.Restart();
            sum = 0;
            foreach (string key in keys)
            {
                sum += standard[key];
            }
            watch.Stop();
            double standardTime = watch.Elapsed.TotalMilliseconds;
            Console.WriteLine("  Dictionary из .NET:    " + standardTime.ToString("0.000") + " мс, контрольная сумма " + sum);

            Console.WriteLine("Мой словарь быстрее перебора списка примерно в " +
                              (listTime / mineTime).ToString("0") + " раз.");
            Console.WriteLine("Почему: список в среднем просматривает половину элементов (" + Size / 2 +
                              "), а словарь сразу идёт в нужную корзину.");
            Console.WriteLine("В моём словаре корзин: " + mine.BucketCount +
                              ", в самой полной корзине пар: " + mine.LongestBucket + " (это коллизии).");
        }

        // =====================================================================
        //  Умный массив
        // =====================================================================

        private static void SmartArrayDemo()
        {
            SmartArray smart = new SmartArray(5);
            Console.WriteLine("Создан массив длиной " + smart.Length + ": " + smart);

            // Обычная работа — как с массивом, через квадратные скобки.
            // На самом деле здесь вызывается блок set индексатора.
            smart[0] = 10;
            smart[4] = 50;
            Console.WriteLine("После smart[0] = 10 и smart[4] = 50: " + smart);

            // А здесь — блок get
            Console.WriteLine("smart[4] = " + smart[4]);

            // Попытка записать за границу массива
            smart[7] = 99;
            Console.WriteLine("smart[7] = 99   → " + smart.LastError);

            smart[-1] = 99;
            Console.WriteLine("smart[-1] = 99  → " + smart.LastError);

            // Попытка записать отрицательное число
            smart[1] = -5;
            Console.WriteLine("smart[1] = -5   → " + smart.LastError);

            // Попытка прочитать за границей: программа не падает, приходит 0
            int value = smart[100];
            Console.WriteLine("smart[100] вернул " + value + " → " + smart.LastError);

            // Массив не испортился: в нём только правильные значения
            Console.WriteLine("Массив после всех попыток: " + smart);

            // Для сравнения — обычный массив в такой же ситуации падает
            int[] usual = new int[5];
            try
            {
                usual[7] = 99;
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Обычный массив: usual[7] = 99 → исключение IndexOutOfRangeException");
            }
        }
    }
}
