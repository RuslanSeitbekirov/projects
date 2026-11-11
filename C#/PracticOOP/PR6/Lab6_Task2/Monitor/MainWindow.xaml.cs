using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using MeasuringDevice;

namespace Monitor
{
    /// <summary>
    /// Окно для проверки событий NewMeasurementTaken (задание 1)
    /// и HeartBeat (задание 2 — обработчик записан лямбда-выражением).
    /// Окно — это ПОДПИСЧИК: оно подписывается на событие устройства
    /// и обновляет поля каждый раз, когда событие срабатывает.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Устройство, за которым наблюдаем
        private MeasureMassDevice device;

        // Task 11, шаг 2: делегат типа EventHandler.
        // Делегат — это переменная, в которой лежит не число и не строка,
        // а ССЫЛКА НА МЕТОД. Сохраняю его в поле, чтобы потом этим же
        // делегатом отписаться от события (в stopCollecting_Click).
        private EventHandler newMeasurementTaken;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>Кнопка Start Collecting.</summary>
        private void startCollecting_Click(object sender, RoutedEventArgs e)
        {
            // Устройство создаём один раз, при первом нажатии
            if (device == null)
            {
                // Файл журнала кладём рядом с exe-файлом программы
                string logFile = Path.Combine(AppContext.BaseDirectory, "LogFile.txt");
                // Вызывается конструктор с двумя параметрами, а он сам
                // подставляет интервал сердцебиения 1000 мс.
                device = new MeasureMassDevice(Units.Metric, logFile);

                // ЗАДАНИЕ 2, Task 10: подписываемся на событие HeartBeat
                // ЛЯМБДА-ВЫРАЖЕНИЕМ.
                //
                // Для события NewMeasurementTaken (ниже) понадобилось три вещи:
                // поле-делегат, отдельный метод device_NewMeasurementTaken
                // и строка с +=. А здесь обработчик написан прямо на месте:
                //   (o, args)  — параметры, как требует делегат HeartBeatEventHandler:
                //                o — кто вызвал событие, args — HeartBeatEventArgs;
                //   =>         — "переходит в";
                //   { ... }    — что делать, когда событие произошло.
                // Лямбда может обращаться к элементам окна (heartBeatTimeStamp),
                // потому что она написана внутри метода этого окна.
                //
                // Подписываемся один раз, при создании устройства. Если бы
                // подписка стояла ниже и выполнялась при каждом Start,
                // обработчиков становилось бы всё больше.
                device.HeartBeat += (o, args) =>
                {
                    // У Label текст задаётся свойством Content
                    heartBeatTimeStamp.Content = "HeartBeat Timestamp: " + args.TimeStamp;
                };
                // Минус лямбды: от неё трудно отписаться (-=), ведь у неё нет
                // имени. Здесь это не нужно: сердцебиение прекращается,
                // когда объект уничтожается кнопкой Dispose Object.

                disposeObject.IsEnabled = true;
            }

            // Task 11, шаг 3: кладём в делегат наш метод-обработчик.
            // Пишется только ИМЯ метода, без скобок — мы его не вызываем,
            // а только говорим, какой метод надо будет вызвать.
            newMeasurementTaken = new EventHandler(device_NewMeasurementTaken);

            // Task 11, шаг 4: подписываемся на событие устройства.
            // += значит "добавь этот метод в список подписчиков".
            device.NewMeasurementTaken += newMeasurementTaken;

            // Очищаем старые данные на экране
            mostRecentMeasureBox.Text = string.Empty;
            metricValueBox.Text = string.Empty;
            imperialValueBox.Text = string.Empty;
            rawDataValues.ItemsSource = null;

            device.StartCollecting();

            // Блокируем Start, чтобы нельзя было подписаться второй раз:
            // тогда обработчик вызывался бы дважды на одно событие.
            startCollecting.IsEnabled = false;
            stopCollecting.IsEnabled = true;
            statusText.Text = "Идёт сбор данных. Единицы: " + device.UnitsToUse +
                              ". Журнал: " + device.GetLoggingFile();
        }

        /// <summary>
        /// Task 12: обработчик события. Его вызываем не мы — его вызывает
        /// устройство, когда у него появляется новое измерение.
        /// Параметры должны быть такими, как требует делегат EventHandler.
        /// </summary>
        private void device_NewMeasurementTaken(object sender, EventArgs e)
        {
            if (device != null)
            {
                // Число надо превратить в строку, чтобы показать в текстовом поле
                mostRecentMeasureBox.Text = device.MostRecentMeasure.ToString();
                metricValueBox.Text = device.MetricValue().ToString();
                imperialValueBox.Text = device.ImperialValue().ToString();

                // Список привязан к массиву измерений. Массив каждый раз тот же
                // самый (меняется только его содержимое), и WPF не замечает
                // изменений. Поэтому сначала "отвязываем" список (null),
                // а потом привязываем заново — тогда он перерисуется.
                rawDataValues.ItemsSource = null;
                rawDataValues.ItemsSource = device.GetRawData();
            }
        }

        /// <summary>Кнопка Stop Collecting.</summary>
        private void stopCollecting_Click(object sender, RoutedEventArgs e)
        {
            device.StopCollecting();

            // Task 13: отписываемся от события оператором -=.
            // После этого наш обработчик больше не вызывается.
            device.NewMeasurementTaken -= newMeasurementTaken;

            startCollecting.IsEnabled = true;
            stopCollecting.IsEnabled = false;
            statusText.Text = "Сбор остановлен, но сердцебиение продолжается " +
                              "(устройство ещё существует).";
        }

        /// <summary>
        /// Задание 2: кнопка Dispose Object — уничтожаем устройство.
        /// После этого сердцебиение прекращается (Task 11, шаг 5).
        /// </summary>
        private void disposeObject_Click(object sender, RoutedEventArgs e)
        {
            device.Dispose();

            // Забываем про старый объект. При следующем Start
            // будет создано новое устройство.
            device = null;

            startCollecting.IsEnabled = true;
            stopCollecting.IsEnabled = false;
            disposeObject.IsEnabled = false;
            statusText.Text = "Объект уничтожен: метка времени больше не обновляется.";
        }

        /// <summary>При закрытии окна освобождаем устройство.</summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (device != null)
            {
                device.Dispose();
            }
        }
    }
}
