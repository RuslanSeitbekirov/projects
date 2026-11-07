using System.ComponentModel;
using System.Windows;
using MeasuringDevice;

namespace Exercise2TestHarness
{
    /// <summary>
    /// Тестовое окно (test harness) для класса MeasureLengthDevice.
    /// По методичке оно намеренно не содержит обработки исключений,
    /// чтобы не скрывать ошибки тестируемого класса. Вместо этого
    /// кнопки блокируются, пока действие не имеет смысла.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Поле device имеет тип интерфейса IMeasuringDevice.
        /// Окно вызывает только методы интерфейса и не знает,
        /// какой конкретный класс устройства ему подсунули.
        /// </summary>
        private IMeasuringDevice device;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Task 4: создание экземпляра MeasureLengthDevice с выбранными единицами.
        /// </summary>
        private void createInstance_Click(object sender, RoutedEventArgs e)
        {
            // Останавливаем предыдущее устройство, если оно было
            device?.StopCollecting();

            if (imperialRadio.IsChecked == true)
            {
                device = new MeasureLengthDevice(Units.Imperial);
            }
            else
            {
                device = new MeasureLengthDevice(Units.Metric);
            }

            ClearResults();
            SetRunning(false);
            getMetricValue.IsEnabled = true;
            getImperialValue.IsEnabled = true;
            getRawData.IsEnabled = true;
            statusText.Text = $"Создан MeasureLengthDevice ({(imperialRadio.IsChecked == true ? "Imperial" : "Metric")}). Нажмите Start Collecting.";
        }

        private void startCollecting_Click(object sender, RoutedEventArgs e)
        {
            device.StartCollecting();
            SetRunning(true);
            statusText.Text = "Сбор данных идёт. Подождите ~10 секунд и запросите значения.";
        }

        private void stopCollecting_Click(object sender, RoutedEventArgs e)
        {
            device.StopCollecting();
            SetRunning(false);
            statusText.Text = "Сбор данных остановлен.";
        }

        private void getMetricValue_Click(object sender, RoutedEventArgs e)
        {
            metricValueBox.Text = device.MetricValue().ToString();
        }

        private void getImperialValue_Click(object sender, RoutedEventArgs e)
        {
            imperialValueBox.Text = device.ImperialValue().ToString();
        }

        private void getRawData_Click(object sender, RoutedEventArgs e)
        {
            int[] raw = device.GetRawData();
            if (raw == null)
            {
                statusText.Text = "Данных нет — сбор ещё не запускался.";
                return;
            }

            // GetRawData каждый раз возвращает ссылку на ТОТ ЖЕ массив.
            // Если присвоить ItemsSource тот же объект, WPF не перерисует
            // список, поэтому сначала сбрасываем источник в null.
            rawDataValues.ItemsSource = null;
            rawDataValues.ItemsSource = raw;
        }

        /// <summary>
        /// При закрытии окна останавливаем фоновый сбор данных.
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            device?.StopCollecting();
        }

        // ---------------- Вспомогательные методы ----------------

        /// <summary>Включает/выключает кнопки Start/Stop по состоянию устройства.</summary>
        private void SetRunning(bool running)
        {
            startCollecting.IsEnabled = !running;
            stopCollecting.IsEnabled = running;
        }

        private void ClearResults()
        {
            metricValueBox.Text = string.Empty;
            imperialValueBox.Text = string.Empty;
            rawDataValues.ItemsSource = null;
        }
    }
}
