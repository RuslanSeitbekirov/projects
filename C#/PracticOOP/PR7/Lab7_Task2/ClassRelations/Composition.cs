using System;

namespace ClassRelations
{
    // =========================================================================
    //  КОМПОЗИЦИЯ — самая сильная связь "целое — часть": часть НЕ может
    //  существовать без целого и принадлежит только ему одному.
    //
    //  Как узнать в коде: целое САМО создаёт свои части (new внутри
    //  конструктора), хранит их в private-поле и никому не отдаёт.
    //  Когда целое уничтожается — вместе с ним пропадают и части.
    //
    //  Пример: автомобиль и его двигатель. Двигатель создаётся вместе
    //  с автомобилем, снаружи к нему доступа нет.
    // =========================================================================

    /// <summary>Двигатель — "часть".</summary>
    public class Engine
    {
        public int Power { get; }

        public Engine(int power)
        {
            Power = power;
        }

        public void Start()
        {
            Console.WriteLine("  двигатель (" + Power + " л.с.) запущен");
        }
    }

    /// <summary>Автомобиль — "целое".</summary>
    public class Car
    {
        public string Model { get; }

        // private readonly: двигатель виден только внутри автомобиля,
        // и заменить его на другой после создания нельзя.
        private readonly Engine engine;

        /// <summary>
        /// В конструктор приходит не готовый двигатель, а только число (мощность).
        /// Сам объект Engine создаётся ЗДЕСЬ — это главный признак композиции.
        /// </summary>
        public Car(string model, int enginePower)
        {
            Model = model;
            engine = new Engine(enginePower);
        }

        /// <summary>
        /// Автомобиль не делает работу сам, а поручает её своей части.
        /// Это называется делегированием.
        /// </summary>
        public void Start()
        {
            Console.WriteLine(Model + " заводится:");
            engine.Start();
        }

        /// <summary>Наружу отдаём только сведения о двигателе, но не сам объект.</summary>
        public int EnginePower
        {
            get { return engine.Power; }
        }
    }

    public static class CompositionDemo
    {
        public static void Run()
        {
            // Создаём только автомобиль — двигатель появляется внутри сам
            Car car = new Car("Лада Веста", 106);
            car.Start();
            Console.WriteLine("Мощность: " + car.EnginePower + " л.с.");

            // Добраться до двигателя нельзя: строка ниже не скомпилируется,
            // потому что поле engine закрытое (private).
            // car.engine.Start();

            // Убираем ссылку на автомобиль. Вместе с ним сборщик мусора
            // удалит и двигатель: больше на него никто не ссылается.
            car = null;
            Console.WriteLine("Автомобиля больше нет — и его двигателя тоже.");
        }
    }
}
