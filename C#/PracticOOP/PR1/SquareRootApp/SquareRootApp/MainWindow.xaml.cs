using System;
using System.Globalization;
using System.Windows;

namespace SquareRootApp
{
    public partial class MainWindow : Window
    {
        // Состояние пошагового вычисления методом Ньютона (для "Обязательного дополнения")
        private decimal _numberDecimal;   // Исходное число, из которого ищем корень
        private decimal _delta;           // Порог точности (допустимая погрешность)
        private decimal _guess;           // Предыдущее приближение (x_n)
        private decimal _result;          // Текущее приближение (x_{n+1})
        private int _iteration;           // Счетчик итераций
        private bool _newtonFinished;     // Флаг завершения вычислений

        public MainWindow() // стандартный метод WPF, который загружает и иницилизирует элементы из XAML
        {
            InitializeComponent();
        }

        // Task 3 + Task 4: расчёт корня и через Math.Sqrt, и через метод Ньютона. Срабатывает при нажатии кнопки "Вычислить".
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
                // если всё ок, записывает число в numberDouble
            }

            if (numberDouble < 0)
            {
                MessageBox.Show("Please enter a positive number");
                return;
            }
            //Вычисление через Sqrt
            double squareRoot = Math.Sqrt(numberDouble);
            frameWorkLabel.Content = string.Format(CultureInfo.InvariantCulture, "{0} (Using the .NET Framework)", squareRoot); //SqrtRes + строка

            // ---- Task 4: вычисление через метод Ньютона (decimal) ----
            decimal numberDecimal;
            if (!decimal.TryParse(inputTextBox.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out numberDecimal))
            {
                MessageBox.Show("Please enter a decimal");
                return;
            }

            _numberDecimal = numberDecimal; // изначально вводимое число сохраняется в переменную и если проходит проверку заносится в поле.
            _delta = Convert.ToDecimal(Math.Pow(10, -28)); // программа остановится когда delta будет 1*10^-28, а не 0,0. Что умньшает количество итераций на 1

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
                FinishNewton(); // end
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