using System;
using System.Text;
using System.Threading;
using MeasuringDevice;

namespace ConsoleTest
{
    /// <summary>
    /// Консольное тестовое приложение для класса MeasureLengthDevice. 
    /// Пункты меню повторяют кнопки WPF-окна из методички (Task 4–5).
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Переменная имеет тип ИНТЕРФЕЙСА, а не класса.
        /// Программе всё равно, какое именно устройство за ней стоит —
        /// достаточно, что объект реализует IMeasuringDevice.
        /// </summary>
        private static IMeasuringDevice device;

        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.Title = "Практическая работа №5, задание 1 — MeasureLengthDevice";

            while (true)
            {
                PrintMenu();
                Console.Write("Ваш выбор: ");
                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1": CreateInstance(Units.Imperial); break;
                    case "2": CreateInstance(Units.Metric); break;
                    case "3": StartCollecting(); break;
                    case "4": StopCollecting(); break;
                    case "5": ShowRawData(); break;
                    case "6": ShowMetricValue(); break;
                    case "7": ShowImperialValue(); break;
                    case "8": RunAutoTest(); break;
                    case "0":
                        device?.StopCollecting();
                        return;
                    default:
                        Console.WriteLine("Нет такого пункта меню.");
                        break;
                }
                Console.WriteLine();
            }
        }

        private static void PrintMenu()
        {
            Console.WriteLine("========== MeasureLengthDevice: тест ==========");
            Console.WriteLine(" 1 — Создать устройство (Imperial, дюймы)");
            Console.WriteLine(" 2 — Создать устройство (Metric, миллиметры)");
            Console.WriteLine(" 3 — Start Collecting (начать сбор)");
            Console.WriteLine(" 4 — Stop Collecting (остановить сбор)");
            Console.WriteLine(" 5 — Get Raw Data (сырые данные)");
            Console.WriteLine(" 6 — Get Metric Value");
            Console.WriteLine(" 7 — Get Imperial Value");
            Console.WriteLine(" 8 — Автотест по сценарию методички (Task 5)");
            Console.WriteLine(" 0 — Выход");
            Console.WriteLine("===============================================");
        }

        // ---------------- Обработчики пунктов меню ----------------

        private static void CreateInstance(Units units)
        {
            // Если было старое устройство — останавливаем, чтобы не висел фоновый поток
            device?.StopCollecting();

            device = new MeasureLengthDevice(units);
            Console.WriteLine($"Создан MeasureLengthDevice, единицы: {units}.");
        }

        private static bool CheckDevice()
        {
            if (device == null)
            {
                Console.WriteLine("Сначала создайте устройство (пункт 1 или 2).");
                return false;
            }
            return true;
        }

        private static void StartCollecting()
        {
            if (!CheckDevice()) return;
            device.StartCollecting();
            Console.WriteLine("Сбор данных запущен. Подождите ~10 секунд, " +
                              "чтобы эмулятор успел выдать несколько значений.");
        }

        private static void StopCollecting()
        {
            if (!CheckDevice()) return;
            device.StopCollecting();
            Console.WriteLine("Сбор данных остановлен.");
        }

        private static void ShowRawData()
        {
            if (!CheckDevice()) return;

            int[] raw = device.GetRawData();
            if (raw == null)
            {
                Console.WriteLine("Данных нет — сбор ещё не запускался.");
                return;
            }

            Console.WriteLine("Сырые данные (кольцевой буфер на 10 значений):");
            for (int i = 0; i < raw.Length; i++)
            {
                Console.WriteLine($"  [{i}] = {raw[i]}");
            }
        }

        private static void ShowMetricValue()
        {
            if (!CheckDevice()) return;
            Console.WriteLine($"Metric value:   {device.MetricValue()} мм");
        }

        private static void ShowImperialValue()
        {
            if (!CheckDevice()) return;
            Console.WriteLine($"Imperial value: {device.ImperialValue()} дюйм.");
        }

        // ---------------- Автоматический прогон (Task 5) ----------------

        /// <summary>
        /// Повторяет шаги 3–15 Task 5 методички без участия пользователя:
        /// сначала устройство в Imperial, затем в Metric. После каждого
        /// прогона проверяет, что "родное" значение есть среди сырых данных.
        /// </summary>
        private static void RunAutoTest()
        {
            TestOneDevice(Units.Imperial);
            Console.WriteLine();
            TestOneDevice(Units.Metric);
        }

        private static void TestOneDevice(Units units)
        {
            Console.WriteLine($"--- Автотест: {units} ---");
            CreateInstance(units);
            device.StartCollecting();

            Console.Write("Ждём 10 секунд");
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                Console.Write('.');
            }
            Console.WriteLine();

            // Останавливаем ДО чтения, чтобы между чтениями не пришло новое значение
            device.StopCollecting();

            decimal metric = device.MetricValue();
            decimal imperial = device.ImperialValue();
            int[] raw = device.GetRawData();

            Console.WriteLine($"Metric value:   {metric} мм");
            Console.WriteLine($"Imperial value: {imperial} дюйм.");
            Console.WriteLine("Raw data: " + string.Join(", ", raw));

            // Значение в "родных" единицах устройства должно быть в сырых данных
            decimal native = units == Units.Metric ? metric : imperial;
            bool found = Array.IndexOf(raw, (int)native) >= 0;
            Console.WriteLine(found
                ? $"OK: значение {native} найдено в сырых данных."
                : $"ОШИБКА: значение {native} не найдено в сырых данных!");
        }
    }
}
