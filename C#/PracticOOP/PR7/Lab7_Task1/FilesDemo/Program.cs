using System;
using System.Globalization;   // CultureInfo — чтобы числа читались одинаково на любом компьютере
using System.IO;              // все классы для работы с файлами лежат здесь
using System.Text;            // Encoding — кодировки текста

namespace FilesDemo
{
    /// <summary>
    /// Практика 7, задание 1: классы File, FileInfo, FileStream,
    /// StreamReader/StreamWriter, BinaryWriter/BinaryReader.
    ///
    /// Как я для себя разделил эти классы:
    ///   File, FileInfo           — работа с файлом ЦЕЛИКОМ (создать, скопировать, удалить, узнать размер);
    ///   FileStream               — работа с СОДЕРЖИМЫМ файла как с набором байтов;
    ///   StreamWriter/Reader      — надстройка над потоком для ТЕКСТА (строки);
    ///   BinaryWriter/Reader      — надстройка над потоком для ЧИСЕЛ и других типов в двоичном виде.
    /// </summary>
    internal class Program
    {
        // Папка, в которой программа создаёт все свои файлы
        private static string folder;

        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            // Path.Combine правильно склеивает части пути (сам ставит "\").
            // AppContext.BaseDirectory — папка, где лежит exe-файл программы.
            folder = Path.Combine(AppContext.BaseDirectory, "data");

            // Создаём папку. Если она уже есть — ошибки не будет.
            Directory.CreateDirectory(folder);
            Console.WriteLine("Все файлы создаются в папке:");
            Console.WriteLine(folder);

            FileClassDemo();
            FileInfoDemo();
            FileStreamDemo();
            StreamWriterReaderDemo();
            BinaryWriterReaderDemo();

            // Ждём клавишу, только если программа запущена в обычной консоли
            if (!Console.IsInputRedirected)
            {
                Console.WriteLine();
                Console.WriteLine("Нажмите любую клавишу...");
                Console.ReadKey();
            }
        }

        /// <summary>Печатает заголовок раздела.</summary>
        private static void Title(string text)
        {
            Console.WriteLine();
            Console.WriteLine("========== " + text + " ==========");
        }

        // =====================================================================
        //  1. Класс File
        // =====================================================================

        /// <summary>
        /// File — СТАТИЧЕСКИЙ класс: объект создавать не нужно, методы
        /// вызываются через имя класса, а путь к файлу передаётся в каждый метод.
        /// Удобен, когда с файлом надо сделать одно-два действия.
        /// </summary>
        private static void FileClassDemo()
        {
            Title("1. Класс File");

            string path = Path.Combine(folder, "notes.txt");

            // WriteAllText создаёт файл (или ПЕРЕЗАПИСЫВАЕТ старый) и пишет в него текст.
            // Файл сам открывается и закрывается — ни о чём думать не надо.
            File.WriteAllText(path, "Первая строка" + Environment.NewLine);

            // AppendAllText ДОПИСЫВАЕТ текст в конец, старое содержимое остаётся
            File.AppendAllText(path, "Вторая строка" + Environment.NewLine);

            // AppendAllLines дописывает сразу несколько строк из массива
            File.AppendAllLines(path, new string[] { "Третья строка", "Четвёртая строка" });

            // Exists проверяет, есть ли такой файл
            Console.WriteLine("Файл существует: " + File.Exists(path));

            // ReadAllText читает весь файл в одну строку
            string allText = File.ReadAllText(path);
            Console.WriteLine("ReadAllText вернул:");
            Console.Write(allText);

            // ReadAllLines читает файл в массив: один элемент = одна строка файла
            string[] lines = File.ReadAllLines(path);
            Console.WriteLine("ReadAllLines вернул строк: " + lines.Length + ", последняя: " + lines[lines.Length - 1]);

            // Copy — копирование. true в конце разрешает заменить файл, если он уже есть
            // (без true при повторном запуске программы было бы исключение).
            string copyPath = Path.Combine(folder, "notes_copy.txt");
            File.Copy(path, copyPath, true);
            Console.WriteLine("Копия создана: " + File.Exists(copyPath));

            // Move — перемещение (заодно так переименовывают файлы)
            string movedPath = Path.Combine(folder, "notes_renamed.txt");
            File.Move(copyPath, movedPath, true);
            Console.WriteLine("После Move: старое имя есть = " + File.Exists(copyPath) +
                              ", новое имя есть = " + File.Exists(movedPath));

            // Delete — удаление
            File.Delete(movedPath);
            Console.WriteLine("После Delete файл существует: " + File.Exists(movedPath));

            // Особенность: ReadAllText/ReadAllLines читают файл в память целиком,
            // поэтому для очень больших файлов они не подходят — там нужны потоки.
        }

        // =====================================================================
        //  2. Класс FileInfo
        // =====================================================================

        /// <summary>
        /// FileInfo умеет почти то же, что File, но это ОБЫЧНЫЙ класс:
        /// сначала создаётся объект для конкретного файла, а потом у него
        /// вызываются методы и читаются свойства. Удобен, когда с одним
        /// файлом делается много действий или нужны сведения о нём.
        /// </summary>
        private static void FileInfoDemo()
        {
            Title("2. Класс FileInfo");

            string path = Path.Combine(folder, "notes.txt");   // файл из прошлого опыта

            // Создание объекта FileInfo НЕ создаёт файл на диске —
            // это только "карточка" с информацией о файле по этому пути.
            FileInfo info = new FileInfo(path);

            Console.WriteLine("Существует:      " + info.Exists);
            Console.WriteLine("Имя:             " + info.Name);
            Console.WriteLine("Расширение:      " + info.Extension);
            Console.WriteLine("Размер, байт:    " + info.Length);
            Console.WriteLine("Папка:           " + info.DirectoryName);
            Console.WriteLine("Создан:          " + info.CreationTime);
            Console.WriteLine("Изменён:         " + info.LastWriteTime);
            Console.WriteLine("Только чтение:   " + info.IsReadOnly);

            // CopyTo копирует файл и возвращает FileInfo уже для копии
            FileInfo copy = info.CopyTo(Path.Combine(folder, "notes_info_copy.txt"), true);
            Console.WriteLine("Копия: " + copy.Name + ", размер " + copy.Length + " байт");

            // Delete удаляет файл
            copy.Delete();

            // Особенность, на которой я споткнулся: FileInfo запоминает сведения
            // о файле и сам их не обновляет. Чтобы увидеть изменения на диске,
            // нужно вызвать Refresh().
            copy.Refresh();
            Console.WriteLine("После Delete копия существует: " + copy.Exists);
        }

        // =====================================================================
        //  3. Класс FileStream
        // =====================================================================

        /// <summary>
        /// FileStream — "поток байтов" файла. Работает на самом низком уровне:
        /// он не знает ни про строки, ни про числа — только про байты (0..255).
        /// Зато можно читать файл по кусочкам и прыгать в любое место файла.
        /// </summary>
        private static void FileStreamDemo()
        {
            Title("3. Класс FileStream");

            string path = Path.Combine(folder, "bytes.dat");

            // FileMode говорит, ЧТО делать с файлом:
            //   Create — создать новый (старый стереть), Open — открыть существующий,
            //   Append — открыть и встать в конец, OpenOrCreate — открыть или создать.
            // FileAccess говорит, ЗАЧЕМ открываем: Read, Write или ReadWrite.
            //
            // using — очень важная вещь. Открытый файл "занят" программой, и его
            // обязательно надо закрыть. using сам вызывает закрытие (Dispose)
            // в конце блока, даже если внутри произошла ошибка.
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                byte[] data = { 10, 20, 30, 40, 50 };

                // Write(массив, с какого элемента начать, сколько байтов записать)
                stream.Write(data, 0, data.Length);

                // Можно записать и один байт
                stream.WriteByte(60);

                // Position — текущее место в файле (как курсор), Length — размер
                Console.WriteLine("Записано байтов: " + stream.Length + ", позиция: " + stream.Position);
            }   // <-- здесь файл закрывается

            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                // Создаём массив нужного размера и читаем в него весь файл.
                // Read возвращает, сколько байтов реально прочитано.
                byte[] buffer = new byte[stream.Length];
                int count = stream.Read(buffer, 0, buffer.Length);
                Console.WriteLine("Прочитано " + count + " байт: " + string.Join(", ", buffer));

                // Seek — "перемотка". Встаём на 2 байта от начала (счёт с нуля).
                stream.Seek(2, SeekOrigin.Begin);
                Console.WriteLine("Байт с номером 2: " + stream.ReadByte());

                // Можно считать и от конца: -1 от конца — это последний байт
                stream.Seek(-1, SeekOrigin.End);
                Console.WriteLine("Последний байт:   " + stream.ReadByte());

                // Когда файл кончился, ReadByte возвращает -1
                Console.WriteLine("Чтение после конца файла: " + stream.ReadByte());
            }

            // Текст через FileStream тоже можно записать, но придётся самому
            // превращать строку в байты и обратно — это неудобно.
            string textPath = Path.Combine(folder, "hello.txt");
            using (FileStream stream = new FileStream(textPath, FileMode.Create))
            {
                byte[] textBytes = Encoding.UTF8.GetBytes("Привет, файл!");
                stream.Write(textBytes, 0, textBytes.Length);
            }
            using (FileStream stream = new FileStream(textPath, FileMode.Open))
            {
                byte[] textBytes = new byte[stream.Length];
                // Обычный Read может прочитать МЕНЬШЕ байтов, чем просили
                // (поэтому он и возвращает количество). ReadExactly читает
                // ровно столько, сколько указано, — компилятор сам подсказал
                // заменить Read на него.
                stream.ReadExactly(textBytes, 0, textBytes.Length);
                Console.WriteLine("Текст из байтов: " + Encoding.UTF8.GetString(textBytes) +
                                  " (13 символов заняли " + textBytes.Length + " байт: русская буква в UTF-8 — 2 байта)");
            }
        }

        // =====================================================================
        //  4. StreamWriter и StreamReader
        // =====================================================================

        /// <summary>
        /// StreamWriter и StreamReader — классы для ТЕКСТОВЫХ файлов.
        /// Они сами переводят строки в байты и обратно (с учётом кодировки)
        /// и умеют работать построчно. Файл получается обычным текстовым,
        /// его можно открыть в Блокноте.
        /// </summary>
        private static void StreamWriterReaderDemo()
        {
            Title("4. StreamWriter и StreamReader");

            string path = Path.Combine(folder, "products.csv");

            // Второй параметр: false — перезаписать файл, true — дописывать в конец.
            // Третий параметр — кодировка. UTF-8 нужна, чтобы русские буквы
            // не превратились в "кракозябры".
            using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                // WriteLine пишет строку и переводит на новую, как Console.WriteLine
                writer.WriteLine("Название;Цена");
                writer.WriteLine("Книга;450.50");
                writer.WriteLine("Ручка;35.00");

                // Write пишет без перевода строки
                writer.Write("Тетрадь;");
                writer.WriteLine("80.25");
            }

            // Открываем тот же файл на ДОЗАПИСЬ (true)
            using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8))
            {
                writer.WriteLine("Рюкзак;2100.00");
            }

            // Чтение по одной строке. Так можно прочитать даже огромный файл:
            // в памяти в каждый момент находится только одна строка.
            decimal total = 0;
            using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
            {
                // Первая строка — заголовок таблицы, просто пропускаем её
                string header = reader.ReadLine();
                Console.WriteLine("Заголовок: " + header);

                string line;
                // ReadLine возвращает null, когда строки кончились.
                // Запись (line = ...) != null значит: "прочитай строку в line
                // и сразу проверь, что это не конец файла".
                while ((line = reader.ReadLine()) != null)
                {
                    // Делим строку "Книга;450.50" на части по точке с запятой
                    string[] parts = line.Split(';');
                    string name = parts[0];

                    // InvariantCulture: в файле дробная часть отделена точкой.
                    // Без этого на русской Windows Parse ждал бы запятую и выдал ошибку.
                    decimal price = decimal.Parse(parts[1], CultureInfo.InvariantCulture);

                    Console.WriteLine("  " + name + " стоит " + price + " руб.");
                    total += price;
                }
            }
            Console.WriteLine("Сумма всех цен: " + total + " руб.");

            // ReadToEnd читает сразу всё, что осталось в файле
            using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
            {
                string everything = reader.ReadToEnd();
                Console.WriteLine("ReadToEnd прочитал символов: " + everything.Length);
            }
        }

        // =====================================================================
        //  5. BinaryWriter и BinaryReader
        // =====================================================================

        /// <summary>
        /// BinaryWriter и BinaryReader пишут и читают данные в ДВОИЧНОМ виде —
        /// так, как они хранятся в памяти: int всегда занимает 4 байта,
        /// decimal — 16, bool — 1. Такой файл нельзя прочитать в Блокноте,
        /// зато числа не надо превращать в текст и обратно.
        /// </summary>
        private static void BinaryWriterReaderDemo()
        {
            Title("5. BinaryWriter и BinaryReader");

            string path = Path.Combine(folder, "products.bin");

            // Данные о товарах (номер в массивах = один товар)
            int[] ids = { 1, 2, 3 };
            string[] names = { "Книга", "Ручка", "Рюкзак" };
            decimal[] prices = { 450.50m, 35.00m, 2100.00m };
            bool[] inStock = { true, false, true };

            // BinaryWriter сам файл не открывает — ему нужен готовый поток.
            // File.Open возвращает FileStream, а BinaryWriter "надевается" на него сверху.
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                // Сначала пишем количество записей, чтобы при чтении знать, сколько их
                writer.Write(ids.Length);

                for (int i = 0; i < ids.Length; i++)
                {
                    // Метод один — Write, но у него много вариантов (перегрузок):
                    // компилятор выбирает нужный по типу параметра.
                    writer.Write(ids[i]);       // int     — 4 байта
                    writer.Write(names[i]);     // string  — сначала длина, потом сами символы
                    writer.Write(prices[i]);    // decimal — 16 байт
                    writer.Write(inStock[i]);   // bool    — 1 байт
                }
            }

            Console.WriteLine("Размер двоичного файла: " + new FileInfo(path).Length + " байт");

            // Посмотрим на "сырые" байты начала файла: это не текст
            byte[] raw = File.ReadAllBytes(path);
            Console.WriteLine("Первые 16 байт: " + BitConverter.ToString(raw, 0, 16));

            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                // САМОЕ ВАЖНОЕ: читать надо строго в том же порядке и теми же
                // типами, как писали. В файле нет подсказок, где что лежит.
                // Если перепутать порядок — получится мусор или исключение.
                int count = reader.ReadInt32();
                Console.WriteLine("Записей в файле: " + count);

                for (int i = 0; i < count; i++)
                {
                    // У чтения, в отличие от Write, для каждого типа свой метод
                    int id = reader.ReadInt32();
                    string name = reader.ReadString();
                    decimal price = reader.ReadDecimal();
                    bool available = reader.ReadBoolean();

                    Console.WriteLine("  №" + id + " " + name + ", " + price + " руб., " +
                                      (available ? "в наличии" : "нет в наличии"));
                }
            }
        }
    }
}
