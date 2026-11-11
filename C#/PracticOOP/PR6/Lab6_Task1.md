# Практическая работа №6. Задание 1 — Использование событий

Папка проекта: `Lab6_Task1/`

> **Как читать этот файл.** Код разбит на фрагменты, под каждым — пояснение по номерам строк (`стр. N`).
> Фигурные скобки, пустые строки и комментарии (`//`, `/// <summary>`) отдельно не расписываются.
>
> Что не разбирается построчно:
>
> - `MeasuringDevice/UnitsEnumeration.cs` — не изменился с практики 5, разбор в [../PR5/Lab5_Task1.md](../PR5/Lab5_Task1.md), раздел 2;
> - `DeviceController/DeviceController.cs` — эмулятор из практики 5 (разбор там же, раздел 5) с одним изменением, оно описано ниже в разделе 7;
> - `Monitor/App.xaml.cs` — заготовка шаблона WPF без своего кода.

---

## 1. Немного теории

### 1.1. Делегат

**Делегат** — тип, переменная которого хранит не число и не строку, а **ссылку на метод**. Делегат описывает, какими должны быть эти методы (параметры и тип результата).

```csharp
// Стандартный делегат из .NET:
public delegate void EventHandler(object sender, EventArgs e);
```

В переменную типа `EventHandler` можно положить любой метод вида `void Имя(object, EventArgs)` и потом вызвать его через эту переменную. Это называют **обратным вызовом** (callback): «вот метод, вызови его, когда что-то случится».

Делегат может хранить **несколько** методов сразу (`MulticastDelegate`). `+=` добавляет метод в список, `-=` убирает. При вызове делегата методы выполняются по очереди.

### 1.2. Событие

**Событие** (`event`) — это делегат, «защищённый» от постороннего вмешательства:

| Действие | Обычное поле-делегат | Событие (`event`) |
|---|---|---|
| Подписаться `+=` / отписаться `-=` | можно всем | можно всем |
| Вызвать (поднять) | можно всем | **только класс-владелец** |
| Присвоить `=` (стереть чужих подписчиков) | можно всем | **только класс-владелец** |

Участники:

- **издатель** (sender) — класс, в котором объявлено событие и который его вызывает. У нас — `MeasureDataDevice`;
- **подписчик** (receiver) — тот, кто дал издателю свой метод-обработчик. У нас — окно `MainWindow`.

Издатель ничего не знает о подписчиках — это снова слабая связанность (как с интерфейсами в практике 5).

### 1.3. Что изменилось по сравнению с практикой 5

| Практика 5 | Практика 6 |
|---|---|
| Окно само спрашивает прибор: кнопки *Get Metric Value*, *Get Raw Data*… | Прибор **сам сообщает** о новом измерении, окно обновляется автоматически |
| Фоновый сбор через `ThreadPool.QueueUserWorkItem` | Фоновый сбор через `BackgroundWorker` |

### 1.4. BackgroundWorker

Компонент, который выполняет работу в фоновом потоке и умеет «докладывать» в поток, который его запустил.

| Член | Что делает | В каком потоке |
|---|---|---|
| `RunWorkerAsync()` | запускает работу | вызывается из потока окна |
| событие `DoWork` | сама работа | **фоновый поток** |
| `ReportProgress(n)` | «доложить о ходе работы» | вызывается из фонового |
| событие `ProgressChanged` | реакция на доклад | **поток окна** |
| `CancelAsync()` | просит остановиться (ставит флажок) | любой |
| `CancellationPending` | тот самый флажок | проверяется в `DoWork` |

Главное правило WPF: **менять элементы окна можно только из потока окна.** Поэтому цепочка такая:

```
фоновый поток:  DoWork → новое измерение → ReportProgress(0)
                                              │
поток окна:     ProgressChanged → OnNewMeasurementTaken() → событие NewMeasurementTaken
                                              │
окно:           device_NewMeasurementTaken → обновить текстовые поля
```

---

## 2. `MeasuringDevice/IMeasuringDevice.cs`

Интерфейс из методички. По сравнению с практикой 5 добавились свойства и журнал.

```csharp
1   namespace MeasuringDevice
8       public interface IMeasuringDevice
11          decimal MetricValue();
14          decimal ImperialValue();
17          void StartCollecting();
20          void StopCollecting();
23          int[] GetRawData();
26          string GetLoggingFile();
32          Units UnitsToUse { get; }
35          int[] DataCaptured { get; }
38          int MostRecentMeasure { get; }
44          string LoggingFileName { get; set; }
```

- **стр. 1** — пространство имён библиотеки.
- **стр. 8** — объявление интерфейса; `public` — виден из проекта окна.
- **стр. 11–23** — пять методов из практики 5: значение в метрических и имперских единицах, запуск, остановка, сырые данные.
- **стр. 26** — новый метод: имя файла журнала.
- **стр. 32–38** — **свойства в интерфейсе**. `{ get; }` — требование: «у класса должно быть свойство, которое можно читать». Единицы, массив измерений, последнее измерение.
- **стр. 44** — `{ get; set; }` — свойство для чтения и записи: имя файла журнала можно поменять.

---

## 3. `MeasuringDevice/IEventEnabledMeasuringDevice.cs`

```csharp
1   using System;
3   namespace MeasuringDevice
13      public interface IEventEnabledMeasuringDevice : IMeasuringDevice
25          event EventHandler NewMeasurementTaken;
```

- **стр. 1** — `System` нужен ради делегата `EventHandler`.
- **стр. 13** — новый интерфейс **наследует** старый (`: IMeasuringDevice`): в нём есть всё из `IMeasuringDevice` плюс событие. Старый интерфейс не тронут, поэтому код, написанный под него, продолжает работать — так в методичке объясняется смысл расширения интерфейсов.
- **стр. 25** — событие в интерфейсе: слово `event`, тип делегата, имя. Класс, реализующий интерфейс, обязан объявить такое же событие.

---

## 4. `MeasuringDevice/MeasureDataDevice.cs` — издатель события

### 4.1. Подключения и заголовок

```csharp
1   using System;
2   using System.ComponentModel;
3   using System.IO;
4   using DeviceControl;
6   namespace MeasuringDevice
24      public abstract class MeasureDataDevice : IEventEnabledMeasuringDevice, IDisposable
```

- **стр. 1** — `EventHandler`, `IDisposable`.
- **стр. 2** — `BackgroundWorker` и типы его событий.
- **стр. 3** — `StreamWriter` для журнала.
- **стр. 4** — эмулятор прибора.
- **стр. 24** — абстрактный класс реализует **расширенный** интерфейс (Task 3 методички) и `IDisposable` — стандартный интерфейс с методом `Dispose()` («освободи ресурсы»).

### 4.2. Поля

```csharp
29          protected Units unitsToUse;
32          protected int[] dataCaptured;
35          protected int mostRecentMeasure;
38          protected DeviceController controller;
41          protected DeviceType measurementType;
46          protected string loggingFileName;
49          private StreamWriter loggingFileWriter;
54          private readonly object logLock = new object();
59          private volatile bool disposed = false;
64          private BackgroundWorker dataCollector;
```

- **стр. 29–41** — поля из практики 5: единицы, буфер на 10 измерений, последнее измерение, контроллер эмулятора, тип измерения. `protected` — доступны наследникам.
- **стр. 46** — имя файла журнала (задаёт конструктор наследника).
- **стр. 49** — объект для записи строк в файл; `null`, пока файл не открыт.
- **стр. 54** — объект-«замок». В файл пишут два потока (фоновый — измерения, поток окна — старт/стоп); `lock` на этом объекте не даёт им писать одновременно.
- **стр. 59** — флаг «объект уничтожен». `volatile` — поле читает фоновый поток, а меняет поток окна; модификатор гарантирует, что фоновый поток увидит свежее значение.
- **стр. 64** — фоновый работник (Task 4).

### 4.3. Событие и метод, который его вызывает

```csharp
72          public event EventHandler NewMeasurementTaken;
79          protected virtual void OnNewMeasurementTaken()
84              if (NewMeasurementTaken != null)
91                  NewMeasurementTaken(this, null);
```

- **стр. 72** — объявление события точно как в интерфейсе.
- **стр. 79** — метод, поднимающий событие. Соглашение .NET: имя `On` + имя события, `protected virtual` — наследник может переопределить.
- **стр. 84** — если подписчиков нет, событие равно `null`; вызов `null` дал бы `NullReferenceException`. Поэтому сначала проверка.
- **стр. 91** — вызов события как метода: выполнятся все подписанные обработчики. `this` — кто поднял событие; второй параметр по методичке `null` (дополнительных данных нет).

### 4.4. Абстрактные методы

```csharp
98          public abstract decimal MetricValue();
101         public abstract decimal ImperialValue();
```

- **стр. 98, 101** — пересчёт единиц у каждого прибора свой, реализуют наследники (как в практике 5).

### 4.5. Запуск и остановка

```csharp
106         public void StartCollecting()
110             if (controller != null)
112                 StopCollecting();
115             controller = DeviceController.StartDevice(measurementType);
116             WriteLog("Collecting started");
119             GetMeasurements();
```

- **стр. 110–112** — защита от двойного запуска: сначала останавливаем старый сбор.
- **стр. 115** — фабричный метод эмулятора создаёт и включает прибор нужного типа.
- **стр. 116** — запись в журнал.
- **стр. 119** — запуск фонового сбора (Task 8).

```csharp
123         public void StopCollecting()
125             if (controller != null)
127                 controller.StopDevice();
128                 controller = null;
129                 WriteLog("Collecting stopped");
136             if (dataCollector != null)
138                 dataCollector.CancelAsync();
```

- **стр. 125–129** — если прибор включён: выключить, обнулить ссылку, записать в журнал.
- **стр. 136–138** — Task 9: `CancelAsync()` **не убивает поток**, а только ставит `CancellationPending = true`. Фоновый цикл сам увидит флажок и завершится.

### 4.6. `GetMeasurements()` — настройка BackgroundWorker (Task 5)

```csharp
147         private void GetMeasurements()
149             dataCollector = new BackgroundWorker();
152             dataCollector.WorkerSupportsCancellation = true;
155             dataCollector.WorkerReportsProgress = true;
161             dataCollector.DoWork += new DoWorkEventHandler(dataCollector_DoWork);
166             dataCollector.ProgressChanged += new ProgressChangedEventHandler(dataCollector_ProgressChanged);
170             dataCollector.RunWorkerAsync();
```

- **стр. 149** — новый работник на каждый запуск сбора.
- **стр. 152** — разрешить отмену (без этого `CancelAsync` выбросит исключение).
- **стр. 155** — разрешить доклады о ходе работы (без этого `ReportProgress` выбросит исключение).
- **стр. 161** — **подписка на событие** `DoWork`. `new DoWorkEventHandler(имя_метода)` создаёт делегат, `+=` добавляет его в список подписчиков. Имя метода пишется **без скобок** — мы его не вызываем, а передаём.
- **стр. 166** — то же для события `ProgressChanged`.
- **стр. 170** — старт. Метод сразу возвращает управление, а `DoWork` начинает выполняться в фоновом потоке.

Здесь класс прибора сам выступает **подписчиком** на события `BackgroundWorker`.

### 4.7. `dataCollector_DoWork` — фоновый поток (Task 6)

```csharp
177         private void dataCollector_DoWork(object sender, DoWorkEventArgs e)
184             BackgroundWorker worker = (BackgroundWorker)sender;
188             DeviceController myController = controller;
191             int[] buffer = new int[10];
192             dataCaptured = buffer;
195             int i = 0;
198             while (!worker.CancellationPending)
203                 int value = myController.TakeMeasurement();
207                 if (worker.CancellationPending || disposed)
209                     break;
212                 buffer[i] = value;
213                 mostRecentMeasure = value;
216                 WriteLog("Measurement - " + mostRecentMeasure);
221                 worker.ReportProgress(0);
225                 i++;
226                 if (i > 9)
228                     i = 0;
```

- **стр. 177** — сигнатура задана делегатом `DoWorkEventHandler`.
- **стр. 184** — `sender` — работник, вызвавший метод. **Отступление от методички:** там проверяют поле `dataCollector`, но после Stop → Start в поле лежит уже *новый* работник, и старый цикл не узнал бы об отмене. Поэтому берётся «свой» работник.
- **стр. 188** — по той же причине запоминается «свой» контроллер (поле после Stop становится `null`).
- **стр. 191–192** — новый буфер на 10 измерений; ссылка сохраняется и в поле.
- **стр. 195** — индекс следующей ячейки.
- **стр. 198** — цикл, пока не попросили остановиться.
- **стр. 203** — `TakeMeasurement()` **блокирует поток на 1–3 секунды**, пока эмулятор «готовит» значение. В потоке окна так делать нельзя — окно зависло бы.
- **стр. 207–209** — пока ждали, сбор могли остановить или объект уничтожить — тогда измерение отбрасываем и выходим.
- **стр. 212–213** — сохраняем в буфер и в «последнее измерение».
- **стр. 216** — строка в журнал.
- **стр. 221** — «доклад». Число 0 ничего не значит (работа бесконечная, процентов нет); важно, что сработает `ProgressChanged` в потоке окна.
- **стр. 225–228** — индекс по кругу 0…9: старые данные затираются новыми.

### 4.8. `dataCollector_ProgressChanged` — поток окна (Task 7)

```csharp
236         private void dataCollector_ProgressChanged(object sender, ProgressChangedEventArgs e)
242             OnNewMeasurementTaken();
```

- **стр. 242** — поднимаем **своё** событие. Так как этот метод выполняется уже в потоке окна, подписчик может напрямую менять текстовые поля.

### 4.9. Данные и свойства

```csharp
248         public int[] GetRawData()        { return dataCaptured; }
254         public string GetLoggingFile()   { return loggingFileName; }
262         public Units UnitsToUse          { get { return unitsToUse; } }
268         public int[] DataCaptured        { get { return dataCaptured; } }
274         public int MostRecentMeasure     { get { return mostRecentMeasure; } }
```

- **стр. 248–256** — методы интерфейса: буфер измерений и имя журнала.
- **стр. 262–276** — свойства только для чтения: наружу отдаются значения `protected`-полей, изменить их снаружи нельзя.

```csharp
280         public string LoggingFileName
282             get { return loggingFileName; }
283             set
285                 lock (logLock)
289                     if (loggingFileWriter != null)
291                         loggingFileWriter.Close();
292                         loggingFileWriter = null;
294                     loggingFileName = value;
```

- **стр. 283** — блок `set` выполняется при присваивании свойству.
- **стр. 285** — под «замком», чтобы фоновый поток в этот момент не писал в файл.
- **стр. 289–292** — закрыть старый файл; новый откроется при первой записи.
- **стр. 294** — `value` — то, что присвоили свойству.

### 4.10. `WriteLog` — запись в журнал

```csharp
300         private void WriteLog(string text)
302             lock (logLock)
304                 if (disposed || string.IsNullOrEmpty(loggingFileName))
306                     return;
311                 if (loggingFileWriter == null)
313                     loggingFileWriter = new StreamWriter(loggingFileName, true);
314                     loggingFileWriter.AutoFlush = true;
317                 loggingFileWriter.WriteLine(text);
```

- **стр. 302** — только один поток за раз.
- **стр. 304–306** — объект уничтожен или имя не задано — ничего не пишем.
- **стр. 311–314** — файл открывается при первой записи. `true` — дописывать в конец, а не стирать. `AutoFlush` — сразу сбрасывать текст на диск.
- **стр. 317** — дописать строку.

### 4.11. `Dispose` (Task 10)

```csharp
327         public void Dispose()
329             StopCollecting();
331             lock (logLock)
333                 disposed = true;
335                 if (loggingFileWriter != null)
337                     loggingFileWriter.Close();
338                     loggingFileWriter = null;
343             if (dataCollector != null)
345                 dataCollector.Dispose();
```

- **стр. 329** — остановить сбор.
- **стр. 331–338** — под замком: пометить объект уничтоженным и закрыть файл.
- **стр. 343–345** — освободить работника (проверка на `null` — он создаётся только после первого Start).

---

## 5. `MeasureMassDevice.cs` и `MeasureLengthDevice.cs`

```csharp
1   using DeviceControl;
10      public class MeasureMassDevice : MeasureDataDevice
15          public MeasureMassDevice(Units deviceUnits, string logFileName)
17              unitsToUse = deviceUnits;
18              measurementType = DeviceType.MASS;
19              loggingFileName = logFileName;
23          public override decimal MetricValue()
25              if (unitsToUse == Units.Metric)
27                  return mostRecentMeasure;
30              return mostRecentMeasure * 0.4536m;
34          public override decimal ImperialValue()
36              if (unitsToUse == Units.Imperial)
38                  return mostRecentMeasure;
40              return mostRecentMeasure * 2.2046m;
```

- **стр. 1** — ради `DeviceType`.
- **стр. 10** — наследник абстрактного класса. Событие, сбор данных, журнал — всё достаётся по наследству.
- **стр. 15–19** — конструктор: единицы, тип измерения (масса) и имя журнала записываются в `protected`-поля базового класса.
- **стр. 23–30** — килограммы: метрический прибор — как есть, иначе фунты × 0.4536. `m` — литерал `decimal`.
- **стр. 34–40** — фунты: имперский прибор — как есть, иначе кг × 2.2046.

`MeasureLengthDevice.cs` устроен так же (строки те же, кроме: **стр. 18** — `DeviceType.LENGTH`, **стр. 29** — `× 25.4m`, **стр. 39** — `× 0.03937m`). В окне этой работы он не используется.

---

## 6. WPF-окно `Monitor`

### 6.1. Элементы разметки `MainWindow.xaml`, с которыми работает код

| Элемент (`x:Name`) | Строка XAML | Тип | Где используется в C# |
|---|---|---|---|
| `<Window Closing="Window_Closing">` | 7 | событие окна | `Window_Closing` |
| `startCollecting` | 22–23 | `Button`, `Click="startCollecting_Click"` | `startCollecting_Click`; `IsEnabled` меняется там же и в `stopCollecting_Click` |
| `stopCollecting` | 24–25 | `Button`, `Click="stopCollecting_Click"`, изначально `IsEnabled="False"` | `stopCollecting_Click`; `IsEnabled` |
| `mostRecentMeasureBox` | 44 | `TextBox` (только чтение) | `device_NewMeasurementTaken`, очистка в `startCollecting_Click` |
| `metricValueBox` | 48 | `TextBox` | то же |
| `imperialValueBox` | 52 | `TextBox` | то же |
| `rawDataValues` | 57 | `ListBox` | свойство `ItemsSource` в `device_NewMeasurementTaken` и `startCollecting_Click` |
| `statusText` | 61 | `TextBlock` | `startCollecting_Click`, `stopCollecting_Click` |

`Click="..."` в разметке — это тоже **подписка на событие** (кнопки), только записанная в XAML вместо `+=`.

Кнопок *Get…* из практики 5 больше нет: поля обновляет обработчик события.

### 6.2. `MainWindow.xaml.cs` построчно

```csharp
1   using System;
2   using System.ComponentModel;
3   using System.IO;
4   using System.Windows;
5   using MeasuringDevice;
7   namespace Monitor
14      public partial class MainWindow : Window
17          private MeasureMassDevice device;
23          private EventHandler newMeasurementTaken;
25          public MainWindow()
27              InitializeComponent();
```

- **стр. 1** — `EventHandler`, `EventArgs`, `AppContext`.
- **стр. 2** — `CancelEventArgs` (событие `Closing`).
- **стр. 3** — `Path`.
- **стр. 4** — `Window`, `RoutedEventArgs`.
- **стр. 5** — библиотека приборов.
- **стр. 14** — `partial`: вторая половина класса создаётся из XAML.
- **стр. 17** — прибор, за которым наблюдает окно.
- **стр. 23** — **поле-делегат** (Task 11, шаг 2). Хранится в поле, чтобы тем же делегатом потом отписаться.
- **стр. 27** — создаёт элементы окна из XAML и связывает поля `x:Name`.

#### `startCollecting_Click` — **стр. 31–64**

**XAML:** кнопка `startCollecting` (`Click`), `stopCollecting`, поля `mostRecentMeasureBox`, `metricValueBox`, `imperialValueBox`, список `rawDataValues`, `statusText`.

```csharp
31          private void startCollecting_Click(object sender, RoutedEventArgs e)
34              if (device == null)
37                  string logFile = Path.Combine(AppContext.BaseDirectory, "LogFile.txt");
38                  device = new MeasureMassDevice(Units.Metric, logFile);
44              newMeasurementTaken = new EventHandler(device_NewMeasurementTaken);
48              device.NewMeasurementTaken += newMeasurementTaken;
51              mostRecentMeasureBox.Text = string.Empty;
52              metricValueBox.Text = string.Empty;
53              imperialValueBox.Text = string.Empty;
54              rawDataValues.ItemsSource = null;
56              device.StartCollecting();
60              startCollecting.IsEnabled = false;
61              stopCollecting.IsEnabled = true;
62              statusText.Text = "Идёт сбор данных. Единицы: " + device.UnitsToUse +
63                                ". Журнал: " + device.GetLoggingFile();
```

- **стр. 34–38** — прибор создаётся один раз. Журнал кладётся рядом с exe-файлом (`AppContext.BaseDirectory`).
- **стр. 44** — Task 11, шаг 3: в делегат кладётся метод-обработчик (имя без скобок).
- **стр. 48** — Task 11, шаг 4: **подписка** на событие прибора.
- **стр. 51–54** — очистка полей и списка от прошлого сеанса.
- **стр. 56** — запуск сбора.
- **стр. 60–61** — Start блокируется, Stop включается. Это ещё и защита от повторной подписки: при двух подписках обработчик вызывался бы дважды.
- **стр. 62–63** — строка состояния; используются свойство `UnitsToUse` и метод `GetLoggingFile()` интерфейса.

#### `device_NewMeasurementTaken` — **стр. 71–87**

**XAML:** `mostRecentMeasureBox`, `metricValueBox`, `imperialValueBox`, `rawDataValues`. Этот метод вызывает **не кнопка**, а прибор.

```csharp
71          private void device_NewMeasurementTaken(object sender, EventArgs e)
73              if (device != null)
76                  mostRecentMeasureBox.Text = device.MostRecentMeasure.ToString();
77                  metricValueBox.Text = device.MetricValue().ToString();
78                  imperialValueBox.Text = device.ImperialValue().ToString();
84                  rawDataValues.ItemsSource = null;
85                  rawDataValues.ItemsSource = device.GetRawData();
```

- **стр. 71** — параметры обязаны совпадать с делегатом `EventHandler`.
- **стр. 73** — на случай, если прибора уже нет.
- **стр. 76–78** — числа превращаются в строки и показываются в полях.
- **стр. 84–85** — список привязан к массиву; массив тот же самый, меняется только содержимое, и WPF этого не замечает. Сброс в `null` и повторное присваивание заставляют список перерисоваться.

#### `stopCollecting_Click` — **стр. 90–101**

**XAML:** кнопка `stopCollecting` (`Click`), `startCollecting`, `statusText`.

```csharp
90          private void stopCollecting_Click(object sender, RoutedEventArgs e)
92              device.StopCollecting();
96              device.NewMeasurementTaken -= newMeasurementTaken;
98              startCollecting.IsEnabled = true;
99              stopCollecting.IsEnabled = false;
100             statusText.Text = "Сбор остановлен. Данные на экране больше не обновляются.";
```

- **стр. 92** — остановить сбор.
- **стр. 96** — Task 13: **отписка** тем же делегатом, что был при подписке.
- **стр. 98–100** — кнопки возвращаются в исходное состояние.

#### `Window_Closing` — **стр. 104–110**

**XAML:** атрибут `Closing="Window_Closing"` у `<Window>`.

```csharp
104         private void Window_Closing(object sender, CancelEventArgs e)
106             if (device != null)
108                 device.Dispose();
```

- **стр. 106–108** — при закрытии окна прибор освобождается: останавливается сбор, закрывается файл журнала.

---

## 7. Изменение в эмуляторе `DeviceController.cs`

В методах `GetLatestMeasure()` обоих эмуляторов (`LengthMeasuringDevice` и `MassMeasuringDevice`) добавлена строка:

```csharp
System.Threading.Thread.Sleep(rnd.Next(1000, 3000));
```

Теперь `TakeMeasurement()` **блокирует** вызвавший поток на 1–3 секунды — как требует методичка («blocks until a new measurement is available»). В практике 5 пауза стояла в самом классе прибора.

---

## 8. Важные моменты (вопросы на защите)

1. **Чем событие отличается от делегата?** — Событие построено на делегате, но вызвать его и присвоить `=` может только класс-владелец; остальным доступны только `+=` и `-=`.
2. **Зачем проверка `!= null` перед вызовом события?** — Без подписчиков событие равно `null`, вызов дал бы `NullReferenceException`. Короткая запись — `NewMeasurementTaken?.Invoke(this, null)`.
3. **Почему метод называется `OnNewMeasurementTaken` и он `protected virtual`?** — Соглашение .NET; наследник может переопределить, чтобы изменить или дополнить поведение.
4. **Зачем хранить делегат в поле окна?** — Чтобы отписаться (`-=`) тем же делегатом.
5. **Что будет, если не отписаться?** — Обработчик продолжит вызываться, а издатель будет удерживать подписчика в памяти (утечка памяти).
6. **Почему окно можно обновлять прямо в обработчике?** — `ProgressChanged` выполняется в потоке окна, и событие поднимается оттуда же.
7. **Убивает ли `CancelAsync` поток?** — Нет, только ставит флажок `CancellationPending`; цикл обязан проверять его сам.
8. **Зачем новый интерфейс вместо правки старого?** — Обратная совместимость: старый код, зависящий от `IMeasuringDevice`, не ломается.
9. **Что такое `sender` и `e`?** — `sender` — объект, поднявший событие; `e` — дополнительные данные (класс `EventArgs` или его наследник).
