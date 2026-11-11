using System;

namespace ClassRelations
{
    // =========================================================================
    //  НАСЛЕДОВАНИЕ или КОМПОЗИЦИЯ?
    //
    //  Наследование — связь "является" (is-a):  Собака ЯВЛЯЕТСЯ животным.
    //  Композиция   — связь "имеет"    (has-a): Робот ИМЕЕТ способ передвижения.
    //
    //  Одну и ту же задачу ("объекты умеют двигаться по-разному") можно
    //  решить обоими способами. Ниже оба варианта, чтобы сравнить.
    // =========================================================================

    // ---------------- Вариант 1: наследование ----------------

    /// <summary>Базовый класс.</summary>
    public class Animal
    {
        public string Name { get; }

        public Animal(string name)
        {
            Name = name;
        }

        // virtual — наследник может заменить этот метод своим
        public virtual void Move()
        {
            Console.WriteLine(Name + " идёт");
        }
    }

    /// <summary>Птица получает всё от Animal и меняет способ движения.</summary>
    public class Bird : Animal
    {
        public Bird(string name) : base(name)
        {
        }

        public override void Move()
        {
            Console.WriteLine(Name + " летит");
        }
    }

    // ---------------- Вариант 2: композиция ----------------

    /// <summary>Способ передвижения вынесен в отдельный интерфейс.</summary>
    public interface IMovement
    {
        string Describe();
    }

    public class Wheels : IMovement
    {
        public string Describe() { return "едет на колёсах"; }
    }

    public class Propeller : IMovement
    {
        public string Describe() { return "летит на пропеллерах"; }
    }

    /// <summary>
    /// Робот ни от кого не наследуется. Вместо этого внутри него лежит
    /// объект-"способ движения", которому он поручает работу.
    /// </summary>
    public class Robot
    {
        public string Name { get; }

        // Свойство с set: способ движения можно поменять прямо во время работы
        public IMovement Movement { get; set; }

        public Robot(string name, IMovement movement)
        {
            Name = name;
            Movement = movement;
        }

        public void Move()
        {
            Console.WriteLine(Name + " " + Movement.Describe());
        }
    }

    public static class InheritanceVsCompositionDemo
    {
        public static void Run()
        {
            Console.WriteLine("Наследование:");
            Animal[] animals = { new Animal("Кот"), new Bird("Воробей") };
            foreach (Animal animal in animals)
            {
                animal.Move();
            }
            // Воробей всегда летает. Сделать так, чтобы ЭТОТ ЖЕ объект начал
            // плавать, нельзя — поведение "зашито" в класс.

            Console.WriteLine("Композиция:");
            Robot robot = new Robot("Робот", new Wheels());
            robot.Move();
            robot.Movement = new Propeller();   // тот же объект, новое поведение
            robot.Move();

            Console.WriteLine();
            Console.WriteLine("Наследование. Плюсы: мало кода, готовый полиморфизм, наследник сразу умеет всё, что предок.");
            Console.WriteLine("  Минусы: сильная связь с предком (изменили предка — могли сломать наследников),");
            Console.WriteLine("  в C# только один базовый класс, поведение нельзя поменять во время работы.");
            Console.WriteLine("Композиция. Плюсы: части легко заменять (даже на ходу), классы слабо связаны,");
            Console.WriteLine("  части можно тестировать и использовать отдельно.");
            Console.WriteLine("  Минусы: больше классов и кода, работу приходится вручную передавать части (делегировать).");
            Console.WriteLine("Общий совет из книг: \"предпочитайте композицию наследованию\".");
            Console.WriteLine();
            Console.WriteLine("Языки, где наследования классов нет совсем, а вместо него композиция:");
            Console.WriteLine("  Go   — структуры \"встраиваются\" одна в другую, общее поведение задают интерфейсы;");
            Console.WriteLine("  Rust — структуры + типажи (trait), наследования структур нет.");
        }
    }
}
