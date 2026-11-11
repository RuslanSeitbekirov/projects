# Практическая работа №5. Задание 2 — Создание абстрактного класса

Папка проекта: `Lab5_Task2/` (файл решения по историческим причинам называется `Lab5_Task1.sln`).

> **Как читать этот файл.** Фигурные скобки и `///`-комментарии отдельно не расписываются.
> Следующие файлы **не изменились** по сравнению с заданием 1 — их построчный разбор см. в [Lab5_Task1.md](Lab5_Task1.md):
>
> - `MeasuringDevice/UnitsEnumeration.cs` — перечисление `Units`;
> - `MeasuringDevice/IMeasuringDevice.cs` — интерфейс;
> - `DeviceController/DeviceController.cs` — эмулятор (тут понадобится `ContosoDevices.MassMeasuringDevice`);
> - `ConsoleTest/Program.cs` — консольный тест, он по-прежнему проверяет только `MeasureLengthDevice`. Прибор массы проверяется через WPF-окно.
>
> Ниже — только новые и изменённые файлы.

---

## 1. Немного теории

**Проблема.** В задании 2 появляется второй прибор — `MeasureMassDevice`. Методы `StartCollecting`, `StopCollecting`,
`GetRawData`, `GetMeasurements` и все поля у него **точно такие же**, как у `MeasureLengthDevice`. Различаются только
`MetricValue`/`ImperialValue` (коэффициенты пересчёта). Дублирование кода — источник ошибок: исправишь баг в одном
классе, забудешь в другом.

**Решение — абстрактный класс** `MeasureDataDevice`:

- общий код пишется **один раз** в базовом классе;
- то, что у каждого прибора своё, объявляется `abstract` — без тела, наследник **обязан** реализовать (`override`);
- объект абстрактного класса создать нельзя (`new MeasureDataDevice()` — ошибка), только объект наследника.

**Интерфейс vs абстрактный класс**

| | Интерфейс | Абстрактный класс |
|---|---|---|
| Поля | нет | есть |
| Реализация методов | нет (классически) | может быть |
| Конструктор | нет | есть |
| Модификаторы доступа членов | всегда `public` | любые (`protected` и т.д.) |
| Сколько можно унаследовать | сколько угодно | только **один** базовый класс |
| Смысл | «что умеет» (контракт) | «чем является» + общий код |

Здесь используются **оба**: интерфейс `IMeasuringDevice` — контракт для внешнего кода (окна), абстрактный класс — общая реализация для приборов.

**Модификаторы доступа**

- `private` — видно только внутри этого класса;
- `protected` — видно в этом классе **и в наследниках**;
- `public` — видно всем.

**Иерархия:**

```
          IMeasuringDevice (интерфейс)
                  ▲ реализует
          MeasureDataDevice (abstract)
             ▲              ▲  наследуют
 MeasureLengthDevice   MeasureMassDevice
```

---

## 2. `MeasuringDevice/MeasureDataDevice.cs` — абстрактный базовый класс

### 2.1. Заголовок и поля

```csharp
1   using System;
2   using DeviceControl;
4   namespace MeasuringDevice
13      public abstract class MeasureDataDevice : IMeasuringDevice
19          protected Units unitsToUse;
22          protected int[] dataCaptured;
25          protected int mostRecentMeasure;
28          protected DeviceController controller;
34          protected DeviceType measurementType;
```

- **стр. 1** — `Random`.
- **стр. 2** — `DeviceController`, `DeviceType` из эмулятора.
- **стр. 13** — `abstract class` — класс-заготовка. Он **реализует** интерфейс: часть методов здесь, часть отдана наследникам через `abstract`.
- **стр. 19–28** — те же поля, что были в `MeasureLengthDevice` задания 1, но теперь **`protected`** (Task 5, шаг 12): наследники должны их читать/писать (например, `unitsToUse` в конструкторе и `mostRecentMeasure` в `MetricValue`).
- **стр. 34** — `measurementType` **больше не `const`** и не инициализирован (Task 5, шаг 13): базовый класс не знает, что измеряет прибор, значение задаст конструктор наследника.

### 2.2. Абстрактные методы

```csharp
37          public abstract decimal MetricValue();
40          public abstract decimal ImperialValue();
```

- **стр. 37, 40** — `abstract` — только сигнатура, тела нет (как в интерфейсе). Каждый наследник обязан написать `public override decimal MetricValue() { ... }`. Так абстрактный класс «выполняет» требования интерфейса, перекладывая конкретику на наследников.

### 2.3. Общие методы (перенесены из `MeasureLengthDevice` задания 1)

```csharp
43          public void StartCollecting()
45              if (controller != null)
47                  StopCollecting();
50              controller = DeviceController.StartDevice(measurementType);
51              GetMeasurements();
```

- **стр. 45–47** — если сбор уже идёт — сначала остановить.
- **стр. 50** — создаём контроллер нужного типа. Здесь видна польза поля `measurementType`: один и тот же код запускает и прибор длины, и прибор массы — в зависимости от того, что записал конструктор наследника.
- **стр. 51** — запуск фонового сбора.

```csharp
55          public void StopCollecting()
57              if (controller != null)
59                  controller.StopDevice();
60                  controller = null;
65          public int[] GetRawData()
67              return dataCaptured;
```

- **стр. 57–60** — выключить прибор и обнулить ссылку (это же сигнал фоновому потоку завершиться).
- **стр. 67** — отдать буфер измерений (`null`, если сбор не запускался).

### 2.4. `GetMeasurements()` — фоновый сбор

```csharp
73          private void GetMeasurements()
75              dataCaptured = new int[10];
77              DeviceController sessionController = controller;
78              int[] buffer = dataCaptured;
80              System.Threading.ThreadPool.QueueUserWorkItem((dummy) =>
82                  int x = 0;
83                  Random timer = new Random();
85                  while (controller != null && controller == sessionController)
87                      System.Threading.Thread.Sleep(timer.Next(1000, 5000));
89                      if (controller != sessionController)
91                          break;
94                      buffer[x] = sessionController.TakeMeasurement();
95                      mostRecentMeasure = buffer[x];
97                      x++;
98                      if (x == 10)
100                         x = 0;
103             });
```

Код идентичен заданию 1 (подробно — в [Lab5_Task1.md](Lab5_Task1.md), раздел 4.7):

- **стр. 73** — `private`: наследникам этот метод не нужен, его вызывает только `StartCollecting` этого же класса.
- **стр. 75** — новый кольцевой буфер на 10 значений.
- **стр. 77–78** — запоминаем контроллер и буфер этого сеанса.
- **стр. 80** — задача в пул потоков, записана лямбдой.
- **стр. 82–83** — индекс буфера и генератор паузы.
- **стр. 85** — работаем, пока прибор включён и сеанс не сменился.
- **стр. 87** — пауза 1–5 с.
- **стр. 89–91** — за время паузы прибор выключили → выходим.
- **стр. 94–95** — снять измерение, записать в буфер и в «последнее».
- **стр. 97–100** — индекс по кругу 0…9.

---

## 3. `MeasuringDevice/MeasureLengthDevice.cs` — теперь наследник

```csharp
1   using DeviceControl;
3   namespace MeasuringDevice
10      public class MeasureLengthDevice : MeasureDataDevice, IMeasuringDevice
13          public MeasureLengthDevice(Units deviceUnits)
15              unitsToUse = deviceUnits;
16              measurementType = DeviceType.LENGTH;
```

- **стр. 1** — нужен ради `DeviceType.LENGTH`.
- **стр. 10** — после двоеточия **сначала базовый класс**, потом интерфейсы (Task 6, шаг 1). Повторное указание `IMeasuringDevice` не обязательно — класс и так реализует его через `MeasureDataDevice`, — но методичка требует оставить, и это подчёркивает, что объект можно использовать как `IMeasuringDevice`.
- **стр. 13** — конструктор. Перед его телом неявно вызывается конструктор базового класса без параметров (`base()`); у `MeasureDataDevice` он сгенерирован компилятором автоматически.
- **стр. 15** — записываем единицы в **унаследованное** `protected`-поле.
- **стр. 16** — сообщаем базовому классу, что это прибор длины (Task 6, шаг 7). Раньше здесь была `const`, теперь присваивание в конструкторе.

Всё остальное (`StartCollecting`, `StopCollecting`, `GetRawData`, `GetMeasurements`, поля) **удалено** из класса — оно приходит из базового.

```csharp
23          public override decimal MetricValue()
25              decimal metricMostRecent = 0;
27              if (unitsToUse == Units.Metric)
29                  metricMostRecent = mostRecentMeasure;
31              else
33                  metricMostRecent = mostRecentMeasure * 25.4m;
35              return metricMostRecent;
```

- **стр. 23** — `override` — «переопределяю абстрактный метод базового класса» (Task 6, шаг 8). Без `override` компилятор выдаст ошибку: абстрактный метод не реализован.
- **стр. 25–35** — логика как в задании 1: метрическое устройство → значение как есть, иначе дюймы × 25.4 = мм. `m` — литерал `decimal`.

**стр. 39–52** `ImperialValue()` — `override`; имперское устройство → как есть, иначе мм × `0.03937m` = дюймы.

---

## 4. `MeasuringDevice/MeasureMassDevice.cs` — новый прибор массы

```csharp
1   using DeviceControl;
3   namespace MeasuringDevice
10      public class MeasureMassDevice : MeasureDataDevice, IMeasuringDevice
13          public MeasureMassDevice(Units deviceUnits)
15              unitsToUse = deviceUnits;
16              measurementType = DeviceType.MASS;
```

- **стр. 10** — тоже наследник `MeasureDataDevice`.
- **стр. 13–16** — конструктор (Task 6, шаг 16): единицы и тип измерения **MASS**. Благодаря этому `StartCollecting` базового класса создаст `ContosoDevices.MassMeasuringDevice`.

```csharp
20          public override decimal MetricValue()
22              decimal metricMostRecent = 0;
24              if (unitsToUse == Units.Metric)
26                  metricMostRecent = mostRecentMeasure;
28              else
30                  metricMostRecent = mostRecentMeasure * 0.4536m;
32              return metricMostRecent;
```

- **стр. 24–26** — метрический прибор уже выдаёт килограммы.
- **стр. 30** — иначе фунты × 0.4536 = кг (Task 2, шаг 15).

```csharp
36          public override decimal ImperialValue()
40              if (unitsToUse == Units.Imperial)
42                  imperialMostRecent = mostRecentMeasure;
46                  imperialMostRecent = mostRecentMeasure * 2.2046m;
```

- **стр. 40–42** — имперский прибор уже выдаёт фунты.
- **стр. 46** — иначе кг × 2.2046 = фунты (Task 2, шаг 16).

Весь класс — 50 строк вместо ~190: это и есть выигрыш от абстрактного класса.

---

## 5. WPF-окно `Exercise2TestHarness`

Связь XAML ↔ C# устроена так же, как в задании 1 (см. [Lab5_Task1.md](Lab5_Task1.md), раздел 7.1):
`x:Name` → поле в C#, `Click="метод"` → обработчик, `partial class` — одна половина класса из XAML, другая — в `.xaml.cs`.

### 5.1. Что изменилось в разметке `MainWindow.xaml`

- **стр. 4** — заголовок окна `Exercise 3 Test Harness — Length / Mass`.
- **стр. 35–40** — новый `GroupBox Header="Device"` с радиокнопками `lengthRadio` (по умолчанию выбрана) и `massRadio` — выбор типа прибора (Task 4, шаг 3 методички: «choose Mass Device»).
- **стр. 42** — кнопка теперь называется просто `Create Instance`.
- **стр. 70, 74** — у подписей появились имена `metricLabel` и `imperialLabel`: код меняет в них единицы (мм/кг, дюйм/фунт).

### 5.2. Элементы разметки, которые использует код

| Элемент XAML (`x:Name`) | Тип | Где используется в C# |
|---|---|---|
| `imperialRadio` (стр. 30) | `RadioButton` | `createInstance_Click` — выбор единиц |
| `metricRadio` (стр. 31) | `RadioButton` | в коде не читается (взаимоисключающая пара) |
| `lengthRadio` (стр. 37) | `RadioButton` | `createInstance_Click` — выбор прибора |
| `massRadio` (стр. 38) | `RadioButton` | в коде не читается (пара к `lengthRadio`) |
| `createInstance` (стр. 42) | `Button`, `Click="createInstance_Click"` | `createInstance_Click` |
| `startCollecting` (стр. 44) | `Button`, `IsEnabled="False"` | `startCollecting_Click`, `SetRunning` |
| `stopCollecting` (стр. 46) | `Button`, `IsEnabled="False"` | `stopCollecting_Click`, `SetRunning` |
| `getMetricValue` (стр. 49) | `Button` | `getMetricValue_Click`; включается в `createInstance_Click` |
| `getImperialValue` (стр. 51) | `Button` | `getImperialValue_Click`; включается в `createInstance_Click` |
| `getRawData` (стр. 53) | `Button` | `getRawData_Click`; включается в `createInstance_Click` |
| `metricLabel` (стр. 70) | `Label` | `UpdateUnitLabels` — свойство `Content` |
| `metricValueBox` (стр. 71) | `TextBox` | `getMetricValue_Click`, `ClearResults` |
| `imperialLabel` (стр. 74) | `Label` | `UpdateUnitLabels` |
| `imperialValueBox` (стр. 75) | `TextBox` | `getImperialValue_Click`, `ClearResults` |
| `rawDataValues` (стр. 80) | `ListBox` | `getRawData_Click`, `ClearResults` |
| `statusText` (стр. 84) | `TextBlock` | почти все обработчики |
| `<Window Closing="Window_Closing">` (стр. 7) | событие | `Window_Closing` |

> **Была ошибка запуска.** В `MainWindow.xaml` между `<Label>` и `<TextBox>` стоял лишний текст `...`. WPF воспринимал его
> как строку и пытался добавить в `Grid` (`XamlParseException: не удаётся добавить экземпляр типа "String" в коллекцию
> UIElementCollection`). В панели (`Grid`, `StackPanel`) могут лежать только элементы интерфейса (`UIElement`), голый текст — нельзя.

### 5.3. `MainWindow.xaml.cs` — изменённые и новые части

Строки **1–25**, **57–103**, **108–119** совпадают с заданием 1 (`using`, поле `device` типа интерфейса,
`InitializeComponent`, `startCollecting_Click`, `stopCollecting_Click`, `getMetricValue_Click`, `getImperialValue_Click`,
`getRawData_Click`, `Window_Closing`, `SetRunning`, `ClearResults`) — построчно см. [Lab5_Task1.md](Lab5_Task1.md), раздел 7.3.
Здесь важно одно: **ни один из этих методов не пришлось менять**, хотя появился новый тип прибора — они работают через `IMeasuringDevice`.

#### `createInstance_Click` — **стр. 32–55**

**XAML:** кнопка `createInstance`, радиокнопки `imperialRadio` и `lengthRadio`, кнопки `getMetricValue`/`getImperialValue`/`getRawData`, `statusText`, а через `UpdateUnitLabels` — `metricLabel`/`imperialLabel`.

```csharp
32          private void createInstance_Click(object sender, RoutedEventArgs e)
35              device?.StopCollecting();
37              Units units = imperialRadio.IsChecked == true ? Units.Imperial : Units.Metric;
39              if (lengthRadio.IsChecked == true)
41                  device = new MeasureLengthDevice(units);
43              else
45                  device = new MeasureMassDevice(units);
48              UpdateUnitLabels();
49              ClearResults();
50              SetRunning(false);
51              getMetricValue.IsEnabled = true;
52              getImperialValue.IsEnabled = true;
53              getRawData.IsEnabled = true;
54              statusText.Text = $"Создан {device.GetType().Name} ({units}). Нажмите Start Collecting.";
```

- **стр. 35** — остановить предыдущий прибор (если был), чтобы не остался фоновый поток.
- **стр. 37** — единицы выбираем тернарным оператором; `IsChecked` типа `bool?`, поэтому `== true`.
- **стр. 39–45** — **главное в задании (Task 3)**: в одну и ту же переменную типа `IMeasuringDevice` кладём объект *разных* классов — **полиморфизм**. Дальше окну всё равно, какой прибор внутри.
- **стр. 48** — обновить подписи единиц под тип прибора.
- **стр. 49–53** — очистить результаты и включить нужные кнопки (как в задании 1).
- **стр. 54** — `GetType()` возвращает **реальный** тип объекта во время выполнения (`MeasureLengthDevice` или `MeasureMassDevice`), даже если переменная объявлена как интерфейс. `.Name` — имя класса без namespace.

#### `UpdateUnitLabels` — **стр. 126–138** (новый метод)

**XAML:** `metricLabel`, `imperialLabel` (свойство `Content`).

```csharp
126         private void UpdateUnitLabels()
128             if (device is MeasureMassDevice)
130                 metricLabel.Content = "Metric Value (кг):";
131                 imperialLabel.Content = "Imperial Value (фунт):";
133             else
135                 metricLabel.Content = "Metric Value (мм):";
136                 imperialLabel.Content = "Imperial Value (дюйм):";
```

- **стр. 128** — оператор **`is`** проверяет, является ли объект, лежащий в переменной-интерфейсе, экземпляром `MeasureMassDevice`.
- **стр. 130–131** — для массы: кг и фунты.
- **стр. 135–136** — для длины: мм и дюймы.
- У `Label` текст хранится в `Content` (а не `Text`, как у `TextBox`/`TextBlock`), потому что `Label` может содержать любой объект, не только строку.

---

## 6. Важные моменты (вопросы на защите)

1. **Зачем абстрактный класс, если есть интерфейс?** — Интерфейс не может хранить общий код и поля; абстрактный класс убирает дублирование `StartCollecting`/`StopCollecting`/`GetRawData`/`GetMeasurements`.
2. **Можно ли написать `new MeasureDataDevice()`?** — Нет, абстрактный класс нельзя инстанцировать.
3. **`abstract` vs `virtual`.** — `abstract` — тела нет, наследник *обязан* переопределить. `virtual` — тело есть, наследник *может* переопределить. Оба переопределяются через `override`.
4. **Почему поля стали `protected`?** — `private` поля базового класса наследнику не видны, а конструкторы и `MetricValue`/`ImperialValue` наследников с ними работают.
5. **Почему `measurementType` больше не `const`?** — Константа одна на класс и задаётся сразу; базовый класс общий для двух приборов, поэтому значение задаёт конструктор каждого наследника.
6. **Сколько базовых классов может быть у класса?** — Один. Интерфейсов — сколько угодно (`class A : Base, I1, I2`).
7. **Что даёт полиморфизм в окне?** — Код кнопок Start/Stop/Get* не изменился вообще, хотя добавился новый прибор.
8. **Порядок вызова конструкторов.** — Сначала базовый (`MeasureDataDevice`), потом наследник.
