# Практическая работа №8. Задание 1 — Коллекции

Папка проекта: `Lab8_Task1/CollectionsDemo/` — консольное приложение, четыре файла.

> **Как читать этот файл.** Код разбит на фрагменты, под каждым — пояснение по номерам строк (`стр. N`).
> Фигурные скобки, пустые строки, комментарии и строки, которые только печатают заголовок, отдельно не расписываются.
> WPF в этой работе нет.
>
> | Пункт задания | Файл |
> |---|---|
> | 1. Коллекция из разнородных элементов и её методы | `HeterogeneousDemo.cs` |
> | 2. Своя реализация `IEnumerable` и `IEnumerator` | `EnumerableDemo.cs` |
> | 3. Стек или очередь с помощью коллекции | `StackQueueDemo.cs` |

---

## 1. Немного теории

**Коллекция** — объект, который хранит набор других объектов и, в отличие от массива, сам меняет свой размер.

| Вид | Пространство имён | Примеры | Что хранит |
|---|---|---|---|
| Необобщённые («старые») | `System.Collections` | `ArrayList`, `Hashtable`, `Stack`, `Queue` | элементы типа `object` — что угодно вперемешку |
| Обобщённые | `System.Collections.Generic` | `List<T>`, `Dictionary<K,V>`, `Stack<T>`, `Queue<T>` | элементы одного типа `T`, проверяется компилятором |
| Потокобезопасные | `System.Collections.Concurrent` | `ConcurrentQueue<T>`, `ConcurrentDictionary<K,V>` | для работы из нескольких потоков |

**Упаковка (boxing).** Когда значение-число кладётся в переменную типа `object`, оно «заворачивается» в объект в куче. Обратное действие — распаковка — требует явного приведения типа. Это медленнее и небезопаснее обобщённых коллекций, поэтому в новом коде обычно используют `List<T>`.

**Иерархия интерфейсов.** Все коллекции реализуют общие интерфейсы — поэтому с ними работают одинаково:

```
IEnumerable   — можно перебрать через foreach
    └── ICollection   — + Count, CopyTo
            ├── IList        — + доступ по индексу, Add, Insert, Remove
            └── IDictionary  — + доступ по ключу
```

**Как работает `foreach`.** Он умеет перебирать любой объект, у которого есть метод `GetEnumerator()`:

```csharp
foreach (string book in shelf) { ... }

// превращается компилятором в:
IEnumerator e = shelf.GetEnumerator();
while (e.MoveNext())
{
    string book = (string)e.Current;
    ...
}
```

**Стек и очередь:**

| | Стек (`Stack`) | Очередь (`Queue`) |
|---|---|---|
| Правило | LIFO — последним пришёл, первым ушёл | FIFO — первым пришёл, первым ушёл |
| Добавить | `Push` | `Enqueue` |
| Забрать | `Pop` | `Dequeue` |
| Посмотреть, не забирая | `Peek` | `Peek` |
| Пример из жизни | стопка тарелок, кнопка «Назад» | очередь в магазине, очередь печати |

---

## 2. `HeterogeneousDemo.cs` — разнородная коллекция `ArrayList`

### 2.1. Свой «сравниватель»

```csharp
1   using System;
2   using System.Collections;
4   namespace CollectionsDemo
27      public class TextComparer : IComparer
33          public int Compare(object x, object y)
37              return string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal);
```

- **стр. 2** — `ArrayList` и `IComparer` (необобщённые коллекции).
- **стр. 27** — класс реализует `IComparer` — интерфейс «умею сравнивать два объекта».
- **стр. 33** — единственный метод интерфейса. Должен вернуть отрицательное число, если `x` «меньше» `y`, ноль — если равны, положительное — если «больше».
- **стр. 37** — оба элемента превращаются в строки и сравниваются как текст. `Ordinal` — простое сравнение по кодам символов.

Зачем он нужен — см. `Sort` ниже.

### 2.2. Вспомогательный `Print`

```csharp
41      public static class HeterogeneousDemo
44          private static void Print(string title, ArrayList list)
46              Console.WriteLine(title + " (элементов: " + list.Count + ")");
47              for (int i = 0; i < list.Count; i++)
50                  Console.WriteLine("   [" + i + "] " + list[i] + "   <- " + list[i].GetType().Name);
```

- **стр. 46** — `Count` — сколько элементов сейчас в коллекции.
- **стр. 47–50** — к элементам можно обращаться по индексу, как в массиве. `GetType().Name` показывает **настоящий** тип элемента, хотя `list[i]` имеет тип `object`.

### 2.3. `Add` и чтение элементов

```csharp
54          public static void Run()
56              ArrayList list = new ArrayList();
59              list.Add(10);
60              list.Add("яблоко");
61              list.Add(3.14);
62              list.Add(true);
63              list.Add('Z');
64              list.Add(new DateTime(2026, 9, 1));
65              Print("После Add", list);
68              int number = (int)list[0];
69              Console.WriteLine("Первый элемент как int: " + number + ", плюс 5 = " + (number + 5));
73              if (list[1] is string text)
75                  Console.WriteLine("Второй элемент — строка длиной " + text.Length);
```

- **стр. 56** — пустая коллекция; размер задавать не нужно.
- **стр. 59–64** — **`Add`** добавляет в конец. Шесть элементов шести разных типов: `int`, `string`, `double`, `bool`, `char`, `DateTime`. Это возможно, потому что любой тип — наследник `object`. Числа при этом упаковываются.
- **стр. 68** — элемент приходит как `object`; чтобы работать с ним как с числом, нужно **явное приведение** `(int)` (распаковка). Ошибка в типе даст `InvalidCastException` во время работы.
- **стр. 73** — безопасный способ: `is string text` проверяет тип и сразу кладёт значение в переменную.

### 2.4. `Insert`, `IndexOf`, `Remove`, `CopyTo`, `Reverse`

```csharp
79              list.Insert(1, "вставлено");
83              Console.WriteLine("IndexOf(\"яблоко\") = " + list.IndexOf("яблоко"));
84              Console.WriteLine("IndexOf(\"груша\")  = " + list.IndexOf("груша") + " (не найдено)");
87              list.Remove(true);
92              object[] array = new object[list.Count];
93              list.CopyTo(array);
94              Console.WriteLine("CopyTo: в массиве " + array.Length + " элементов, последний: " + array[array.Length - 1]);
97              list.Reverse();
```

- **стр. 79** — **`Insert(индекс, элемент)`** вставляет на указанное место, остальные сдвигаются вправо.
- **стр. 83–84** — **`IndexOf`** возвращает номер первого найденного элемента или `-1`, если такого нет. `\"` — кавычка внутри строки.
- **стр. 87** — **`Remove`** удаляет первый элемент с таким значением.
- **стр. 92–93** — **`CopyTo`** копирует элементы в обычный массив; массив должен быть типа `object[]` и достаточного размера.
- **стр. 97** — **`Reverse`** переворачивает порядок.

После каждой операции вызывается `Print` (стр. 80, 88, 98), чтобы увидеть результат.

### 2.5. `Sort` и `Clear`

```csharp
107                 list.Sort();
109             catch (InvalidOperationException ex)
111                 Console.WriteLine("Sort() без параметров не сработал: " + ex.GetType().Name);
117             list.Sort(new TextComparer());
118             Print("После Sort(new TextComparer()) — по алфавиту как текст", list);
121             list.Clear();
122             Console.WriteLine("После Clear элементов: " + list.Count);
```

- **стр. 107** — обычный **`Sort()`** сравнивает элементы между собой. Сравнить число `10` и строку `"яблоко"` невозможно — возникает исключение.
- **стр. 109–111** — оно перехватывается, программа не падает.
- **стр. 117** — решение: передать в `Sort` свой объект-«сравниватель», который умеет сравнивать **любые** два элемента (здесь — как текст).
- **стр. 121–122** — **`Clear`** удаляет все элементы; `Count` становится 0.

---

## 3. `EnumerableDemo.cs` — свои `IEnumerable` и `IEnumerator`

### 3.1. Коллекция

```csharp
1   using System;
2   using System.Collections;
3   using System.Collections.Generic;
31      public class Bookshelf : IEnumerable
33          private readonly string[] books;
36          public Bookshelf(params string[] books)
38              this.books = books;
45          public IEnumerator GetEnumerator()
47              return new BookEnumerator(books);
```

- **стр. 2** — `IEnumerable`, `IEnumerator`; **стр. 3** — обобщённый `IEnumerable<T>`.
- **стр. 31** — книжная полка реализует `IEnumerable` («перечислимый») — теперь её можно обходить через `foreach`.
- **стр. 33** — книги хранятся в закрытом массиве.
- **стр. 36** — `params` — книги можно перечислить через запятую прямо при вызове.
- **стр. 45–47** — единственный метод интерфейса. Каждый вызов создаёт **новый** перечислитель, стоящий в начале.

### 3.2. Перечислитель

```csharp
52      public class BookEnumerator : IEnumerator
54          private readonly string[] books;
58          private int position = -1;
60          public BookEnumerator(string[] books)
62              this.books = books;
66          public bool MoveNext()
68              position++;
69              return position < books.Length;
73          public object Current
75              get { return books[position]; }
79          public void Reset()
81              position = -1;
```

- **стр. 52** — перечислитель — «закладка», которая движется по коллекции.
- **стр. 58** — **важно:** сначала закладка стоит **перед** первым элементом (`-1`), а не на нём.
- **стр. 66–69** — `MoveNext`: шаг вперёд; `true`, если там есть элемент.
- **стр. 73–75** — `Current`: элемент под закладкой. Тип `object` — так требует необобщённый интерфейс.
- **стр. 79–81** — `Reset`: вернуться в начало.

Два интерфейса вместо одного нужны, чтобы одну коллекцию можно было обходить **несколькими независимыми закладками** одновременно.

### 3.3. То же через `yield`

```csharp
97      public class YieldBookshelf : IEnumerable
106         public IEnumerator GetEnumerator()
108             for (int i = 0; i < books.Length; i++)
110                 yield return books[i];
```

- **стр. 97–103** — такая же полка (поле и конструктор — стр. 99–103).
- **стр. 110** — `yield return` — «отдай элемент и **запомни**, где остановился». Компилятор сам создаёт скрытый класс-перечислитель с `MoveNext`/`Current`. Отдельный класс вроде `BookEnumerator` писать не нужно.

### 3.4. Демонстрация

```csharp
122         private static IEnumerable<int> Squares(int count)
124             for (int i = 1; i <= count; i++)
126                 Console.WriteLine("      (метод Squares вычисляет квадрат числа " + i + ")");
127                 yield return i * i;
```

- **стр. 122–127** — `yield` в обычном методе, возвращающем `IEnumerable<int>`. Числа нигде не хранятся — каждое вычисляется в момент, когда его попросили.

```csharp
133             Bookshelf shelf = new Bookshelf("Война и мир", "Мастер и Маргарита", "Идиот");
137             foreach (string book in shelf)
139                 Console.WriteLine("   " + book);
145             IEnumerator enumerator = shelf.GetEnumerator();
146             while (enumerator.MoveNext())
148                 string book = (string)enumerator.Current;
149                 Console.WriteLine("   " + book);
```

- **стр. 137–139** — `foreach` работает с моим классом.
- **стр. 145–149** — то же **вручную**: взять перечислитель, шагать `MoveNext`, читать `Current` с приведением типа. Именно в такой код компилятор превращает `foreach`.

```csharp
154             foreach (string first in shelf)
156                 foreach (string second in shelf)
159                     if (string.CompareOrdinal(first, second) < 0)
161                         Console.WriteLine("   " + first + " + " + second);
168             foreach (string book in new YieldBookshelf("Обломов", "Ревизор"))
176             foreach (int square in Squares(3))
178                 Console.WriteLine("   получили " + square);
```

- **стр. 154–161** — вложенные циклы: два независимых перечислителя на одной полке. Сравнение строк нужно, чтобы каждая пара вывелась один раз.
- **стр. 168** — полка на `yield` обходится так же.
- **стр. 176–178** — «ленивость»: в выводе сообщения «вычисляет квадрат» и «получили» **чередуются** — значение считается только тогда, когда цикл до него дошёл.

---

## 4. `StackQueueDemo.cs` — стек и очередь

### 4.1. Свой стек на основе `List<T>`

```csharp
1   using System;
2   using System.Collections.Generic;
35      public class MyStack<T>
39          private readonly List<T> items = new List<T>();
42          public int Count
44              get { return items.Count; }
48          public void Push(T item)
50              items.Add(item);
```

- **стр. 35** — `<T>` — **обобщённый класс**: вместо `T` подставляется тип, указанный при создании (`MyStack<string>`).
- **стр. 39** — внутри — список. Вершина стека — **последний** элемент: добавлять и удалять в конце списка быстро, ничего сдвигать не надо.
- **стр. 42–44** — количество элементов.
- **стр. 48–50** — `Push`: положить на вершину.

```csharp
54          public T Pop()
56              if (items.Count == 0)
59                  throw new InvalidOperationException("Стек пуст");
62              int last = items.Count - 1;
63              T item = items[last];
64              items.RemoveAt(last);
65              return item;
69          public T Peek()
71              if (items.Count == 0)
73                  throw new InvalidOperationException("Стек пуст");
75              return items[items.Count - 1];
```

- **стр. 56–59** — из пустого стека брать нечего — сообщаем исключением (так же ведёт себя стандартный `Stack<T>`).
- **стр. 62–65** — `Pop`: запомнить последний элемент, удалить его из списка, вернуть.
- **стр. 69–75** — `Peek`: вернуть вершину, **не удаляя**.

### 4.2. Демонстрация

```csharp
85              MyStack<string> history = new MyStack<string>();
86              history.Push("главная");
87              history.Push("каталог");
88              history.Push("товар");
89              Console.WriteLine("   открыто страниц: " + history.Count + ", сейчас на экране: " + history.Peek());
92–93           Console.WriteLine("   Назад: закрыли «" + history.Pop() + "», теперь «" + history.Peek() + "»");
95              history.Pop();
98                  history.Pop();
100             catch (InvalidOperationException ex)
102                 Console.WriteLine("   Ещё раз Назад: исключение «" + ex.Message + "»");
```

- **стр. 85–88** — история страниц браузера: три страницы в стеке.
- **стр. 89** — на вершине последняя открытая.
- **стр. 92–93** — кнопка «Назад»: `Pop` снимает верхнюю, `Peek` показывает новую вершину. Страницы закрываются в порядке, **обратном** открытию.
- **стр. 95** — снята последняя, стек пуст.
- **стр. 98–102** — ещё один `Pop` — исключение с нашим текстом.

```csharp
107             Stack<int> stack = new Stack<int>();
108–110         stack.Push(1); stack.Push(2); stack.Push(3);
112             Console.Write("  ");
113             while (stack.Count > 0)
115                 Console.Write(" " + stack.Pop());
```

- **стр. 107–115** — готовый `Stack<int>`: положили 1, 2, 3 — достали 3, 2, 1.

```csharp
121             Queue<string> printQueue = new Queue<string>();
124–126         printQueue.Enqueue("отчёт.docx"); ... ("таблица.xlsx"); ... ("фото.png");
127             Console.WriteLine("   в очереди: " + printQueue.Count + ", первым напечатается: " + printQueue.Peek());
130             while (printQueue.Count > 0)
132                 Console.WriteLine("   печатается " + printQueue.Dequeue() + ", осталось: " + printQueue.Count);
137             string next;
138             bool got = printQueue.TryDequeue(out next);
139             Console.WriteLine("   TryDequeue на пустой очереди вернул " + got);
```

- **стр. 121–126** — очередь печати: `Enqueue` ставит в конец.
- **стр. 127** — `Peek` у очереди показывает **первого**.
- **стр. 130–132** — `Dequeue` забирает первого: документы печатаются в порядке поступления.
- **стр. 137–139** — `TryDequeue` — безопасный вариант: на пустой очереди не бросает исключение, а возвращает `false`; результат отдаётся через `out`-параметр.

### 4.3. Преимущества перед массивом (для отчёта)

1. **Размер.** Массив создаётся с фиксированным размером; коллекция растёт сама.
2. **Не нужно следить за вершиной** — помнить индекс последнего занятого элемента и проверять переполнение.
3. **Удаление.** Чтобы убрать первый элемент массива, остальные надо сдвигать вручную; у очереди есть `Dequeue`.
4. **Понятность.** `Push`/`Pop`/`Enqueue`/`Dequeue` сразу говорят о намерении; с массивом были бы «голые» индексы.
5. **Защита от ошибок.** У стека доступна только вершина — нельзя случайно изменить середину.

---

## 5. `Program.cs`

```csharp
1   using System;
2   using System.Text;
4   namespace CollectionsDemo
18      internal class Program
20          private static void Main()
22              Console.OutputEncoding = Encoding.UTF8;
25              HeterogeneousDemo.Run();
29              EnumerableDemo.Run();
33              StackQueueDemo.Run();
36              if (!Console.IsInputRedirected)
40                  Console.ReadKey();
```

- **стр. 22** — UTF-8 для вывода кириллицы.
- **стр. 25–33** — три демонстрации по очереди; перед каждой печатается заголовок (стр. 24, 28, 32).
- **стр. 36–40** — ожидание клавиши, если программа запущена в обычной консоли.

---

## 6. Важные моменты (вопросы на защите)

1. **Почему в `ArrayList` можно положить элементы разных типов?** — Он хранит `object`, а любой тип — наследник `object`.
2. **Что такое упаковка и распаковка?** — Превращение значения (например, `int`) в объект в куче и обратно; обратное требует явного приведения типа.
3. **Почему `Sort()` упал?** — Элементы разных типов нельзя сравнить между собой; нужен свой `IComparer`.
4. **`ArrayList` или `List<T>`?** — `List<T>`: типы проверяет компилятор, нет упаковки и приведений. `ArrayList` — когда действительно нужны разные типы.
5. **Чем `IEnumerable` отличается от `IEnumerator`?** — Первый — коллекция («меня можно обойти»), второй — «закладка», которая обходит.
6. **Почему `position` начинается с −1?** — `foreach` сначала вызывает `MoveNext`, и только потом читает `Current`.
7. **Что делает `yield return`?** — Отдаёт элемент и запоминает место; компилятор сам генерирует перечислитель. Вычисления «ленивые».
8. **Можно ли менять коллекцию внутри `foreach`?** — У стандартных коллекций нельзя: будет `InvalidOperationException`.
9. **Стек и очередь — в чём разница?** — LIFO и FIFO.
10. **Чем `Pop` отличается от `Peek`?** — `Pop` удаляет элемент, `Peek` только показывает.
