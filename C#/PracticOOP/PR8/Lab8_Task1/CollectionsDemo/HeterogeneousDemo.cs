using System;
using System.Collections;   // ArrayList, IComparer — "старые" (необобщённые) коллекции

namespace CollectionsDemo
{
    // =========================================================================
    //  Пункт 1. Коллекция из РАЗНОРОДНЫХ элементов.
    //
    //  Обычный массив int[] хранит только int, и размер у него задаётся
    //  один раз. ArrayList — "резиновый" массив: он сам растёт при добавлении,
    //  а хранит элементы типа object. Так как ЛЮБОЙ тип в C# является
    //  наследником object, в ArrayList можно положить вперемешку что угодно:
    //  числа, строки, даты...
    //
    //  Плата за это:
    //    - при чтении элемент приходит как object, и его надо приводить
    //      к настоящему типу вручную — (int)list[0];
    //    - числа при добавлении "упаковываются" в объект (boxing), это медленнее;
    //    - компилятор не проверяет типы — ошибка всплывёт только при запуске.
    //  Поэтому в новом коде обычно берут List<T>, где тип один и известен заранее.
    // =========================================================================

    /// <summary>
    /// "Сравниватель" для сортировки разнородных элементов.
    /// Зачем он нужен — см. комментарий у вызова Sort ниже.
    /// </summary>
    public class TextComparer : IComparer
    {
        /// <summary>
        /// Правило сравнения: должен вернуть отрицательное число, если x "меньше" y,
        /// ноль — если равны, положительное — если "больше".
        /// </summary>
        public int Compare(object x, object y)
        {
            // Превращаем оба элемента в строки и сравниваем как текст.
            // Ordinal — простое сравнение по кодам символов.
            return string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal);
        }
    }

    public static class HeterogeneousDemo
    {
        /// <summary>Печатает все элементы вместе с их настоящими типами.</summary>
        private static void Print(string title, ArrayList list)
        {
            Console.WriteLine(title + " (элементов: " + list.Count + ")");
            for (int i = 0; i < list.Count; i++)
            {
                // GetType() показывает настоящий тип объекта, хотя list[i] — это object
                Console.WriteLine("   [" + i + "] " + list[i] + "   <- " + list[i].GetType().Name);
            }
        }

        public static void Run()
        {
            ArrayList list = new ArrayList();

            // ---- Add: добавить в конец ----
            list.Add(10);                         // int
            list.Add("яблоко");                   // string
            list.Add(3.14);                       // double
            list.Add(true);                       // bool
            list.Add('Z');                        // char
            list.Add(new DateTime(2026, 9, 1));   // DateTime
            Print("После Add", list);

            // Чтение: элемент имеет тип object, поэтому нужно приведение типа.
            int number = (int)list[0];
            Console.WriteLine("Первый элемент как int: " + number + ", плюс 5 = " + (number + 5));

            // Если ошибиться с типом — будет InvalidCastException. Безопаснее
            // сначала проверить тип через is (заодно сразу получаем переменную).
            if (list[1] is string text)
            {
                Console.WriteLine("Второй элемент — строка длиной " + text.Length);
            }

            // ---- Insert: вставить на указанное место (остальные сдвигаются) ----
            list.Insert(1, "вставлено");
            Print("После Insert(1, \"вставлено\")", list);

            // ---- IndexOf: найти номер элемента (или -1, если такого нет) ----
            Console.WriteLine("IndexOf(\"яблоко\") = " + list.IndexOf("яблоко"));
            Console.WriteLine("IndexOf(\"груша\")  = " + list.IndexOf("груша") + " (не найдено)");

            // ---- Remove: удалить первый найденный элемент с таким значением ----
            list.Remove(true);
            Print("После Remove(true)", list);

            // ---- CopyTo: скопировать элементы в обычный массив ----
            // Массив должен быть типа object[] и достаточного размера.
            object[] array = new object[list.Count];
            list.CopyTo(array);
            Console.WriteLine("CopyTo: в массиве " + array.Length + " элементов, последний: " + array[array.Length - 1]);

            // ---- Reverse: перевернуть порядок ----
            list.Reverse();
            Print("После Reverse", list);

            // ---- Sort: отсортировать ----
            // Здесь я наткнулся на ошибку. Чтобы сортировать, элементы нужно
            // сравнивать между собой. А как сравнить число 10 и строку "яблоко"?
            // Никак — поэтому обычный Sort() для разнородной коллекции
            // выбрасывает исключение.
            try
            {
                list.Sort();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Sort() без параметров не сработал: " + ex.GetType().Name);
            }

            // Решение: передать в Sort свой объект-"сравниватель" (IComparer),
            // который объясняет, как сравнивать ЛЮБЫЕ два элемента.
            // Наш TextComparer сравнивает их как строки.
            list.Sort(new TextComparer());
            Print("После Sort(new TextComparer()) — по алфавиту как текст", list);

            // ---- Clear: удалить всё ----
            list.Clear();
            Console.WriteLine("После Clear элементов: " + list.Count);
        }
    }
}
