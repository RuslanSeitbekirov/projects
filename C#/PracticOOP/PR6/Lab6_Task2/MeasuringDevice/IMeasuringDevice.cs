namespace MeasuringDevice
{
    /// <summary>
    /// Интерфейс измерительного устройства (дан в методичке, Task 1).
    /// По сравнению с практикой 5 сюда добавились свойства и работа
    /// с файлом журнала (лог-файлом).
    /// </summary>
    public interface IMeasuringDevice
    {
        /// <summary>Последнее измерение в метрических единицах.</summary>
        decimal MetricValue();

        /// <summary>Последнее измерение в имперских единицах.</summary>
        decimal ImperialValue();

        /// <summary>Запустить устройство.</summary>
        void StartCollecting();

        /// <summary>Остановить устройство.</summary>
        void StopCollecting();

        /// <summary>"Сырые" данные в родных единицах устройства.</summary>
        int[] GetRawData();

        /// <summary>Имя файла, в который устройство пишет журнал.</summary>
        string GetLoggingFile();

        // Ниже — свойства. В интерфейсе пишется только, что у свойства
        // должно быть: { get; } — только чтение, { get; set; } — чтение и запись.

        /// <summary>Единицы, в которых работает устройство.</summary>
        Units UnitsToUse { get; }

        /// <summary>Массив измерений.</summary>
        int[] DataCaptured { get; }

        /// <summary>Самое последнее измерение.</summary>
        int MostRecentMeasure { get; }

        /// <summary>
        /// Имя файла журнала. Если его поменять, старый файл закрывается
        /// и дальше запись идёт в новый.
        /// </summary>
        string LoggingFileName { get; set; }
    }
}
