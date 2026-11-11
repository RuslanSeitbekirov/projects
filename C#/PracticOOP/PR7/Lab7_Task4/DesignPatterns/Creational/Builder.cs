using System;
using System.Collections.Generic;

namespace DesignPatterns.Creational
{
    // =========================================================================
    //  СТРОИТЕЛЬ (Builder) — порождающий паттерн.
    //
    //  Задача: создать сложный объект из многих частей ПО ШАГАМ.
    //
    //  В магазине: покупатель собирает компьютер — процессор, память, диск,
    //  по желанию видеокарта и разные дополнения. Без паттерна понадобился бы
    //  конструктор с кучей параметров:
    //      new Computer("Ryzen 5", 16, 512, null, true, false, ...)
    //  и непонятно, что значит каждый параметр.
    //
    //  Участники:
    //    Computer         — "продукт" (то, что строим);
    //    ComputerBuilder  — "строитель" (метод на каждый шаг + Build);
    //    ComputerDirector — "распорядитель" (знает готовые "рецепты").
    // =========================================================================

    /// <summary>Продукт: собранный компьютер.</summary>
    public class Computer
    {
        public string Cpu { get; set; }
        public int RamGb { get; set; }
        public int SsdGb { get; set; }
        public string Gpu { get; set; }

        // Список дополнений (сначала пустой)
        public List<string> Extras { get; } = new List<string>();

        public override string ToString()
        {
            string text = "процессор " + Cpu + ", память " + RamGb + " ГБ, SSD " + SsdGb + " ГБ";
            text += ", видеокарта: " + (Gpu == null ? "встроенная" : Gpu);
            if (Extras.Count > 0)
            {
                text += ", дополнительно: " + string.Join(", ", Extras);
            }
            return text;
        }
    }

    /// <summary>Строитель: собирает компьютер шаг за шагом.</summary>
    public class ComputerBuilder
    {
        // Компьютер, который сейчас собирается
        private Computer computer = new Computer();

        // Каждый метод настраивает одну часть и возвращает САМОГО строителя
        // (return this). Благодаря этому вызовы можно писать цепочкой через точку.

        public ComputerBuilder SetCpu(string cpu)
        {
            computer.Cpu = cpu;
            return this;
        }

        public ComputerBuilder SetRam(int gigabytes)
        {
            computer.RamGb = gigabytes;
            return this;
        }

        public ComputerBuilder SetSsd(int gigabytes)
        {
            computer.SsdGb = gigabytes;
            return this;
        }

        public ComputerBuilder SetGpu(string gpu)
        {
            computer.Gpu = gpu;
            return this;
        }

        public ComputerBuilder AddExtra(string extra)
        {
            computer.Extras.Add(extra);
            return this;
        }

        /// <summary>Последний шаг: отдать готовый компьютер.</summary>
        public Computer Build()
        {
            Computer result = computer;

            // Начинаем новый пустой компьютер, чтобы строителем
            // можно было пользоваться ещё раз.
            computer = new Computer();
            return result;
        }
    }

    /// <summary>Распорядитель: хранит готовые наборы шагов.</summary>
    public class ComputerDirector
    {
        public Computer BuildOfficePc(ComputerBuilder builder)
        {
            return builder.SetCpu("Core i3").SetRam(8).SetSsd(256).Build();
        }

        public Computer BuildGamingPc(ComputerBuilder builder)
        {
            return builder.SetCpu("Ryzen 7").SetRam(32).SetSsd(1000)
                          .SetGpu("RTX 4070").AddExtra("водяное охлаждение").Build();
        }
    }

    public static class BuilderDemo
    {
        public static void Run()
        {
            ComputerBuilder builder = new ComputerBuilder();

            // Собираем сами: по коду сразу понятно, что есть что
            Computer custom = builder
                .SetCpu("Ryzen 5")
                .SetRam(16)
                .SetSsd(512)
                .AddExtra("Wi-Fi")
                .AddExtra("подсветка")
                .Build();
            Console.WriteLine("Свой:    " + custom);

            // Или просим распорядителя собрать по готовому рецепту
            ComputerDirector director = new ComputerDirector();
            Console.WriteLine("Офисный: " + director.BuildOfficePc(builder));
            Console.WriteLine("Игровой: " + director.BuildGamingPc(builder));
        }
    }
}
