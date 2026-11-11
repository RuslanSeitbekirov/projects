using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;   // атрибуты [JsonPropertyName], [JsonIgnore]

namespace JsonDemo
{
    // =========================================================================
    //  Классы, объекты которых будем превращать в JSON и обратно.
    //
    //  Что я выяснил про требования к таким классам:
    //    - в JSON попадают только PUBLIC СВОЙСТВА (с get). Обычные поля
    //      и private-свойства по умолчанию пропускаются;
    //    - чтобы объект можно было восстановить из JSON, у свойств должен
    //      быть set, а у класса — конструктор без параметров (если никакого
    //      конструктора не писать, он есть автоматически).
    // =========================================================================

    /// <summary>Адрес — вложенный объект внутри студента.</summary>
    public class Address
    {
        public string City { get; set; }
        public string Street { get; set; }
    }

    /// <summary>Студент.</summary>
    public class Student
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public double AverageMark { get; set; }
        public bool IsBudget { get; set; }
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Атрибут в квадратных скобках — "пометка" для сериализатора.
        /// [JsonPropertyName] задаёт, как свойство будет называться в JSON.
        /// В C# оно GroupName, а в файле будет "group".
        /// </summary>
        [JsonPropertyName("group")]
        public string GroupName { get; set; }

        /// <summary>
        /// [JsonIgnore] — "это свойство в JSON не записывать".
        /// Пароль в файл сохранять нельзя.
        /// </summary>
        [JsonIgnore]
        public string Password { get; set; }

        /// <summary>Список — в JSON станет массивом [ ... ].</summary>
        public List<int> Marks { get; set; }

        /// <summary>Вложенный объект — в JSON станет вложенными { ... }.</summary>
        public Address Address { get; set; }
    }
}
