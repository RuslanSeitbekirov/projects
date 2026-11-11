using System;

namespace MeasuringDevice
{
    /// <summary>
    /// Task 2: новый интерфейс, который РАСШИРЯЕТ старый IMeasuringDevice.
    ///
    /// Почему не дописать событие прямо в IMeasuringDevice?
    /// Потому что старый код, который уже работает с IMeasuringDevice,
    /// тогда пришлось бы переделывать. А так старый интерфейс не тронут,
    /// и новый код просто пользуется новым интерфейсом.
    /// </summary>
    public interface IEventEnabledMeasuringDevice : IMeasuringDevice
    {
        // Событие объявляется словом event.
        // EventHandler — это готовый (стандартный) тип делегата из .NET:
        //     void ИмяМетода(object sender, EventArgs e)
        // То есть подписаться на событие можно любым методом
        // с такими параметрами.

        /// <summary>
        /// Событие "снято новое измерение". Устройство вызывает его само,
        /// когда у него появилось новое значение.
        /// </summary>
        event EventHandler NewMeasurementTaken;

        // ----- Добавлено в задании 2 (Task 4) -----

        /// <summary>
        /// Событие "сердцебиение": устройство вызывает его через равные
        /// промежутки времени, пока оно "живо". Тип события — наш
        /// собственный делегат HeartBeatEventHandler (см. HeartBeatEvent.cs).
        /// </summary>
        event HeartBeatEventHandler HeartBeat;

        /// <summary>
        /// Интервал между "ударами сердца" в миллисекундах.
        /// Только для чтения — задаётся один раз в конструкторе.
        /// </summary>
        int HeartBeatInterval { get; }
    }
}
