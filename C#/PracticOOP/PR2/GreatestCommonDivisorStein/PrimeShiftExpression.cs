using System.Text;

namespace GreatestCommonDivisorStein;

public static class PrimeShiftExpression
{
    // Единица, записанная только через цифру 2: (2+2)>>2 = 4>>2 = 1
    private const string One = "((2+2)>>2)"; // готовая запись числа 1

    // ----- Поиск наибольшего простого числа, меньшего n -----
    public static int? FindLargestPrimeBelow(int n) 
    {
        for (int candidate = n - 1; candidate >= 2; candidate--) // Как только IsPrime вернет true, мы немедленно делаем return candidate
        {
            if (IsPrime(candidate))   return candidate;
        }
        return null;    // простых чисел меньше n нет (n <= 2)
    }

    public static bool IsPrime(int n)
    {
        if (n < 2) return false; // int? в FindLargest чтобы вернуть null, если простых чисел меньше n не существует (например, при n <= 2).
        if (n % 2 == 0) return n == 2; // Быстрая отсечка всех четных чисел, кроме 2.

        for (long i = 3; i * i <= n; i += 2) // Перебираем только нечетные делители до                         
        {                                    // квадратного корня из n. Использование long для i предотвращает
            if (n % i == 0) return false;    // переполнение при вычислении i * i для больших чисел.
        }                                  
        return true;
    }

    // ----- Построение выражения из цифры 2, сдвигов, скобок, + и - -----
    public static string Build(int n)
    {
        if (n < 1) // автоматически подставляет имя переменной "n" в виде строки, что защищает от ошибок при переименовании переменной
            throw new ArgumentOutOfRangeException(nameof(n), "Число должно быть положительным");

        return Expr(n).Text;
    }

    // NAF-разложение n -> строка вида  term ± term ± term ...
    private static (string Text, int Terms) Expr(int n)
    {
        // digits[i] ∈ {-1, 0, 1} — коэффициент при 2^i
        var digits = new List<int>();
        long m = n;

        while (m > 0)
        {
            int d = 0;
            if ((m & 1) != 0)// Побитовая проверка на нечетность
            {
                d = 2 - (int)(m & 3);   // m mod 4 == 1 -> +1;  m mod 4 == 3 -> -1
                m -= d; //Корректируем число и сдвигаем его вправо на 1 бит (эквивалент целочисленного деления на 2).
            }
            digits.Add(d);
            m >>= 1;//Корректируем число и сдвигаем его вправо на 1 бит (эквивалент целочисленного деления на 2).
        }

        // Старший ненулевой коэффициент NAF всегда положителен,
        // поэтому запись начинается без знака.
        var sb = new StringBuilder();
        int terms = 0;

        for (int i = digits.Count - 1; i >= 0; i--)
        {
            if (digits[i] == 0) continue;

            if (terms > 0)
                sb.Append(digits[i] > 0 ? "+" : "-");

            sb.Append(PowerOfTwo(i));
            terms++;
        }

        return (sb.ToString(), terms);
    }

    // 2^k, выраженное через цифру 2 и сдвиги
    private static string PowerOfTwo(int k)
    {
        if (k == 0) return One;                     // 1
        if (k == 1) return "2";                     // 2
        return "(2<<" + Wrap(k - 1) + ")";          // 2 << (k-1) = 2^k
    }

    // Показатель сдвига: если в нём несколько слагаемых — берём в скобки
    private static string Wrap(int n)
    {
       // Вызывает Expr(n) для показателя степени. Если показатель состоит
       // из нескольких слагаемых (e.Terms > 1, например, 2+1)
       // он оборачивает его в скобки: (2+1).
        var e = Expr(n);
        return e.Terms > 1 ? "(" + e.Text + ")" : e.Text;
    }

    // ----- Проверка результата -----

    /// <summary>В выражении нет ни одной цифры, кроме 2.</summary>
    public static bool ContainsOnlyDigitTwo(string expression)
    {//Строгая проверка, что в сгенерированной строке нет запрещенных цифр (0, 1, 3-9).
        foreach (char c in expression)
        {// Если символ является цифрой, но это не '2', метод немедленно возвращает false
            if (char.IsDigit(c) && c != '2')// для +/()<> isDigit вернёт false
                return false; 
        }
        return true;
    }

    /// <summary>
    /// Вычисляет выражение с приоритетами C#:
    /// сложение/вычитание выше сдвигов, скобки выше всего.
    /// </summary>
    public static long Evaluate(string expression)
    {
        string s = expression.Replace(" ", "");
        int pos = 0;
        // внутренняя функция
        long ParseShift() 
        // Сначала вызывает ParseAdditive() для получения левого операнда.
        // Затем в цикле while проверяет наличие двух символов << или >> (s[pos] == '<' && s[pos + 1] == '<').
        // Если сдвиг найден, сдвигает pos на 2, вычисляет величину сдвига (вызывая ParseAdditive(), так как правая
        // часть сдвига может быть выражением, например 2 << (1+1)), и выполняет реальную побитовую операцию value << amount или value >> amount.
        {
            long value = ParseAdditive();
            while (pos + 1 < s.Length &&
                   ((s[pos] == '<' && s[pos + 1] == '<') || (s[pos] == '>' && s[pos + 1] == '>')))
            {
                bool left = s[pos] == '<';
                pos += 2;
                int amount = (int)ParseAdditive();
                value = left ? value << amount : value >> amount;
            }
            return value;
        }
        // внутренняя функция
        long ParseAdditive()
        // Сначала вызывает ParsePrimary() для получения левого операнда.
        // Затем в цикле while проверяет, есть ли следующие символы '+' или '-'. Если есть, запоминает оператор, сдвигает pos,
        // получает правый операнд (снова через ParsePrimary) и выполняет операцию.
        // Примечание: В C# приоритет сложения выше, чем у побитовых сдвигов. Поэтому ParseAdditive вызывает именно ParsePrimary, 
        // а не ParseShift.
        {
            long value = ParsePrimary();
            while (pos < s.Length && (s[pos] == '+' || s[pos] == '-'))
            {
                char op = s[pos++];
                long right = ParsePrimary();
                value = op == '+' ? value + right : value - right;
            }
            return value;
        }
        // внутренняя функция
        long ParsePrimary() // Проверяет, является ли текущий символ '2'. Если да, сдвигает pos и возвращает 2.
//      Проверяет, является ли текущий символ '('. Если да, сдвигает pos, рекурсивно вызывает ParseShift() 
//      для вычисления содержимого скобок, проверяет наличие закрывающей ')' и возвращает результат.
//      Если встретился любой другой символ, выбрасывает FormatException (защита от некорректного ввода).
        {
            if (pos < s.Length && s[pos] == '2')
            {
                pos++;
                return 2;
            }

            if (pos < s.Length && s[pos] == '(')
            {
                pos++;
                long value = ParseShift();
                if (pos >= s.Length || s[pos] != ')')
                    throw new FormatException("Ожидалась закрывающая скобка");
                pos++;
                return value;
            }

            throw new FormatException("Неожиданный символ в позиции " + pos);
        }
        // Запускает весь процесс парсинга с самого высокого уровня приоритета.
        long result = ParseShift();
        if (pos != s.Length)
            throw new FormatException("Лишние символы в конце выражения");
        return result;
    }
}