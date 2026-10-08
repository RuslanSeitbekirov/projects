using System.Diagnostics;

namespace GreatestCommonDivisorStein;

public static class GCDAlgorithms
{
    /// <summary>
    /// НОД двух чисел по алгоритму Евклида (вычитанием).
    /// time — время выполнения в тиках Stopwatch.
    /// </summary>
    public static int FindGCDEuclid(int a, int b, out long time)
    {
        time = 0;
        Stopwatch sw = Stopwatch.StartNew();

        int result;
        if (a == 0)
        {
            result = b;
        }
        else
        { // Классический алгоритм. Пока второе число не равно нулю, из большего вычитается меньшее.
            while (b != 0)
            {
                if (a > b)
                    a -= b;
                else
                    b -= a;
            }
            result = a;
        }

        sw.Stop();
        time = sw.ElapsedTicks;
        return result;
    }

    /// <summary>
    /// НОД двух чисел по алгоритму Штейна (побитовые сдвиги).
    /// time — время выполнения в тиках Stopwatch.
    /// </summary>
    public static int FindGCDStein(int u, int v, out long time)
    {
        time = 0;
        Stopwatch sw = Stopwatch.StartNew();
        int k;

        // Шаг 1: gcd(0, v) = v; gcd(u, 0) = u; gcd(0, 0) = 0
        if (u == 0 || v == 0)
        {
            sw.Stop();
            time = sw.ElapsedTicks;     // время фиксируется перед первым return
            return u | v;
        }

        // Шаг 2: если u и v чётные, gcd(u, v) = 2 * gcd(u/2, v/2)
        for (k = 0; ((u | v) & 1) == 0; ++k)
        {
            u >>= 1; //Побитовый сдвиг вправо на 1. Математически эквивалентен целочисленному делению на 2
            v >>= 1;
        }

        // Шаг 3: если u чётное, а v нечётное, gcd(u, v) = gcd(u/2, v)
        while ((u & 1) == 0)
            u >>= 1;

        // Шаги 4-5: оба нечётные -> gcd(u, v) = gcd((u - v)/2, v)
        do
        {
            while ((v & 1) == 0)
                v >>= 1;

            if (u < v)
            {
                v -= u;
            }
            else
            {
                int diff = u - v;
                u = v;
                v = diff;
            }
            v >>= 1;
        } while (v != 0);

        u <<= k;

        sw.Stop();
        time = sw.ElapsedTicks;         // время фиксируется перед вторым return
        return u;
    }   
}