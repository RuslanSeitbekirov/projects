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
        /// <summary>
        /// Task 9, шаги 2–3: основной конструктор. В задании 2 у него
        /// появился третий параметр — интервал сердцебиения.
        /// </summary>
        /// <param name="deviceUnits">Metric (кг) или Imperial (фунты).</param>
        /// <param name="logFileName">Имя файла журнала.</param>
        /// <param name="heartBeatInterval">Интервал сердцебиения, мс.</param>
        public MeasureMassDevice(Units deviceUnits, string logFileName, int heartBeatInterval)
        {
            unitsToUse = deviceUnits;
            measurementType = DeviceType.MASS;
            loggingFileName = logFileName;

            // Сохраняем интервал в protected-поле базового класса
            heartBeatIntervalTime = heartBeatInterval;
        }

        /// <summary>
        /// Task 9, шаги 4–5: "цепочка конструкторов".
        /// Запись ": this(...)" означает: сначала вызови ДРУГОЙ конструктор
        /// этого же класса с такими параметрами. Интервал здесь не спрашиваем,
        /// а подставляем значение по умолчанию — 1000 мс (1 секунда).
        /// Так не приходится повторять код первого конструктора.
        /// </summary>
        /// <param name="deviceUnits">Metric (кг) или Imperial (фунты).</param>
        /// <param name="logFileName">Имя файла журнала.</param>
        public MeasureMassDevice(Units deviceUnits, string logFileName)
            : this(deviceUnits, logFileName, 1000)
        {
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
