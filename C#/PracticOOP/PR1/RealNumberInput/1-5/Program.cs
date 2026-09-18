using System;
using System.Globalization;

namespace TypeConversionDemo
{
    // =====================================================================
    // ПУНКТ 4: Пользовательские типы для демонстрации implicit/explicit
    // =====================================================================
    public struct Temperature
    {
        public double Celsius { get; set; }

        // Пользовательское НЕЯВНОЕ преобразование: double -> Temperature (в градусах Цельсия)
        public static implicit operator Temperature(double celsius)
        {
            return new Temperature { Celsius = celsius };
        }

        // Пользовательское ЯВНОЕ преобразование: Temperature -> double (в градусах Фаренгейта)
        public static explicit operator double(Temperature temp)
        {
            return temp.Celsius * 9.0 / 5.0 + 32.0;
        }

        public override string ToString() => $"{Celsius} °C";
    }

    // Базовый и производный классы для пунктов 1, 2, 3
    public class Animal { public string Name = "Animal"; }
    public class Dog : Animal { public Dog() { Name = "Dog"; } public void Bark() => Console.WriteLine("Гав!"); }
    public class Cat : Animal { public Cat() { Name = "Cat"; } }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- 1. НЕЯВНЫЕ ПРЕОБРАЗОВАНИЯ ---");
            
            // 1.1 Неявное преобразование простых (значимых) типов
            // Выполняется автоматически, так как тип "приемник" больше по размеру/диапазону. Потерь данных нет.
            byte b = 100;
            int i = b;       // byte -> int
            double d = i;    // int -> double
            Console.WriteLine($"byte({b}) -> int({i}) -> double({d})");

            // 1.2 Неявное преобразование ссылочных типов
            // Преобразование от производного класса к базовому (восходящее преобразование).
            Animal myDog = new Dog(); // Dog -> Animal
            Console.WriteLine($"Ссылочное неявное: {myDog.Name}");


            Console.WriteLine("\n--- 2. ЯВНЫЕ ПРЕОБРАЗОВАНИЯ ---");

            // 2.1 Явное преобразование простых (значимых) типов
            // Требует указания типа в скобках. Возможно усечение данных или потеря точности.
            double pi = 3.14159;
            int intPi = (int)pi;     // double -> int (дробная часть отбрасывается)
            int largeNum = 300;
            byte smallNum = (byte)largeNum; // int -> byte (произойдет переполнение, останется 44)
            Console.WriteLine($"double({pi}) -> int({intPi})");
            Console.WriteLine($"int({largeNum}) -> byte({smallNum}) [с переполнением]");

            // 2.2 Явное преобразование ссылочных типов
            // Преобразование от базового класса к производному (нисходящее преобразование).
            // Требует явного приведения типов и работает только если объект действительно является производным.
            Dog castedDog = (Dog)myDog; // Animal -> Dog
            castedDog.Bark();


            Console.WriteLine("\n--- 3. БЕЗОПАСНОЕ ПРИВЕДЕНИЕ (as и is) ---");
            Animal myCat = new Cat();

            // Оператор is: проверяет совместимость типов, возвращает bool
            if (myDog is Dog)
            {
                Console.WriteLine("myDog является экземпляром Dog (is)");
            }

            // Оператор as: пытается привести тип. Если не получается, возвращает null (вместо исключения InvalidCastException)
            Dog dogFromCat = myCat as Dog; 
            if (dogFromCat == null)
            {
                Console.WriteLine("myCat не является Dog, оператор as вернул null (безопасное приведение)");
            }


            Console.WriteLine("\n--- 4. ПОЛЬЗОВАТЕЛЬСКОЕ ПРЕОБРАЗОВАНИЕ (implicit / explicit) ---");
            
            // Неявное преобразование (вызывается автоматически)
            Temperature tempC = 25.5; 
            Console.WriteLine($"Неявное (double -> Temperature): {tempC}");

            // Явное преобразование (требует указания типа)
            double tempF = (double)tempC; 
            Console.WriteLine($"Явное (Temperature -> double в Фаренгейтах): {tempF} °F");


            Console.WriteLine("\n--- 5. Convert, Parse, TryParse ---");
            string strNum = "12345";
            string strDouble = "3.14";
            string invalidStr = "abc";

            // Класс Convert (универсален, поддерживает много типов, включая bool, DateTime)
            int convInt = Convert.ToInt32(strNum);
            bool convBool = Convert.ToBoolean(1); // 1 -> true
            Console.WriteLine($"Convert.ToInt32(\"{strNum}\") = {convInt}");

            // Метод Parse (строгий, вызывает исключение при ошибке)
            int parsedInt = int.Parse(strNum);
            // Для Parse важно учитывать культуру (разделитель дробной части)
            double parsedDouble = double.Parse(strDouble, CultureInfo.InvariantCulture); 
            Console.WriteLine($"int.Parse(\"{strNum}\") = {parsedInt}");
            Console.WriteLine($"double.Parse(\"{strDouble}\") = {parsedDouble}");

            // Метод TryParse (безопасный, не вызывает исключений, возвращает bool)
            if (int.TryParse(invalidStr, out int result))
            {
                Console.WriteLine($"Успешно: {result}");
            }
            else
            {
                Console.WriteLine($"TryParse не смог преобразовать \"{invalidStr}\" в число.");
            }

            Console.ReadLine();
        }
    }
}

/* =========================================================================
 ТАБЛИЦЫ ПРЕОБРАЗОВАНИЙ (Справочная информация в виде комментариев)
 =========================================================================

 1) ТАБЛИЦА НЕЯВНЫХ ПРЕОБРАЗОВАНИЙ (Implicit)
    Выполняются компилятором автоматически. Всегда безопасны (нет потери данных).
    
    Значимые типы (Value Types):
    - sbyte -> short, int, long, float, double, decimal
    - byte  -> short, ushort, int, uint, long, ulong, float, double, decimal
    - short -> int, long, float, double, decimal
    - ushort -> int, uint, long, ulong, float, double, decimal
    - int   -> long, float, double, decimal
    - uint  -> long, ulong, float, double, decimal
    - long  -> float, double, decimal
    - ulong -> float, double, decimal
    - float -> double
    - char  -> ushort, int, uint, long, ulong, float, double, decimal

    Ссылочные типы (Reference Types):
    - Любой производный класс -> его базовый класс (и интерфейсы, которые он реализует).
    - Любой класс -> интерфейс, который он реализует.
    - Любой тип -> object.
    - Массив с большим количеством измерений -> массив с меньшим (только для ссылочных типов).

 --------------------------------------------------------------------------

 2) ТАБЛИЦА ЯВНЫХ ПРЕОБРАЗОВАНИЙ (Explicit)
    Требуют указания типа в скобках `(type)`. Могут приводить к потере данных или исключениям.
    
    Значимые типы (Value Types):
    - Обратные преобразования из таблицы неявных (например, double -> int, int -> byte).
    - int <-> uint, long <-> ulong (возможна потеря знака/переполнение).
    - float <-> double (потеря точности).
    - Числовые типы <-> char, enum.

    Ссылочные типы (Reference Types):
    - Базовый класс -> производный класс (может вызвать InvalidCastException, если объект не является производным).
    - Интерфейс -> класс (реализующий этот интерфейс).
    - object -> любой тип (распаковка для значимых типов или приведение для ссылочных).
*/