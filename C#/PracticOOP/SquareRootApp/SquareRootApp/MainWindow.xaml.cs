using System;
using System.Globalization;
using System.Windows;

namespace SquareRootApp
{
    public partial class MainWindow : Window
    {
        // Состояние пошагового вычисления методом Ньютона (для "Обязательного дополнения")
        private decimal _numberDecimal;
        private decimal _delta;
        private decimal _guess;
        private decimal _result;
        private int _iteration;
        private bool _newtonFinished;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Task 3 + Task 4: расчёт корня и через Math.Sqrt, и через метод Ньютона
        private void calculateButton_Click(object sender, RoutedEventArgs e)
        {
            iterationsListBox.Items.Clear();
            nextStepButton.IsEnabled = false;
            iterationCountText.Text = string.Empty;

            // ---- Task 3: вычисление через Math.Sqrt (double) ----
            double numberDouble;
            if (!double.TryParse(inputTextBox.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out numberDouble))
            {
                MessageBox.Show("Please enter a double");
                return;
            }

            if (numberDouble < 0)
            {
                MessageBox.Show("Please enter a positive number");
                return;
            }

            double squareRoot = Math.Sqrt(numberDouble);
            frameWorkLabel.Content = string.Format(CultureInfo.InvariantCulture,
                "{0} (Using the .NET Framework)", squareRoot);

            // ---- Task 4: вычисление через метод Ньютона (decimal) ----
            decimal numberDecimal;
            if (!decimal.TryParse(inputTextBox.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out numberDecimal))
            {
                MessageBox.Show("Please enter a decimal");
                return;
            }

            _numberDecimal = numberDecimal;
            _delta = Convert.ToDecimal(Math.Pow(10, -28));

            // Особый случай: корень из нуля
            if (_numberDecimal == 0)
            {
                _result = 0;
                newtonLabel.Content = "0 (Using Newton's method)";
                _newtonFinished = true;
                return;
            }

            // Начальное приближение
            _guess = _numberDecimal / 2;
            _result = ((_numberDecimal / _guess) + _guess) / 2;
            _iteration = 1;
            _newtonFinished = false;

            LogIteration(_iteration, _result, Math.Abs(_result - _guess));

            // Если уже достаточно точно с первой итерации
            if (Math.Abs(_result - _guess) <= _delta)
            {
                FinishNewton();
            }
            else
            {
                nextStepButton.IsEnabled = true;
                iterationCountText.Text = "Итерация: " + _iteration;
            }
        }

        // Task 5 / "Обязательное дополнение": показ значения корня и погрешности
        // после каждой итерации по нажатию кнопки
        private void nextStepButton_Click(object sender, RoutedEventArgs e)
        {
            if (_newtonFinished) return;

            _guess = _result;
            _result = ((_numberDecimal / _guess) + _guess) / 2;
            _iteration++;

            decimal error = Math.Abs(_result - _guess);
            LogIteration(_iteration, _result, error);
            iterationCountText.Text = "Итерация: " + _iteration;

            if (error <= _delta)
            {
                FinishNewton();
            }
        }

        private void FinishNewton()
        {
            newtonLabel.Content = string.Format(CultureInfo.InvariantCulture,
                "{0} (Using Newton's method)", _result);
            nextStepButton.IsEnabled = false;
            _newtonFinished = true;
        }

        private void LogIteration(int number, decimal value, decimal error)
        {
            iterationsListBox.Items.Add(
                string.Format(CultureInfo.InvariantCulture,
                    "Итерация {0,-3} значение = {1,-32} погрешность = {2}",
                    number, value, error));
            iterationsListBox.ScrollIntoView(iterationsListBox.Items[iterationsListBox.Items.Count - 1]);
        }
    }
}