using System;
using DeviceControl;

namespace MeasuringDevice
{
    /// <summary>
    /// Абстрактный базовый класс измерительного устройства.
    /// Содержит общий для всех устройств код (запуск, остановка, сбор данных),
    /// а методы пересчёта единиц объявлены abstract — их обязан
    /// реализовать каждый класс-наследник.
    /// Создать объект абстрактного класса через new нельзя.
    /// </summary>
    public abstract class MeasureDataDevice : IMeasuringDevice
    {
        // Поля сделаны protected (Task 5, шаг 12): они видны наследникам,
        // но скрыты от внешнего кода.

        /// <summary>Единицы, в которых устройство выдаёт данные.</summary>
        protected Units unitsToUse;

        /// <summary>Кольцевой буфер на 10 измерений.</summary>
        protected int[] dataCaptured;

        /// <summary>Самое последнее измерение.</summary>
        protected int mostRecentMeasure;

        /// <summary>Контроллер эмулятора устройства.</summary>
        protected DeviceController controller;

        /// <summary>
        /// Тип измерения. Уже НЕ const и не инициализирован при объявлении
        /// (Task 5, шаг 13): значение задаёт конструктор наследника.
        /// </summary>
        protected DeviceType measurementType;

        /// <summary>Перевод в метрические единицы. Реализуется в наследнике.</summary>
        public abstract decimal MetricValue();

        /// <summary>Перевод в имперские единицы. Реализуется в наследнике.</summary>
        public abstract decimal ImperialValue();

        /// <summary>Запуск устройства и фонового сбора данных.</summary>
        public void StartCollecting()
        {
            if (controller != null)
            {
                StopCollecting();
            }

            controller = DeviceController.StartDevice(measurementType);
            GetMeasurements();
        }

        /// <summary>Остановка устройства.</summary>
        public void StopCollecting()
        {
            if (controller != null)
            {
                controller.StopDevice();
                controller = null;
            }
        }

        /// <summary>Сырые данные в "родных" единицах устройства.</summary>
        public int[] GetRawData()
        {
            return dataCaptured;
        }

        /// <summary>
        /// Сбор измерений в фоновом потоке (кольцевой буфер на 10 значений).
        /// </summary>
        private void GetMeasurements()
        {
            dataCaptured = new int[10];

            DeviceController sessionController = controller;
            int[] buffer = dataCaptured;

            System.Threading.ThreadPool.QueueUserWorkItem((dummy) =>
            {
                int x = 0;
                Random timer = new Random();

                while (controller != null && controller == sessionController)
                {
                    System.Threading.Thread.Sleep(timer.Next(1000, 5000));

                    if (controller != sessionController)
                    {
                        break;
                    }

                    buffer[x] = sessionController.TakeMeasurement();
                    mostRecentMeasure = buffer[x];

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