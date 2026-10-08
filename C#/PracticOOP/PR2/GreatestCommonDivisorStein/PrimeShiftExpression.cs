using System.Text;

namespace GreatestCommonDivisorStein;

public static class PrimeShiftExpression
{
    // Единица, записанная только через цифру 2: (2+2)>>2 = 4>>2 = 1
    private const string One = "((2+2)>>2)";

    // ----- Поиск наибольшего простого числа, меньшего n -----
    public static int? FindLargestPrimeBelow(int n)
    {
        for (int candidate = n - 1; candidate >= 2; candidate--)
        {
            if (IsPrime(candidate))
                return candidate;
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
        if (n < 1)
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
            if ((m & 1) != 0)
            {
                d = 2 - (int)(m & 3);   // m mod 4 == 1 -> +1;  m mod 4 == 3 -> -1
                m -= d;
            }
            digits.Add(d);
            m >>= 1;
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
        var e = Expr(n);
        return e.Terms > 1 ? "(" + e.Text + ")" : e.Text;
    }

    // ----- Проверка результата -----

    /// <summary>В выражении нет ни одной цифры, кроме 2.</summary>
    public static bool ContainsOnlyDigitTwo(string expression)
    {
        foreach (char c in expression)
        {
            if (char.IsDigit(c) && c != '2')
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

        long ParseShift()
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

        long ParseAdditive()
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
 
        long ParsePrimary()
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

        long result = ParseShift();
        if (pos != s.Length)
            throw new FormatException("Лишние символы в конце выражения");
        return result;
    }
}