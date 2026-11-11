# Практическая работа №6. Задание 3 — Многопоточные приложения и асинхронные методы

Папка проекта: `Lab6_Task3/ThreadsDemo/` — одно WPF-приложение.

> **Как читать этот файл.** Код разбит на фрагменты, под каждым — пояснение по номерам строк (`стр. N`).
> Фигурные скобки, пустые строки и комментарии отдельно не расписываются. Строки вида `log("...")` / `Log("...")`
> просто выводят текст в окно — они упоминаются, только если в них есть что-то кроме текста.
> `App.xaml.cs` — заготовка шаблона WPF без своего кода.
>
> Соответствие пунктам задания:
>
> | Пункт задания | Где в коде |
> |---|---|
> | 1. Движение объектов, потоки с приоритетами, анализ состояния, `Join`, `Barrier` | `MainWindow.xaml.cs` — гонка шариков |
> | 2. `lock` и `Monitor` | `Demos.LockAndMonitor` |
> | 3. Мьютекс и семафор | `Demos.MutexAndSemaphore` |
> | 4. `Task`, преимущества TPL | `Demos.Tasks` |
> | 5. `Parallel.For` | `Demos.ParallelFor` |
> | 6. `async` / `await` | `Demos.SlowSumAsync` + `asyncDemo_Click` |

---

## 1. Немного теории

**Поток** (`Thread`) — отдельная «линия выполнения» внутри программы. Несколько потоков работают одновременно (на разных ядрах) или по очереди маленькими кусочками времени.

**UI-поток** — поток, создавший окно. Главное правило WPF: **элементы окна можно менять только из UI-потока.** Из фонового потока нужно просить об этом через `Dispatcher.BeginInvoke(...)`. Если UI-поток занят долгой работой — окно «зависает».

**Состояние гонки** — два потока одновременно меняют общие данные, и результат зависит от случайного порядка их действий. Лечится **синхронизацией**:

| Средство | Сколько потоков пускает | Особенность |
|---|---|---|
| `lock` / `Monitor` | 1 | самое быстрое и простое; только внутри одной программы |
| `Mutex` | 1 | объект операционной системы; с именем работает между программами |
| `Semaphore` | не больше N | ограничение числа одновременных «посетителей» |
| `Barrier` | — | точка встречи: все участники ждут друг друга и идут дальше вместе |
| `Interlocked` | — | атомарные операции над числом (`++` без потерь) |

**`Thread` → `Task` → `async/await`** — три поколения многопоточности в C#:

- `Thread` — создаём поток вручную, сами ждём (`Join`), результат вернуть неудобно;
- `Task` (библиотека TPL) — «задача», которую выполнит поток из пула; может вернуть результат, есть ожидание групп, продолжения;
- `async/await` — способ писать асинхронный код так, будто он обычный последовательный; построен на `Task`.

---

## 2. `Demos.cs` — опыты к пунктам 2–6

### 2.1. Подключения и заголовок

```csharp
1   using System;
2   using System.Diagnostics;
3   using System.Threading;
4   using System.Threading.Tasks;
6   namespace ThreadsDemo
15      public static class Demos
```

- **стр. 1** — `Action`, `Environment`.
- **стр. 2** — `Stopwatch` (секундомер).
- **стр. 3** — `Thread`, `Monitor`, `Mutex`, `Semaphore`, `Interlocked`.
- **стр. 4** — `Task`, `Parallel`.
- **стр. 15** — статический класс: объект не создаётся, методы вызываются через имя класса.

Каждый метод получает параметр `Action<string> log` — **делегат** «метод, принимающий строку». Окно передаёт туда свой метод `Log`, и текст попадает в поле вывода. Класс `Demos` поэтому ничего не знает про WPF.

### 2.2. `RunThreads` — запустить несколько потоков и дождаться всех

```csharp
25          private static void RunThreads(int count, Action<int> work)
27              Thread[] threads = new Thread[count];
29              for (int i = 0; i < count; i++)
36                  int number = i + 1;
40                  threads[i] = new Thread(() => work(number));
43                  threads[i].Start();
48              foreach (Thread t in threads)
50                  t.Join();
```

- **стр. 25** — `work` — делегат «что делать потоку»; получает номер потока.
- **стр. 27** — массив для потоков.
- **стр. 36** — **важная тонкость.** Лямбда запоминает не значение, а саму *переменную*. Если написать `work(i + 1)`, то к моменту запуска потока `i` уже изменится. Поэтому для каждого потока делается своя копия.
- **стр. 40** — поток создаётся из метода, который он выполнит (здесь — лямбда).
- **стр. 43** — до `Start()` поток не работает.
- **стр. 48–50** — `Join()` — «жди, пока этот поток закончится». После цикла завершены все.

### 2.3. Пункт 2: `LockAndMonitor`

```csharp
61          public static void LockAndMonitor(Action<string> log)
63              const int ThreadCount = 4;
64              const int Steps = 1000000;
65              int expected = ThreadCount * Steps;
71              int counter = 0;
75              object locker = new object();
```

- **стр. 63–65** — 4 потока по миллиону прибавлений; правильный итог — 4 000 000.
- **стр. 71** — общий счётчик, который меняют все потоки.
- **стр. 75** — объект-«замок»: сам ничего не хранит, нужен только чтобы его «запирать».

**а) Без блокировки**

```csharp
82              RunThreads(ThreadCount, number =>
84                  for (int i = 0; i < Steps; i++)
86                      counter++;
89              if (counter == expected)
```

- **стр. 86** — `counter++` — на самом деле три действия: прочитать, прибавить, записать. Два потока могут прочитать одно и то же значение — одно прибавление теряется.
- **стр. 89** — сравнение с ожидаемым: обычно получается заметно меньше 4 000 000 (при проверке — около миллиона).

**б) `lock`**

```csharp
101             counter = 0;
102             RunThreads(ThreadCount, number =>
104                 for (int i = 0; i < Steps; i++)
106                     lock (locker)
108                         counter++;
```

- **стр. 106** — внутрь блока `lock` в каждый момент входит только **один** поток; остальные ждут. Итог — ровно 4 000 000.

**в) `Monitor.Enter` / `Monitor.Exit`**

```csharp
118             counter = 0;
119             RunThreads(ThreadCount, number =>
123                     Monitor.Enter(locker);
126                         counter++;
130                         Monitor.Exit(locker);
```

- **стр. 123, 130** — то же самое, что `lock`, но вручную: `lock` — это сокращение для `Monitor.Enter` + `try/finally` + `Monitor.Exit`. `finally` (стр. 128) гарантирует, что замок откроется даже при исключении — иначе остальные потоки ждали бы вечно.

**г) `Monitor.TryEnter`**

```csharp
139             Thread holder = new Thread(() =>
141                 lock (locker)
143                     Thread.Sleep(500);
146             holder.Start();
147             Thread.Sleep(50);
149             bool entered = Monitor.TryEnter(locker, 100);
151             if (entered)
153                 Monitor.Exit(locker);
156             holder.Join();
158             entered = Monitor.TryEnter(locker, 100);
160             if (entered)
162                 Monitor.Exit(locker);
```

- **стр. 139–146** — вспомогательный поток занимает замок на полсекунды.
- **стр. 147** — даём ему время успеть.
- **стр. 149** — `TryEnter` ждёт не дольше 100 мс и возвращает `true`/`false`. Здесь — `false`: замок занят. Через `lock` так сделать нельзя — он ждёт бесконечно.
- **стр. 151–153** — если всё-таки вошли — обязательно выйти.
- **стр. 156** — дождаться, пока поток отпустит замок.
- **стр. 158–162** — вторая попытка: `true`, и замок сразу освобождается.

**д) `Monitor.Wait` / `Monitor.Pulse`**

```csharp
169             bool ready = false;
171             Thread waiter = new Thread(() =>
173                 lock (locker)
177                     while (!ready)
179                         Monitor.Wait(locker);
184             waiter.Start();
186             Thread.Sleep(300);
187             lock (locker)
189                 ready = true;
191                 Monitor.Pulse(locker);
193             waiter.Join();
```

- **стр. 169** — условие, которого ждёт поток.
- **стр. 173** — `Wait` и `Pulse` можно вызывать только внутри `lock` на том же объекте.
- **стр. 177–179** — `Wait` **отпускает замок и усыпляет поток** до сигнала. Проверка в цикле `while` — стандартная защита на случай, если поток разбудили, а условие ещё не выполнено.
- **стр. 187–191** — главный поток под замком выставляет условие и вызывает `Pulse` — будит один ждущий поток.
- **стр. 193** — ждём завершения.

### 2.4. Пункт 3: `MutexAndSemaphore`

```csharp
205         public static void MutexAndSemaphore(Action<string> log)
212             using (Mutex mutex = new Mutex())
214                 RunThreads(3, number =>
216                     mutex.WaitOne();
220                         Thread.Sleep(300);
225                         mutex.ReleaseMutex();
```

- **стр. 212** — мьютекс — системный ресурс; `using` сам вызовет `Dispose` в конце блока.
- **стр. 216** — `WaitOne()` — дождаться, пока мьютекс свободен, и занять.
- **стр. 220** — имитация работы; строки `log` до и после показывают «вошёл»/«вышел».
- **стр. 225** — освободить (в `finally`, чтобы выполнилось всегда). В выводе потоки не перемешиваются: вошёл — вышел, вошёл — вышел.

```csharp
237             using (Semaphore semaphore = new Semaphore(2, 2))
239                 int inside = 0;
241                 RunThreads(5, number =>
243                     semaphore.WaitOne();
248                         int now = Interlocked.Increment(ref inside);
250                         Thread.Sleep(400);
251                         Interlocked.Decrement(ref inside);
255                         semaphore.Release();
```

- **стр. 237** — семафор: первое число — сколько мест свободно сейчас, второе — сколько всего. Как парковка на 2 машины.
- **стр. 239** — счётчик «сколько потоков внутри».
- **стр. 241** — желающих войти — 5.
- **стр. 243** — занять место или ждать.
- **стр. 248** — `Interlocked.Increment` — потокобезопасное `++` без `lock`; возвращает новое значение. В выводе оно никогда не больше 2.
- **стр. 251** — потокобезопасное `--`.
- **стр. 255** — освободить место.

### 2.5. Пункт 4: `Tasks`

```csharp
270         public static void Tasks(Action<string> log)
278             Task simple = Task.Run(() =>
282             simple.Wait();
```

- **стр. 278** — `Task.Run` выполняет лямбду на потоке из **пула потоков** (новый поток не создаётся). В выводе видно, что номер потока другой.
- **стр. 282** — `Wait()` — дождаться окончания (аналог `Join`).

```csharp
287             Task<int> sumTask = Task.Run(() =>
289                 int sum = 0;
290                 for (int i = 1; i <= 100; i++)
292                     sum += i;
294                 return sum;
```

- **стр. 287** — `Task<int>` — задача, которая **вернёт `int`**. У `Thread` вернуть результат неудобно.
- **стр. 289–294** — сумма 1…100; `return` внутри лямбды — результат задачи.
- Результат читается через `sumTask.Result` (в строке `log`); если задача не закончена — `Result` подождёт.

```csharp
300             Task<int>[] tasks = new Task<int>[3];
301             for (int i = 0; i < tasks.Length; i++)
303                 int number = i + 1;
304                 tasks[i] = Task.Run(() =>
306                     Thread.Sleep(300 * number);
307                     return number * number;
310             Task.WaitAll(tasks);
```

- **стр. 300–307** — три задачи разной длительности, каждая возвращает квадрат своего номера (копия `number` — та же тонкость с замыканием).
- **стр. 310** — `WaitAll` ждёт сразу все задачи.

```csharp
316             Task<int> first = Task.Run(() => 21);
317             Task continuation = first.ContinueWith(previous =>
321             continuation.Wait();
```

- **стр. 316** — короткая лямбда без фигурных скобок: значение выражения — результат.
- **стр. 317** — `ContinueWith` — **продолжение**: «когда задача закончится, выполни ещё вот это». В `previous` приходит завершившаяся задача.
- **стр. 321** — ждём продолжение.

В конце метод выводит список преимуществ `Task` перед `Thread` — его можно взять в отчёт (п. 4 задания): потоки из пула, возврат результата, `WaitAll`/`WhenAll`, продолжения и отмена, исключения не теряются, основа для `async/await`.

### 2.6. Пункт 5: `ParallelFor`

```csharp
339         public static void ParallelFor(Action<string> log)
341             const int Max = 3000000;
347             Stopwatch watch = Stopwatch.StartNew();
348             int count1 = 0;
349             for (int i = 2; i <= Max; i++)
351                 if (IsPrime(i))
353                     count1++;
356             watch.Stop();
357             long time1 = watch.ElapsedMilliseconds;
```

- **стр. 341** — считаем простые числа до 3 000 000.
- **стр. 347** — секундомер создан и запущен.
- **стр. 348–353** — обычный цикл: всё делает один поток, поэтому обычный `count1++` безопасен.
- **стр. 356–357** — остановить секундомер, запомнить время.

```csharp
363             watch.Restart();
364             int count2 = 0;
365             Parallel.For(2, Max + 1, i =>
367                 if (IsPrime(i))
371                     Interlocked.Increment(ref count2);
374             watch.Stop();
375             long time2 = watch.ElapsedMilliseconds;
378             if (time2 > 0)
```

- **стр. 363** — секундомер с нуля.
- **стр. 365** — `Parallel.For(от, до_не_включая, тело)` — диапазон делится между несколькими потоками. Поэтому `Max + 1`.
- **стр. 371** — `count2` общая для всех потоков — нужен `Interlocked.Increment`, иначе потери (как в опыте 2а).
- **стр. 378** — защита от деления на ноль при подсчёте ускорения. При проверке на 12 ядрах ускорение было примерно в 5–6 раз.

```csharp
386             string order = "";
387             object orderLock = new object();
388             Parallel.For(0, 10, i =>
390                 Thread.Sleep(30);
391                 lock (orderLock)
393                     order += i + " ";
```

- **стр. 388–393** — показ того, что итерации идут **не по порядку**: строка получается вроде `6 7 5 1 4 0 8 2 3 9`. Значит, `Parallel.For` нельзя применять, если шаги зависят друг от друга. `lock` нужен, потому что строку `order` меняют несколько потоков.

```csharp
401         private static bool IsPrime(int n)
403             if (n < 2)
405                 return false;
408             for (int d = 2; (long)d * d <= n; d++)
410                 if (n % d == 0)
412                     return false;
415             return true;
```

- **стр. 403–405** — 0 и 1 не простые.
- **стр. 408** — делители достаточно проверять до корня из `n`; `(long)` — чтобы `d * d` не переполнил `int`.
- **стр. 410–412** — нашёлся делитель — число составное.

### 2.7. Пункт 6: `SlowSumAsync`

```csharp
431         public static async Task<long> SlowSumAsync(int n, Action<string> log)
439             await Task.Delay(2000);
443             long result = await Task.Run(() =>
445                 long sum = 0;
446                 for (int i = 1; i <= n; i++)
448                     sum += i;
450                 return sum;
454             return result;
```

- **стр. 431** — признаки асинхронного метода: слово `async`, тип результата `Task<long>` («обещание вернуть `long`»), имя по соглашению заканчивается на `Async`.
- **стр. 439** — `Task.Delay` — пауза, **не занимающая поток** (в отличие от `Thread.Sleep`). `await`: метод «ставится на паузу» и возвращает управление вызвавшему; когда задача завершится — продолжится со следующей строки.
- **стр. 443–450** — тяжёлый расчёт отдаётся в фоновый поток (`Task.Run`), и метод снова ждёт через `await`. `await` сам «достаёт» `long` из `Task<long>`.
- **стр. 454** — пишется обычный `return` с числом; компилятор сам завернёт его в `Task<long>`.

---

## 3. Разметка `MainWindow.xaml` — с чем работает код

| Элемент (`x:Name`) | Строка XAML | Тип | Где используется в C# |
|---|---|---|---|
| `startRace` | 19–20 | `Button`, `Click="startRace_Click"` | `startRace_Click`; `IsEnabled` — там же и в `WaitForFinish` |
| `intervalBox` | 23–24 | `ComboBox`, `SelectionChanged="intervalBox_SelectionChanged"`, пункты 100/250/500/1000 | `intervalBox_SelectionChanged` (через `sender`) |
| `track` | 36 | `Canvas` 700×150 | в коде напрямую не используется; на нём лежат шарики |
| линия «Barrier» | 38 | `Line`, X = 352 | соответствует константе `Checkpoint` (шаг 50) |
| линия финиша | 41 | `Line`, X = 672 | соответствует `Finish` (шаг 100) |
| `ball0`, `ball1`, `ball2` | 44–46 | `Ellipse` 30×30 | массив `balls`; `Canvas.SetLeft` в `timer_Tick` |
| `threadStates` | 51 | `TextBlock` | `timer_Tick` — свойство `Text` |
| `lockDemo`, `mutexDemo`, `taskDemo`, `parallelDemo` | 56–59 | `Button`, `Click="..._Click"` | одноимённые обработчики; `IsEnabled` в `RunDemoAsync` |
| `asyncDemo` | 60 | `Button`, `Click="asyncDemo_Click"` | `asyncDemo_Click` |
| `clearLog` | 61 | `Button`, `Click="clearLog_Click"` | `clearLog_Click` |
| `logBox` | 65 | `TextBox` (только чтение) | `Log`, `clearLog_Click` |

`Canvas` — панель, где у каждого элемента заданы координаты `Canvas.Left` и `Canvas.Top`. Код меняет `Canvas.Left` шариков — так они «едут».

Откуда числа: шарик шириной 30 проходит путь `TrackPixels = 640`. На шаге 50 его левый край на 320, правый — на 350 (линия барьера на 352); на шаге 100 — левый на 640, правый на 670 (линия финиша на 672).

---

## 4. `MainWindow.xaml.cs`

### 4.1. Подключения, константы, поля

```csharp
1   using System;
2   using System.Threading;
3   using System.Threading.Tasks;
4   using System.Windows;
5   using System.Windows.Controls;
6   using System.Windows.Shapes;
7   using System.Windows.Threading;
9   namespace ThreadsDemo
27      public partial class MainWindow : Window
```

- **стр. 2** — `Thread`, `Barrier`, `ThreadPriority`.
- **стр. 3** — `Task`.
- **стр. 4** — `Window`, `RoutedEventArgs`.
- **стр. 5** — `Button`, `ComboBox`, `ComboBoxItem`, `Canvas`.
- **стр. 6** — `Ellipse`.
- **стр. 7** — `DispatcherTimer`.
- **стр. 27** — `partial`: вторая половина класса создаётся из XAML.

```csharp
31          private const int BallCount = 3;
32          private const int Finish = 100;
33          private const int Checkpoint = 50;
34          private const double TrackPixels = 640;
37          private readonly string[] names = { "Красный", "Зелёный", "Синий" };
38          private readonly ThreadPriority[] priorities =
40              ThreadPriority.Lowest,
41              ThreadPriority.Normal,
42              ThreadPriority.Highest
```

- **стр. 31–34** — число шариков (и потоков), длина пути в шагах, шаг с барьером, длина пути в пикселях.
- **стр. 37–42** — имена и приоритеты потоков; номер в массиве = номер шарика.

```csharp
49          private readonly double[] positions = new double[BallCount];
52          private readonly object positionsLock = new object();
55          private Thread[] threads;
58          private Barrier barrier;
63          private Ellipse[] balls;
69          private readonly DispatcherTimer timer = new DispatcherTimer
71              Interval = TimeSpan.FromMilliseconds(250)
```

- **стр. 49** — **общие данные**: сколько шагов прошёл каждый шарик. Пишут фоновые потоки, читает UI-поток.
- **стр. 52** — «замок» для этого массива.
- **стр. 55** — потоки-шарики.
- **стр. 58** — барьер.
- **стр. 63** — шарики из разметки, собранные в массив.
- **стр. 69–71** — `DispatcherTimer` — таймер WPF: его событие `Tick` срабатывает **в UI-потоке**. Создаётся при объявлении поля, чтобы уже существовать, когда `InitializeComponent` вызовет `intervalBox_SelectionChanged`.

```csharp
74          public MainWindow()
76              InitializeComponent();
78              balls = new Ellipse[] { ball0, ball1, ball2 };
81              timer.Tick += timer_Tick;
```

- **стр. 76** — создать элементы окна из XAML.
- **стр. 78** — **XAML:** `ball0`, `ball1`, `ball2` → массив.
- **стр. 81** — подписка на событие таймера (события — задание 1).

### 4.2. `startRace_Click` — запуск гонки

**XAML:** кнопка `startRace` (`Click="startRace_Click"`).

```csharp
89          private void startRace_Click(object sender, RoutedEventArgs e)
92              startRace.IsEnabled = false;
95              lock (positionsLock)
97                  for (int i = 0; i < BallCount; i++)
99                      positions[i] = 0;
109             barrier = new Barrier(BallCount, b =>
111                 Log("Barrier: все три шарика на контрольной точке, едем дальше вместе.");
```

- **стр. 92** — пока идёт гонка, вторую запускать нельзя.
- **стр. 95–99** — под замком вернуть шарики на старт.
- **стр. 109** — `Barrier(3, действие)` — барьер на трёх участников. Поток, вызвавший `SignalAndWait`, спит, пока не подойдут **все**. Второй параметр — лямбда, которая выполняется один раз, когда все собрались.

```csharp
116             threads = new Thread[BallCount];
117             for (int i = 0; i < BallCount; i++)
120                 threads[i] = new Thread(MoveBall);
122                 threads[i].Name = names[i];
125                 threads[i].Priority = priorities[i];
129                 threads[i].IsBackground = true;
133                 threads[i].Start(i);
139             Thread waiter = new Thread(WaitForFinish);
140             waiter.IsBackground = true;
141             waiter.Start();
144             timer.Start();
```

- **стр. 120** — поток будет выполнять метод `MoveBall`.
- **стр. 122** — имя потока (удобно при отладке).
- **стр. 125** — **приоритет** — подсказка системе, кому чаще давать процессор.
- **стр. 129** — **фоновый поток** не мешает программе закрыться: при закрытии окна такие потоки завершаются сами.
- **стр. 133** — запуск; номер шарика передаётся параметром (придёт в `MoveBall` как `object`).
- **стр. 139–141** — отдельный поток, который ждёт конца гонки. Ждать прямо здесь нельзя: метод выполняется в UI-потоке, окно зависло бы.
- **стр. 144** — запуск таймера перерисовки.

### 4.3. `MoveBall` — работа потока-шарика (фоновый поток)

К элементам окна напрямую не обращается.

```csharp
151         private void MoveBall(object parameter)
153             int index = (int)parameter;
156             Random random = new Random();
158             for (int step = 1; step <= Finish; step++)
163                 DoSomeMath();
164                 Thread.Sleep(random.Next(15, 45));
168                 lock (positionsLock)
170                     positions[index] = step;
174                 if (step == Checkpoint)
176                     Log(names[index] + " доехал до барьера и ждёт остальных.");
177                     barrier.SignalAndWait();
181             Log(names[index] + " финишировал.");
```

- **стр. 153** — распаковка параметра: номер шарика.
- **стр. 156** — у каждого потока свой генератор случайных чисел.
- **стр. 158** — 100 шагов.
- **стр. 163–164** — «вычисление координаты»: немного нагрузки на процессор и случайная пауза, чтобы шарики ехали с разной скоростью.
- **стр. 168–170** — запись координаты **под замком**: пока поток пишет, таймер не читает, и наоборот.
- **стр. 174–177** — на шаге 50 поток сообщает о прибытии и засыпает на барьере до прихода остальных.
- **стр. 181** — после цикла метод заканчивается — поток завершается сам.

```csharp
186         private static void DoSomeMath()
188             double x = 0;
189             for (int i = 0; i < 200000; i++)
191                 x += Math.Sqrt(i);
```

- **стр. 186–191** — бесполезные вычисления, чтобы поток был занят делом.

### 4.4. `WaitForFinish` — ожидание через `Join` (отдельный поток)

**XAML:** кнопка `startRace` (через `Dispatcher`).

```csharp
198         private void WaitForFinish()
202             foreach (Thread t in threads)
204                 t.Join();
213             barrier.Dispose();
217             Dispatcher.BeginInvoke(new Action(() =>
219                 startRace.IsEnabled = true;
```

- **стр. 202–204** — `Join` останавливает **текущий** поток, пока не закончится `t`. Пройдя по всем, точно знаем, что завершились все три.
- **стр. 207–210** — сообщения в лог, в том числе честное замечание: на многоядерном процессоре приоритет почти не влияет на результат.
- **стр. 213** — барьер больше не нужен.
- **стр. 217–219** — мы в фоновом потоке, а кнопку менять можно только из UI-потока. `Dispatcher.BeginInvoke` — «выполни эту лямбду в UI-потоке».

### 4.5. `timer_Tick` — перерисовка и анализ состояния потоков (UI-поток)

**XAML:** шарики `ball0…ball2` (через массив `balls`), текст `threadStates`.

```csharp
227         private void timer_Tick(object sender, EventArgs e)
229             if (threads == null)
231                 return;
236             double[] copy = new double[BallCount];
237             lock (positionsLock)
239                 positions.CopyTo(copy, 0);
242             string text = "";
243             for (int i = 0; i < BallCount; i++)
246                 Canvas.SetLeft(balls[i], copy[i] / Finish * TrackPixels);
255                 text += string.Format("{0,-8} приоритет {1,-8} пройдено {2,3}%   IsAlive = {3,-5}  состояние: {4}",
256                                       names[i], priorities[i], copy[i], threads[i].IsAlive, threads[i].ThreadState);
257                 text += Environment.NewLine;
259             threadStates.Text = text.TrimEnd();
```

- **стр. 229–231** — гонку ещё не запускали.
- **стр. 236–239** — быстро скопировать координаты под замком, а рисовать уже без него — чтобы не задерживать потоки-шарики.
- **стр. 246** — шаги → пиксели; `Canvas.SetLeft` двигает шарик.
- **стр. 255–256** — **анализ состояния потока**: `IsAlive` — поток запущен и не завершён; `ThreadState` — что он делает (`Running`, `WaitSleepJoin` — спит или ждёт, `Stopped`; `Background` — признак фонового). В формате `{0,-8}` число после запятой — ширина колонки (минус — выравнивание влево). Приоритет берётся из массива: у завершившегося потока свойство `Priority` читать нельзя — исключение.
- **стр. 259** — вывести текст без последнего перевода строки.

### 4.6. `intervalBox_SelectionChanged` — интервал проверки

**XAML:** `ComboBox intervalBox` (`SelectionChanged`), его пункты `ComboBoxItem`.

```csharp
263         private void intervalBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
267             ComboBox box = (ComboBox)sender;
268             ComboBoxItem item = (ComboBoxItem)box.SelectedItem;
269             if (item == null)
271                 return;
274             int milliseconds = int.Parse(item.Content.ToString());
275             timer.Interval = TimeSpan.FromMilliseconds(milliseconds);
```

- **стр. 267** — список берётся из `sender`: событие срабатывает ещё при создании окна, когда поле `intervalBox` может быть не заполнено.
- **стр. 268–271** — выбранный пункт; если ничего не выбрано — выйти.
- **стр. 274** — текст пункта («250») → число.
- **стр. 275** — новый интервал таймера — «заданный промежуток времени» из задания.

### 4.7. `Log` и `clearLog_Click`

**XAML:** `logBox`; кнопка `clearLog`.

```csharp
286         private void Log(string text)
288             Dispatcher.BeginInvoke(new Action(() =>
290                 logBox.AppendText(text + Environment.NewLine);
291                 logBox.ScrollToEnd();
295         private void clearLog_Click(object sender, RoutedEventArgs e)
297             logBox.Clear();
```

- **стр. 288** — метод можно вызывать из любого потока: запись всё равно выполнится в UI-потоке.
- **стр. 290–291** — дописать строку и прокрутить вниз.
- **стр. 297** — очистить поле.

### 4.8. Запуск опытов 2–5

**XAML:** кнопки `lockDemo`, `mutexDemo`, `taskDemo`, `parallelDemo`.

```csharp
308         private async Task RunDemoAsync(Button button, Action demo)
310             button.IsEnabled = false;
314             await Task.Run(demo);
316             button.IsEnabled = true;
322         private async void lockDemo_Click(object sender, RoutedEventArgs e)
324             await RunDemoAsync(lockDemo, () => Demos.LockAndMonitor(Log));
```

- **стр. 310** — выключить кнопку на время опыта.
- **стр. 314** — опыт выполняется в фоновом потоке, `await` ждёт его, **не блокируя окно**.
- **стр. 316** — после `await` выполнение продолжается в UI-потоке, поэтому кнопку можно менять.
- **стр. 322–324** — обработчик кнопки: опыт передаётся лямбдой, внутрь опыта передаётся метод `Log`.

`mutexDemo_Click` (**стр. 327–329**), `taskDemo_Click` (**стр. 332–334**) и `parallelDemo_Click` (**стр. 337–339**) устроены так же и вызывают `Demos.MutexAndSemaphore`, `Demos.Tasks`, `Demos.ParallelFor`.

### 4.9. `asyncDemo_Click` — опыт 6

**XAML:** кнопка `asyncDemo`.

```csharp
352         private async void asyncDemo_Click(object sender, RoutedEventArgs e)
354             asyncDemo.IsEnabled = false;
357             Log("До await: поток №" + Environment.CurrentManagedThreadId + " (это UI-поток)");
363             long sum = await Demos.SlowSumAsync(200000000, Log);
365             Log("После await: поток №" + Environment.CurrentManagedThreadId + " (снова UI-поток)");
370             asyncDemo.IsEnabled = true;
```

- **стр. 352** — `async void` — единственный допустимый случай: обработчик события обязан возвращать `void`.
- **стр. 357** — номер потока до `await`.
- **стр. 363** — на `await` обработчик «ставится на паузу» и отдаёт управление окну: его можно двигать, можно запустить гонку. Когда `SlowSumAsync` закончит — продолжение со следующей строки, результат уже в `sum`.
- **стр. 365** — номер потока тот же: после `await` WPF возвращает выполнение в UI-поток.
- **стр. 370** — включить кнопку обратно.

---

## 5. Важные моменты (вопросы на защите)

1. **Почему фоновый поток не может сам подвинуть шарик?** — Элементы WPF привязаны к создавшему их потоку; обращение из другого даёт `InvalidOperationException`. Нужен `Dispatcher` или таймер UI-потока.
2. **Что делает `Join`?** — Блокирует вызвавший поток до завершения указанного.
3. **Чем `Barrier` отличается от `Join`?** — `Join` ждёт *завершения* потока; `Barrier` — точка встречи *посреди* работы, после которой все продолжают.
4. **Влияет ли приоритет на скорость?** — Только когда потоков больше, чем свободных ядер. На многоядерном процессоре в этой программе разницы почти нет.
5. **Чем фоновый поток (`IsBackground`) отличается от основного?** — Программа не ждёт фоновые потоки при закрытии.
6. **Почему `counter++` теряет прибавления?** — Это три операции (чтение, сложение, запись), и потоки перебивают друг друга.
7. **`lock` и `Monitor` — в чём разница?** — `lock` — сокращённая запись `Monitor.Enter/Exit` с `try/finally`. `Monitor` дополнительно умеет `TryEnter` (с таймаутом), `Wait` и `Pulse`.
8. **Мьютекс vs `lock`?** — Мьютекс — объект ОС, медленнее, но может быть именованным и работать между процессами.
9. **Семафор vs мьютекс?** — Мьютекс пускает одного, семафор — до N одновременно.
10. **Чем `Task` лучше `Thread`?** — Пул потоков, возврат результата, ожидание групп, продолжения, удобная обработка исключений, основа для `async/await`.
11. **Когда нельзя применять `Parallel.For`?** — Когда итерации зависят друг от друга или важен порядок; общие переменные нужно защищать.
12. **`Thread.Sleep` vs `Task.Delay`?** — `Sleep` блокирует поток, `await Task.Delay` поток освобождает.
13. **Что будет, если вместо `await` написать `.Result` в обработчике кнопки?** — UI-поток заблокируется, окно зависнет (а возможна и взаимная блокировка).
14. **Зачем копия `int number = i + 1` перед лямбдой?** — Лямбда захватывает переменную, а не значение; без копии все потоки увидели бы изменившийся `i`.
