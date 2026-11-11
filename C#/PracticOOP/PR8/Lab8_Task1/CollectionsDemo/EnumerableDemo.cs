using System;
using System.Collections;           // IEnumerable, IEnumerator
using System.Collections.Generic;   // IEnumerable<T>

namespace CollectionsDemo
{
    // =========================================================================
    //  Пункт 2. Своя реализация IEnumerable и IEnumerator.
    //
    //  Раньше я не задумывался, почему foreach работает с массивами и списками.
    //  Оказалось, foreach умеет перебирать ЛЮБОЙ объект, у которого есть
    //  метод GetEnumerator(), то есть который реализует интерфейс IEnumerable.
    //
    //  Интерфейсов два, и у них разные роли:
    //
    //    IEnumerable ("перечислимый") — сама коллекция. Один метод:
    //        GetEnumerator() — "дай мне того, кто будет тебя обходить".
    //
    //    IEnumerator ("перечислитель") — "закладка", которая движется
    //    по коллекции. Три члена:
    //        MoveNext() — шагнуть к следующему элементу; false, если элементы кончились;
    //        Current    — элемент, на котором сейчас стоит закладка;
    //        Reset()    — вернуться в начало.
    //
    //  Зачем два интерфейса, а не один? Чтобы одну коллекцию можно было
    //  обходить несколькими независимыми "закладками" одновременно
    //  (например, вложенными циклами foreach).
    // =========================================================================

    /// <summary>Моя коллекция: книжная полка.</summary>
    public class Bookshelf : IEnumerable
    {
        private readonly string[] books;

        // params — книги можно перечислить через запятую прямо при создании
        public Bookshelf(params string[] books)
        {
            this.books = books;
        }

        /// <summary>
        /// Единственный метод интерфейса IEnumerable.
        /// Каждый вызов создаёт НОВУЮ закладку, стоящую в начале.
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return new BookEnumerator(books);
        }
    }

    /// <summary>Перечислитель ("закладка") для книжной полки.</summary>
    public class BookEnumerator : IEnumerator
    {
        private readonly string[] books;

        // Важный момент: сначала закладка стоит ПЕРЕД первым элементом
        // (позиция -1), а не на нём. Первый же MoveNext() переведёт её на 0.
        private int position = -1;

        public BookEnumerator(string[] books)
        {
            this.books = books;
        }

        /// <summary>Шаг вперёд. Возвращает true, если там есть элемент.</summary>
        public bool MoveNext()
        {
            position++;
            return position < books.Length;
        }

        /// <summary>Текущий элемент. Тип object — так требует интерфейс.</summary>
        public object Current
        {
            get { return books[position]; }
        }

        /// <summary>Вернуть закладку в начало.</summary>
        public void Reset()
        {
            position = -1;
        }
    }

    // =========================================================================
    //  То же самое через yield.
    //
    //  Писать отдельный класс-перечислитель каждый раз долго. Слово yield
    //  просит компилятор сделать это за нас: он сам создаёт скрытый класс
    //  с MoveNext/Current.
    //
    //  yield return значит: "отдай этот элемент и ЗАПОМНИ, где остановился".
    //  Когда foreach попросит следующий элемент, метод продолжится с того же места.
    // =========================================================================

    /// <summary>Та же полка, но перечислитель написан через yield.</summary>
    public class YieldBookshelf : IEnumerable
    {
        private readonly string[] books;

        public YieldBookshelf(params string[] books)
        {
            this.books = books;
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < books.Length; i++)
            {
                yield return books[i];
            }
        }
    }

    public static class EnumerableDemo
    {
        /// <summary>
        /// yield можно писать и в обычном методе, который возвращает IEnumerable.
        /// Числа здесь не хранятся в массиве — каждое вычисляется в момент,
        /// когда его попросили.
        /// </summary>
        private static IEnumerable<int> Squares(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                Console.WriteLine("      (метод Squares вычисляет квадрат числа " + i + ")");
                yield return i * i;
            }
        }

        public static void Run()
        {
            Bookshelf shelf = new Bookshelf("Война и мир", "Мастер и Маргарита", "Идиот");

            // 1) foreach работает с моим классом, потому что тот реализует IEnumerable
            Console.WriteLine("Обход через foreach:");
            foreach (string book in shelf)
            {
                Console.WriteLine("   " + book);
            }

            // 2) То же самое "вручную" — именно в такой код компилятор
            //    превращает foreach.
            Console.WriteLine("Обход вручную (то, что делает foreach):");
            IEnumerator enumerator = shelf.GetEnumerator();   // взяли закладку
            while (enumerator.MoveNext())                     // шаг; есть ли элемент?
            {
                string book = (string)enumerator.Current;     // взяли элемент
                Console.WriteLine("   " + book);
            }

            // 3) Две независимые закладки на одной полке (вложенные циклы)
            Console.WriteLine("Все пары разных книг (два перечислителя сразу):");
            foreach (string first in shelf)
            {
                foreach (string second in shelf)
                {
                    // Сравнение, чтобы каждая пара вывелась один раз
                    if (string.CompareOrdinal(first, second) < 0)
                    {
                        Console.WriteLine("   " + first + " + " + second);
                    }
                }
            }

            // 4) Версия с yield — класса-перечислителя писать не пришлось
            Console.WriteLine("Полка, написанная через yield:");
            foreach (string book in new YieldBookshelf("Обломов", "Ревизор"))
            {
                Console.WriteLine("   " + book);
            }

            // 5) "Ленивость" yield: по сообщениям видно, что квадрат считается
            //    только тогда, когда цикл дошёл до него, а не все сразу заранее.
            Console.WriteLine("Метод с yield return (считает по одному):");
            foreach (int square in Squares(3))
            {
                Console.WriteLine("   получили " + square);
            }
        }
    }
}
