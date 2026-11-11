using DeviceControl;

namespace MeasuringDevice
{
    /// <summary>
    /// Устройство для измерения длины (мм / дюймы).
    /// В окне этой работы не используется, но оставлено из практики 5:
    /// событие у него тоже есть, потому что оно описано в базовом классе.
    /// </summary>
    public class MeasureLengthDevice : MeasureDataDevice
    {
        /// <summary>Создаёт устройство.</summary>
        /// <param name="deviceUnits">Metric (мм) или Imperial (дюймы).</param>
        /// <param name="logFileName">Имя файла журнала.</param>
        public MeasureLengthDevice(Units deviceUnits, string logFileName)
        {
            unitsToUse = deviceUnits;
            measurementType = DeviceType.LENGTH;
            loggingFileName = logFileName;
            heartBeatIntervalTime = 1000;   // сердцебиение раз в секунду
        }

        /// <summary>Последнее измерение в миллиметрах (1 дюйм = 25.4 мм).</summary>
        public override decimal MetricValue()
        {
            if (unitsToUse == Units.Metric)
            {
                return mostRecentMeasure;
            }
            return mostRecentMeasure * 25.4m;
        }

        /// <summary>Последнее измерение в дюймах (1 мм ≈ 0.03937 дюйма).</summary>
        public override decimal ImperialValue()
        {
            if (unitsToUse == Units.Imperial)
            {
                return mostRecentMeasure;
            }
            return mostRecentMeasure * 0.03937m;
        }
    }
}
