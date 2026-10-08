using System.Diagnostics;
using System.Windows;

namespace GreatestCommonDivisorStein;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // «Прогрев» JIT: при первом вызове метод компилируется,
        // и это время попало бы в измерение первого алгоритма.
        // убирает время на компиляцию методов из итого подсчета времени 
        GCDAlgorithms.FindGCDEuclid(48, 18, out _);
        GCDAlgorithms.FindGCDStein(48, 18, out _);
    }

    // ===================================================================
    // Вкладка 1. Сравнение алгоритмов Евклида и Штейна
    // ===================================================================
    private void findGCD_Click(object sender, RoutedEventArgs e)
    {
        if (!TryReadNumber(firstNumberTextBox.Text, "First number", out int firstNumber)) return;
        if (!TryReadNumber(secondNumberTextBox.Text, "Second number", out int secondNumber)) return;
        // чтобы не писать проверки для каждого метода, выносим её отдельно и передаёт текст строки её имя(для вывода ошибок), ссылка на переменную 

        long timeEuclid;
        long timeStein;

        int euclid = GCDAlgorithms.FindGCDEuclid(firstNumber, secondNumber, out timeEuclid);
        int stein = GCDAlgorithms.FindGCDStein(firstNumber, secondNumber, out timeStein);

        resultEuclid.Content = string.Format("Euclid: {0}, Time (ticks): {1}", euclid, timeEuclid);
        resultStein.Content = string.Format("Stein: {0}, Time (ticks): {1}", stein, timeStein);

        // Дополнительно: во сколько раз один алгоритм быстрее другого
        if (timeStein > 0 && timeEuclid > 0)
        {
            double ratio = (double)timeEuclid / timeStein; //явное приведение необходимо чтобы деление небыло целочисленным
            comparisonText.Text = string.Format(
                "Время Евклида / время Штейна ≈ {0:F2}. Частота таймера: {1:N0} тиков/с.",
                ratio, Stopwatch.Frequency);
                // Статическое свойство, возвращающее количество тиков (тиков процессора/таймера) 
                // в одной секунде. Нужно для понимания масштаба измеренного времени.
        }
        else
        {
            comparisonText.Text = string.Format(
                "Время слишком мало для сравнения. Частота таймера: {0:N0} тиков/с.",
                Stopwatch.Frequency);
        }
    }
 
    private static bool TryReadNumber(string text, string name, out int value)
    {
        if (!int.TryParse(text, out value))
        {
            MessageBox.Show(name + ": TextBox does not contain an integer");
            return false;
        }

        // Евклид через вычитание зацикливается на отрицательных числах
        if (value < 0)
        {
            MessageBox.Show(name + ": please enter a positive number or zero");
            return false;
        }

        return true;
    }

    // ===================================================================
    // Вкладка 2. Наибольшее простое < N, записанное через 2 и сдвиги
    // ===================================================================
    private void findPrimeButton_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(nTextBox.Text, out int n))
        {
            MessageBox.Show("TextBox does not contain an integer");
            return;
        }

        int? prime = PrimeShiftExpression.FindLargestPrimeBelow(n);
        if (prime == null)
        {
            MessageBox.Show("Простых чисел, меньших N, нет. Введите N > 2.");
            return;
        }

        string expression = PrimeShiftExpression.Build(prime.Value);
        long value = PrimeShiftExpression.Evaluate(expression);
        bool onlyTwos = PrimeShiftExpression.ContainsOnlyDigitTwo(expression);

        primeLabel.Content = string.Format("Наибольшее простое число < {0}: {1}", n, prime.Value);
        expressionTextBox.Text = expression;
        verifyText.Text = string.Format(
            "Проверка: значение выражения = {0} ({1}); других цифр, кроме 2: {2}; длина записи: {3} символов.",
            value,
            value == prime.Value ? "совпадает" : "ОШИБКА",
            onlyTwos ? "нет" : "ЕСТЬ",
            expression.Length);
    }
}