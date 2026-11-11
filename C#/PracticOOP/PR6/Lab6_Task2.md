# Практическая работа №6. Задание 2 — Использование лямбда-выражений

Папка проекта: `Lab6_Task2/`

> **Как читать этот файл.** Проект — это задание 1 с добавленным событием `HeartBeat` («сердцебиение»).
> Всё, что осталось без изменений, построчно разобрано в [Lab6_Task1.md](Lab6_Task1.md); здесь — только **новое и изменённое**.
> Номера строк даны по файлам из `Lab6_Task2` (из-за вставок они сдвинулись относительно задания 1).
>
> Без изменений: `IMeasuringDevice.cs`, `UnitsEnumeration.cs`, `DeviceController.cs`, `App.xaml.cs`.

---

## 1. Немного теории

### 1.1. Лямбда-выражение

**Лямбда-выражение** — метод без имени, записанный прямо там, где он нужен:

```csharp
(параметры) => { тело }
```

Знак `=>` читается «переходит в». Сравнение двух способов подписаться на событие:

```csharp
// Задание 1: обычный метод + делегат (три места в коде)
private EventHandler handler;                              // 1) поле
handler = new EventHandler(device_NewMeasurementTaken);    // 2) делегат
device.NewMeasurementTaken += handler;
private void device_NewMeasurementTaken(object sender, EventArgs e) { ... }   // 3) метод

// Задание 2: лямбда (одно место)
device.HeartBeat += (o, args) => { label.Content = args.TimeStamp; };
```

Особенности:

- **Типы параметров не пишутся** — компилятор берёт их из делегата события.
- **Замыкание**: лямбда «видит» поля класса и локальные переменные метода, в котором написана.
- **Минус**: от лямбды трудно отписаться (`-=`), потому что у неё нет имени.

### 1.2. Свой делегат и свои аргументы события

Стандартный `EventHandler` передаёт обработчику пустой `EventArgs`. Чтобы передать вместе с событием данные (здесь — время удара), делают две вещи:

1. класс-наследник `EventArgs` с нужными свойствами (`HeartBeatEventArgs`);
2. свой тип делегата, у которого второй параметр — этот класс (`HeartBeatEventHandler`).

### 1.3. Как работает сердцебиение

Второй `BackgroundWorker` (`heartBeatTimer`) в фоновом потоке спит заданный интервал (1000 мс) и вызывает `ReportProgress`; в потоке окна `ProgressChanged` поднимает событие `HeartBeat`. Оба обработчика записаны лямбдами.

Сердцебиение показывает, что объект прибора «жив»: оно **не останавливается** кнопкой Stop и прекращается только после `Dispose`.

---

## 2. `MeasuringDevice/HeartBeatEvent.cs` — новый файл

```csharp
1   using System;
3   namespace MeasuringDevice
14      public class HeartBeatEventArgs : EventArgs
22          public DateTime TimeStamp { get; private set; }
28          public HeartBeatEventArgs()
29              : base()
31              TimeStamp = DateTime.Now;
44      public delegate void HeartBeatEventHandler(object sender, HeartBeatEventArgs args);
```

- **стр. 1** — `EventArgs`, `DateTime`.
- **стр. 14** — Task 2: класс аргументов события наследуется от `EventArgs`. По соглашению имя заканчивается на `EventArgs`.
- **стр. 22** — **автоматическое свойство** (компилятор сам создаёт скрытое поле). `private set` — менять можно только внутри класса, для остальных свойство только для чтения.
- **стр. 28–29** — конструктор без параметров; `: base()` — сначала вызвать конструктор базового класса `EventArgs`.
- **стр. 31** — запоминаем момент создания объекта — это и есть время удара.
- **стр. 44** — Task 3: объявление **своего типа делегата**. Похоже на заголовок метода со словом `delegate`: подходящие методы ничего не возвращают и принимают `object` и `HeartBeatEventArgs`.

---

## 3. `MeasuringDevice/IEventEnabledMeasuringDevice.cs` — дополнен

```csharp
25          event EventHandler NewMeasurementTaken;
34          event HeartBeatEventHandler HeartBeat;
40          int HeartBeatInterval { get; }
```

- **стр. 25** — событие из задания 1.
- **стр. 34** — Task 4: новое событие, его тип — наш делегат `HeartBeatEventHandler`.
- **стр. 40** — свойство только для чтения: интервал между ударами в миллисекундах.

---

## 4. `MeasuringDevice/MeasureDataDevice.cs` — что добавлено

### 4.1. Подключение

```csharp
4   using System.Threading;
```

- **стр. 4** — ради `Thread.Sleep`.

### 4.2. Поля, свойство и событие сердцебиения

```csharp
75          private BackgroundWorker heartBeatTimer;
81          protected int heartBeatIntervalTime;
87          public int HeartBeatInterval
89              get { return heartBeatIntervalTime; }
90              private set { heartBeatIntervalTime = value; }
98          public event HeartBeatEventHandler HeartBeat;
```

- **стр. 75** — Task 6, шаг 1: второй фоновый работник.
- **стр. 81** — Task 5, шаг 2: интервал; `protected` — его задаёт конструктор наследника.
- **стр. 87–90** — Task 5, шаг 3: свойство интерфейса. `get` доступен всем, `set` — `private` (только этому классу).
- **стр. 98** — Task 5, шаг 4: событие, объявленное в интерфейсе.

### 4.3. `OnHeartBeat()` — поднимает событие

```csharp
103         protected virtual void OnHeartBeat()
106             if (disposed)
108                 return;
112             if (HeartBeat != null)
117                 HeartBeat(this, new HeartBeatEventArgs());
```

- **стр. 106–108** — объект уже уничтожен — не стучим (защита от «опоздавшего» удара).
- **стр. 112** — есть ли подписчики.
- **стр. 117** — вызов события. Второй параметр — новый объект `HeartBeatEventArgs`; его конструктор запоминает текущее время, и подписчик получит его в `args.TimeStamp`.

### 4.4. Запуск из `StartCollecting`

```csharp
161         public void StartCollecting()
177             StartHeartBeat();
```

- **стр. 177** — Task 7: вместе со сбором запускается сердцебиение. Остальные строки метода — как в задании 1.

### 4.5. `StartHeartBeat()` — два обработчика-лямбды (Task 6)

```csharp
318         private void StartHeartBeat()
323             if (heartBeatTimer != null)
325                 return;
328             heartBeatTimer = new BackgroundWorker();
329             heartBeatTimer.WorkerSupportsCancellation = true;
330             heartBeatTimer.WorkerReportsProgress = true;
337             heartBeatTimer.DoWork += (o, args) =>
340                 while (true)
345                     Thread.Sleep(HeartBeatInterval);
348                     if (disposed)
350                         break;
354                     heartBeatTimer.ReportProgress(0);
360             heartBeatTimer.ProgressChanged += (o, args) =>
362                 OnHeartBeat();
366             heartBeatTimer.RunWorkerAsync();
```

- **стр. 323–325** — **отступление от методички**: сердцебиение при Stop не останавливается, поэтому при повторном Start второе запускать нельзя — иначе событий стало бы вдвое больше.
- **стр. 328–330** — создать работника, разрешить отмену и доклады.
- **стр. 337** — подписка на `DoWork` **лямбдой**. Параметры `o` и `args` — как требует делегат `DoWorkEventHandler` (кто вызвал и данные события); типы не указаны. Тело выполняется в **фоновом потоке**. Сравните с заданием 1, где для этого был отдельный метод `dataCollector_DoWork`.
- **стр. 340** — бесконечный цикл, выход только через `break`.
- **стр. 345** — поток спит один интервал. `HeartBeatInterval` — свойство класса; лямбда видит его благодаря **замыканию**.
- **стр. 348–350** — объект уничтожен — выходим из цикла, фоновая работа заканчивается.
- **стр. 354** — «удар»: вызовет `ProgressChanged` в потоке окна.
- **стр. 360–362** — вторая лямбда, обработчик `ProgressChanged`: выполняется в потоке окна и поднимает событие `HeartBeat`.
- **стр. 366** — запуск фоновой работы.

### 4.6. Освобождение в `Dispose`

```csharp
451         public void Dispose()
474             if (heartBeatTimer != null)
476                 heartBeatTimer.Dispose();
```

- **стр. 474–476** — Task 8: освободить работника сердцебиения. Сам цикл остановится, увидев `disposed == true` (флаг ставится выше в этом же методе).

Остальные методы файла (`OnNewMeasurementTaken` — стр. 134, `StopCollecting` — стр. 181, `GetMeasurements` — стр. 205, `dataCollector_DoWork` — стр. 235, `dataCollector_ProgressChanged` — стр. 294, `GetRawData` — стр. 372, `WriteLog` — стр. 424) не изменились — их разбор в [Lab6_Task1.md](Lab6_Task1.md), раздел 4.

---

## 5. `MeasureMassDevice.cs` — цепочка конструкторов (Task 9)

```csharp
19          public MeasureMassDevice(Units deviceUnits, string logFileName, int heartBeatInterval)
21              unitsToUse = deviceUnits;
22              measurementType = DeviceType.MASS;
23              loggingFileName = logFileName;
26              heartBeatIntervalTime = heartBeatInterval;
38          public MeasureMassDevice(Units deviceUnits, string logFileName)
39              : this(deviceUnits, logFileName, 1000)
```

- **стр. 19** — основной конструктор получил третий параметр — интервал сердцебиения.
- **стр. 21–23** — как в задании 1.
- **стр. 26** — интервал сохраняется в `protected`-поле базового класса.
- **стр. 38–39** — второй конструктор с двумя параметрами. `: this(...)` — **цепочка конструкторов**: сначала вызывается первый конструктор этого же класса, интервал подставляется по умолчанию — 1000 мс. Тело пустое, код не дублируется.

`MetricValue` (стр. 44–51) и `ImperialValue` (стр. 55–61) не изменились.

В `MeasureLengthDevice.cs` добавлена одна строка — **стр. 20**: `heartBeatIntervalTime = 1000;` (без неё интервал был бы 0, и цикл сердцебиения крутился бы без пауз).

---

## 6. WPF-окно `Monitor`

### 6.1. Что добавлено в разметку `MainWindow.xaml`

| Элемент (`x:Name`) | Строка XAML | Тип | Где используется в C# |
|---|---|---|---|
| `heartBeatTimeStamp` | 21 | `Label` | лямбда-обработчик события `HeartBeat` в `startCollecting_Click` — свойство `Content` |
| `disposeObject` | 32–33 | `Button`, `Click="disposeObject_Click"`, изначально `IsEnabled="False"` | `disposeObject_Click`; `IsEnabled` включается в `startCollecting_Click` |

Также добавлена третья строка в сетку окна (для метки времени), а `statusText` переехал в `Grid.Row="2"` (стр. 69).

Остальные элементы те же, что в задании 1; их строки в этом файле: `startCollecting` — 27–28, `stopCollecting` — 29–30, `mostRecentMeasureBox` — 52, `metricValueBox` — 56, `imperialValueBox` — 60, `rawDataValues` — 65.

### 6.2. `MainWindow.xaml.cs` — изменённые и новые части

Строки **1–28** (подключения, поля `device` и `newMeasurementTaken`, конструктор), `device_NewMeasurementTaken` (**стр. 101–117**) и `Window_Closing` (**стр. 153–159**) совпадают с заданием 1.

#### `startCollecting_Click` — **стр. 32–94**

**XAML:** кнопка `startCollecting`; новое — метка `heartBeatTimeStamp` и кнопка `disposeObject`; а также `stopCollecting`, три текстовых поля, `rawDataValues`, `statusText`.

```csharp
32          private void startCollecting_Click(object sender, RoutedEventArgs e)
35              if (device == null)
38                  string logFile = Path.Combine(AppContext.BaseDirectory, "LogFile.txt");
41                  device = new MeasureMassDevice(Units.Metric, logFile);
59                  device.HeartBeat += (o, args) =>
62                      heartBeatTimeStamp.Content = "HeartBeat Timestamp: " + args.TimeStamp;
68                  disposeObject.IsEnabled = true;
74              newMeasurementTaken = new EventHandler(device_NewMeasurementTaken);
78              device.NewMeasurementTaken += newMeasurementTaken;
81–84           (очистка полей и списка)
86              device.StartCollecting();
90              startCollecting.IsEnabled = false;
91              stopCollecting.IsEnabled = true;
92–93           statusText.Text = ...
```

- **стр. 35–41** — прибор создаётся при первом нажатии. Вызывается конструктор с двумя параметрами, он по цепочке подставляет интервал 1000 мс.
- **стр. 59** — **Task 10: подписка на `HeartBeat` лямбдой.** `(o, args)` — параметры по делегату `HeartBeatEventHandler`: `o` — прибор, `args` — `HeartBeatEventArgs`.
- **стр. 62** — тело лямбды: в метку пишется время удара из `args.TimeStamp`. У `Label` текст задаётся свойством `Content`. Лямбда обращается к элементу окна, потому что написана внутри метода окна (замыкание), и выполняется в потоке окна.
- Подписка стоит **внутри** `if (device == null)` — выполняется один раз на объект. Если бы она выполнялась при каждом Start, обработчиков становилось бы всё больше, а отписать лямбду нельзя.
- **стр. 68** — включить кнопку Dispose Object.
- **стр. 74–93** — как в задании 1: делегат, подписка на `NewMeasurementTaken`, очистка, запуск, переключение кнопок.

#### `stopCollecting_Click` — **стр. 120–132**

**XAML:** `stopCollecting`, `startCollecting`, `statusText`.

```csharp
122             device.StopCollecting();
126             device.NewMeasurementTaken -= newMeasurementTaken;
128–129         (кнопки)
130–131         statusText.Text = "Сбор остановлен, но сердцебиение продолжается ...";
```

- Изменился только текст в строке состояния: измерения прекращаются, а метка времени продолжает обновляться — от `HeartBeat` мы не отписывались, и прибор его не останавливает.

#### `disposeObject_Click` — **стр. 138–150** (новый)

**XAML:** кнопка `disposeObject` (`Click`), `startCollecting`, `stopCollecting`, `statusText`.

```csharp
138         private void disposeObject_Click(object sender, RoutedEventArgs e)
140             device.Dispose();
144             device = null;
146             startCollecting.IsEnabled = true;
147             stopCollecting.IsEnabled = false;
148             disposeObject.IsEnabled = false;
149             statusText.Text = "Объект уничтожен: метка времени больше не обновляется.";
```

- **стр. 140** — уничтожить прибор: останавливается сбор, закрывается журнал, цикл сердцебиения завершается.
- **стр. 144** — забыть ссылку; при следующем Start будет создан новый объект (и новая подписка на `HeartBeat`).
- **стр. 146–148** — Start доступен, Stop и Dispose — нет.
- **стр. 149** — сообщение (Task 11, шаг 5: метка времени перестала обновляться).

---

## 7. Важные моменты (вопросы на защите)

1. **Что такое лямбда-выражение?** — Анонимный метод вида `(параметры) => тело`; компилятор превращает его в делегат.
2. **Откуда лямбда знает типы `o` и `args`?** — Из типа делегата события, на которое её подписывают.
3. **Что такое замыкание?** — Лямбда использует переменные и поля из окружающего кода (`HeartBeatInterval`, `heartBeatTimeStamp`), даже когда выполняется позже.
4. **Как отписаться от лямбды?** — Напрямую никак: нужно сохранить её в переменную-делегат и использовать эту переменную в `+=` и `-=`.
5. **Зачем свой `HeartBeatEventArgs`?** — Чтобы передать обработчику данные события (время). В `EventArgs` данных нет.
6. **Зачем свой делегат?** — Чтобы второй параметр обработчика был типа `HeartBeatEventArgs`. (В современном C# вместо этого часто используют готовый `EventHandler<HeartBeatEventArgs>`.)
7. **Что значит `: this(...)` у конструктора?** — Вызвать другой конструктор этого же класса; `: base(...)` — конструктор базового класса.
8. **Почему после Stop метка времени продолжает идти?** — Сердцебиение показывает, что объект существует, а не что идёт сбор; оно останавливается только в `Dispose`.
9. **Почему `private set` у `TimeStamp` и `HeartBeatInterval`?** — Значение задаётся один раз внутри класса; снаружи его можно только читать.
