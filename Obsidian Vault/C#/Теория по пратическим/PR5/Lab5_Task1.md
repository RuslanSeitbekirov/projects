# Практическая работа №5. Задание 1 — Использование интерфейсов

Папка проекта: `Lab5_Task1/`

> **Как читать этот файл.** Код разбит на фрагменты, под каждым фрагментом — пояснение по номерам строк (`стр. N`).
> Фигурные скобки `{ }` и XML-комментарии `/// <summary>...</summary>` отдельно не расписываются:
> скобки просто открывают/закрывают блок, а `///`-комментарии превращаются в XML-документацию
> (в `.csproj` включено `<GenerateDocumentationFile>true</GenerateDocumentationFile>`) и подсказки IntelliSense.
> Файл `App.xaml.cs` — стандартная заготовка шаблона WPF, в нём нет своего кода.

---

## 1. Немного теории

**Интерфейс** (`interface`) — это *контракт*: список методов/свойств, которые класс **обязан** реализовать.
В интерфейсе нет полей и (в классическом варианте) нет тел методов. Класс, который пишет `: IMeasuringDevice`,
обещает предоставить все методы интерфейса — иначе ошибка компиляции.

Зачем это нужно:

| Преимущество | Как проявляется в этой работе |
|---|---|
| **Слабая связанность** | Тестовые приложения хранят устройство в переменной типа `IMeasuringDevice` и не знают, какой класс внутри. |
| **Заменяемость** | В задании 2 в ту же переменную кладётся `MeasureMassDevice` — код окна почти не меняется. |
| **Разделение работы** | Интерфейс можно написать заранее, а реализацию — потом (или другим программистом). |
| **Тестирование** | Вместо настоящего прибора подставляется эмулятор `DeviceController`. |

**Перечисление** (`enum`) — тип с фиксированным набором именованных констант (`Units.Metric`, `Units.Imperial`).
Под капотом это целые числа (0, 1), но в коде используются осмысленные имена.

**Структура решения** (кто на кого ссылается):

```
DeviceController   (эмулятор прибора, библиотека)
        ▲
MeasuringDevice    (Units, IMeasuringDevice, MeasureLengthDevice — библиотека)
        ▲                      ▲
ConsoleTest (консоль)   Exercise2TestHarness (WPF-окно)
```

Каждый проект — отдельная **сборка** (`.dll`/`.exe`). Поэтому всё, что используется из другого
проекта, должно быть объявлено `public`.

---

## 2. `MeasuringDevice/UnitsEnumeration.cs`

```csharp
1   namespace MeasuringDevice
...
10      public enum Units
11      {
15          Metric,
20          Imperial
21      }
```

- **стр. 1** — пространство имён `MeasuringDevice`. Все типы библиотеки лежат в нём; в других проектах подключается через `using MeasuringDevice;`.
- **стр. 3–5** — обычные комментарии `//`: почему перечисление `public`.
- **стр. 10** — объявление перечисления `Units`. `public` — чтобы его видели другие сборки (консоль и WPF).
- **стр. 15** — `Metric` (значение 0): метрическая система (мм, кг).
- **стр. 20** — `Imperial` (значение 1): имперская система (дюймы, фунты).

---

## 3. `MeasuringDevice/IMeasuringDevice.cs`

```csharp
10      public interface IMeasuringDevice
11      {
17          decimal MetricValue();
24          decimal ImperialValue();
29          void StartCollecting();
34          void StopCollecting();
41          int[] GetRawData();
42      }
```

- **стр. 10** — объявление интерфейса. По соглашению имена интерфейсов начинаются с `I`. Модификаторы доступа у членов интерфейса не пишутся — они по умолчанию `public`.
- **стр. 17** — `MetricValue()` — вернуть последнее измерение в метрических единицах. Тип `decimal` — точный десятичный тип (без ошибок округления, как у `double`).
- **стр. 24** — `ImperialValue()` — то же в имперских единицах.
- **стр. 29** — `StartCollecting()` — включить устройство и начать сбор данных.
- **стр. 34** — `StopCollecting()` — выключить устройство.
- **стр. 41** — `GetRawData()` — вернуть массив «сырых» измерений в родных единицах прибора.

Обратите внимание: здесь **только сигнатуры** (тип результата, имя, параметры) и `;` вместо тела метода.

---

## 4. `MeasuringDevice/MeasureLengthDevice.cs`

### 4.1. Подключения и заголовок класса

```csharp
1   using System;
2   using DeviceControl;
4   namespace MeasuringDevice
12      public class MeasureLengthDevice : IMeasuringDevice
```

- **стр. 1** — `using System;` нужен для класса `Random`.
- **стр. 2** — подключаем пространство имён эмулятора (`DeviceController`, `DeviceType`) из проекта DeviceController.
- **стр. 4** — тот же namespace, что у интерфейса.
- **стр. 12** — класс `MeasureLengthDevice` **реализует** интерфейс `IMeasuringDevice` (двоеточие + имя интерфейса). Теперь компилятор потребует все 5 методов интерфейса.

### 4.2. Поля

```csharp
19          private Units unitsToUse;
25          private int[] dataCaptured;
28          private int mostRecentMeasure;
31          private DeviceController controller;
38          private const DeviceType measurementType = DeviceType.LENGTH;
```

- **стр. 19** — в каких единицах работает устройство (задаётся в конструкторе).
- **стр. 25** — массив на 10 последних измерений — **кольцевой буфер**: заполнили до конца → пишем снова с начала, затирая старые значения.
- **стр. 28** — самое свежее измерение. Именно его пересчитывают `MetricValue`/`ImperialValue`.
- **стр. 31** — ссылка на контроллер эмулятора. `null` = устройство выключено.
- **стр. 38** — `const` — константа, значение известно при компиляции и не меняется. Это устройство всегда измеряет длину. (`const` неявно статический.)

Все поля `private` — **инкапсуляция**: снаружи к ним доступа нет, только через методы интерфейса.

### 4.3. Конструктор

```csharp
48          public MeasureLengthDevice(Units deviceUnits)
49          {
50              unitsToUse = deviceUnits;
51          }
```

- **стр. 48** — конструктор: имя совпадает с именем класса, типа результата нет. Принимает единицы измерения.
- **стр. 50** — сохраняем их в поле. Вызов: `new MeasureLengthDevice(Units.Metric)`.

### 4.4. `MetricValue()` и `ImperialValue()`

```csharp
62          public decimal MetricValue()
64              decimal metricMostRecent = 0;
66              if (unitsToUse == Units.Metric)
68                  metricMostRecent = mostRecentMeasure;
70              else
73                  metricMostRecent = mostRecentMeasure * 25.4m;
75              return metricMostRecent;
```

- **стр. 62** — реализация метода интерфейса. Должна быть `public` и с точно такой же сигнатурой.
- **стр. 64** — локальная переменная для результата.
- **стр. 66–68** — если устройство и так метрическое, значение уже в мм — возвращаем как есть (`int` неявно расширяется до `decimal`).
- **стр. 70–73** — иначе прибор дал дюймы → умножаем на 25.4 (1 дюйм = 25.4 мм). Суффикс **`m`** делает литерал типом `decimal`; без него `25.4` был бы `double`, и `decimal * double` не скомпилируется.
- **стр. 75** — возвращаем результат.

`ImperialValue()` (**стр. 83–96**) устроен зеркально: если устройство имперское — значение как есть (**стр. 87–89**), иначе мм × `0.03937m` (**стр. 93**), потому что 1 мм ≈ 0.03937 дюйма.

### 4.5. `StartCollecting()`

```csharp
103         public void StartCollecting()
107             if (controller != null)
109                 StopCollecting();
112             controller = DeviceController.StartDevice(measurementType);
113             GetMeasurements();
```

- **стр. 107–109** — защита от двойного запуска: если устройство уже работает, сначала останавливаем старый сеанс (иначе остался бы лишний фоновый поток).
- **стр. 112** — **фабричный метод** `StartDevice` (статический метод, который создаёт и возвращает объект) создаёт контроллер с запущенным прибором типа LENGTH.
- **стр. 113** — запускаем фоновый сбор измерений.

### 4.6. `StopCollecting()` и `GetRawData()`

```csharp
122             if (controller != null)
124                 controller.StopDevice();
125                 controller = null;
...
135             return dataCaptured;
```

- **стр. 122** — останавливать есть что, только если устройство запущено (иначе `NullReferenceException`).
- **стр. 124** — выключаем прибор.
- **стр. 125** — обнуляем ссылку. Это ещё и **сигнал фоновому потоку** завершиться (см. условие цикла ниже).
- **стр. 135** — отдаём массив измерений. До первого `StartCollecting` массив не создан → вернётся `null` (тестовые приложения это проверяют).

### 4.7. `GetMeasurements()` — фоновый сбор

```csharp
147         private void GetMeasurements()
149             dataCaptured = new int[10];
155             DeviceController sessionController = controller;
156             int[] buffer = dataCaptured;
158             System.Threading.ThreadPool.QueueUserWorkItem((dummy) =>
160                 int x = 0;
161                 Random timer = new Random();
163                 while (controller != null && controller == sessionController)
166                     System.Threading.Thread.Sleep(timer.Next(1000, 5000));
169                     if (controller != sessionController)
171                         break;
174                     buffer[x] = sessionController.TakeMeasurement();
175                     mostRecentMeasure = buffer[x];
178                     x++;
179                     if (x == 10)
181                         x = 0;
184             });
```

- **стр. 147** — `private`: метод не входит в интерфейс, это внутренняя кухня класса.
- **стр. 149** — новый пустой буфер на 10 элементов (все нули).
- **стр. 155–156** — запоминаем контроллер и буфер **этого** сеанса. Если пользователь сделает Stop → Start, у нового сеанса будет другой контроллер, и старый поток поймёт, что пора выходить. (Это небольшое улучшение кода из методички.)
- **стр. 158** — `ThreadPool.QueueUserWorkItem` ставит задачу в **пул потоков**: код внутри выполнится в фоновом потоке, не блокируя интерфейс. Сама задача записана **лямбда-выражением** `(dummy) => { ... }`; параметр `dummy` не используется (так требует сигнатура делегата `WaitCallback`).
- **стр. 160** — `x` — индекс текущей ячейки кольцевого буфера.
- **стр. 161** — генератор случайных чисел для паузы.
- **стр. 163** — крутимся, пока устройство включено и это всё ещё наш сеанс.
- **стр. 166** — спим 1–5 секунд (имитация того, что прибор измеряет не мгновенно). `Next(1000, 5000)` даёт число от 1000 до 4999 мс.
- **стр. 169–171** — пока спали, устройство могли выключить → выходим из цикла (`break`).
- **стр. 174** — снимаем показание и пишем в буфер.
- **стр. 175** — копируем его в «последнее измерение».
- **стр. 178–181** — сдвигаем индекс; после 9 возвращаемся к 0 — это и делает буфер кольцевым.

> **Важно (может спросить преподаватель).** Поля `mostRecentMeasure` и `dataCaptured` пишет фоновый поток,
> а читает поток интерфейса. В учебном примере это допустимо (запись `int` атомарна), но в реальном коде
> для общих данных между потоками используют `lock`, `volatile` или `Interlocked`.

---

## 5. `DeviceController/DeviceController.cs` — эмулятор (выдан готовым)

Этот проект в методичке «provided complete» — его не надо писать, но полезно понимать.

```csharp
1   using System;
11  namespace DeviceControl
16      public enum DeviceType { LENGTH, MASS }
27      public interface IControllableDevice
30          void StartDevice();
32          void StopDevice();
34          int GetLatestMeasure();
41      public class DeviceController : IDisposable
43          private IControllableDevice device;
```

- **стр. 1** — `System` нужен для `Random` и `IDisposable`.
- **стр. 11** — namespace `DeviceControl` (именно его подключает `using DeviceControl;`).
- **стр. 16–22** — `DeviceType`: какой прибор создать — длины или массы.
- **стр. 27–35** — интерфейс «железного» прибора: включить, выключить, получить последнее значение. Тут интерфейсы используются ещё раз: контроллер работает с любым прибором через `IControllableDevice`.
- **стр. 41** — `DeviceController` реализует стандартный интерфейс `IDisposable` (метод `Dispose` для освобождения ресурсов).
- **стр. 43** — поле с конкретным прибором, хранится как интерфейс.

```csharp
50          public static DeviceController StartDevice(DeviceType MeasurementType)
52              DeviceController controller = new DeviceController();
53              switch (MeasurementType)
55                  case DeviceType.LENGTH:
56                      controller.device = new FabrikamDevices.LengthMeasuringDevice();
57                      break;
58                  case DeviceType.MASS:
59                      controller.device = new ContosoDevices.MassMeasuringDevice();
60                      break;
63              if (controller.device != null)
65                  controller.device.StartDevice();
67              return controller;
```

- **стр. 50** — `static` — метод вызывается через имя класса, без объекта: `DeviceController.StartDevice(...)`. Это паттерн **«фабричный метод»**.
- **стр. 52** — создаём пустой контроллер.
- **стр. 53–61** — по типу измерения выбираем, какой прибор создать (полное имя с namespace, т.к. классы лежат в других пространствах имён).
- **стр. 63–65** — включаем прибор.
- **стр. 67** — отдаём готовый контроллер.

```csharp
71          public void StopDevice()        { device.StopDevice(); }
78          public int TakeMeasurement()    { return device.GetLatestMeasure(); }
84          public void Dispose()           { }
```

- **стр. 71–74** — выключение передаётся прибору.
- **стр. 78–81** — снять измерение = спросить у прибора последнее значение.
- **стр. 84–86** — пустой `Dispose`: в эмуляторе освобождать нечего.

```csharp
90  namespace FabrikamDevices
95      public class LengthMeasuringDevice : DeviceControl.IControllableDevice
97          private readonly Random rnd = new Random();
98          private bool running;
99          private int lastValue;
101         public void StartDevice() { running = true; }
103         public void StopDevice() { running = false; }
105         public int GetLatestMeasure()
108             if (running)
110                 lastValue = rnd.Next(1, 1000);
112             return lastValue;
```

- **стр. 90** — отдельный namespace «производителя» Fabrikam.
- **стр. 95** — прибор длины реализует интерфейс прибора.
- **стр. 97** — `readonly` — поле можно присвоить только при объявлении или в конструкторе.
- **стр. 98–99** — флаг «включён» и последнее значение.
- **стр. 101, 103** — включить/выключить = поменять флаг.
- **стр. 108–110** — пока прибор включён, каждое обращение даёт новое случайное число 1…999.
- **стр. 112** — выключенный прибор возвращает последнее значение.

**стр. 117–140** — `ContosoDevices.MassMeasuringDevice` — то же самое для массы, диапазон 1…199. Используется в задании 2.

---

## 6. `ConsoleTest/Program.cs` — консольное тестовое приложение

Это дополнительный (к методичке) способ проверить класс без WPF.

### 6.1. Подключения, поле, `Main`

```csharp
1   using System;
2   using System.Text;
3   using System.Threading;
4   using MeasuringDevice;
6   namespace ConsoleTest
12      internal class Program
19          private static IMeasuringDevice device;
21          private static void Main()
23              Console.OutputEncoding = Encoding.UTF8;
24              Console.InputEncoding = Encoding.UTF8;
25              Console.Title = "Практическая работа №5, задание 1 — MeasureLengthDevice";
```

- **стр. 1** — `Console`, `Array`.
- **стр. 2** — `Encoding` (кодировки).
- **стр. 3** — `Thread.Sleep` в автотесте.
- **стр. 4** — наша библиотека: `IMeasuringDevice`, `MeasureLengthDevice`, `Units`.
- **стр. 12** — `internal` — класс виден только внутри своей сборки.
- **стр. 19** — **ключевая строка**: поле имеет тип **интерфейса**. Все вызовы ниже идут через интерфейс. `static` — потому что `Main` статический.
- **стр. 21** — точка входа программы.
- **стр. 23–24** — UTF-8, чтобы кириллица корректно выводилась и вводилась в консоли Windows.
- **стр. 25** — заголовок окна консоли.

### 6.2. Главный цикл меню

```csharp
27              while (true)
29                  PrintMenu();
30                  Console.Write("Ваш выбор: ");
31                  string choice = Console.ReadLine();
32                  Console.WriteLine();
34                  switch (choice)
36                      case "1": CreateInstance(Units.Imperial); break;
37                      case "2": CreateInstance(Units.Metric); break;
38                      case "3": StartCollecting(); break;
39                      case "4": StopCollecting(); break;
40                      case "5": ShowRawData(); break;
41                      case "6": ShowMetricValue(); break;
42                      case "7": ShowImperialValue(); break;
43                      case "8": RunAutoTest(); break;
44                      case "0":
45                          device?.StopCollecting();
46                          return;
47                      default:
48                          Console.WriteLine("Нет такого пункта меню.");
51                  Console.WriteLine();
```

- **стр. 27** — бесконечный цикл; выход — по пункту «0».
- **стр. 29** — печатаем меню.
- **стр. 30–31** — приглашение и чтение строки, введённой пользователем.
- **стр. 32** — пустая строка для читаемости.
- **стр. 34** — `switch` по строке.
- **стр. 36–43** — каждый пункт вызывает свой метод; `break` завершает `case`.
- **стр. 45** — `?.` — **null-условный оператор**: метод вызовется, только если `device` не `null`. Останавливаем фоновый поток перед выходом.
- **стр. 46** — `return` выходит из `Main` → программа завершается.
- **стр. 47–48** — любой другой ввод.
- **стр. 51** — пустая строка между итерациями.

### 6.3. `PrintMenu()` — **стр. 55–68**

Только `Console.WriteLine` с пунктами меню. Номера пунктов соответствуют `case` в `switch`.

### 6.4. Обработчики пунктов меню

```csharp
72          private static void CreateInstance(Units units)
75              device?.StopCollecting();
77              device = new MeasureLengthDevice(units);
78              Console.WriteLine($"Создан MeasureLengthDevice, единицы: {units}.");
```

- **стр. 75** — если устройство уже было — останавливаем, чтобы не висел старый фоновый поток.
- **стр. 77** — создаём объект **класса**, но кладём в переменную **интерфейса** — неявное приведение к интерфейсу.
- **стр. 78** — `$"..."` — **интерполяция строк**: `{units}` подставит имя значения перечисления (`Metric`/`Imperial`).

```csharp
81          private static bool CheckDevice()
83              if (device == null)
85                  Console.WriteLine("Сначала создайте устройство (пункт 1 или 2).");
86                  return false;
88              return true;
```

- **стр. 81–88** — защита: прежде чем вызывать методы устройства, проверяем, что оно создано. Иначе `device.StartCollecting()` упал бы с `NullReferenceException`.

```csharp
93              if (!CheckDevice()) return;
94              device.StartCollecting();
95              Console.WriteLine("Сбор данных запущен. ...");
```

- **стр. 93** — нет устройства → выходим из метода.
- **стр. 94** — вызов метода **через интерфейс**.
- **стр. 95–96** — подсказка (строка разбита на две и склеена `+`).

**стр. 99–104** `StopCollecting()` — аналогично: проверка → `device.StopCollecting()` → сообщение.

```csharp
110             int[] raw = device.GetRawData();
111             if (raw == null)
113                 Console.WriteLine("Данных нет — сбор ещё не запускался.");
114                 return;
117             Console.WriteLine("Сырые данные (кольцевой буфер на 10 значений):");
118             for (int i = 0; i < raw.Length; i++)
120                 Console.WriteLine($"  [{i}] = {raw[i]}");
```

- **стр. 110** — получаем массив измерений.
- **стр. 111–114** — до первого запуска массива нет (`null`).
- **стр. 118–120** — печатаем каждый элемент с индексом.

**стр. 124–128** и **130–134** — печать `device.MetricValue()` и `device.ImperialValue()` с подписью единиц.

### 6.5. Автотест (повторяет Task 5 методички)

```csharp
143         private static void RunAutoTest()
145             TestOneDevice(Units.Imperial);
146             Console.WriteLine();
147             TestOneDevice(Units.Metric);
```

- **стр. 145–147** — проверяем устройство сначала в дюймах, потом в мм — как в шагах 3–15 методички.

```csharp
150         private static void TestOneDevice(Units units)
152             Console.WriteLine($"--- Автотест: {units} ---");
153             CreateInstance(units);
154             device.StartCollecting();
156             Console.Write("Ждём 10 секунд");
157             for (int i = 0; i < 10; i++)
159                 Thread.Sleep(1000);
160                 Console.Write('.');
162             Console.WriteLine();
165             device.StopCollecting();
167             decimal metric = device.MetricValue();
168             decimal imperial = device.ImperialValue();
169             int[] raw = device.GetRawData();
171             Console.WriteLine($"Metric value:   {metric} мм");
172             Console.WriteLine($"Imperial value: {imperial} дюйм.");
173             Console.WriteLine("Raw data: " + string.Join(", ", raw));
176             decimal native = units == Units.Metric ? metric : imperial;
177             bool found = Array.IndexOf(raw, (int)native) >= 0;
178             Console.WriteLine(found
179                 ? $"OK: значение {native} найдено в сырых данных."
180                 : $"ОШИБКА: значение {native} не найдено в сырых данных!");
```

- **стр. 153–154** — создать устройство и запустить.
- **стр. 156–162** — 10 раз по секунде спим и печатаем точку — видимый «прогресс».
- **стр. 165** — останавливаем **до** чтения, чтобы между вызовами не пришло новое измерение и проверка была честной.
- **стр. 167–169** — читаем три результата через интерфейс.
- **стр. 173** — `string.Join` склеивает элементы массива через запятую.
- **стр. 176** — **тернарный оператор** `условие ? a : b`: «родное» значение — то, которое устройство не пересчитывало.
- **стр. 177** — `Array.IndexOf` ищет элемент в массиве; вернёт −1, если не найден. `(int)native` — явное приведение `decimal → int` (родное значение всегда целое).
- **стр. 178–180** — печатаем OK или ОШИБКА. Именно эту проверку делают вручную в шагах 8 и 14 методички.

---

## 7. WPF-окно `Exercise2TestHarness`

### 7.1. Как связаны XAML и C#

- `MainWindow.xaml` — **разметка** (как выглядит окно), `MainWindow.xaml.cs` — **code-behind** (что происходит).
- Оба файла описывают **один и тот же** класс `MainWindow` — поэтому он `partial` (частичный).
- `x:Name="imperialRadio"` в XAML → в C# появляется поле `imperialRadio`, с которым можно работать.
- `Click="createInstance_Click"` в XAML → при нажатии кнопки WPF вызывает метод `createInstance_Click` из C#.
- `App.xaml` содержит `StartupUri="MainWindow.xaml"` — какое окно открыть при запуске.

### 7.2. Элементы разметки, которые использует код

| Элемент XAML (`x:Name`) | Тип | Где используется в C# |
|---|---|---|
| `imperialRadio` (стр. 30) | `RadioButton` | `createInstance_Click` — читается `IsChecked` |
| `metricRadio` (стр. 31) | `RadioButton` | в коде не читается: радиокнопки в одной панели взаимоисключающие, достаточно проверить `imperialRadio` |
| `createInstance` (стр. 35) | `Button`, `Click="createInstance_Click"` | вызывает `createInstance_Click` |
| `startCollecting` (стр. 37) | `Button`, `Click="startCollecting_Click"`, `IsEnabled="False"` | `startCollecting_Click`; включается/выключается в `SetRunning` |
| `stopCollecting` (стр. 39) | `Button`, `Click="stopCollecting_Click"` | `stopCollecting_Click`; `SetRunning` |
| `getMetricValue` (стр. 42) | `Button` | `getMetricValue_Click`; включается в `createInstance_Click` |
| `getImperialValue` (стр. 44) | `Button` | `getImperialValue_Click`; включается в `createInstance_Click` |
| `getRawData` (стр. 46) | `Button` | `getRawData_Click`; включается в `createInstance_Click` |
| `metricValueBox` (стр. 64) | `TextBox` (только чтение) | `getMetricValue_Click`, `ClearResults` |
| `imperialValueBox` (стр. 68) | `TextBox` | `getImperialValue_Click`, `ClearResults` |
| `rawDataValues` (стр. 73) | `ListBox` | `getRawData_Click`, `ClearResults` — свойство `ItemsSource` |
| `statusText` (стр. 77) | `TextBlock` | почти все обработчики пишут в `Text` |
| `<Window ... Closing="Window_Closing">` (стр. 7) | событие окна | `Window_Closing` |

Прочее в разметке: `Window.Resources` со `Style TargetType="Button"` (стр. 9–14) — общий отступ для всех кнопок;
`Grid` с двумя колонками (стр. 16–24): слева `StackPanel` с управлением, справа — результаты, внизу строка состояния.

### 7.3. `MainWindow.xaml.cs` построчно

```csharp
1   using System.ComponentModel;
2   using System.Windows;
3   using MeasuringDevice;
5   namespace Exercise2TestHarness
13      public partial class MainWindow : Window
20          private IMeasuringDevice device;
22          public MainWindow()
24              InitializeComponent();
```

- **стр. 1** — `CancelEventArgs` (аргумент события `Closing`).
- **стр. 2** — `Window`, `RoutedEventArgs` — базовые типы WPF.
- **стр. 3** — наша библиотека.
- **стр. 13** — `partial` — вторая половина класса генерируется из XAML; `: Window` — наследуемся от окна WPF.
- **стр. 20** — поле типа **интерфейса** — так требует методичка (Task 4).
- **стр. 24** — `InitializeComponent()` (сгенерирован из XAML) создаёт все элементы окна и связывает поля `x:Name`. Без этого вызова окно было бы пустым.

```csharp
30          private void createInstance_Click(object sender, RoutedEventArgs e)
33              device?.StopCollecting();
35              if (imperialRadio.IsChecked == true)
37                  device = new MeasureLengthDevice(Units.Imperial);
39              else
41                  device = new MeasureLengthDevice(Units.Metric);
44              ClearResults();
45              SetRunning(false);
46              getMetricValue.IsEnabled = true;
47              getImperialValue.IsEnabled = true;
48              getRawData.IsEnabled = true;
49              statusText.Text = $"Создан MeasureLengthDevice (...). Нажмите Start Collecting.";
```

**XAML:** кнопка `createInstance` (`Click="createInstance_Click"`), радиокнопка `imperialRadio`, кнопки `getMetricValue`, `getImperialValue`, `getRawData`, `statusText`.

- **стр. 30** — сигнатура обработчика события WPF: `sender` — кто вызвал (кнопка), `e` — данные события.
- **стр. 33** — остановить старое устройство, если было.
- **стр. 35** — `IsChecked` имеет тип `bool?` (может быть `null`), поэтому сравниваем с `true` явно.
- **стр. 37 / 41** — **Task 4 методички**: создаём `MeasureLengthDevice` с выбранными единицами и кладём в поле-интерфейс.
- **стр. 44** — очищаем старые результаты.
- **стр. 45** — Start доступен, Stop — нет.
- **стр. 46–48** — включаем кнопки получения значений (в XAML они изначально `IsEnabled="False"`).
- **стр. 49** — сообщение в строке состояния (внутри интерполяции — тернарный оператор).

```csharp
52          private void startCollecting_Click(object sender, RoutedEventArgs e)
54              device.StartCollecting();
55              SetRunning(true);
56              statusText.Text = "Сбор данных идёт. ...";
```

**XAML:** кнопка `startCollecting`, `statusText`.
- **стр. 54** — запуск через интерфейс. Проверка на `null` не нужна: кнопка включается только после создания устройства.
- **стр. 55** — блокируем Start, разблокируем Stop.

**стр. 59–64** `stopCollecting_Click` — **XAML:** кнопка `stopCollecting`, `statusText`. Останавливает устройство и меняет доступность кнопок обратно.

```csharp
66          private void getMetricValue_Click(object sender, RoutedEventArgs e)
68              metricValueBox.Text = device.MetricValue().ToString();
```

**XAML:** кнопка `getMetricValue` → текстовое поле `metricValueBox`.
- **стр. 68** — `decimal` → строка через `ToString()` и в свойство `Text`.

**стр. 71–74** `getImperialValue_Click` — то же для `imperialValueBox`.

```csharp
76          private void getRawData_Click(object sender, RoutedEventArgs e)
78              int[] raw = device.GetRawData();
79              if (raw == null)
81                  statusText.Text = "Данных нет — сбор ещё не запускался.";
82                  return;
88              rawDataValues.ItemsSource = null;
89              rawDataValues.ItemsSource = raw;
```

**XAML:** кнопка `getRawData` → список `rawDataValues`, `statusText`.
- **стр. 79–82** — сбор не запускали → массива нет.
- **стр. 88–89** — **привязка данных**: `ItemsSource` — коллекция, элементы которой `ListBox` показывает строками. `GetRawData` всегда возвращает **тот же** объект массива; если присвоить его повторно, WPF решит, что ничего не изменилось, и не перерисует список. Поэтому сначала `null`, потом массив.

```csharp
95          private void Window_Closing(object sender, CancelEventArgs e)
97              device?.StopCollecting();
```

**XAML:** атрибут `Closing="Window_Closing"` у `<Window>`.
- **стр. 97** — при закрытии окна гасим фоновый поток.

```csharp
103         private void SetRunning(bool running)
105             startCollecting.IsEnabled = !running;
106             stopCollecting.IsEnabled = running;
109         private void ClearResults()
111             metricValueBox.Text = string.Empty;
112             imperialValueBox.Text = string.Empty;
113             rawDataValues.ItemsSource = null;
```

- **стр. 103–106** — **XAML:** `startCollecting`, `stopCollecting`. Работает только одна из двух кнопок.
- **стр. 109–113** — **XAML:** `metricValueBox`, `imperialValueBox`, `rawDataValues`. Очистка полей и списка.

---

## 8. Важные моменты (вопросы на защите)

1. **Почему поле `device` имеет тип `IMeasuringDevice`, а не `MeasureLengthDevice`?** — Чтобы код окна зависел от контракта, а не от реализации. В задании 2 туда же кладётся `MeasureMassDevice` без переписывания окна.
2. **Что будет, если не реализовать один метод интерфейса?** — Ошибка компиляции CS0535.
3. **Можно ли создать объект интерфейса (`new IMeasuringDevice()`)?** — Нет, у интерфейса нет реализации.
4. **Зачем суффикс `m` в `25.4m`?** — Это литерал `decimal`; без него — `double`, а смешивать `decimal` и `double` без приведения нельзя.
5. **Почему `measurementType` — `const`?** — Устройство длины всегда измеряет длину, значение известно при компиляции. В задании 2 это поле перестанет быть константой.
6. **Что такое кольцевой буфер?** — Массив фиксированного размера, запись в который идёт по кругу (индекс 9 → 0), старые данные затираются.
7. **Зачем `ThreadPool`?** — Измерения идут бесконечно с паузами; в главном потоке это заморозило бы окно/консоль.
8. **Почему `public`, а не `internal`?** — Типы используются из других сборок (консоль, WPF).
