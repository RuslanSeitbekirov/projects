using System;

// =====================================================================
//  Проект DeviceController — ЭМУЛЯТОР измерительного устройства.
//  В методичке он выдаётся готовым ("The DeviceController project is
//  provided complete"). Реального прибора нет, поэтому здесь устройство
//  имитируется генератором случайных чисел.
//  Этот проект НЕ является частью задания — его не нужно изменять.
// =====================================================================

namespace DeviceControl
{
    /// <summary>
    /// Тип измерения, которое выполняет устройство.
    /// </summary>
    public enum DeviceType
    {
        /// <summary>Измерение длины.</summary>
        LENGTH,
        /// <summary>Измерение массы.</summary>
        MASS
    }

    /// <summary>
    /// Интерфейс "железного" устройства, которым управляет контроллер.
    /// </summary>
    public interface IControllableDevice
    {
        /// <summary>Включить устройство.</summary>
        void StartDevice();
        /// <summary>Выключить устройство.</summary>
        void StopDevice();
        /// <summary>Получить последнее измеренное значение.</summary>
        int GetLatestMeasure();
    }

    /// <summary>
    /// Контроллер устройства: фабричный метод StartDevice создаёт и запускает
    /// нужное устройство, TakeMeasurement снимает с него показание.
    /// </summary>
    public class DeviceController : IDisposable
    {
        private IControllableDevice device;

        /// <summary>
        /// Фабричный метод: создаёт и запускает устройство заданного типа.
        /// </summary>
        /// <param name="MeasurementType">LENGTH или MASS.</param>
        /// <returns>Контроллер с запущенным устройством.</returns>
        public static DeviceController StartDevice(DeviceType MeasurementType)
        {
            DeviceController controller = new DeviceController();
            switch (MeasurementType)
            {
                case DeviceType.LENGTH:
                    controller.device = new FabrikamDevices.LengthMeasuringDevice();
                    break;
                case DeviceType.MASS:
                    controller.device = new ContosoDevices.MassMeasuringDevice();
                    break;
            }

            if (controller.device != null)
            {
                controller.device.StartDevice();
            }
            return controller;
        }

        /// <summary>Остановить устройство.</summary>
        public void StopDevice()
        {
            device.StopDevice();
        }

        /// <summary>Заставить устройство выполнить измерение.</summary>
        /// <returns>Результат измерения (в "родных" единицах устройства).</returns>
        public int TakeMeasurement()
        {
            return device.GetLatestMeasure();
        }

        /// <summary>Освобождение ресурсов (в эмуляторе ничего не делает).</summary>
        public void Dispose()
        {
        }
    }
}

namespace FabrikamDevices
{
    /// <summary>
    /// Эмулятор устройства для измерения длины: выдаёт случайные целые числа.
    /// </summary>
    public class LengthMeasuringDevice : DeviceControl.IControllableDevice
    {
        private readonly Random rnd = new Random();
        private bool running;
        private int lastValue;

        public void StartDevice() { running = true; }

        public void StopDevice() { running = false; }

        public int GetLatestMeasure()
        {
            // Пока устройство работает — каждое обращение даёт новое значение
            if (running)
            {
                lastValue = rnd.Next(1, 1000);
            }
            return lastValue;
        }
    }
}

namespace ContosoDevices
{
    /// <summary>
    /// Эмулятор устройства для измерения массы (понадобится в Задании 2).
    /// </summary>
    public class MassMeasuringDevice : DeviceControl.IControllableDevice
    {
        private readonly Random rnd = new Random();
        private bool running;
        private int lastValue;

        public void StartDevice() { running = true; }

        public void StopDevice() { running = false; }

        public int GetLatestMeasure()
        {
            if (running)
            {
                lastValue = rnd.Next(1, 200);
            }
            return lastValue;
        }
    }
}
