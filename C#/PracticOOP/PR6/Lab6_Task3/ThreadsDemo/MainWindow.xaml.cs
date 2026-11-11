using System;
using System.Threading;            // Thread, Barrier, ThreadPriority
using System.Threading.Tasks;      // Task
using System.Windows;
using System.Windows.Controls;     // Button, ComboBox, Canvas
using System.Windows.Shapes;       // Ellipse
using System.Windows.Threading;    // DispatcherTimer

namespace ThreadsDemo
{
    /// <summary>
    /// Главное окно.
    ///
    /// Верхняя часть — пункт 1 задания: три шарика, координату каждого
    /// считает СВОЙ поток со своим приоритетом. Таймер через заданные
    /// промежутки времени перерисовывает шарики и показывает состояние потоков.
    ///
    /// Нижняя часть — кнопки, которые запускают опыты из файла Demos.cs
    /// (пункты 2–6).
    ///
    /// Главное правило, которое я усвоил: элементы окна можно менять только
    /// из того потока, который создал окно (его называют UI-потоком).
    /// Если фоновый поток сам попробует подвинуть шарик или написать текст,
    /// программа упадёт с InvalidOperationException. Поэтому фоновые потоки
    /// здесь только СЧИТАЮТ, а рисует UI-поток.
    /// </summary>
    public partial class MainWindow : Window
    {
        // ---------------- Настройки гонки ----------------

        private const int BallCount = 3;        // сколько шариков (и потоков)
        private const int Finish = 100;         // длина пути в "шагах"
        private const int Checkpoint = 50;      // на этом шаге стоит барьер
        private const double TrackPixels = 640; // длина пути на экране в пикселях

        // Названия и приоритеты потоков (номер в массиве = номер шарика)
        private readonly string[] names = { "Красный", "Зелёный", "Синий" };
        private readonly ThreadPriority[] priorities =
        {
            ThreadPriority.Lowest,    // самый низкий приоритет
            ThreadPriority.Normal,    // обычный
            ThreadPriority.Highest    // самый высокий
        };

        // ---------------- Общие данные потоков ----------------

        // Сколько шагов прошёл каждый шарик. Сюда ПИШУТ фоновые потоки,
        // а ЧИТАЕТ UI-поток (в таймере). Раз данные общие — нужен lock.
        private readonly double[] positions = new double[BallCount];

        // Объект-"замок" для массива positions
        private readonly object positionsLock = new object();

        // Сами потоки
        private Thread[] threads;

        // Барьер: точка, где потоки ждут друг друга
        private Barrier barrier;

        // ---------------- Интерфейс ----------------

        // Шарики из разметки, собранные в массив, чтобы обращаться по номеру
        private Ellipse[] balls;

        // DispatcherTimer — таймер WPF. Его событие Tick срабатывает
        // в UI-потоке, поэтому в обработчике можно спокойно менять окно.
        // Создаю его прямо здесь, чтобы он уже существовал, когда
        // InitializeComponent вызовет intervalBox_SelectionChanged.
        private readonly DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(250)   // значение по умолчанию
        };

        public MainWindow()
        {
            InitializeComponent();

            balls = new Ellipse[] { ball0, ball1, ball2 };

            // Подписываемся на событие таймера (события — задание 1)
            timer.Tick += timer_Tick;
        }

        // =====================================================================
        //  Пункт 1. Потоки, приоритеты, Join и Barrier
        // =====================================================================

        /// <summary>Кнопка "Старт гонки".</summary>
        private void startRace_Click(object sender, RoutedEventArgs e)
        {
            // Пока идёт гонка, вторую запускать нельзя
            startRace.IsEnabled = false;

            // Возвращаем шарики на старт
            lock (positionsLock)
            {
                for (int i = 0; i < BallCount; i++)
                {
                    positions[i] = 0;
                }
            }

            // Barrier(3, ...) — барьер на 3 участников. Поток, дошедший
            // до барьера, вызывает SignalAndWait и засыпает, пока до барьера
            // не дойдут ВСЕ участники. Тогда барьер "открывается" и все
            // потоки продолжают работу одновременно.
            // Второй параметр — действие, которое выполняется один раз
            // в момент, когда все собрались (здесь — лямбда с сообщением).
            barrier = new Barrier(BallCount, b =>
            {
                Log("Barrier: все три шарика на контрольной точке, едем дальше вместе.");
            });

            Log("=== 1. Гонка: 3 потока с разными приоритетами ===");

            threads = new Thread[BallCount];
            for (int i = 0; i < BallCount; i++)
            {
                // Поток будет выполнять метод MoveBall
                threads[i] = new Thread(MoveBall);

                threads[i].Name = names[i];

                // Приоритет — подсказка системе, кому чаще давать процессор
                threads[i].Priority = priorities[i];

                // Фоновый поток не мешает программе закрыться: если закрыть
                // окно посреди гонки, такие потоки завершатся сами.
                threads[i].IsBackground = true;

                // Запуск. Номер шарика передаём как параметр
                // (он придёт в MoveBall в виде object).
                threads[i].Start(i);
            }

            // Отдельный поток, который ждёт окончания гонки через Join.
            // Ждать прямо здесь нельзя: этот метод выполняется в UI-потоке,
            // и окно зависло бы до конца гонки.
            Thread waiter = new Thread(WaitForFinish);
            waiter.IsBackground = true;
            waiter.Start();

            // Запускаем таймер, который рисует шарики и состояния потоков
            timer.Start();
        }

        /// <summary>
        /// Метод, который выполняет каждый поток-шарик.
        /// Он работает в ФОНОВОМ потоке, поэтому к элементам окна не обращается.
        /// </summary>
        private void MoveBall(object parameter)
        {
            int index = (int)parameter;   // номер шарика: 0, 1 или 2

            // У каждого потока свой генератор случайных чисел
            Random random = new Random();

            for (int step = 1; step <= Finish; step++)
            {
                // "Вычисляем" следующую координату: немного нагружаем
                // процессор и делаем случайную паузу, чтобы шарики
                // ехали с разной скоростью.
                DoSomeMath();
                Thread.Sleep(random.Next(15, 45));

                // Записываем новую координату в общий массив.
                // lock: пока мы пишем, таймер не сможет читать, и наоборот.
                lock (positionsLock)
                {
                    positions[index] = step;
                }

                // Доехали до контрольной точки — ждём остальных
                if (step == Checkpoint)
                {
                    Log(names[index] + " доехал до барьера и ждёт остальных.");
                    barrier.SignalAndWait();
                }
            }

            Log(names[index] + " финишировал.");
            // Метод закончился — поток завершается сам.
        }

        /// <summary>Бесполезные вычисления, чтобы поток был занят делом.</summary>
        private static void DoSomeMath()
        {
            double x = 0;
            for (int i = 0; i < 200000; i++)
            {
                x += Math.Sqrt(i);
            }
        }

        /// <summary>
        /// Ждёт окончания всех потоков-шариков. Выполняется в отдельном потоке.
        /// </summary>
        private void WaitForFinish()
        {
            // Join останавливает ТЕКУЩИЙ поток, пока не закончится поток t.
            // Пройдя по всем, мы точно знаем, что закончились все три.
            foreach (Thread t in threads)
            {
                t.Join();
            }

            Log("Join: все потоки завершились, гонка окончена.");
            Log("(На многоядерном процессоре приоритет почти не влияет на результат: " +
                "каждому потоку хватает своего ядра.)");
            Log("");

            // Барьер больше не нужен
            barrier.Dispose();

            // Мы в фоновом потоке, а кнопку трогать можно только из UI-потока.
            // Dispatcher.BeginInvoke — "выполни этот код в UI-потоке".
            Dispatcher.BeginInvoke(new Action(() =>
            {
                startRace.IsEnabled = true;
            }));
        }

        /// <summary>
        /// Срабатывает по таймеру (в UI-потоке): перерисовывает шарики
        /// и показывает состояние потоков.
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {
            if (threads == null)
            {
                return;
            }

            // Быстро копируем координаты под замком, а рисуем уже без него,
            // чтобы не задерживать потоки-шарики.
            double[] copy = new double[BallCount];
            lock (positionsLock)
            {
                positions.CopyTo(copy, 0);
            }

            string text = "";
            for (int i = 0; i < BallCount; i++)
            {
                // Переводим шаги в пиксели и двигаем шарик
                Canvas.SetLeft(balls[i], copy[i] / Finish * TrackPixels);

                // Анализ состояния потока:
                //   ThreadState — что поток делает (Running — работает,
                //                 WaitSleepJoin — спит или ждёт, Stopped — закончился;
                //                 Background — признак фонового потока);
                //   IsAlive     — true, пока поток запущен и не завершился.
                // Приоритет беру из массива: у завершившегося потока свойство
                // Priority читать нельзя — будет исключение.
                text += string.Format("{0,-8} приоритет {1,-8} пройдено {2,3}%   IsAlive = {3,-5}  состояние: {4}",
                                      names[i], priorities[i], copy[i], threads[i].IsAlive, threads[i].ThreadState);
                text += Environment.NewLine;
            }
            threadStates.Text = text.TrimEnd();
        }

        /// <summary>Выбор интервала проверки состояния потоков.</summary>
        private void intervalBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Событие срабатывает ещё во время создания окна, когда поле
            // intervalBox может быть не заполнено, поэтому список берём из sender.
            ComboBox box = (ComboBox)sender;
            ComboBoxItem item = (ComboBoxItem)box.SelectedItem;
            if (item == null)
            {
                return;   // ничего не выбрано — оставляем прежний интервал
            }

            int milliseconds = int.Parse(item.Content.ToString());
            timer.Interval = TimeSpan.FromMilliseconds(milliseconds);
        }

        // =====================================================================
        //  Вывод текста в окно из любого потока
        // =====================================================================

        /// <summary>
        /// Дописывает строку в поле внизу окна. Метод можно вызывать
        /// из любого потока: сама запись всё равно выполнится в UI-потоке.
        /// </summary>
        private void Log(string text)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                logBox.AppendText(text + Environment.NewLine);
                logBox.ScrollToEnd();
            }));
        }

        private void clearLog_Click(object sender, RoutedEventArgs e)
        {
            logBox.Clear();
        }

        // =====================================================================
        //  Пункты 2–5: запуск опытов из Demos.cs
        // =====================================================================

        /// <summary>
        /// Запускает опыт в фоновом потоке, чтобы окно не зависало,
        /// и на это время выключает кнопку.
        /// </summary>
        private async Task RunDemoAsync(Button button, Action demo)
        {
            button.IsEnabled = false;

            // Task.Run выполняет опыт в фоне, а await ждёт его окончания,
            // НЕ блокируя окно (подробнее — в опыте 6).
            await Task.Run(demo);

            button.IsEnabled = true;
        }

        // В каждом обработчике опыт передаётся лямбдой "() => ...",
        // а внутрь опыта передаётся наш метод Log для вывода текста.

        private async void lockDemo_Click(object sender, RoutedEventArgs e)
        {
            await RunDemoAsync(lockDemo, () => Demos.LockAndMonitor(Log));
        }

        private async void mutexDemo_Click(object sender, RoutedEventArgs e)
        {
            await RunDemoAsync(mutexDemo, () => Demos.MutexAndSemaphore(Log));
        }

        private async void taskDemo_Click(object sender, RoutedEventArgs e)
        {
            await RunDemoAsync(taskDemo, () => Demos.Tasks(Log));
        }

        private async void parallelDemo_Click(object sender, RoutedEventArgs e)
        {
            await RunDemoAsync(parallelDemo, () => Demos.ParallelFor(Log));
        }

        // =====================================================================
        //  Пункт 6: async / await
        // =====================================================================

        /// <summary>
        /// Обработчик кнопки сам является асинхронным методом (слово async).
        /// Обычно async-метод возвращает Task, но у обработчиков событий
        /// тип результата обязан быть void — это единственный случай,
        /// когда пишут "async void".
        /// </summary>
        private async void asyncDemo_Click(object sender, RoutedEventArgs e)
        {
            asyncDemo.IsEnabled = false;

            Log("=== 6. async / await ===");
            Log("До await: поток №" + Environment.CurrentManagedThreadId + " (это UI-поток)");
            Log("Пока идёт расчёт, окно НЕ зависает: его можно двигать, можно запустить гонку.");

            // На слове await обработчик "ставится на паузу" и отдаёт управление
            // окну. Когда SlowSumAsync закончит, выполнение продолжится
            // со следующей строки — причём снова в UI-потоке.
            long sum = await Demos.SlowSumAsync(200000000, Log);

            Log("После await: поток №" + Environment.CurrentManagedThreadId + " (снова UI-поток)");
            Log("Результат: сумма чисел от 1 до 200 000 000 = " + sum);
            Log("Если бы вместо await стояло .Result или Thread.Sleep, окно зависло бы на всё это время.");
            Log("");

            asyncDemo.IsEnabled = true;
        }
    }
}
