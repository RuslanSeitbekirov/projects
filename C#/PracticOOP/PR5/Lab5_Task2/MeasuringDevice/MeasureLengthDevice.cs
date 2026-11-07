using DeviceControl;

namespace MeasuringDevice
{
    /// <summary>
    /// Устройство для измерения длины (мм / дюймы).
    /// Наследует общий код от MeasureDataDevice и реализует только
    /// специфичный для длины пересчёт единиц.
    /// </summary>
    public class MeasureLengthDevice : MeasureDataDevice, IMeasuringDevice
    {
        /// <param name="deviceUnits">Metric (мм) или Imperial (дюймы).</param>
        public MeasureLengthDevice(Units deviceUnits)
        {
            unitsToUse = deviceUnits;
            measurementType = DeviceType.LENGTH;
        }

        /// <summary>
        /// Последнее измерение в миллиметрах (1 дюйм = 25.4 мм).
        /// override — переопределяем абстрактный метод базового класса.
        /// </summary>
        public override decimal MetricValue()
        {
            decimal metricMostRecent = 0;

            if (unitsToUse == Units.Metric)
            {
                metricMostRecent = mostRecentMeasure;
            }
            else
            {
                metricMostRecent = mostRecentMeasure * 25.4m;
            }
            return metricMostRecent;
        }

        /// <summary>Последнее измерение в дюймах (1 мм ≈ 0.03937 дюйма).</summary>
        public override decimal ImperialValue()
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
    }
}