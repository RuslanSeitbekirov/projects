namespace MeasuringDevice
{
    // Task 2: Create the Units enumeration.
    // Модификатор public — перечисление должно быть доступно из других сборок
    // (консольного и WPF тестовых приложений).

    /// <summary>
    /// Система единиц, в которой устройство выдаёт "сырые" данные.
    /// </summary>
    public enum Units
    {
        /// <summary>
        /// Метрическая система (длина — миллиметры, масса — килограммы).
        /// </summary>
        Metric,

        /// <summary>
        /// Имперская (английская) система (длина — дюймы, масса — фунты).
        /// </summary>
        Imperial
    }
}
