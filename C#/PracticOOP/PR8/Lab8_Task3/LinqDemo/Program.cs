using System;
using System.Collections.Generic;
using System.Linq;   // БЕЗ этой строки методы Where, Select и др. у коллекций не появятся
using System.Text;

namespace LinqDemo
{
    /// <summary>
    /// Практика 8, задание 3: LINQ (Language Integrated Query —
    /// "запросы, встроенные в язык").
    ///
    /// LINQ позволяет выбирать, сортировать и группировать данные из коллекций
    /// короткими запросами вместо циклов с if. Запрос говорит, ЧТО нужно
    /// получить, а не КАК перебирать элементы.
    ///
    /// Один и тот же запрос можно записать двумя способами:
    ///
    ///   1) Операторы запросов — похоже на язык SQL:
    ///          from s in students where s.Age > 18 select s
    ///
    ///   2) Методы расширения — цепочка вызовов с лямбда-выражениями:
    ///          students.Where(s => s.Age > 18)
    ///
    /// Компилятор всё равно превращает первый способ во второй,
    /// так что это просто две формы записи.
    /// </summary>
    internal class Program
    {
        // Исходные данные — обычные списки
        private static readonly List<Student> students = new List<Student>
        {
            new Student("Иванов",   "ПИ-21", 19, 4.6),
            new Student("Петрова",  "ПИ-21", 20, 3.9),
            new Student("Сидоров",  "ИВТ-22", 18, 4.9),
            new Student("Кузнецова", "ИВТ-22", 21, 3.2),
            new Student("Алиев",    "КБ-23", 19, 4.4),
            new Student("Морозова", "ПИ-21", 22, 5.0),
            new Student("Волков",   "КБ-23", 18, 2.8)
        };

        private static readonly List<Curator> curators = new List<Curator>
        {
            new Curator("ПИ-21", "Смирнов А.В."),
            new Curator("ИВТ-22", "Орлова Е.Н."),
            new Curator("КБ-23", "Белов Д.С.")
        };

        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("Все студенты:");
            PrintAll(students);

            QuerySyntax();
            MethodSyntax();
            DeferredExecution();
            OwnExtensions();

            if (!Console.IsInputRedirected)
            {
                Console.WriteLine();
                Console.WriteLine("Нажмите любую клавишу...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Печатает любую последовательность. Параметр типа IEnumerable&lt;T&gt;
        /// принимает и список, и массив, и результат запроса LINQ.
        /// </summary>
        private static void PrintAll<T>(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Console.WriteLine("   " + item);
            }
        }

        private static void Title(string text)
        {
            Console.WriteLine();
            Console.WriteLine("========== " + text + " ==========");
        }

        // =====================================================================
        //  1. Операторы запросов (синтаксис, похожий на SQL)
        // =====================================================================

        private static void QuerySyntax()
        {
            Title("1. Операторы запросов");

            // ---- where + orderby + select ----
            // Читается почти как по-русски:
            //   from s in students   — "для каждого s из students"
            //   where ...            — "у которого..."        (фильтр)
            //   orderby ...          — "упорядочить по..."    (сортировка)
            //   select s             — "взять самого s"       (что вернуть)
            // В отличие от SQL запрос начинается с from, а не с select.
            IEnumerable<Student> goodStudents =
                from s in students
                where s.Mark >= 4.5
                orderby s.Name
                select s;

            Console.WriteLine("Балл не ниже 4.5, по алфавиту:");
            PrintAll(goodStudents);

            // ---- select может вернуть не весь объект, а его часть ----
            IEnumerable<string> names =
                from s in students
                where s.Group == "ПИ-21"
                select s.Name;

            Console.WriteLine("Фамилии группы ПИ-21: " + string.Join(", ", names));

            // ---- Сортировка по убыванию и по двум полям ----
            // descending — по убыванию; через запятую — второй ключ сортировки
            // (он применяется, когда первый совпал).
            var byAge =
                from s in students
                orderby s.Age descending, s.Name
                select s;

            Console.WriteLine("По возрасту (от старших), при равном возрасте — по фамилии:");
            PrintAll(byAge);

            // ---- select new { ... } — анонимный тип ----
            // Можно "на лету" собрать новый объект только из нужных полей.
            // У такого типа нет имени, поэтому переменную приходится объявлять
            // через var — компилятор сам знает её настоящий тип.
            var shortInfo =
                from s in students
                where s.Age < 20
                select new { s.Name, Category = s.Mark >= 4 ? "хорошист" : "отстающий" };

            Console.WriteLine("Младше 20 лет (анонимный тип из двух полей):");
            foreach (var item in shortInfo)
            {
                Console.WriteLine("   " + item.Name + " — " + item.Category);
            }

            // ---- group ... by — группировка ----
            // Результат — набор групп. У каждой группы есть Key (то, по чему
            // группировали), а сама группа — это последовательность студентов.
            var groups =
                from s in students
                group s by s.Group;

            Console.WriteLine("Группировка по учебной группе:");
            foreach (var g in groups)
            {
                Console.WriteLine("   " + g.Key + " (" + g.Count() + " чел.):");
                foreach (Student s in g)
                {
                    Console.WriteLine("      " + s.Name);
                }
            }

            // ---- join — соединение двух коллекций ----
            // Для каждого студента находим куратора, у которого совпадает группа.
            // Важно: в join пишется слово equals, а не ==.
            var withCurators =
                from s in students
                join c in curators on s.Group equals c.Group
                where s.Mark < 3.5
                select new { s.Name, c.Teacher };

            Console.WriteLine("Отстающие (балл ниже 3.5) и их кураторы:");
            foreach (var item in withCurators)
            {
                Console.WriteLine("   " + item.Name + " → куратор " + item.Teacher);
            }
        }

        // =====================================================================
        //  2. Методы расширения LINQ
        // =====================================================================

        private static void MethodSyntax()
        {
            Title("2. Методы расширения");

            // В скобках — лямбда-выражение: s => s.Mark >= 4.5 читается
            // "студент s подходит, если его балл не ниже 4.5".
            // Каждый метод возвращает новую последовательность, поэтому
            // вызовы можно соединять в цепочку через точку.

            // ---- Where (фильтр), OrderBy (сортировка), Select (преобразование) ----
            IEnumerable<string> result = students
                .Where(s => s.Mark >= 4.5)
                .OrderBy(s => s.Name)
                .Select(s => s.Name + " — " + s.Mark);

            Console.WriteLine("Where + OrderBy + Select (тот же запрос, что первый в разделе 1):");
            PrintAll(result);

            // ---- Сортировка по убыванию и по второму ключу ----
            Console.WriteLine("OrderByDescending(балл).ThenBy(фамилия), первые три — Take(3):");
            PrintAll(students.OrderByDescending(s => s.Mark).ThenBy(s => s.Name).Take(3));

            // ---- Skip — пропустить, Take — взять (так делают "страницы") ----
            Console.WriteLine("Skip(2).Take(2) — третий и четвёртый по списку: " +
                              string.Join(", ", students.Skip(2).Take(2).Select(s => s.Name)));

            // ---- Методы, которые возвращают ОДНО значение ----
            Console.WriteLine("Count() — всего студентов:              " + students.Count());
            Console.WriteLine("Count(условие) — старше 19:             " + students.Count(s => s.Age > 19));
            Console.WriteLine("Average — средний балл:                 " + students.Average(s => s.Mark).ToString("0.00"));
            Console.WriteLine("Max / Min — лучший и худший балл:       " + students.Max(s => s.Mark) + " / " + students.Min(s => s.Mark));
            Console.WriteLine("Sum — сумма возрастов:                  " + students.Sum(s => s.Age));

            // ---- Проверки: возвращают true или false ----
            Console.WriteLine("Any — есть ли хоть один отличник (5.0): " + students.Any(s => s.Mark == 5.0));
            Console.WriteLine("All — все ли совершеннолетние:          " + students.All(s => s.Age >= 18));

            // ---- Поиск одного элемента ----
            // First бросает исключение, если ничего не найдено.
            // FirstOrDefault в этом случае возвращает null — он безопаснее.
            Student first = students.First(s => s.Group == "КБ-23");
            Console.WriteLine("First — первый из КБ-23:                " + first.Name);

            Student nobody = students.FirstOrDefault(s => s.Age > 100);
            Console.WriteLine("FirstOrDefault — старше 100 лет:        " + (nobody == null ? "никого (null)" : nobody.Name));

            // ---- Distinct — убрать повторы ----
            Console.WriteLine("Select(группа).Distinct():              " +
                              string.Join(", ", students.Select(s => s.Group).Distinct()));

            // ---- GroupBy + подсчёты внутри каждой группы ----
            Console.WriteLine("GroupBy — средний балл по группам:");
            var stats = students
                .GroupBy(s => s.Group)
                .Select(g => new { Group = g.Key, Count = g.Count(), Average = g.Average(s => s.Mark) })
                .OrderByDescending(x => x.Average);

            foreach (var x in stats)
            {
                Console.WriteLine("   " + x.Group + ": " + x.Count + " чел., средний балл " + x.Average.ToString("0.00"));
            }

            // ---- ToList / ToArray / ToDictionary — превратить результат в коллекцию ----
            List<Student> adults = students.Where(s => s.Age >= 20).ToList();
            Dictionary<string, double> marks = students.ToDictionary(s => s.Name, s => s.Mark);
            Console.WriteLine("ToList: в списке " + adults.Count + " чел.; ToDictionary: балл Алиева = " + marks["Алиев"]);

            // LINQ работает не только со списками объектов — с любым массивом
            int[] numbers = { 5, 12, 7, 20, 3, 18 };
            Console.WriteLine("Чётные числа из массива по возрастанию: " +
                              string.Join(", ", numbers.Where(n => n % 2 == 0).OrderBy(n => n)));
        }

        // =====================================================================
        //  3. Отложенное выполнение
        // =====================================================================

        /// <summary>
        /// Особенность LINQ, о которой надо знать: запрос выполняется не там,
        /// где он НАПИСАН, а там, где его результат начинают ПЕРЕБИРАТЬ.
        /// </summary>
        private static void DeferredExecution()
        {
            Title("3. Отложенное выполнение");

            List<int> numbers = new List<int> { 1, 2, 3 };

            // Здесь запрос только ОПИСАН. Ни одно число ещё не проверено.
            IEnumerable<int> big = numbers.Where(n => n > 1);

            // ToList выполняет запрос СРАЗУ и запоминает результат
            List<int> snapshot = numbers.Where(n => n > 1).ToList();

            // Меняем исходный список уже ПОСЛЕ написания запросов
            numbers.Add(10);

            // Запрос big выполняется только сейчас, при переборе (внутри Join),
            // поэтому он "видит" добавленное число 10.
            Console.WriteLine("Запрос без ToList (выполнился сейчас): " + string.Join(", ", big));
            Console.WriteLine("Запрос с ToList (выполнился заранее):  " + string.Join(", ", snapshot));
            Console.WriteLine("Вывод: если результат нужен \"как на тот момент\" — вызывай ToList() или ToArray().");
        }

        // =====================================================================
        //  4. Свои методы расширения
        // =====================================================================

        private static void OwnExtensions()
        {
            Title("4. Свои методы расширения (файл MyExtensions.cs)");

            // OlderThan написан мной, но вызывается так же, как Where или
            // OrderBy, и свободно соединяется с ними в одну цепочку.
            Console.WriteLine("students.OlderThan(19).OrderBy(...):");
            PrintAll(students.OlderThan(19).OrderBy(s => s.Age));

            // Метод Initial() я добавил ко всем строкам
            Console.WriteLine("Фамилия → инициал: " +
                              string.Join(" ", students.Select(s => s.Name.Initial())));
        }
    }
}
