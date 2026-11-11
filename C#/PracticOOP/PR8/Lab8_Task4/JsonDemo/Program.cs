using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;   // JavaScriptEncoder — настройка записи символов
using System.Text.Json;            // JsonSerializer — главный класс для работы с JSON
using System.Text.Unicode;         // UnicodeRanges — наборы символов (латиница, кириллица)

namespace JsonDemo
{
    /// <summary>
    /// Практика 8, задание 4: сериализация и десериализация в JSON.
    ///
    /// СЕРИАЛИЗАЦИЯ   — превращение объекта из памяти в текст (или байты),
    ///                  который можно сохранить в файл или отправить по сети.
    /// ДЕСЕРИАЛИЗАЦИЯ — обратное действие: из текста снова получается объект.
    ///
    /// JSON — самый распространённый текстовый формат для этого. Правила простые:
    ///     { }            — объект: пары "имя": значение через запятую
    ///     [ ]            — массив (список)
    ///     "текст"        — строка, всегда в двойных кавычках
    ///     12, 4.5        — числа (дробная часть через точку)
    ///     true / false   — логические значения
    ///     null           — "ничего"
    ///
    /// В .NET всё нужное уже встроено — пространство имён System.Text.Json,
    /// ничего дополнительно устанавливать не надо.
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            // Объект, с которым будем работать
            Student student = new Student
            {
                Name = "Иванов Иван",
                Age = 19,
                AverageMark = 4.6,
                IsBudget = true,
                BirthDate = new DateTime(2007, 3, 15),
                GroupName = "ПИ-21",
                Password = "qwerty123",
                Marks = new List<int> { 5, 4, 5, 4 },
                Address = new Address { City = "Симферополь", Street = "ул. Ленина, 1" }
            };

            // =================================================================
            Title("1. Сериализация с настройками по умолчанию");
            // =================================================================

            // Весь объект превращается в строку ОДНИМ вызовом.
            // Вложенный адрес и список оценок обрабатываются сами.
            string compactJson = JsonSerializer.Serialize(student);
            Console.WriteLine(compactJson);

            Console.WriteLine();
            Console.WriteLine("Что я заметил:");
            Console.WriteLine(" - всё в одну строку (для программы удобно, человеку читать трудно);");
            Console.WriteLine(" - русские буквы записаны кодами вида \\u0418 — так сериализатор");
            Console.WriteLine("   перестраховывается по умолчанию; данные при этом не портятся;");
            Console.WriteLine(" - свойство GroupName называется \"group\" (атрибут [JsonPropertyName]);");
            Console.WriteLine(" - пароля нет совсем (атрибут [JsonIgnore]).");

            // =================================================================
            Title("2. Сериализация с настройками (JsonSerializerOptions)");
            // =================================================================

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                // Писать с отступами и переносами строк — "красиво"
                WriteIndented = true,

                // Разрешить записывать латиницу и кириллицу как есть, без кодов \uXXXX
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
            };

            // Тот же метод, но вторым параметром передаём настройки
            string prettyJson = JsonSerializer.Serialize(student, options);
            Console.WriteLine(prettyJson);

            // =================================================================
            Title("3. Десериализация: из строки JSON обратно в объект");
            // =================================================================

            // В угловых скобках указываем, объект КАКОГО класса хотим получить.
            // Сериализатор создаёт новый объект Student и заполняет его свойства
            // значениями из текста (сопоставляя их по именам).
            Student copy = JsonSerializer.Deserialize<Student>(prettyJson);

            Console.WriteLine("Имя:            " + copy.Name);
            Console.WriteLine("Возраст:        " + copy.Age);
            Console.WriteLine("Группа:         " + copy.GroupName);
            Console.WriteLine("Дата рождения:  " + copy.BirthDate.ToShortDateString());
            Console.WriteLine("Оценки:         " + string.Join(", ", copy.Marks));
            Console.WriteLine("Город:          " + copy.Address.City);

            // Пароль в JSON не записывался, поэтому в копии его нет
            Console.WriteLine("Пароль:         " + (copy.Password == null ? "null (не сохранялся)" : copy.Password));

            // copy — это НОВЫЙ объект с теми же данными, а не тот же самый
            Console.WriteLine("Это тот же самый объект? " + ReferenceEquals(student, copy));

            // =================================================================
            Title("4. Сохранение в файл и чтение из файла");
            // =================================================================

            string path = Path.Combine(AppContext.BaseDirectory, "student.json");

            // JSON — обычный текст, поэтому записываем его как текстовый файл
            // (класс File — практика 7). Такой файл открывается в Блокноте.
            File.WriteAllText(path, prettyJson, Encoding.UTF8);
            Console.WriteLine("Записан файл: " + path);
            Console.WriteLine("Размер: " + new FileInfo(path).Length + " байт");

            // Читаем текст из файла и превращаем обратно в объект
            string textFromFile = File.ReadAllText(path, Encoding.UTF8);
            Student fromFile = JsonSerializer.Deserialize<Student>(textFromFile);
            Console.WriteLine("Из файла прочитан студент: " + fromFile.Name + ", балл " + fromFile.AverageMark);

            // =================================================================
            Title("5. Список объектов");
            // =================================================================

            List<Student> group = new List<Student>
            {
                new Student { Name = "Петрова Анна", Age = 20, GroupName = "ПИ-21", Marks = new List<int> { 5, 5 } },
                new Student { Name = "Сидоров Олег", Age = 18, GroupName = "ИВТ-22", Marks = new List<int> { 3, 4 } }
            };

            // Список сериализуется точно так же; в JSON это массив [ {...}, {...} ]
            string listJson = JsonSerializer.Serialize(group, options);
            Console.WriteLine(listJson);
            Console.WriteLine("(Незаполненные свойства тоже записались: Address — null, IsBudget — false.)");

            // При чтении указываем тип List<Student>
            List<Student> groupCopy = JsonSerializer.Deserialize<List<Student>>(listJson);
            Console.WriteLine("Прочитано студентов: " + groupCopy.Count + ", второй: " + groupCopy[1].Name);

            // =================================================================
            Title("6. JSON, написанный вручную, и ошибки");
            // =================================================================

            // JSON может прийти откуда угодно (из интернета, от другой программы).
            // @"..." — строка, в которой кавычки пишутся удвоенными "".
            string manual = @"{ ""Name"": ""Гость"", ""Age"": 25, ""Hobby"": ""шахматы"" }";
            Student guest = JsonSerializer.Deserialize<Student>(manual);

            // Свойства Hobby в классе нет — оно молча пропущено.
            // Свойств, которых нет в JSON, сериализатор не трогает: там остались
            // значения по умолчанию (null, 0, false).
            Console.WriteLine("Имя: " + guest.Name + ", возраст: " + guest.Age +
                              ", группа: " + (guest.GroupName == null ? "null" : guest.GroupName));

            // По умолчанию регистр букв в именах важен: "name" и "Name" — разные имена
            string lowerCase = @"{ ""name"": ""Гость"", ""age"": 25 }";
            Student strict = JsonSerializer.Deserialize<Student>(lowerCase);
            Console.WriteLine("JSON с именами с маленькой буквы: Name = " + (strict.Name == null ? "null" : strict.Name) +
                              " (не совпал регистр)");

            // Это исправляется настройкой PropertyNameCaseInsensitive
            JsonSerializerOptions relaxed = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            Student tolerant = JsonSerializer.Deserialize<Student>(lowerCase, relaxed);
            Console.WriteLine("То же с PropertyNameCaseInsensitive:  Name = " + tolerant.Name);

            // Если текст — неправильный JSON, будет исключение JsonException.
            // Чужие данные всегда лучше читать внутри try/catch (практика 3).
            string broken = @"{ ""Name"": ""Гость"", ""Age"": }";
            try
            {
                JsonSerializer.Deserialize<Student>(broken);
            }
            catch (JsonException)
            {
                Console.WriteLine("Испорченный JSON → исключение JsonException (программа не упала)");
            }

            if (!Console.IsInputRedirected)
            {
                Console.WriteLine();
                Console.WriteLine("Нажмите любую клавишу...");
                Console.ReadKey();
            }
        }

        private static void Title(string text)
        {
            Console.WriteLine();
            Console.WriteLine("========== " + text + " ==========");
        }
    }
}
