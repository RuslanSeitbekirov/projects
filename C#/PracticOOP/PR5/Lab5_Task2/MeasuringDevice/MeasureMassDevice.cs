using DeviceControl;

namespace MeasuringDevice
{
    /// <summary>
    /// Устройство для измерения массы (кг / фунты).
    /// Наследует общий код от MeasureDataDevice и реализует только
    /// специфичный для массы пересчёт единиц.
    /// </summary>
    public class MeasureMassDevice : MeasureDataDevice, IMeasuringDevice
    {
        /// <param name="deviceUnits">Metric (кг) или Imperial (фунты).</param>
        public MeasureMassDevice(Units deviceUnits)
        {
            unitsToUse = deviceUnits;
            measurementType = DeviceType.MASS;
        }

        /// <summary>Последнее измерение в килограммах (1 фунт ≈ 0.4536 кг).</summary>
        public override decimal MetricValue()
        {
            decimal metricMostRecent = 0;

            if (unitsToUse == Units.Metric)
            {
                metricMostRecent = mostRecentMeasure;
            }
            else
            {
                metricMostRecent = mostRecentMeasure * 0.4536m;
            }
            return metricMostRecent;
        }

        /// <summary>Последнее измерение в фунтах (1 кг ≈ 2.2046 фунта).</summary>
        public override decimal ImperialValue()
        {
            decimal imperialMostRecent = 0;

            if (unitsToUse == Units.Imperial)
            {
                imperialMostRecent = mostRecentMeasure;
            }
            else
            {
                imperialMostRecent = mostRecentMeasure * 2.2046m;
            }
            return imperialMostRecent;
        }
    }
}