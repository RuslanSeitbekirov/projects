using System;
using System.ComponentModel;   // здесь лежит BackgroundWorker
using System.IO;               // StreamWriter — запись в текстовый файл
using DeviceControl;           // эмулятор прибора (DeviceController)

namespace MeasuringDevice
{
    /// <summary>
    /// Абстрактный класс измерительного устройства (из практики 5),
    /// в который добавлено СОБЫТИЕ NewMeasurementTaken.
    ///
    /// Как я понял идею событий:
    ///   - раньше окно само спрашивало устройство "есть что-нибудь новое?"
    ///     (надо было нажимать кнопки Get...);
    ///   - теперь устройство само сообщает "у меня новое измерение!",
    ///     а окно на это сообщение ПОДПИСАНО и сразу обновляется.
    ///
    /// Класс, который вызывает событие, называют издателем (sender),
    /// а тех, кто подписался, — подписчиками (receivers).
    /// </summary>
    // Task 3, шаг 3: класс теперь реализует расширенный интерфейс
    // IEventEnabledMeasuringDevice (вместо IMeasuringDevice).
    // IDisposable — стандартный интерфейс с методом Dispose() "прибери за собой".
    public abstract class MeasureDataDevice : IEventEnabledMeasuringDevice, IDisposable
    {
        // ---------------- Поля (как в практике 5) ----------------

        /// <summary>Единицы, в которых устройство выдаёт данные.</summary>
        protected Units unitsToUse;

        /// <summary>Буфер на 10 последних измерений (заполняется по кругу).</summary>
        protected int[] dataCaptured;

        /// <summary>Самое последнее измерение.</summary>
        protected int mostRecentMeasure;

        /// <summary>Контроллер эмулятора прибора.</summary>
        protected DeviceController controller;

        /// <summary>Что измеряем (длину или массу) — задаёт наследник.</summary>
        protected DeviceType measurementType;

        // ---------------- Новые поля практики 6 ----------------

        /// <summary>Имя файла журнала.</summary>
        protected string loggingFileName;

        // Объект, который умеет дописывать строки в текстовый файл.
        private StreamWriter loggingFileWriter;

        // В файл пишут сразу два потока (фоновый — измерения, главный —
        // "старт/стоп"). Чтобы они не писали одновременно и не испортили
        // файл, запись идёт внутри lock (подробно про lock — в задании 3).
        private readonly object logLock = new object();

        // true — объект уже "выброшен" (вызван Dispose), работать с ним нельзя.
        // volatile нужен потому, что переменную читает фоновый поток,
        // а меняет главный: так поток всегда увидит свежее значение.
        private volatile bool disposed = false;

        // Task 4: BackgroundWorker — готовый компонент, который выполняет
        // работу в фоновом потоке и умеет сообщать о ходе работы обратно
        // в тот поток, который его запустил (у нас — в поток окна).
        private BackgroundWorker dataCollector;

        // ---------------- Событие (Task 3) ----------------

        /// <summary>
        /// Событие "снято новое измерение".
        /// Объявление должно совпадать с тем, что записано в интерфейсе.
        /// </summary>
        public event EventHandler NewMeasurementTaken;

        /// <summary>
        /// Метод, который ВЫЗЫВАЕТ (поднимает) событие.
        /// protected virtual — чтобы наследник мог при желании его переопределить.
        /// По традиции такие методы называют On + имя события.
        /// </summary>
        protected virtual void OnNewMeasurementTaken()
        {
            // Если на событие никто не подписан, то NewMeasurementTaken == null,
            // и попытка его вызвать закончится исключением NullReferenceException.
            // Поэтому сначала проверяем, есть ли подписчики.
            if (NewMeasurementTaken != null)
            {
                // Вызываем событие как обычный метод. Все подписанные методы
                // выполнятся по очереди.
                //   1-й параметр — кто вызвал событие (этот объект, this);
                //   2-й параметр — дополнительные данные. Нам передавать нечего,
                //                  поэтому по методичке пишем null.
                NewMeasurementTaken(this, null);
            }
        }

        // ---------------- Абстрактные методы (у каждого прибора свои) ----------------

        /// <summary>Перевод в метрические единицы.</summary>
        public abstract decimal MetricValue();

        /// <summary>Перевод в имперские единицы.</summary>
        public abstract decimal ImperialValue();

        // ---------------- Запуск и остановка ----------------

        /// <summary>Запускает устройство и фоновый сбор измерений.</summary>
        public void StartCollecting()
        {
            // Если сбор уже идёт — сначала останавливаем старый,
            // иначе получится два фоновых потока сразу.
            if (controller != null)
            {
                StopCollecting();
            }

            controller = DeviceController.StartDevice(measurementType);
            WriteLog("Collecting started");

            // Task 8: запускаем сбор измерений
            GetMeasurements();
        }

        /// <summary>Останавливает устройство.</summary>
        public void StopCollecting()
        {
            if (controller != null)
            {
                controller.StopDevice();
                controller = null;
                WriteLog("Collecting stopped");
            }

            // Task 9: просим фоновую работу остановиться.
            // CancelAsync НЕ убивает поток! Он только ставит флажок
            // CancellationPending = true. Поток сам должен этот флажок
            // проверять и выходить из цикла (см. dataCollector_DoWork).
            if (dataCollector != null)
            {
                dataCollector.CancelAsync();
            }
        }

        // ---------------- Сбор измерений в фоне (Task 5–7) ----------------

        /// <summary>
        /// Task 5: настраивает BackgroundWorker и запускает его.
        /// </summary>
        private void GetMeasurements()
        {
            dataCollector = new BackgroundWorker();

            // Разрешаем отмену (без этого CancelAsync выдаст исключение)
            dataCollector.WorkerSupportsCancellation = true;

            // Разрешаем сообщать о ходе работы (без этого нельзя вызвать ReportProgress)
            dataCollector.WorkerReportsProgress = true;

            // У BackgroundWorker есть свои события. Подписываемся на них оператором +=.
            // DoWork — "что делать в фоновом потоке".
            // new DoWorkEventHandler(...) создаёт делегат — объект, который
            // "помнит", какой метод надо вызвать.
            dataCollector.DoWork += new DoWorkEventHandler(dataCollector_DoWork);

            // ProgressChanged — срабатывает, когда фоновый поток вызвал ReportProgress.
            // Самое важное: этот обработчик выполняется уже НЕ в фоновом потоке,
            // а в потоке, который запустил BackgroundWorker (в потоке окна).
            dataCollector.ProgressChanged += new ProgressChangedEventHandler(dataCollector_ProgressChanged);

            // Запуск. Метод сразу возвращает управление, а DoWork
            // начинает выполняться в фоновом потоке.
            dataCollector.RunWorkerAsync();
        }

        /// <summary>
        /// Task 6: этот метод выполняется в ФОНОВОМ потоке.
        /// Он в цикле ждёт новое измерение и сохраняет его.
        /// </summary>
        private void dataCollector_DoWork(object sender, DoWorkEventArgs e)
        {
            // sender — это тот BackgroundWorker, который нас вызвал.
            // В методичке проверяют поле dataCollector, но я заметил проблему:
            // если нажать Stop, а потом Start, в поле dataCollector окажется уже
            // НОВЫЙ BackgroundWorker, и старый цикл не узнает, что его отменили.
            // Поэтому беру "своего" работника из sender.
            BackgroundWorker worker = (BackgroundWorker)sender;

            // По той же причине запоминаю "свой" контроллер:
            // после Stop поле controller становится null.
            DeviceController myController = controller;

            // Массив на 10 измерений
            int[] buffer = new int[10];
            dataCaptured = buffer;

            // i — номер ячейки, куда пишем следующее измерение
            int i = 0;

            // Крутимся, пока нас не попросили остановиться
            while (!worker.CancellationPending)
            {
                // TakeMeasurement "зависает" на 1–3 секунды, пока прибор
                // не выдаст новое значение. В главном потоке так делать нельзя —
                // окно бы зависло. В фоновом — можно.
                int value = myController.TakeMeasurement();

                // Пока ждали, устройство могли остановить или уничтожить —
                // тогда это измерение уже никому не нужно.
                if (worker.CancellationPending || disposed)
                {
                    break;
                }

                buffer[i] = value;
                mostRecentMeasure = value;

                // Пишем измерение в файл журнала
                WriteLog("Measurement - " + mostRecentMeasure);

                // Сообщаем "есть прогресс". Обычно сюда передают проценты
                // выполнения, но наша работа бесконечная, поэтому просто 0.
                // Нам важно только то, что сработает событие ProgressChanged.
                worker.ReportProgress(0);

                // Переходим к следующей ячейке; после 9-й снова идёт 0-я,
                // то есть самые старые данные затираются новыми.
                i++;
                if (i > 9)
                {
                    i = 0;
                }
            }
        }

        /// <summary>
        /// Task 7: выполняется в потоке ОКНА, когда фоновый поток вызвал ReportProgress.
        /// </summary>
        private void dataCollector_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Поднимаем наше собственное событие — сообщаем подписчикам (окну),
            // что появилось новое измерение.
            // Так как мы уже в потоке окна, подписчик может спокойно менять
            // текст в полях (из фонового потока WPF этого делать не разрешает).
            OnNewMeasurementTaken();
        }

        // ---------------- Данные и журнал ----------------

        /// <summary>Сырые данные.</summary>
        public int[] GetRawData()
        {
            return dataCaptured;
        }

        /// <summary>Имя файла журнала.</summary>
        public string GetLoggingFile()
        {
            return loggingFileName;
        }

        // Свойства только для чтения: наружу отдаём значения protected-полей.

        /// <summary>Единицы устройства.</summary>
        public Units UnitsToUse
        {
            get { return unitsToUse; }
        }

        /// <summary>Массив измерений.</summary>
        public int[] DataCaptured
        {
            get { return dataCaptured; }
        }

        /// <summary>Последнее измерение.</summary>
        public int MostRecentMeasure
        {
            get { return mostRecentMeasure; }
        }

        /// <summary>Имя файла журнала (можно поменять).</summary>
        public string LoggingFileName
        {
            get { return loggingFileName; }
            set
            {
                lock (logLock)
                {
                    // Закрываем старый файл. Новый откроется сам
                    // при первой записи (см. WriteLog).
                    if (loggingFileWriter != null)
                    {
                        loggingFileWriter.Close();
                        loggingFileWriter = null;
                    }
                    loggingFileName = value;   // value — то, что присвоили свойству
                }
            }
        }

        /// <summary>Дописывает строку в файл журнала.</summary>
        private void WriteLog(string text)
        {
            lock (logLock)
            {
                if (disposed || string.IsNullOrEmpty(loggingFileName))
                {
                    return;
                }

                // Открываем файл при первой записи.
                // true во втором параметре — дописывать в конец, а не стирать файл.
                if (loggingFileWriter == null)
                {
                    loggingFileWriter = new StreamWriter(loggingFileName, true);
                    loggingFileWriter.AutoFlush = true;   // сразу сбрасывать текст на диск
                }

                loggingFileWriter.WriteLine(text);
            }
        }

        // ---------------- Уничтожение объекта (Task 10) ----------------

        /// <summary>
        /// Освобождает всё, чем пользовался объект: останавливает сбор,
        /// выбрасывает BackgroundWorker и закрывает файл журнала.
        /// </summary>
        public void Dispose()
        {
            StopCollecting();

            lock (logLock)
            {
                disposed = true;

                if (loggingFileWriter != null)
                {
                    loggingFileWriter.Close();
                    loggingFileWriter = null;
                }
            }

            // Task 10: освобождаем BackgroundWorker
            if (dataCollector != null)
            {
                dataCollector.Dispose();
            }
        }
    }
}
