using DeviceControl;

namespace MeasuringDevice
{
    /// <summary>
    /// Устройство для измерения массы (кг / фунты).
    /// Событие NewMeasurementTaken и весь сбор данных достаются ему
    /// по наследству от MeasureDataDevice — здесь только пересчёт единиц.
    /// </summary>
    public class MeasureMassDevice : MeasureDataDevice
    {
        /// <summary>Создаёт устройство.</summary>
        /// <param name="deviceUnits">Metric (кг) или Imperial (фунты).</param>
        /// <param name="logFileName">Имя файла журнала.</param>
        public MeasureMassDevice(Units deviceUnits, string logFileName)
        {
            unitsToUse = deviceUnits;
            measurementType = DeviceType.MASS;
            loggingFileName = logFileName;
        }

        /// <summary>Последнее измерение в килограммах (1 фунт ≈ 0.4536 кг).</summary>
        public override decimal MetricValue()
        {
            if (unitsToUse == Units.Metric)
            {
                return mostRecentMeasure;
            }
            // m на конце — число типа decimal
            return mostRecentMeasure * 0.4536m;
        }

        /// <summary>Последнее измерение в фунтах (1 кг ≈ 2.2046 фунта).</summary>
        public override decimal ImperialValue()
        {
            if (unitsToUse == Units.Imperial)
            {
                return mostRecentMeasure;
            }
            return mostRecentMeasure * 2.2046m;
        }
    }
}
