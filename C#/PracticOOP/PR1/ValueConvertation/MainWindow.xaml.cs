using System;
using System.Text;
using System.Windows;

namespace ValueConvertation
{
    public partial class MainWindow : Window
    {
        // Словарь шестнадцатеричных символов — используется для перевода
        // в любую систему счисления от 2 до 16 (задание 3.2)
        private const string HexDigits = "0123456789ABCDEF";

        public MainWindow()
        {
            InitializeComponent();
        }

        // ===================================================================
        // Задание 3.1. Перевод целого десятичного числа в двоичную систему.
        // Алгоритм:
        // - TryParse для проверки ввода int;
        // - проверка на отрицательное значение;
        // - do-цикл: остаток от деления на 2 добавляется в начало строки
        //   (StringBuilder.Insert(0, ...)), затем число делится на 2 нацело;
        // - цикл завершается, когда i <= 0.
        // ===================================================================
        private void convertButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Считываем и проверяем ввод
            int i;
            if (!int.TryParse(inputTextBox.Text, out i))
            {
                MessageBox.Show("TextBox does not contain an integer");
                return;
            }

            // 2. Проверяем, что число неотрицательное
            if (i < 0)
            {
                MessageBox.Show("Please enter a positive number or zero");
                return;
            }

            // 3. Особый случай: 0 переводится в "0"
            if (i == 0)
            {
                binaryLabel.Content = "0";
                return;
            }

            // 4. Остаток от деления, используемый на каждой итерации
            int remainder = 0;

            // 5. StringBuilder для построения двоичного представления
            StringBuilder binary = new StringBuilder();

            // 6. Основной цикл перевода в двоичную систему
            do
            {
                remainder = i % 2;              // a. остаток от деления i на 2
                i = i / 2;                       // b. целочисленное деление i на 2
                binary.Insert(0, remainder);     // c. добавляем цифру в начало строки
            }
            while (i > 0);

            // 7. Выводим результат
            binaryLabel.Content = binary.ToString();

            // string result = Convert.ToString(i, 2);
            // binaryLabel.Content = result;
        }

        // ===================================================================
        // Задание 3.2 (обязательное дополнение).
        // Перевод целого десятичного числа в произвольную систему счисления
        // с основанием от 2 до 16, с использованием строки-словаря HexDigits.
        // ===================================================================
        private void convertBaseButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверяем корректность введённого числа
            int number;
            if (!int.TryParse(baseInputTextBox.Text, out number))
            {
                MessageBox.Show("TextBox does not contain an integer");
                return;
            }

            // 2. Проверяем корректность основания системы счисления
            int radix;
            if (!int.TryParse(radixTextBox.Text, out radix))
            {
                MessageBox.Show("Radix must be an integer");
                return;
            }

            if (radix < 2 || radix > 16)
            {
                MessageBox.Show("Radix must be between 2 and 16");
                return;
            }

            // 3. Запоминаем знак числа и работаем с модулем
            bool isNegative = number < 0;
            int value = Math.Abs(number);

            // 4. Особый случай: 0 в любой системе счисления — "0"
            if (value == 0)
            {
                baseResultLabel.Content = "0";
                return;
            }

            // 5. Строим результат делением на основание, беря остатки
            //    как индексы в строке HexDigits
            StringBuilder result = new StringBuilder();

            while (value > 0)
            {
                int remainder = value % radix;
                result.Insert(0, HexDigits[remainder]);
                value = value / radix;
            }

            // 6. Добавляем знак минус, если исходное число было отрицательным
            if (isNegative)
                result.Insert(0, '-');

            baseResultLabel.Content = result.ToString();
        }

                // ===================================================================
        // Задание 3.3 (обязательное дополнение).
        // Перевод арабского числа в римское и обратно, с использованием
        // таблицы соответствия значений и римских символов.
        //
        // Примечание: в таблице задания опечатка — пара (9,"IV") дублирует
        // (4,"IV"). Корректная пара для девяти — (9,"IX"). Ниже используется
        // исправленная таблица.
        // ===================================================================

        // Таблица соответствия, ОБЯЗАТЕЛЬНО отсортирована по убыванию значения —
        // это нужно для «жадного» алгоритма перевода в обе стороны.
        private static readonly (int Value, string Symbol)[] RomanTable =
        {
            (1000, "M"),
            (900,  "CM"),
            (500,  "D"),
            (400,  "CD"),
            (100,  "C"),
            (90,   "XC"),
            (50,   "L"),
            (40,   "XL"),
            (10,   "X"),
            (9,    "IX"),   
            (5,    "V"),
            (4,    "IV"),
            (1,    "I")
        };

        // ---- Арабское -> Римское ----
        private void toRomanButton_Click(object sender, RoutedEventArgs e)
        {
            int number;
            if (!int.TryParse(arabicTextBox.Text, out number))
            {
                MessageBox.Show("TextBox does not contain an integer");
                return;
            }

            // Римские цифры классически определены для диапазона 1..3999
            if (number < 1 || number > 3999)
            {
                MessageBox.Show("Please enter a number from 1 to 3999");
                return;
            }

            romanResultLabel.Content = ArabicToRoman(number);
        }

        private static string ArabicToRoman(int number)
        {
            StringBuilder roman = new StringBuilder();

            // Жадный алгоритм: берём наибольшее подходящее значение из таблицы,
            // сколько раз возможно вычитаем его из числа и добавляем символ
            foreach (var pair in RomanTable)
            {
                while (number >= pair.Value)
                {
                    roman.Append(pair.Symbol);
                    number -= pair.Value;
                }
            }

            return roman.ToString();
        }

        // ---- Римское -> Арабское ----
        private void toArabicButton_Click(object sender, RoutedEventArgs e)
        {
            string roman = romanTextBox.Text.Trim().ToUpperInvariant();

            if (string.IsNullOrEmpty(roman))
            {
                MessageBox.Show("Please enter a roman numeral");
                return;
            }

            int? result = RomanToArabic(roman);

            if (result == null)
            {
                MessageBox.Show("TextBox does not contain a valid roman numeral");
                return;
            }

            arabicResultLabel.Content = result.Value.ToString();
        }

        private static int? RomanToArabic(string roman)
        {
            int result = 0;

            // Идём по той же таблице сверху вниз: пока оставшаяся строка
            // начинается с символа из таблицы — «снимаем» его и прибавляем значение
            foreach (var pair in RomanTable)
            {
                while (roman.StartsWith(pair.Symbol, StringComparison.Ordinal))
                {
                    result += pair.Value;
                    roman = roman.Substring(pair.Symbol.Length);
                }
            }

            // Если после разбора остались непреобразованные символы — строка некорректна
            if (roman.Length > 0)
                return null;

            return result;
        }
    }
}