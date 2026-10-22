using System.Globalization;

namespace ComplexNumbers;

/// <summary>Комплексное число z = Re + Im·i (прямоугольная форма).</summary>
public readonly struct Complex
{
    private const double Eps = 1e-9;   // допуск для сравнения вещественных чисел

    public double Re { get; }          // действительная часть
    public double Im { get; }          // мнимая часть

    public Complex(double re, double im)
    {
        Re = re;
        Im = im;
    }

    // ---- Модуль и аргумент (свойства только для чтения) ----
    public double Modulus => Math.Sqrt(Re * Re + Im * Im);   // |z| = √(Re² + Im²)
    public double Argument => Math.Atan2(Im, Re);            // φ = arg z, в радианах

    // ---- Арифметические операторы ----
    public static Complex operator +(Complex a, Complex b)
        => new(a.Re + b.Re, a.Im + b.Im);

    public static Complex operator -(Complex a, Complex b)
        => new(a.Re - b.Re, a.Im - b.Im);

    // (a+bi)(c+di) = (ac − bd) + (ad + bc)i
    public static Complex operator *(Complex a, Complex b)
        => new(a.Re * b.Re - a.Im * b.Im,
               a.Re * b.Im + a.Im * b.Re);

    // (a+bi)/(c+di) = ((ac + bd) + (bc − ad)i) / (c² + d²)
    public static Complex operator /(Complex a, Complex b)
    {
        double denominator = b.Re * b.Re + b.Im * b.Im;
        if (denominator == 0)
            throw new DivideByZeroException("Деление на комплексный ноль");

        return new((a.Re * b.Re + a.Im * b.Im) / denominator,
                   (a.Im * b.Re - a.Re * b.Im) / denominator);
    }

    // ---- Сравнение. Перегрузка == требует перегрузить и !=, Equals, GetHashCode ----
    public static bool operator ==(Complex a, Complex b)
        => a.Re == b.Re && a.Im == b.Im;

    public static bool operator !=(Complex a, Complex b) => !(a == b);

    public override bool Equals(object? obj) => obj is Complex c && this == c;

    public override int GetHashCode() => HashCode.Combine(Re, Im);

    /// <summary>Сравнение с допуском: нужно после вычислений с тригонометрией,
    /// где получаются числа вроде 7,000000000000001.</summary>
    public bool ApproxEquals(Complex other, double eps = Eps)
        => Math.Abs(Re - other.Re) < eps && Math.Abs(Im - other.Im) < eps;

    // ---- Вывод ----
    public override string ToString()
    {
        string re = Re.ToString("0.####", CultureInfo.CurrentCulture);
        string im = Math.Abs(Im).ToString("0.####", CultureInfo.CurrentCulture);
        return Im < 0 ? $"{re} - {im}i" : $"{re} + {im}i";
    }

    // ---- Прямоугольная <-> экспоненциальная форма ----

    /// <summary>Прямоугольная -> экспоненциальная: z = r·e^(iφ). Возвращает (r, φ).</summary>
    public (double R, double Phi) ToExponential() => (Modulus, Argument);

    /// <summary>Экспоненциальная -> прямоугольная: Re = r·cos φ, Im = r·sin φ.</summary>
    public static Complex FromExponential(double r, double phi)
        => new(r * Math.Cos(phi), r * Math.Sin(phi));

    /// <summary>Текстовая запись экспоненциальной формы.</summary>
    public string ToExponentialString()
    {
        var (r, phi) = ToExponential();
        return string.Format(CultureInfo.CurrentCulture, "{0:0.####}·e^(i·{1:0.####})", r, phi);
    }
}