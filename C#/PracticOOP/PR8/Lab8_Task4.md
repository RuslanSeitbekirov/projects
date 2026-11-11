# Практическая работа №8. Задание 4 — Сериализация в JSON

Папка проекта: `Lab8_Task4/JsonDemo/` — консольное приложение, два файла.

> **Как читать этот файл.** Код разбит на фрагменты, под каждым — пояснение по номерам строк (`стр. N`).
> Фигурные скобки, пустые строки, комментарии и строки, которые только печатают пояснительный текст, отдельно не расписываются.
> WPF в этой работе нет.

---

## 1. Немного теории

**Сериализация** — превращение объекта из памяти в последовательность символов или байтов, которую можно сохранить в файл или передать по сети. **Десериализация** — обратное действие: из текста снова получается объект.

**JSON** (JavaScript Object Notation) — самый распространённый текстовый формат для этого:

| Запись | Что означает | Чему соответствует в C# |
|---|---|---|
| `{ "имя": значение, ... }` | объект | объект класса |
| `[ значение, ... ]` | массив | массив, `List<T>` |
| `"текст"` | строка (всегда в двойных кавычках) | `string`, `DateTime` |
| `12`, `4.6` | число (дробная часть через точку) | `int`, `double`, `decimal` |
| `true` / `false` | логическое значение | `bool` |
| `null` | «ничего» | `null` |

В .NET всё нужное встроено — пространство имён `System.Text.Json`, главный класс — `JsonSerializer` с двумя методами:

```csharp
string json = JsonSerializer.Serialize(объект);            // объект → текст
T obj       = JsonSerializer.Deserialize<T>(json);         // текст → объект
```

**Требования к классу:**

- в JSON попадают только **публичные свойства** (поля и `private`-свойства по умолчанию пропускаются);
- для десериализации у свойств нужен `set`, а у класса — конструктор без параметров.

**Атрибут** — «пометка» в квадратных скобках перед свойством или классом; сериализатор читает её и меняет своё поведение (`[JsonPropertyName]`, `[JsonIgnore]`).

---

## 2. `Student.cs` — классы для сериализации

```csharp
1   using System;
2   using System.Collections.Generic;
3   using System.Text.Json.Serialization;
5   namespace JsonDemo
19      public class Address
21          public string City { get; set; }
22          public string Street { get; set; }
```

- **стр. 1** — `DateTime`; **стр. 2** — `List<T>`; **стр. 3** — атрибуты `[JsonPropertyName]`, `[JsonIgnore]`.
- **стр. 19–22** — адрес: будет **вложенным объектом** внутри студента. Конструктор не написан — значит, есть автоматический конструктор без параметров.

```csharp
26      public class Student
28          public string Name { get; set; }
29          public int Age { get; set; }
30          public double AverageMark { get; set; }
31          public bool IsBudget { get; set; }
32          public DateTime BirthDate { get; set; }
39          [JsonPropertyName("group")]
40          public string GroupName { get; set; }
46          [JsonIgnore]
47          public string Password { get; set; }
50          public List<int> Marks { get; set; }
53          public Address Address { get; set; }
```

- **стр. 28–32** — свойства пяти разных типов: строка, целое, дробное, логическое, дата. Все публичные, с `get` и `set`.
- **стр. 39–40** — атрибут задаёт **имя в JSON**: в C# свойство называется `GroupName`, в тексте будет `"group"`.
- **стр. 46–47** — атрибут **исключает** свойство из JSON: пароль не сохраняется и не читается.
- **стр. 50** — список → массив `[ ... ]`.
- **стр. 53** — вложенный объект → вложенные `{ ... }`.

---

## 3. `Program.cs`

### 3.1. Подключения и исходный объект

```csharp
1   using System;
2   using System.Collections.Generic;
3   using System.IO;
4   using System.Text;
5   using System.Text.Encodings.Web;
6   using System.Text.Json;
7   using System.Text.Unicode;
9   namespace JsonDemo
29      internal class Program
31          private static void Main()
33              Console.OutputEncoding = Encoding.UTF8;
```

- **стр. 3** — `File`, `Path`, `FileInfo`.
- **стр. 5** — `JavaScriptEncoder` — настройка записи символов.
- **стр. 6** — `JsonSerializer`, `JsonSerializerOptions`, `JsonException`.
- **стр. 7** — `UnicodeRanges` — наборы символов.

```csharp
36              Student student = new Student
38                  Name = "Иванов Иван",
39                  Age = 19,
40                  AverageMark = 4.6,
41                  IsBudget = true,
42                  BirthDate = new DateTime(2007, 3, 15),
43                  GroupName = "ПИ-21",
44                  Password = "qwerty123",
45                  Marks = new List<int> { 5, 4, 5, 4 },
46                  Address = new Address { City = "Симферополь", Street = "ул. Ленина, 1" }
```

- **стр. 36–46** — **инициализатор объекта**: создание и заполнение свойств в одном выражении. Вложенный адрес и список оценок создаются тут же.

### 3.2. Раздел 1 — сериализация по умолчанию

```csharp
55              string compactJson = JsonSerializer.Serialize(student);
56              Console.WriteLine(compactJson);
```

- **стр. 55** — весь объект превращается в строку **одним вызовом**; вложенный адрес и список обрабатываются сами.
- **стр. 56** — результат (строки 58–64 поясняют его):
  - всё в одну строку — компактно, но человеку читать трудно;
  - русские буквы записаны кодами вида `И` — сериализатор по умолчанию перестраховывается; данные при этом не портятся;
  - вместо `GroupName` стоит `"group"`;
  - пароля нет.

### 3.3. Раздел 2 — сериализация с настройками

```csharp
70              JsonSerializerOptions options = new JsonSerializerOptions
73                  WriteIndented = true,
76                  Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
80              string prettyJson = JsonSerializer.Serialize(student, options);
81              Console.WriteLine(prettyJson);
```

- **стр. 70** — объект настроек.
- **стр. 73** — писать с отступами и переносами строк.
- **стр. 76** — разрешить записывать латиницу и кириллицу **как есть**, без кодов `\uXXXX`.
- **стр. 80** — тот же метод, настройки — вторым параметром. Результат:

```json
{
  "Name": "Иванов Иван",
  "Age": 19,
  "AverageMark": 4.6,
  "IsBudget": true,
  "BirthDate": "2007-03-15T00:00:00",
  "group": "ПИ-21",
  "Marks": [ 5, 4, 5, 4 ],
  "Address": {
    "City": "Симферополь",
    "Street": "ул. Ленина, 1"
  }
}
```

Дата записана строкой в стандартном формате ISO 8601. (В настоящем выводе каждый элемент массива стоит на отдельной строке.)

### 3.4. Раздел 3 — десериализация

```csharp
90              Student copy = JsonSerializer.Deserialize<Student>(prettyJson);
92–97           Console.WriteLine(... copy.Name / copy.Age / copy.GroupName / copy.BirthDate.ToShortDateString() / string.Join(", ", copy.Marks) / copy.Address.City);
100             Console.WriteLine("Пароль:         " + (copy.Password == null ? "null (не сохранялся)" : copy.Password));
103             Console.WriteLine("Это тот же самый объект? " + ReferenceEquals(student, copy));
```

- **стр. 90** — в угловых скобках указывается, объект **какого класса** нужен. Сериализатор создаёт новый `Student` и заполняет свойства значениями из текста, сопоставляя их по именам.
- **стр. 92–97** — все данные восстановились, включая дату, список и вложенный адрес.
- **стр. 100** — пароль не записывался — в копии он `null`.
- **стр. 103** — `ReferenceEquals` → `False`: копия — **новый объект** с теми же данными.

### 3.5. Раздел 4 — файл

```csharp
109             string path = Path.Combine(AppContext.BaseDirectory, "student.json");
113             File.WriteAllText(path, prettyJson, Encoding.UTF8);
114             Console.WriteLine("Записан файл: " + path);
115             Console.WriteLine("Размер: " + new FileInfo(path).Length + " байт");
118             string textFromFile = File.ReadAllText(path, Encoding.UTF8);
119             Student fromFile = JsonSerializer.Deserialize<Student>(textFromFile);
120             Console.WriteLine("Из файла прочитан студент: " + fromFile.Name + ", балл " + fromFile.AverageMark);
```

- **стр. 109** — файл рядом с exe-файлом программы.
- **стр. 113** — JSON — обычный текст, поэтому сохраняется как текстовый файл (класс `File` — практика 7). Такой файл открывается в Блокноте.
- **стр. 115** — размер через `FileInfo`.
- **стр. 118–119** — прочитать текст и десериализовать.

### 3.6. Раздел 5 — список объектов

```csharp
126             List<Student> group = new List<Student>
128                 new Student { Name = "Петрова Анна", Age = 20, GroupName = "ПИ-21", Marks = new List<int> { 5, 5 } },
129                 new Student { Name = "Сидоров Олег", Age = 18, GroupName = "ИВТ-22", Marks = new List<int> { 3, 4 } }
133             string listJson = JsonSerializer.Serialize(group, options);
134             Console.WriteLine(listJson);
138             List<Student> groupCopy = JsonSerializer.Deserialize<List<Student>>(listJson);
139             Console.WriteLine("Прочитано студентов: " + groupCopy.Count + ", второй: " + groupCopy[1].Name);
```

- **стр. 126–129** — список из двух студентов; часть свойств не заполнена.
- **стр. 133** — список сериализуется так же; в JSON это массив объектов `[ {...}, {...} ]`.
- Незаполненные свойства тоже записываются со значениями по умолчанию: `"Address": null`, `"IsBudget": false`, `"AverageMark": 0`, дата `0001-01-01`.
- **стр. 138** — при чтении указывается тип `List<Student>`.

### 3.7. Раздел 6 — «чужой» JSON и ошибки

```csharp
147             string manual = @"{ ""Name"": ""Гость"", ""Age"": 25, ""Hobby"": ""шахматы"" }";
148             Student guest = JsonSerializer.Deserialize<Student>(manual);
153–154         Console.WriteLine("Имя: " + guest.Name + ", возраст: " + guest.Age + ", группа: " + (guest.GroupName == null ? "null" : guest.GroupName));
```

- **стр. 147** — `@"..."` — **буквальная строка**: кавычка внутри неё пишется удвоенной `""`.
- **стр. 148–154** — свойства `Hobby` в классе нет — оно **молча пропущено**. Свойств, которых нет в JSON, сериализатор не трогает — в них остаются значения по умолчанию (`null`).

```csharp
157             string lowerCase = @"{ ""name"": ""Гость"", ""age"": 25 }";
158             Student strict = JsonSerializer.Deserialize<Student>(lowerCase);
159–160         ... (strict.Name == null ? "null" : strict.Name) + " (не совпал регистр)"
163             JsonSerializerOptions relaxed = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
164             Student tolerant = JsonSerializer.Deserialize<Student>(lowerCase, relaxed);
165             Console.WriteLine("То же с PropertyNameCaseInsensitive:  Name = " + tolerant.Name);
```

- **стр. 157–160** — по умолчанию **регистр имён важен**: `"name"` и `Name` — разные имена, свойство осталось `null`.
- **стр. 163–165** — настройка `PropertyNameCaseInsensitive` отключает учёт регистра — имя прочиталось.

```csharp
169             string broken = @"{ ""Name"": ""Гость"", ""Age"": }";
172                 JsonSerializer.Deserialize<Student>(broken);
174             catch (JsonException)
179             if (!Console.IsInputRedirected)
183                 Console.ReadKey();
187         private static void Title(string text)
190             Console.WriteLine("========== " + text + " ==========");
```

- **стр. 169** — испорченный JSON: после `"Age":` нет значения.
- **стр. 172–174** — десериализация бросает `JsonException`; оно перехвачено (строка 176 сообщает об этом). Чужие данные всегда читают внутри `try/catch`.
- **стр. 179–183** — ожидание клавиши в обычной консоли.
- **стр. 187–190** — заголовок раздела (строка 189 печатает пустую строку перед ним).

---

## 4. Важные моменты (вопросы на защите)

1. **Что такое сериализация?** — Преобразование объекта в текст или байты для хранения или передачи; десериализация — обратное.
2. **Какие члены класса попадают в JSON?** — Публичные свойства с `get`. Поля, закрытые свойства и методы — нет.
3. **Что нужно классу для десериализации?** — Конструктор без параметров и `set` у свойств.
4. **Зачем `[JsonPropertyName]` и `[JsonIgnore]`?** — Первое задаёт имя свойства в JSON, второе исключает свойство (например, пароль).
5. **Почему русские буквы превратились в `И`?** — По умолчанию сериализатор экранирует все символы, кроме базовой латиницы; это исправляется настройкой `Encoder`. При десериализации коды читаются правильно в любом случае.
6. **Что будет с лишним свойством в JSON?** — Оно игнорируется. Отсутствующее свойство остаётся со значением по умолчанию.
7. **Учитывается ли регистр имён?** — Да, по умолчанию; отключается `PropertyNameCaseInsensitive = true`.
8. **Что происходит при неправильном JSON?** — Исключение `JsonException`.
9. **Является ли десериализованный объект тем же самым?** — Нет, это новый объект с такими же значениями свойств.
10. **Чем JSON отличается от двоичного формата (практика 7)?** — JSON — текст: читается человеком и любой программой на любом языке, но занимает больше места; в двоичном файле нужно точно знать порядок и типы данных.
