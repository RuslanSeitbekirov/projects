using System.Globalization;
using System.Text;
using System.Windows;

namespace BernoulliLaplace;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void calculateButton_Click(object sender, RoutedEventArgs e)
    {
        // ---- Чтение и проверка данных ----
        if (!int.TryParse(nTextBox.Text, out int n) || n < 1)
        {
            Error("n должно быть целым числом не меньше 1");
            return;
        }

        if (!TryParseDouble(pTextBox.Text, out double p) || p <= 0 || p >= 1)
        {
            Error("p должно быть числом строго между 0 и 1");
            return;
        }

        if (!int.TryParse(kTextBox.Text, out int k) || k < 0 || k > n)
        {
            Error("k должно быть целым числом от 0 до n");
            return;
        }

        if (!int.TryParse(k1TextBox.Text, out int k1) ||
            !int.TryParse(k2TextBox.Text, out int k2) ||
            k1 < 0 || k2 > n || k1 > k2)
        {
            Error("Должно быть 0 ≤ k1 ≤ k2 ≤ n (целые числа)");
            return;
        }

        // ---- Вычисления ----
        double q = 1 - p;
        double mean = n * p;
        double sigma = Math.Sqrt(n * p * q);

        double exactPoint = ProbabilityMath.Bernoulli(n, k, p);
        double xPoint = ProbabilityMath.X(n, k, p);
        double localLaplace = ProbabilityMath.LocalLaplace(n, k, p);

        double exactRange = ProbabilityMath.BernoulliRange(n, k1, k2, p);
        double x1 = ProbabilityMath.X(n, k1, p);
        double x2 = ProbabilityMath.X(n, k2, p);
        double integralLaplace = ProbabilityMath.IntegralLaplace(n, k1, k2, p);

        // ---- Вывод ----
        var sb = new StringBuilder();
        sb.AppendLine($"n = {n}, p = {Fmt(p)}, q = {Fmt(q)}");
        sb.AppendLine($"np = {Fmt(mean)},  √(npq) = {Fmt(sigma)}");

        if (n * p * q < 10)
        {
            sb.AppendLine();
            sb.AppendLine("ВНИМАНИЕ: npq < 10, формулы Лапласа дают плохое приближение.");
        }

        sb.AppendLine();
        sb.AppendLine($"=== Ровно k = {k} успехов ===");
        sb.AppendLine($"Формула Бернулли (точно):        P = {Fmt(exactPoint)}");
        sb.AppendLine($"Локальная формула Лапласа:       x = {Fmt(xPoint)}, φ(x) = {Fmt(ProbabilityMath.Phi(xPoint))}");
        sb.AppendLine($"                                 P ≈ {Fmt(localLaplace)}");
        sb.AppendLine($"Погрешность: {Fmt(Math.Abs(exactPoint - localLaplace))}  ({RelError(exactPoint, localLaplace)})");

        sb.AppendLine();
        sb.AppendLine($"=== От k1 = {k1} до k2 = {k2} успехов ===");
        sb.AppendLine($"Формула Бернулли (сумма, точно): P = {Fmt(exactRange)}");
        sb.AppendLine($"Интегральная формула Лапласа:    x1 = {Fmt(x1)}, x2 = {Fmt(x2)}");
        sb.AppendLine($"                                 Φ₀(x1) = {Fmt(ProbabilityMath.Laplace(x1))}");
        sb.AppendLine($"                                 Φ₀(x2) = {Fmt(ProbabilityMath.Laplace(x2))}");
        sb.AppendLine($"                                 P ≈ Φ₀(x2) − Φ₀(x1) = {Fmt(integralLaplace)}");
        sb.AppendLine($"Погрешность: {Fmt(Math.Abs(exactRange - integralLaplace))}  ({RelError(exactRange, integralLaplace)})");

        resultTextBox.Text = sb.ToString();
    }

    // Принимаем и «0.2», и «0,2» независимо от региональных настроек
    private static bool TryParseDouble(string text, out double value)
    {
        return double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value)
            || double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    private static string Fmt(double v) => v.ToString("0.########", CultureInfo.CurrentCulture);

    private static string RelError(double exact, double approx)
    {
        if (exact == 0) return "—";
        return (Math.Abs(exact - approx) / exact * 100).ToString("0.##", CultureInfo.CurrentCulture) + " %";
    }

    private static void Error(string message)
    {
        MessageBox.Show(message, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}