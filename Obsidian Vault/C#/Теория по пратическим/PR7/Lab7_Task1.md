# Практическая работа №7. Задание 1 — Работа с файлами

Папка проекта: `Lab7_Task1/FilesDemo/` — консольное приложение, весь код в одном файле `Program.cs`.

> **Как читать этот файл.** Код разбит на фрагменты, под каждым — пояснение по номерам строк (`стр. N`).
> Фигурные скобки, пустые строки и комментарии отдельно не расписываются. WPF в этой работе нет.

---

## 1. Немного теории

Все классы для работы с файлами лежат в пространстве имён `System.IO`. Их удобно разделить на три уровня:

| Уровень | Классы | С чем работают |
|---|---|---|
| Файл целиком | `File`, `FileInfo` | создать, скопировать, переименовать, удалить, узнать размер и даты |
| Байты | `FileStream` | содержимое файла как последовательность байтов (0…255) |
| Надстройки над потоком | `StreamWriter` / `StreamReader` | **текст**: строки, кодировки |
| | `BinaryWriter` / `BinaryReader` | **двоичные данные**: `int`, `decimal`, `bool`, `string` в том виде, как они лежат в памяти |

**`File` и `FileInfo`** умеют почти одно и то же, отличие в способе использования:

| | `File` | `FileInfo` |
|---|---|---|
| Вид | статический класс | обычный класс, нужен объект |
| Путь к файлу | передаётся в каждый метод | задаётся один раз в конструкторе |
| Когда удобен | одно-два действия | много действий с одним файлом, нужны сведения о нём |

**Поток** (`Stream`) — «труба», по которой данные идут по порядку. У потока есть текущая позиция (как курсор); чтение и запись её сдвигают.

**`using`** — открытый файл занят программой, пока его не закроют. Блок `using (...) { }` гарантирует вызов `Dispose()` (закрытие файла) в конце блока — даже если внутри случилось исключение.

**Текстовый и двоичный файл.** Число `1000` в текстовом файле — это четыре символа `'1' '0' '0' '0'`; в двоичном — четыре байта `E8 03 00 00`, как в памяти. Текстовый файл можно открыть в Блокноте, двоичный — компактнее и не требует преобразования чисел в текст и обратно.

**Кодировка** — правило, как символы превращаются в байты. В UTF-8 латинская буква занимает 1 байт, русская — 2.

---

## 2. Подключения, поле, `Main`

```csharp
1   using System;
2   using System.Globalization;
3   using System.IO;
4   using System.Text;
6   namespace FilesDemo
18      internal class Program
21          private static string folder;
```

- **стр. 1** — `Console`, `Environment`, `AppContext`, `BitConverter`.
- **стр. 2** — `CultureInfo` — чтобы дробные числа читались одинаково на любом компьютере.
- **стр. 3** — все файловые классы.
- **стр. 4** — `Encoding` (кодировки).
- **стр. 18** — `internal` — класс виден только внутри своей сборки.
- **стр. 21** — папка, где программа создаёт файлы; `static`, потому что используется из статических методов.

```csharp
23          private static void Main()
25              Console.OutputEncoding = Encoding.UTF8;
29              folder = Path.Combine(AppContext.BaseDirectory, "data");
32              Directory.CreateDirectory(folder);
33              Console.WriteLine("Все файлы создаются в папке:");
34              Console.WriteLine(folder);
36              FileClassDemo();
37              FileInfoDemo();
38              FileStreamDemo();
39              StreamWriterReaderDemo();
40              BinaryWriterReaderDemo();
43              if (!Console.IsInputRedirected)
45                  Console.WriteLine();
46                  Console.WriteLine("Нажмите любую клавишу...");
47                  Console.ReadKey();
```

- **стр. 25** — UTF-8 для вывода кириллицы в консоль.
- **стр. 29** — `Path.Combine` правильно склеивает части пути (сам ставит `\`). `AppContext.BaseDirectory` — папка с exe-файлом, то есть файлы окажутся в `bin/Debug/net10.0/data`.
- **стр. 32** — создать папку; если она уже есть, ошибки не будет.
- **стр. 33–34** — показать, где искать файлы.
- **стр. 36–40** — пять опытов по очереди, по одному на группу классов.
- **стр. 43–47** — ждать клавишу, только если программа запущена в обычной консоли (при перенаправленном вводе `ReadKey` выбросил бы исключение).

```csharp
52          private static void Title(string text)
54              Console.WriteLine();
55              Console.WriteLine("========== " + text + " ==========");
```

- **стр. 52–55** — вспомогательный метод: пустая строка и заголовок раздела.

---

## 3. `FileClassDemo()` — класс `File`

```csharp
67          private static void FileClassDemo()
69              Title("1. Класс File");
71              string path = Path.Combine(folder, "notes.txt");
75              File.WriteAllText(path, "Первая строка" + Environment.NewLine);
78              File.AppendAllText(path, "Вторая строка" + Environment.NewLine);
81              File.AppendAllLines(path, new string[] { "Третья строка", "Четвёртая строка" });
84              Console.WriteLine("Файл существует: " + File.Exists(path));
```

- **стр. 71** — полный путь к файлу.
- **стр. 75** — `WriteAllText` создаёт файл или **перезаписывает** существующий. Файл сам открывается и закрывается. `Environment.NewLine` — перевод строки, принятый в системе.
- **стр. 78** — `AppendAllText` **дописывает** в конец.
- **стр. 81** — `AppendAllLines` дописывает сразу несколько строк из массива (перевод строки добавляет сам).
- **стр. 84** — `Exists` — есть ли такой файл.

```csharp
87              string allText = File.ReadAllText(path);
88              Console.WriteLine("ReadAllText вернул:");
89              Console.Write(allText);
92              string[] lines = File.ReadAllLines(path);
93              Console.WriteLine("ReadAllLines вернул строк: " + lines.Length + ", последняя: " + lines[lines.Length - 1]);
```

- **стр. 87** — весь файл одной строкой.
- **стр. 89** — `Write` без перевода строки: текст уже заканчивается переводом.
- **стр. 92** — файл в виде массива: один элемент — одна строка.
- **стр. 93** — количество строк и последняя (индекс `Length - 1`).

```csharp
97              string copyPath = Path.Combine(folder, "notes_copy.txt");
98              File.Copy(path, copyPath, true);
99              Console.WriteLine("Копия создана: " + File.Exists(copyPath));
102             string movedPath = Path.Combine(folder, "notes_renamed.txt");
103             File.Move(copyPath, movedPath, true);
104–105         Console.WriteLine("После Move: старое имя есть = ... , новое имя есть = ...");
108             File.Delete(movedPath);
109             Console.WriteLine("После Delete файл существует: " + File.Exists(movedPath));
```

- **стр. 98** — копирование. Третий параметр `true` разрешает заменить существующий файл — без него повторный запуск программы закончился бы исключением.
- **стр. 103** — перемещение; так же переименовывают файлы.
- **стр. 104–105** — проверка: старого имени нет, новое есть.
- **стр. 108–109** — удаление и проверка.

**Особенность:** `ReadAllText` и `ReadAllLines` читают файл в память целиком — для очень больших файлов нужны потоки.

---

## 4. `FileInfoDemo()` — класс `FileInfo`

```csharp
125         private static void FileInfoDemo()
129             string path = Path.Combine(folder, "notes.txt");
133             FileInfo info = new FileInfo(path);
135             Console.WriteLine("Существует:      " + info.Exists);
136             Console.WriteLine("Имя:             " + info.Name);
137             Console.WriteLine("Расширение:      " + info.Extension);
138             Console.WriteLine("Размер, байт:    " + info.Length);
139             Console.WriteLine("Папка:           " + info.DirectoryName);
140             Console.WriteLine("Создан:          " + info.CreationTime);
141             Console.WriteLine("Изменён:         " + info.LastWriteTime);
142             Console.WriteLine("Только чтение:   " + info.IsReadOnly);
```

- **стр. 129** — тот же файл, что создан в прошлом опыте.
- **стр. 133** — создание объекта `FileInfo` **не создаёт файл** — это «карточка» со сведениями о файле по этому пути.
- **стр. 135–142** — свойства: существует ли, имя, расширение, размер в байтах, папка, дата создания, дата изменения, признак «только чтение».

```csharp
145             FileInfo copy = info.CopyTo(Path.Combine(folder, "notes_info_copy.txt"), true);
146             Console.WriteLine("Копия: " + copy.Name + ", размер " + copy.Length + " байт");
149             copy.Delete();
154             copy.Refresh();
155             Console.WriteLine("После Delete копия существует: " + copy.Exists);
```

- **стр. 145** — `CopyTo` копирует файл и возвращает `FileInfo` уже для копии.
- **стр. 149** — удаление.
- **стр. 154** — **важная особенность:** `FileInfo` запоминает сведения о файле и сам их не обновляет. Чтобы увидеть изменения на диске, вызывается `Refresh()`.
- **стр. 155** — теперь `Exists` даёт `False`.

---

## 5. `FileStreamDemo()` — класс `FileStream`

### 5.1. Запись байтов

```csharp
167         private static void FileStreamDemo()
171             string path = Path.Combine(folder, "bytes.dat");
181             using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
183                 byte[] data = { 10, 20, 30, 40, 50 };
186                 stream.Write(data, 0, data.Length);
189                 stream.WriteByte(60);
192                 Console.WriteLine("Записано байтов: " + stream.Length + ", позиция: " + stream.Position);
193             }
```

- **стр. 181** — открытие потока. `FileMode` — **что делать с файлом**: `Create` (создать, старый стереть), `Open` (открыть существующий), `Append` (встать в конец), `OpenOrCreate`. `FileAccess` — **зачем открываем**: `Read`, `Write`, `ReadWrite`. `using` закроет файл в конце блока.
- **стр. 183** — массив байтов для записи.
- **стр. 186** — `Write(массив, с какого элемента, сколько байтов)`.
- **стр. 189** — запись одного байта.
- **стр. 192** — `Length` — размер файла, `Position` — текущее место «курсора» (после записи шести байтов — 6).
- **стр. 193** — конец `using`: здесь файл закрывается.

### 5.2. Чтение и перемотка

```csharp
195             using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
199                 byte[] buffer = new byte[stream.Length];
200                 int count = stream.Read(buffer, 0, buffer.Length);
201                 Console.WriteLine("Прочитано " + count + " байт: " + string.Join(", ", buffer));
204                 stream.Seek(2, SeekOrigin.Begin);
205                 Console.WriteLine("Байт с номером 2: " + stream.ReadByte());
208                 stream.Seek(-1, SeekOrigin.End);
209                 Console.WriteLine("Последний байт:   " + stream.ReadByte());
212                 Console.WriteLine("Чтение после конца файла: " + stream.ReadByte());
```

- **стр. 195** — открыть существующий файл на чтение.
- **стр. 199** — массив размером с файл.
- **стр. 200** — `Read` возвращает, **сколько байтов реально прочитано** (может быть меньше, чем просили).
- **стр. 201** — `string.Join` склеивает элементы массива через запятую.
- **стр. 204–205** — `Seek` — перемотка: 2 байта от начала (счёт с нуля) → байт `30`.
- **стр. 208–209** — отсчёт от конца: `-1` — последний байт → `60`.
- **стр. 212** — после конца файла `ReadByte` возвращает `-1`.

### 5.3. Текст через байты

```csharp
217             string textPath = Path.Combine(folder, "hello.txt");
218             using (FileStream stream = new FileStream(textPath, FileMode.Create))
220                 byte[] textBytes = Encoding.UTF8.GetBytes("Привет, файл!");
221                 stream.Write(textBytes, 0, textBytes.Length);
223             using (FileStream stream = new FileStream(textPath, FileMode.Open))
225                 byte[] textBytes = new byte[stream.Length];
230                 stream.ReadExactly(textBytes, 0, textBytes.Length);
231–232             Console.WriteLine("Текст из байтов: " + Encoding.UTF8.GetString(textBytes) + ...);
```

- **стр. 220** — `FileStream` не знает про строки: строку приходится самому превращать в байты (`GetBytes`).
- **стр. 221** — запись этих байтов.
- **стр. 225** — буфер под весь файл.
- **стр. 230** — `ReadExactly` читает **ровно** указанное число байтов. Обычный `Read` здесь вызывал предупреждение компилятора CA2022: его результат нельзя игнорировать.
- **стр. 231–232** — обратно в строку (`GetString`). 13 символов заняли 23 байта: 10 русских букв по 2 байта и 3 знака по 1.

Вывод: текст через `FileStream` записать можно, но неудобно — для этого есть `StreamWriter`.

---

## 6. `StreamWriterReaderDemo()` — текстовые файлы

### 6.1. Запись

```csharp
246         private static void StreamWriterReaderDemo()
250             string path = Path.Combine(folder, "products.csv");
255             using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
258                 writer.WriteLine("Название;Цена");
259                 writer.WriteLine("Книга;450.50");
260                 writer.WriteLine("Ручка;35.00");
263                 writer.Write("Тетрадь;");
264                 writer.WriteLine("80.25");
268             using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8))
270                 writer.WriteLine("Рюкзак;2100.00");
```

- **стр. 255** — параметры: путь; `false` — перезаписать файл (`true` — дописывать); кодировка UTF-8, чтобы русские буквы не превратились в «кракозябры».
- **стр. 258–260** — `WriteLine` пишет строку и перевод строки, как `Console.WriteLine`. Получается простая таблица в формате CSV: поля разделены `;`.
- **стр. 263–264** — `Write` пишет без перевода строки; вместе две строки кода дают одну строку файла.
- **стр. 268–270** — тот же файл открыт на **дозапись** (`true`).

### 6.2. Построчное чтение

```csharp
275             decimal total = 0;
276             using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
279                 string header = reader.ReadLine();
280                 Console.WriteLine("Заголовок: " + header);
282                 string line;
286                 while ((line = reader.ReadLine()) != null)
289                     string[] parts = line.Split(';');
290                     string name = parts[0];
294                     decimal price = decimal.Parse(parts[1], CultureInfo.InvariantCulture);
296                     Console.WriteLine("  " + name + " стоит " + price + " руб.");
297                     total += price;
300             Console.WriteLine("Сумма всех цен: " + total + " руб.");
```

- **стр. 275** — сумма цен.
- **стр. 279–280** — первая строка — заголовок таблицы, читаем отдельно.
- **стр. 286** — стандартная запись построчного чтения: `ReadLine` возвращает `null`, когда строки кончились. Выражение `(line = reader.ReadLine()) != null` сначала присваивает, потом сравнивает. В памяти в каждый момент только одна строка — так читают даже огромные файлы.
- **стр. 289–290** — `Split(';')` делит строку на части; первая — название.
- **стр. 294** — `InvariantCulture`: в файле дробная часть отделена точкой. Без этого на русской Windows `Parse` ждал бы запятую и выбросил `FormatException`.
- **стр. 296–297** — вывод и накопление суммы.

```csharp
303             using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
305                 string everything = reader.ReadToEnd();
306                 Console.WriteLine("ReadToEnd прочитал символов: " + everything.Length);
```

- **стр. 305** — `ReadToEnd` читает всё, что осталось в файле, одной строкой.

---

## 7. `BinaryWriterReaderDemo()` — двоичные файлы

### 7.1. Запись

```csharp
320         private static void BinaryWriterReaderDemo()
324             string path = Path.Combine(folder, "products.bin");
327             int[] ids = { 1, 2, 3 };
328             string[] names = { "Книга", "Ручка", "Рюкзак" };
329             decimal[] prices = { 450.50m, 35.00m, 2100.00m };
330             bool[] inStock = { true, false, true };
334             using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
337                 writer.Write(ids.Length);
339                 for (int i = 0; i < ids.Length; i++)
343                     writer.Write(ids[i]);
344                     writer.Write(names[i]);
345                     writer.Write(prices[i]);
346                     writer.Write(inStock[i]);
```

- **стр. 327–330** — данные о трёх товарах; одинаковый индекс в четырёх массивах — один товар. `m` — литерал `decimal`.
- **стр. 334** — `BinaryWriter` сам файл не открывает — ему нужен готовый поток. `File.Open` возвращает `FileStream`, писатель «надевается» сверху. `using` закроет и писателя, и поток.
- **стр. 337** — сначала количество записей — чтобы при чтении знать, сколько их.
- **стр. 343–346** — метод один (`Write`), но у него много **перегрузок**: компилятор выбирает нужную по типу аргумента. `int` — 4 байта, `string` — длина и символы, `decimal` — 16 байт, `bool` — 1 байт.

### 7.2. Размер и «сырые» байты

```csharp
350             Console.WriteLine("Размер двоичного файла: " + new FileInfo(path).Length + " байт");
353             byte[] raw = File.ReadAllBytes(path);
354             Console.WriteLine("Первые 16 байт: " + BitConverter.ToString(raw, 0, 16));
```

- **стр. 350** — размер через `FileInfo` (102 байта).
- **стр. 353–354** — весь файл как массив байтов; `BitConverter.ToString` показывает их в шестнадцатеричном виде. Первые четыре `03-00-00-00` — это число 3 (количество записей), следующие `01-00-00-00` — идентификатор первого товара.

### 7.3. Чтение

```csharp
356             using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
361                 int count = reader.ReadInt32();
362                 Console.WriteLine("Записей в файле: " + count);
364                 for (int i = 0; i < count; i++)
367                     int id = reader.ReadInt32();
368                     string name = reader.ReadString();
369                     decimal price = reader.ReadDecimal();
370                     bool available = reader.ReadBoolean();
372–373             Console.WriteLine("  №" + id + " " + name + ", " + price + " руб., " + (available ? "в наличии" : "нет в наличии"));
```

- **стр. 361** — количество записей.
- **стр. 367–370** — **главное правило:** читать строго **в том же порядке и теми же типами**, как писали. В файле нет подсказок, где что лежит; перепутать порядок — получить мусор или исключение. У чтения для каждого типа свой метод (`ReadInt32`, `ReadString`, `ReadDecimal`, `ReadBoolean`).
- **стр. 372–373** — вывод; тернарный оператор превращает `bool` в текст.

---

## 8. Важные моменты (вопросы на защите)

1. **Чем `File` отличается от `FileInfo`?** — `File` статический, путь передаётся в каждый метод; `FileInfo` — объект для конкретного файла со свойствами (размер, даты). Для разовых действий удобнее `File`, для многих действий с одним файлом — `FileInfo`.
2. **Зачем `using`?** — Гарантированно закрыть файл (вызвать `Dispose`), даже при исключении. Незакрытый файл остаётся заблокированным, а данные могут не записаться на диск.
3. **Что задают `FileMode` и `FileAccess`?** — Что делать с файлом (создать, открыть, дописать) и для чего он открыт (чтение, запись).
4. **Почему `Read` возвращает число?** — Он может прочитать меньше байтов, чем просили; результат нужно проверять или использовать `ReadExactly`.
5. **Чем `StreamWriter` лучше `FileStream` для текста?** — Сам переводит строки в байты с учётом кодировки, умеет `WriteLine`.
6. **Чем двоичный файл отличается от текстового?** — В двоичном данные лежат как в памяти: компактно и без преобразований, но человеку не прочитать, и порядок чтения должен точно совпадать с порядком записи.
7. **Зачем `CultureInfo.InvariantCulture`?** — Чтобы разбор числа не зависел от языка системы (точка или запятая).
8. **Что делает `Seek`?** — Перемещает позицию в потоке: от начала, от текущего места или от конца.
9. **Почему у `FileInfo` нужен `Refresh()`?** — Объект кеширует сведения о файле и не следит за изменениями на диске.
