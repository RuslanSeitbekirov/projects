using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RealNumberInput
{
    public partial class MainWindow : Window
    {
        // Допустимый (в т.ч. промежуточный) формат вещественного числа со знаком:
        // необязательный знак, цифры, необязательная точка/запятая, цифры
        // Разбор регулярного выражения ^[-+]?\d*([.,]\d*)?$:
        // ^ : Начало строки.
        // [-+]? : Необязательный (0 или 1 раз) знак плюс или минус.
        // \d* : Ноль или более цифр. (Звездочка * критически важна: она 
        // разрешает промежуточные состояния, например, пользователь только
        // нажал "-", но еще не ввел цифру).
        // ([.,]\d*)? : Необязательная группа: либо точка, либо запятая, за 
        // которыми следуют ноль или более цифр.
        // $ : Конец строки.
        private static readonly Regex ValidNumberRegex =
            new Regex(@"^[-+]?\d*([.,]\d*)?$");

        public MainWindow()
        {
            InitializeComponent();
            // Подписка на событие изменения текста.
            NumberTextBox.TextChanged += (s, e) =>
                ResultText.Text = "Текущее значение: " + NumberTextBox.Text;
        }

        // Фильтрация вводимых символов
        // Это событие срабатывает до того, как символ физически появится в TextBox
        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Мы явно указываем компилятору, что это TextBox, чтобы получить доступ к его свойствам.
            var textBox = (TextBox)sender;
            string proposedText = GetProposedText(textBox, e.Text);

            e.Handled = !IsValid(proposedText);
        }

        // Запрет пробела (Space по умолчанию не проходит через PreviewTextInput как обычный символ,
        // но на всякий случай блокируем явно)
        private void NumberTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        // Проверка текста при вставке из буфера обмена
        private void NumberTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {   // проверка что вставляется текст
            if (!e.DataObject.GetDataPresent(typeof(string)))
            { // отмена вставки
                e.CancelCommand();
                return;
            }

            string pastedText = (string)e.DataObject.GetData(typeof(string));
            var textBox = (TextBox)sender;
            string proposedText = GetProposedText(textBox, pastedText);
            // если не подходит отменяем
            if (!IsValid(proposedText))
                e.CancelCommand();
        }

        // Формирует текст, который получится после вставки newText в текущую позицию/выделение
        private static string GetProposedText(TextBox textBox, string newText)
        {
            string text = textBox.Text;

            if (textBox.SelectionLength > 0)
                text = text.Remove(textBox.SelectionStart, textBox.SelectionLength);

            text = text.Insert(textBox.SelectionStart, newText);
            return text;
        }

        // Проверка строки на соответствие формату (включая промежуточные состояния ввода)
        private static bool IsValid(string text)
        {
            if (text.Length == 0 || text == "-" || text == "+")
                return true; // допустимые промежуточные состояния при наборе

            return ValidNumberRegex.IsMatch(text);
        }

        private void NumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}