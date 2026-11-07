namespace MeasuringDevice
{
    /// <summary>
    /// Интерфейс измерительного устройства (дан в стартовом проекте, Task 1).
    /// Интерфейс задаёт только КОНТРАКТ — набор методов, которые обязан иметь
    /// любой класс-устройство. Реализации (тел методов) в нём нет.
    /// Благодаря этому тестовые приложения работают с переменной типа
    /// IMeasuringDevice и не зависят от конкретного класса устройства.
    /// </summary>
    public interface IMeasuringDevice
    {
        /// <summary>
        /// Converts the raw data collected by the measuring device into a metric value.
        /// Переводит последнее измерение в метрические единицы.
        /// </summary>
        /// <returns>The latest measurement from the device converted to metric units.</returns>
        decimal MetricValue();

        /// <summary>
        /// Converts the raw data collected by the measuring device into an imperial value.
        /// Переводит последнее измерение в имперские единицы.
        /// </summary>
        /// <returns>The latest measurement from the device converted to imperial units.</returns>
        decimal ImperialValue();

        /// <summary>
        /// Starts the measuring device. Запускает устройство.
        /// </summary>
        void StartCollecting();

        /// <summary>
        /// Stops the measuring device. Останавливает устройство.
        /// </summary>
        void StopCollecting();

        /// <summary>
        /// Enables access to the raw data from the device in whatever units are native to the device.
        /// Возвращает "сырые" данные в родных единицах устройства.
        /// </summary>
        /// <returns>The raw data from the device in native format.</returns>
        int[] GetRawData();
    }
}
