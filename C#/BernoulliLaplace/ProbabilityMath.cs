namespace BernoulliLaplace;

public static class ProbabilityMath
{
    // ln(n!). Для малых n — сумма логарифмов, для больших — ряд Стирлинга.
    // Логарифмы нужны, чтобы n! и C(n,k) не переполняли double.
    public static double LogFactorial(int n)
    {
        if (n < 2) return 0.0;

        if (n <= 170)
        {
            double s = 0;
            for (int i = 2; i <= n; i++) s += Math.Log(i);
            return s;
        }

        double x = n;
        return x * Math.Log(x) - x + 0.5 * Math.Log(2 * Math.PI * x)
               + 1 / (12 * x) - 1 / (360 * x * x * x) + 1 / (1260 * Math.Pow(x, 5));
    }

    // Формула Бернулли: P(k) = C(n,k) p^k q^(n-k)
    public static double Bernoulli(int n, int k, double p)
    {
        if (k < 0 || k > n) return 0.0;
        double q = 1 - p;
        double ln = LogFactorial(n) - LogFactorial(k) - LogFactorial(n - k)
                    + k * Math.Log(p) + (n - k) * Math.Log(q);
        return Math.Exp(ln);
    }

    // Сумма P(k) для k от k1 до k2 (точное значение для сравнения)
    public static double BernoulliRange(int n, int k1, int k2, double p)
    {
        double sum = 0;
        for (int k = k1; k <= k2; k++)
            sum += Bernoulli(n, k, p);
        return Math.Min(sum, 1.0);
    }

    // Нормированное отклонение x = (k - np) / sqrt(npq)
    public static double X(int n, int k, double p)
    {
        return (k - n * p) / Math.Sqrt(n * p * (1 - p));
    }

    // Функция Гаусса φ(x) = e^(-x²/2) / sqrt(2π)
    public static double Phi(double x)
    {
        return Math.Exp(-x * x / 2) / Math.Sqrt(2 * Math.PI);
    }

    // Локальная формула Лапласа: P_n(k) ≈ φ(x) / sqrt(npq)
    public static double LocalLaplace(int n, int k, double p)
    {
        double x = X(n, k, p);
        return Phi(x) / Math.Sqrt(n * p * (1 - p));
    }

    // Функция Лапласа Φ₀(x) = (1/sqrt(2π)) ∫₀ˣ e^(-t²/2) dt = erf(x/√2) / 2
    public static double Laplace(double x)
    {
        return 0.5 * Erf(x / Math.Sqrt(2));
    }

    // Интегральная формула Лапласа: P(k1 ≤ m ≤ k2) ≈ Φ₀(x2) - Φ₀(x1)
    public static double IntegralLaplace(int n, int k1, int k2, double p)
    {
        return Laplace(X(n, k2, p)) - Laplace(X(n, k1, p));
    }

    // В .NET нет Math.Erf, поэтому используем ряд:
    // erf(x) = 2/sqrt(π) · e^(-x²) · Σ 2^n x^(2n+1) / (1·3·5···(2n+1))
    public static double Erf(double x)
    {
        if (x == 0) return 0;

        double ax = Math.Abs(x);
        if (ax > 6) return Math.Sign(x);   // erf(6) = 1 с точностью double

        double term = ax, sum = ax;
        for (int n = 1; n < 500; n++)
        {
            term *= 2 * ax * ax / (2 * n + 1);
            sum += term;
            if (term < 1e-17 * sum) break;
        }

        double r = 2 / Math.Sqrt(Math.PI) * Math.Exp(-ax * ax) * sum;
        return x < 0 ? -r : r;
    }
}