using System;
using DeviceControl;   // Task 3, шаг 5: пространство имён эмулятора устройства

namespace MeasuringDevice
{
    /// <summary>
    /// Устройство для измерения длины.
    /// Реализует интерфейс <see cref="IMeasuringDevice"/>: класс ОБЯЗАН
    /// предоставить тела всех пяти методов интерфейса, иначе проект
    /// не скомпилируется.
    /// </summary>
    public class MeasureLengthDevice : IMeasuringDevice
    {
        // ------------------------------------------------------------------
        //  Поля класса (Task 3, шаги 6–7)
        // ------------------------------------------------------------------

        /// <summary>Единицы, в которых устройство выдаёт данные.</summary>
        private Units unitsToUse;

        /// <summary>
        /// Кольцевой буфер на 10 измерений. Когда буфер заполнен,
        /// новые значения затирают самые старые.
        /// </summary>
        private int[] dataCaptured;

        /// <summary>Самое последнее измерение.</summary>
        private int mostRecentMeasure;

        /// <summary>Контроллер, через который идёт работа с устройством.</summary>
        private DeviceController controller;

        /// <summary>
        /// Тип измерения. Объявлен константой (const) и сразу
        /// инициализирован значением DeviceType.LENGTH — это устройство
        /// всегда измеряет только длину.
        /// </summary>
        private const DeviceType measurementType = DeviceType.LENGTH;

        // ------------------------------------------------------------------
        //  Конструктор (Task 3, шаг 16)
        // ------------------------------------------------------------------

        /// <summary>
        /// Создаёт устройство, работающее в заданной системе единиц.
        /// </summary>
        /// <param name="deviceUnits">Metric или Imperial.</param>
        public MeasureLengthDevice(Units deviceUnits)
        {
            unitsToUse = deviceUnits;
        }

        // ------------------------------------------------------------------
        //  Реализация методов интерфейса IMeasuringDevice
        // ------------------------------------------------------------------

        /// <summary>
        /// Возвращает последнее измерение в метрических единицах (мм).
        /// Если устройство и так работает в метрике — возвращаем значение
        /// как есть; если в дюймах — умножаем на 25.4 (1 дюйм = 25.4 мм).
        /// </summary>
        public decimal MetricValue()
        {
            decimal metricMostRecent = 0;

            if (unitsToUse == Units.Metric)
            {
                metricMostRecent = mostRecentMeasure;
            }
            else
            {
                // Суффикс m — литерал типа decimal (без него 25.4 был бы double)
                metricMostRecent = mostRecentMeasure * 25.4m;
            }
            return metricMostRecent;
        }

        /// <summary>
        /// Возвращает последнее измерение в имперских единицах (дюймы).
        /// Если устройство работает в дюймах — возвращаем как есть;
        /// если в миллиметрах — умножаем на 0.03937 (1 мм ≈ 0.03937 дюйма).
        /// </summary>
        public decimal ImperialValue()
        {
            decimal imperialMostRecent = 0;

            if (unitsToUse == Units.Imperial)
            {
                imperialMostRecent = mostRecentMeasure;
            }
            else
            {
                imperialMostRecent = mostRecentMeasure * 0.03937m;
            }
            return imperialMostRecent;
        }

        /// <summary>
        /// Запускает устройство (Task 3, шаги 8–9):
        /// создаём контроллер статическим фабричным методом StartDevice
        /// и запускаем фоновый сбор измерений.
        /// </summary>
        public void StartCollecting()
        {
            // Защита от повторного запуска: если сбор уже идёт —
            // сначала корректно останавливаем старый (дополнение к методичке)
            if (controller != null)
            {
                StopCollecting();
            }

            controller = DeviceController.StartDevice(measurementType);
            GetMeasurements();
        }

        /// <summary>
        /// Останавливает устройство (Task 3, шаги 11–12).
        /// Обнуление controller заодно завершает цикл в фоновом потоке.
        /// </summary>
        public void StopCollecting()
        {
            if (controller != null)
            {
                controller.StopDevice();
                controller = null;
            }
        }

        /// <summary>
        /// Возвращает массив "сырых" данных (Task 3, шаг 13).
        /// До первого вызова StartCollecting возвращает null.
        /// </summary>
        public int[] GetRawData()
        {
            return dataCaptured;
        }

        // ------------------------------------------------------------------
        //  Вспомогательный закрытый метод (Task 3, шаг 10)
        // ------------------------------------------------------------------

        /// <summary>
        /// Сбор измерений в фоновом потоке из пула потоков.
        /// Раз в 1–5 секунд снимается показание и записывается в кольцевой
        /// буфер dataCaptured; индекс x после 9 сбрасывается в 0.
        /// </summary>
        private void GetMeasurements()
        {
            dataCaptured = new int[10];

            // Запоминаем контроллер ЭТОГО сеанса сбора. Если пользователь
            // остановит и снова запустит устройство, старый поток увидит,
            // что контроллер сменился, и завершится, не мешая новому
            // (небольшое улучшение кода из методички).
            DeviceController sessionController = controller;
            int[] buffer = dataCaptured;

            System.Threading.ThreadPool.QueueUserWorkItem((dummy) =>
            {
                int x = 0;
                Random timer = new Random();

                while (controller != null && controller == sessionController)
                {
                    // Имитация ожидания следующего измерения: 1–5 секунд
                    System.Threading.Thread.Sleep(timer.Next(1000, 5000));

                    // Пока спали, устройство могли остановить — проверяем
                    if (controller != sessionController)
                    {
                        break;
                    }

                    buffer[x] = sessionController.TakeMeasurement();
                    mostRecentMeasure = buffer[x];

                    // Кольцевой буфер: после последней ячейки — снова в начало
                    x++;
                    if (x == 10)
                    {
                        x = 0;
                    }
                }
            });
        }
    }
}
