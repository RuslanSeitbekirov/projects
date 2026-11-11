using System.Collections.Generic;

namespace LinqDemo
{
    // =========================================================================
    //  МЕТОДЫ РАСШИРЕНИЯ.
    //
    //  Меня удивило: у List<T> в самом классе нет методов Where, Select,
    //  OrderBy... Откуда же они берутся после точки?
    //
    //  Это "методы расширения". Они написаны в ДРУГОМ классе
    //  (System.Linq.Enumerable), но вызываются так, будто принадлежат
    //  самой коллекции. Так можно добавить методы к чужому классу, не меняя его.
    //
    //  Как написать свой метод расширения — три правила:
    //    1) класс должен быть static;
    //    2) метод должен быть static;
    //    3) перед первым параметром стоит слово this — оно показывает,
    //       какой тип мы "расширяем".
    //
    //  Все методы LINQ расширяют IEnumerable<T>, поэтому работают с массивами,
    //  списками, словарями — со всем, что можно перебрать через foreach.
    // =========================================================================

    public static class MyExtensions
    {
        /// <summary>
        /// Мой собственный метод в стиле LINQ: оставляет студентов старше
        /// указанного возраста.
        ///
        /// "this IEnumerable&lt;Student&gt; source" — метод появится у любой
        /// последовательности студентов. При вызове students.OlderThan(19)
        /// сама коллекция students попадает в параметр source, а 19 — в age.
        /// </summary>
        public static IEnumerable<Student> OlderThan(this IEnumerable<Student> source, int age)
        {
            foreach (Student student in source)
            {
                if (student.Age > age)
                {
                    // yield return (см. задание 1): отдаём элементы по одному.
                    // Так устроены и настоящие методы LINQ.
                    yield return student;
                }
            }
        }

        /// <summary>
        /// Расширять можно любые типы, даже встроенные. Этот метод
        /// появится у всех строк: "Иванов Иван".Initial() вернёт "И.".
        /// </summary>
        public static string Initial(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            return text.Substring(0, 1) + ".";
        }
    }
}
