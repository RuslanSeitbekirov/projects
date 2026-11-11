using System;

namespace MeasuringDevice
{
    /// <summary>
    /// Task 2: свой класс "аргументов события" для события HeartBeat.
    ///
    /// Стандартный EventHandler передаёт подписчику только EventArgs,
    /// в котором нет никаких данных. А нам вместе с событием хочется
    /// передать время, когда оно произошло. Для этого делают свой класс,
    /// наследник EventArgs, и добавляют в него нужные свойства.
    /// По традиции имя такого класса заканчивается на EventArgs.
    /// </summary>
    public class HeartBeatEventArgs : EventArgs
    {
        /// <summary>
        /// Время, когда произошло событие.
        /// Автоматическое свойство: компилятор сам создаёт скрытое поле.
        /// private set — менять значение можно только внутри этого класса,
        /// то есть для всех остальных свойство только для чтения.
        /// </summary>
        public DateTime TimeStamp { get; private set; }

        /// <summary>
        /// Конструктор запоминает текущие дату и время.
        /// ": base()" — сначала вызвать конструктор базового класса EventArgs.
        /// </summary>
        public HeartBeatEventArgs()
            : base()
        {
            TimeStamp = DateTime.Now;
        }
    }

    /// <summary>
    /// Task 3: СВОЙ тип делегата.
    ///
    /// Объявление делегата похоже на объявление метода без тела, только
    /// со словом delegate. Оно описывает, какими должны быть методы-подписчики:
    /// ничего не возвращают (void) и принимают object и HeartBeatEventArgs.
    /// </summary>
    /// <param name="sender">Кто вызвал событие.</param>
    /// <param name="args">Данные события (время).</param>
    public delegate void HeartBeatEventHandler(object sender, HeartBeatEventArgs args);
}
