using System;

namespace SolidPrinciples.Liskov
{
    // =========================================================================
    //  L — Liskov Substitution Principle
    //      ПРИНЦИП ПОДСТАНОВКИ БАРБАРЫ ЛИСКОВ
    //
    //  "Вместо объекта базового класса можно подставить объект наследника,
    //  и программа должна работать так же правильно".
    //  Проще: наследник не должен ломать то, что обещал предок.
    //
    //  Самый известный пример нарушения — квадрат и прямоугольник.
    //  В математике квадрат — это прямоугольник, и кажется логичным написать
    //  class Square : Rectangle. Но в программе это ломается (см. ниже):
    //  прямоугольник "обещает", что ширину и высоту можно менять независимо,
    //  а квадрат это обещание нарушает.
    // =========================================================================

    // ---------------- ПЛОХО: наследник меняет поведение предка ----------------

    public class BadRectangle
    {
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        public int Area()
        {
            return Width * Height;
        }
    }

    public class BadSquare : BadRectangle
    {
        // Чтобы квадрат оставался квадратом, при изменении одной стороны
        // тайком меняется и другая. Вот тут и нарушается принцип.
        public override int Width
        {
            get { return base.Width; }
            set { base.Width = value; base.Height = value; }
        }

        public override int Height
        {
            get { return base.Height; }
            set { base.Width = value; base.Height = value; }
        }
    }

    // ---------------- ХОРОШО: общий предок без лишних обещаний ----------------

    /// <summary>
    /// Фигура обещает только одно: у неё можно узнать площадь.
    /// Это обещание могут честно выполнить и прямоугольник, и квадрат.
    /// </summary>
    public abstract class Shape
    {
        public abstract int Area();
    }

    public class Rectangle : Shape
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public override int Area() { return Width * Height; }
    }

    public class Square : Shape
    {
        public int Side { get; set; }

        public override int Area() { return Side * Side; }
    }

    public static class Demo
    {
        /// <summary>
        /// Метод написан для прямоугольника и рассчитывает на его поведение:
        /// поставили 5 и 4 — площадь должна быть 20.
        /// </summary>
        private static void SetSizeAndCheck(BadRectangle rectangle, string name)
        {
            rectangle.Width = 5;
            rectangle.Height = 4;

            int area = rectangle.Area();
            Console.WriteLine("  " + name + ": ждали 5 * 4 = 20, получили " + area +
                              (area == 20 ? " — верно" : " — ОШИБКА!"));
        }

        public static void Run()
        {
            Console.WriteLine("  Плохой вариант (Square : Rectangle):");
            SetSizeAndCheck(new BadRectangle(), "прямоугольник");

            // Подставляем наследника вместо предка — и метод перестал работать
            // правильно. Значит, такое наследование нарушает принцип Лисков.
            SetSizeAndCheck(new BadSquare(), "квадрат      ");

            Console.WriteLine("  Хороший вариант (оба наследуют Shape):");
            Shape[] shapes = { new Rectangle { Width = 5, Height = 4 }, new Square { Side = 4 } };
            foreach (Shape shape in shapes)
            {
                // Любую фигуру можно подставить — сюрпризов нет
                Console.WriteLine("  " + shape.GetType().Name + ": площадь " + shape.Area());
            }
        }
    }
}
