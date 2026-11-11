using System;
using System.Collections.Generic;   // List<T>, Stack<T>, Queue<T>

namespace CollectionsDemo
{
    // =========================================================================
    //  Пункт 3. Стек и очередь с помощью коллекции.
    //
    //  СТЕК (Stack)   — "последним пришёл — первым ушёл" (LIFO).
    //                   Как стопка тарелок: берём всегда верхнюю.
    //  ОЧЕРЕДЬ (Queue) — "первым пришёл — первым ушёл" (FIFO).
    //                   Как очередь в магазине.
    //
    //  Сначала я написал свой стек на основе List<T>, чтобы понять, как он
    //  устроен, а потом попробовал готовые Stack<T> и Queue<T> из .NET.
    //
    //  Чем это лучше обычного массива (для отчёта):
    //    1) Размер. Массив создаётся сразу с заданным размером. Чтобы добавить
    //       11-й элемент в массив на 10, надо создать новый массив и всё
    //       переписать. Коллекция растёт сама.
    //    2) Не нужно самому следить за "вершиной" — помнить номер последнего
    //       занятого элемента, проверять переполнение и т. д.
    //    3) Удаление. Чтобы убрать первый элемент массива, остальные надо
    //       сдвигать вручную. У очереди есть готовый Dequeue.
    //    4) Понятность. Названия Push/Pop/Enqueue/Dequeue сразу говорят,
    //       что делает код. С массивом это были бы "голые" индексы.
    //    5) Защита от ошибок. У стека нельзя случайно залезть в середину —
    //       доступна только вершина.
    // =========================================================================

    /// <summary>
    /// Мой стек. &lt;T&gt; — обобщённый тип: вместо T подставляется тот тип,
    /// который укажут при создании (MyStack&lt;string&gt;, MyStack&lt;int&gt;...).
    /// </summary>
    public class MyStack<T>
    {
        // Внутри — обычный список. Вершина стека — ПОСЛЕДНИЙ элемент списка:
        // добавлять и удалять в конце списка быстро, сдвигать ничего не надо.
        private readonly List<T> items = new List<T>();

        /// <summary>Сколько элементов в стеке.</summary>
        public int Count
        {
            get { return items.Count; }
        }

        /// <summary>Положить элемент на вершину.</summary>
        public void Push(T item)
        {
            items.Add(item);
        }

        /// <summary>Снять элемент с вершины и вернуть его.</summary>
        public T Pop()
        {
            if (items.Count == 0)
            {
                // Из пустого стека брать нечего — сообщаем об ошибке исключением
                throw new InvalidOperationException("Стек пуст");
            }

            int last = items.Count - 1;
            T item = items[last];
            items.RemoveAt(last);
            return item;
        }

        /// <summary>Посмотреть на вершину, не снимая элемент.</summary>
        public T Peek()
        {
            if (items.Count == 0)
            {
                throw new InvalidOperationException("Стек пуст");
            }
            return items[items.Count - 1];
        }
    }

    public static class StackQueueDemo
    {
        public static void Run()
        {
            // ---------------- Свой стек ----------------
            Console.WriteLine("Мой стек MyStack<string> — история страниц в браузере:");
            MyStack<string> history = new MyStack<string>();
            history.Push("главная");
            history.Push("каталог");
            history.Push("товар");
            Console.WriteLine("   открыто страниц: " + history.Count + ", сейчас на экране: " + history.Peek());

            // Кнопка "Назад": снимаем верхнюю страницу
            Console.WriteLine("   Назад: закрыли «" + history.Pop() + "», теперь «" + history.Peek() + "»");
            Console.WriteLine("   Назад: закрыли «" + history.Pop() + "», теперь «" + history.Peek() + "»");

            history.Pop();   // сняли последнюю, стек пуст
            try
            {
                history.Pop();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("   Ещё раз Назад: исключение «" + ex.Message + "»");
            }

            // ---------------- Готовый Stack<T> ----------------
            Console.WriteLine("Готовый Stack<int>: кладём 1, 2, 3 — достаём в обратном порядке:");
            Stack<int> stack = new Stack<int>();
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);

            Console.Write("  ");
            while (stack.Count > 0)
            {
                Console.Write(" " + stack.Pop());
            }
            Console.WriteLine();

            // ---------------- Готовая Queue<T> ----------------
            Console.WriteLine("Готовая Queue<string> — очередь печати документов:");
            Queue<string> printQueue = new Queue<string>();

            // Enqueue — встать в конец очереди
            printQueue.Enqueue("отчёт.docx");
            printQueue.Enqueue("таблица.xlsx");
            printQueue.Enqueue("фото.png");
            Console.WriteLine("   в очереди: " + printQueue.Count + ", первым напечатается: " + printQueue.Peek());

            // Dequeue — забрать того, кто стоит первым
            while (printQueue.Count > 0)
            {
                Console.WriteLine("   печатается " + printQueue.Dequeue() + ", осталось: " + printQueue.Count);
            }

            // TryDequeue — безопасный вариант: не бросает исключение на пустой
            // очереди, а возвращает false.
            string next;
            bool got = printQueue.TryDequeue(out next);
            Console.WriteLine("   TryDequeue на пустой очереди вернул " + got);

            Console.WriteLine("Преимущества перед массивом: размер растёт сам, не надо следить за индексом");
            Console.WriteLine("вершины и сдвигать элементы, названия методов понятны, нельзя залезть в середину.");
        }
    }
}
