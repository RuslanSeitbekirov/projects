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
        private static readonly Regex ValidNumberRegex =
            new Regex(@"^[-+]?\d*([.,]\d*)?$");

        public MainWindow()
        {
            InitializeComponent();
            NumberTextBox.TextChanged += (s, e) =>
                ResultText.Text = "Текущее значение: " + NumberTextBox.Text;
        }

        // Фильтрация вводимых символов
        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
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
        {
            if (!e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return;
            }

            string pastedText = (string)e.DataObject.GetData(typeof(string));
            var textBox = (TextBox)sender;
            string proposedText = GetProposedText(textBox, pastedText);

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