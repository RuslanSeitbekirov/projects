# Практическая работа №8. Задание 3 — LINQ

Папка проекта: `Lab8_Task3/LinqDemo/` — консольное приложение, три файла.

> **Как читать этот файл.** Код разбит на фрагменты, под каждым — пояснение по номерам строк (`стр. N`).
> Фигурные скобки, пустые строки, комментарии и строки, которые только печатают заголовок, отдельно не расписываются.
> WPF в этой работе нет.

---

## 1. Немного теории

**LINQ** (Language Integrated Query) — запросы к данным, встроенные в язык C#. Вместо циклов с `if` пишется короткое описание того, **что** нужно получить.

Один и тот же запрос записывается двумя способами:

```csharp
// 1) Операторы запросов (похоже на SQL)
var result = from s in students
             where s.Age > 18
             orderby s.Name
             select s;

// 2) Методы расширения с лямбда-выражениями
var result = students.Where(s => s.Age > 18).OrderBy(s => s.Name);
```

Компилятор превращает первый способ во второй — это две формы записи одного и того же.

| Оператор запроса | Метод расширения | Что делает |
|---|---|---|
| `where` | `Where` | фильтр |
| `orderby … descending` | `OrderBy`, `OrderByDescending`, `ThenBy` | сортировка |
| `select` | `Select` | что вернуть (преобразование) |
| `group … by` | `GroupBy` | группировка |
| `join … on … equals` | `Join` | соединение двух коллекций |
| — | `Count`, `Sum`, `Average`, `Min`, `Max` | одно итоговое значение |
| — | `First`, `FirstOrDefault`, `Any`, `All` | поиск и проверки |
| — | `Take`, `Skip`, `Distinct` | часть последовательности, без повторов |
| — | `ToList`, `ToArray`, `ToDictionary` | превратить результат в коллекцию |

**Метод расширения** — статический метод, который вызывается так, будто он принадлежит другому типу. Признак — слово `this` перед первым параметром. Все методы LINQ — расширения интерфейса `IEnumerable<T>` из класса `System.Linq.Enumerable`; поэтому они работают с массивами, списками, словарями и появляются только после `using System.Linq;`.

**Отложенное выполнение.** Запрос выполняется не там, где он написан, а там, где его результат начинают перебирать (`foreach`, `ToList`, `Count`…). Переменная запроса хранит *описание*, а не готовые данные.

**Анонимный тип** — объект без имени класса, собранный «на лету»: `new { s.Name, s.Age }`. Имени у типа нет, поэтому переменную объявляют через `var`.

---

## 2. `Student.cs` — данные

```csharp
1   namespace LinqDemo
4       public class Student
6           public string Name { get; set; }
7           public string Group { get; set; }
8           public int Age { get; set; }
9           public double Mark { get; set; }
11          public Student(string name, string group, int age, double mark)
13–16           Name = name; Group = group; Age = age; Mark = mark;
20          public override string ToString()
22              return Name + " (" + Group + ", возраст " + Age + ", балл " + Mark + ")";
```

- **стр. 4–9** — студент: фамилия, группа, возраст, средний балл.
- **стр. 11–16** — конструктор заполняет свойства.
- **стр. 20–22** — `ToString`, чтобы `Console.WriteLine(student)` печатал понятную строку.

```csharp
27      public class Curator
29          public string Group { get; set; }
30          public string Teacher { get; set; }
32          public Curator(string group, string teacher)
34–35           Group = group; Teacher = teacher;
```

- **стр. 27–35** — куратор группы: вторая «таблица» для примера с `join`.

---

## 3. `MyExtensions.cs` — свои методы расширения

```csharp
1   using System.Collections.Generic;
3   namespace LinqDemo
25      public static class MyExtensions
35          public static IEnumerable<Student> OlderThan(this IEnumerable<Student> source, int age)
37              foreach (Student student in source)
39                  if (student.Age > age)
43                      yield return student;
```

- **стр. 25** — правило 1: класс с методами расширения должен быть `static`.
- **стр. 35** — правило 2: метод `static`; правило 3: перед первым параметром стоит **`this`** — оно показывает, какой тип «расширяется». Метод появится у любой последовательности студентов. При вызове `students.OlderThan(19)` коллекция `students` попадает в `source`, а `19` — в `age`.
- **стр. 37–43** — перебор и `yield return` (задание 1): элементы отдаются по одному, «лениво» — так же устроены настоящие методы LINQ.

```csharp
52          public static string Initial(this string text)
54              if (string.IsNullOrEmpty(text))
56                  return "";
58              return text.Substring(0, 1) + ".";
```

- **стр. 52** — расширять можно и встроенные типы: метод появится у всех строк.
- **стр. 54–56** — защита от пустой строки и `null`.
- **стр. 58** — первая буква и точка: `"Иванов".Initial()` → `"И."`.

---

## 4. `Program.cs`

### 4.1. Подключения, данные, `Main`

```csharp
1   using System;
2   using System.Collections.Generic;
3   using System.Linq;
4   using System.Text;
6   namespace LinqDemo
27      internal class Program
30          private static readonly List<Student> students = new List<Student>
32–38           new Student("Иванов", "ПИ-21", 19, 4.6), ... (семь студентов)
41          private static readonly List<Curator> curators = new List<Curator>
43–45           new Curator("ПИ-21", "Смирнов А.В."), ... (три куратора)
```

- **стр. 3** — **самая важная строка**: без `using System.Linq;` методов `Where`, `Select` и других у коллекций нет.
- **стр. 30–38** — **инициализатор коллекции**: список сразу заполняется объектами в фигурных скобках. Семь студентов трёх групп.
- **стр. 41–45** — три куратора.

```csharp
48          private static void Main()
50              Console.OutputEncoding = Encoding.UTF8;
53              PrintAll(students);
55              QuerySyntax();
56              MethodSyntax();
57              DeferredExecution();
58              OwnExtensions();
60              if (!Console.IsInputRedirected)
64                  Console.ReadKey();
```

- **стр. 53** — исходные данные.
- **стр. 55–58** — четыре раздела.
- **стр. 60–64** — ожидание клавиши в обычной консоли.

```csharp
72          private static void PrintAll<T>(IEnumerable<T> items)
74              foreach (T item in items)
76                  Console.WriteLine("   " + item);
80          private static void Title(string text)
83              Console.WriteLine("========== " + text + " ==========");
```

- **стр. 72** — **обобщённый метод**: параметр `IEnumerable<T>` принимает и список, и массив, и результат любого запроса LINQ.
- **стр. 80–83** — заголовок раздела (строка 82 печатает пустую строку перед ним).

### 4.2. `QuerySyntax()` — операторы запросов

```csharp
101             IEnumerable<Student> goodStudents =
102                 from s in students
103                 where s.Mark >= 4.5
104                 orderby s.Name
105                 select s;
108             PrintAll(goodStudents);
```

- **стр. 102** — `from s in students` — «для каждого `s` из `students`». В отличие от SQL запрос начинается с `from`: так IntelliSense заранее знает тип `s`.
- **стр. 103** — `where` — фильтр: балл не ниже 4.5.
- **стр. 104** — `orderby` — сортировка по фамилии.
- **стр. 105** — `select s` — вернуть самого студента.
- **стр. 108** — запрос **выполняется здесь**, при переборе внутри `PrintAll`.

```csharp
111             IEnumerable<string> names =
112                 from s in students
113                 where s.Group == "ПИ-21"
114                 select s.Name;
116             Console.WriteLine("Фамилии группы ПИ-21: " + string.Join(", ", names));
```

- **стр. 114** — `select` может вернуть **часть** объекта: результат — последовательность строк, а не студентов.

```csharp
121             var byAge =
122                 from s in students
123                 orderby s.Age descending, s.Name
124                 select s;
```

- **стр. 121** — `var` — тип переменной определяет компилятор.
- **стр. 123** — `descending` — по убыванию; через запятую — второй ключ сортировки (применяется при равенстве первого).

```csharp
133             var shortInfo =
134                 from s in students
135                 where s.Age < 20
136                 select new { s.Name, Category = s.Mark >= 4 ? "хорошист" : "отстающий" };
139             foreach (var item in shortInfo)
141                 Console.WriteLine("   " + item.Name + " — " + item.Category);
```

- **стр. 136** — **анонимный тип**: новый объект из двух свойств — `Name` (имя взято от `s.Name`) и вычисляемого `Category`. У типа нет имени — поэтому обязателен `var` (стр. 133 и 139).
- **стр. 141** — свойства анонимного объекта доступны как обычные.

```csharp
147             var groups =
148                 from s in students
149                 group s by s.Group;
152             foreach (var g in groups)
154                 Console.WriteLine("   " + g.Key + " (" + g.Count() + " чел.):");
155                 foreach (Student s in g)
157                     Console.WriteLine("      " + s.Name);
```

- **стр. 149** — **группировка**: результат — набор групп. Запрос с `group` заканчивается без `select`.
- **стр. 154** — у группы есть `Key` (по чему группировали) и методы LINQ (`Count()`).
- **стр. 155** — сама группа — последовательность студентов, её можно перебрать.

```csharp
164             var withCurators =
165                 from s in students
166                 join c in curators on s.Group equals c.Group
167                 where s.Mark < 3.5
168                 select new { s.Name, c.Teacher };
171             foreach (var item in withCurators)
173                 Console.WriteLine("   " + item.Name + " → куратор " + item.Teacher);
```

- **стр. 166** — **соединение** двух коллекций: для каждого студента находится куратор с той же группой. В `join` пишется слово **`equals`**, а не `==`.
- **стр. 167** — фильтр: балл ниже 3.5.
- **стр. 168** — результат собирается из данных **обеих** коллекций.

### 4.3. `MethodSyntax()` — методы расширения

```csharp
191             IEnumerable<string> result = students
192                 .Where(s => s.Mark >= 4.5)
193                 .OrderBy(s => s.Name)
194                 .Select(s => s.Name + " — " + s.Mark);
197             PrintAll(result);
```

- **стр. 192** — в скобках **лямбда-выражение**: `s => s.Mark >= 4.5` — «студент `s` подходит, если…».
- **стр. 191–194** — каждый метод возвращает новую последовательность, поэтому вызовы соединяются **цепочкой**. Это тот же запрос, что первый в разделе 4.2.
- **стр. 194** — `Select` преобразует каждый элемент — здесь в строку.

```csharp
201             PrintAll(students.OrderByDescending(s => s.Mark).ThenBy(s => s.Name).Take(3));
204–205         ... string.Join(", ", students.Skip(2).Take(2).Select(s => s.Name)) ...
```

- **стр. 201** — сортировка по убыванию балла, при равенстве — по фамилии (`ThenBy`); `Take(3)` — первые три.
- **стр. 204–205** — `Skip(2)` пропускает два элемента, `Take(2)` берёт следующие два — так делают постраничный вывод.

```csharp
208             ... students.Count()
209             ... students.Count(s => s.Age > 19)
210             ... students.Average(s => s.Mark).ToString("0.00")
211             ... students.Max(s => s.Mark) + " / " + students.Min(s => s.Mark)
212             ... students.Sum(s => s.Age)
215             ... students.Any(s => s.Mark == 5.0)
216             ... students.All(s => s.Age >= 18)
```

- **стр. 208–209** — количество всех и количество по условию.
- **стр. 210** — среднее; `"0.00"` — формат с двумя знаками после запятой.
- **стр. 211–212** — максимум, минимум, сумма.
- **стр. 215** — `Any` — есть ли **хотя бы один** подходящий.
- **стр. 216** — `All` — **все** ли подходят.

Эти методы возвращают одно значение и выполняют запрос **немедленно**.

```csharp
221             Student first = students.First(s => s.Group == "КБ-23");
224             Student nobody = students.FirstOrDefault(s => s.Age > 100);
225             ... (nobody == null ? "никого (null)" : nobody.Name)
228–229         ... students.Select(s => s.Group).Distinct()
```

- **стр. 221** — `First` — первый подходящий; если такого нет — **исключение**.
- **стр. 224–225** — `FirstOrDefault` в этом случае возвращает значение по умолчанию (`null`) — безопаснее.
- **стр. 228–229** — `Distinct` убирает повторы: из семи названий групп остаются три.

```csharp
233             var stats = students
234                 .GroupBy(s => s.Group)
235                 .Select(g => new { Group = g.Key, Count = g.Count(), Average = g.Average(s => s.Mark) })
236                 .OrderByDescending(x => x.Average);
238             foreach (var x in stats)
240                 Console.WriteLine("   " + x.Group + ": " + x.Count + " чел., средний балл " + x.Average.ToString("0.00"));
```

- **стр. 234** — группировка по учебной группе.
- **стр. 235** — для каждой группы — анонимный объект: название, количество, средний балл.
- **стр. 236** — сортировка групп по среднему баллу.

```csharp
244             List<Student> adults = students.Where(s => s.Age >= 20).ToList();
245             Dictionary<string, double> marks = students.ToDictionary(s => s.Name, s => s.Mark);
249             int[] numbers = { 5, 12, 7, 20, 3, 18 };
250–251         ... numbers.Where(n => n % 2 == 0).OrderBy(n => n) ...
```

- **стр. 244** — `ToList` выполняет запрос и превращает результат в список.
- **стр. 245** — `ToDictionary`: первая лямбда — ключ, вторая — значение.
- **стр. 249–251** — LINQ работает с любым массивом: чётные числа по возрастанию.

### 4.4. `DeferredExecution()` — отложенное выполнение

```csharp
266             List<int> numbers = new List<int> { 1, 2, 3 };
269             IEnumerable<int> big = numbers.Where(n => n > 1);
272             List<int> snapshot = numbers.Where(n => n > 1).ToList();
275             numbers.Add(10);
279             Console.WriteLine("Запрос без ToList (выполнился сейчас): " + string.Join(", ", big));
280             Console.WriteLine("Запрос с ToList (выполнился заранее):  " + string.Join(", ", snapshot));
```

- **стр. 269** — запрос только **описан**: ни одно число ещё не проверено.
- **стр. 272** — `ToList` выполняет запрос **сразу** и запоминает результат: `2, 3`.
- **стр. 275** — исходный список меняется **после** написания запросов.
- **стр. 279** — запрос `big` выполняется только здесь, при переборе, и «видит» добавленное число: `2, 3, 10`.
- **стр. 280** — снимок остался прежним: `2, 3`.

### 4.5. `OwnExtensions()` — свои методы расширения

```csharp
295             PrintAll(students.OlderThan(19).OrderBy(s => s.Age));
298–299         ... string.Join(" ", students.Select(s => s.Name.Initial())) ...
```

- **стр. 295** — мой метод `OlderThan` вызывается так же, как `Where`, и свободно соединяется в цепочку со стандартными методами.
- **стр. 298–299** — метод `Initial()` вызывается у строки внутри лямбды: из фамилий получаются инициалы.

---

## 5. Важные моменты (вопросы на защите)

1. **Что такое LINQ?** — Единый способ писать запросы (фильтр, сортировка, группировка) к любым коллекциям прямо в коде C#.
2. **Чем отличаются два синтаксиса?** — Только записью: операторы запросов компилятор переводит в вызовы методов расширения. Некоторые операции (`Count`, `Take`, `First`) есть только в виде методов.
3. **Что такое метод расширения?** — Статический метод статического класса со словом `this` перед первым параметром; вызывается как метод расширяемого типа.
4. **Почему без `using System.Linq` нет `Where`?** — Методы расширения видны, только когда подключено их пространство имён.
5. **Что такое отложенное выполнение?** — Запрос выполняется при переборе результата, а не при создании; `ToList`/`ToArray`/`Count` выполняют его немедленно.
6. **Чем опасно отложенное выполнение?** — Запрос выполняется заново при каждом переборе и видит изменения исходных данных.
7. **`First` и `FirstOrDefault`?** — `First` бросает исключение, если ничего не найдено; `FirstOrDefault` возвращает `null`/0.
8. **Зачем `var`?** — Для анонимных типов он обязателен: у типа нет имени.
9. **Почему в `join` пишется `equals`?** — Это требование синтаксиса: слева ключ первой коллекции, справа — второй.
10. **`Select` и `Where`?** — `Where` отбирает элементы, не меняя их; `Select` преобразует каждый элемент во что-то другое.
