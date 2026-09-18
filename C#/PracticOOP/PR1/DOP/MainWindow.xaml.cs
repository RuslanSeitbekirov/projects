using System;
using System.Globalization;
using System.Windows;

namespace TypeConversionApp
{
    public partial class MainWindow : Window
    {
        // Ограничиваемся типами по заданию
        private static readonly string[] TypeNames =
            { "char", "string", "byte", "int", "float", "double", "decimal", "bool", "object" };

        public MainWindow()
        {
            InitializeComponent();
            sourceTypeCombo.ItemsSource = TypeNames;
            targetTypeCombo.ItemsSource = TypeNames;
            sourceTypeCombo.SelectedIndex = 3; // int
            targetTypeCombo.SelectedIndex = 5; // double
            valueTextBox.Text = "42";
        }

        private void convertButton_Click(object sender, RoutedEventArgs e)
        {
            resultsListBox.Items.Clear();

            string sourceType = (string)sourceTypeCombo.SelectedItem;
            string targetType = (string)targetTypeCombo.SelectedItem;
            string input = valueTextBox.Text;

            // 1. Создаём значение исходного типа и кладём его в dynamic
            dynamic value;
            try
            {
                value = CreateValue(sourceType, input);
            }
            catch (Exception ex)
            {
                Log($"Не удалось разобрать значение как {sourceType}: {ex.Message}");
                return;
            }

            Log($"Исходное значение: {value}  (тип во время выполнения: {((object)value).GetType().Name})");

            // 2. Демонстрация оператора is
            bool matchesTarget = IsOfType(value, targetType);
            Log($"value is {targetType}  →  {matchesTarget}");

            // 3. Боксинг в object + оператор as (as применим к ссылочным случаям)
            object boxed = value;
            string viaAs = boxed as string;
            Log($"(boxed as string)  →  {(viaAs ?? "null (as не сработал — типы не совпали)")}");

            // 4. Неявное преобразование (через простое присваивание переменной dynamic)
            Log($"Неявное  (T x = value;)      в {targetType,-8} → {TryImplicitConvert(value, targetType)}");

            // 5. Явное преобразование (через приведение типа)
            Log($"Явное    ((T)value;)          в {targetType,-8} → {TryExplicitConvert(value, targetType)}");
        }

        private void Log(string message) => resultsListBox.Items.Add(message);

        // ---- Создание значения нужного типа из введённой строки ----
        private static dynamic CreateValue(string typeName, string text)
        {
            switch (typeName)
            {
                case "char":
                    if (text.Length == 0) throw new FormatException("пустая строка для char");
                    return text[0];
                case "string":
                    return text;
                case "byte":
                    return byte.Parse(text, CultureInfo.InvariantCulture);
                case "int":
                    return int.Parse(text, CultureInfo.InvariantCulture);
                case "float":
                    return float.Parse(text, CultureInfo.InvariantCulture);
                case "double":
                    return double.Parse(text, CultureInfo.InvariantCulture);
                case "decimal":
                    return decimal.Parse(text, CultureInfo.InvariantCulture);
                case "bool":
                    return bool.Parse(text);
                case "object":
                    return (object)text; // произвольный объект — здесь просто строка в обёртке object
                default:
                    throw new NotSupportedException(typeName);
            }
        }

        // ---- Проверка типа через is ----
        private static bool IsOfType(dynamic value, string typeName)
        {
            switch (typeName)
            {
                case "char": return value is char;
                case "string": return value is string;
                case "byte": return value is byte;
                case "int": return value is int;
                case "float": return value is float;
                case "double": return value is double;
                case "decimal": return value is decimal;
                case "bool": return value is bool;
                case "object": return value is object; // всегда true
                default: return false;
            }
        }

        // ---- Проверка неявного преобразования через dynamic-присваивание ----
        private static string TryImplicitConvert(dynamic value, string targetType)
        {
            try
            {
                switch (targetType)
                {
                    case "char": { char r = value; return r.ToString(); }
                    case "string": { string r = value; return r ?? "null"; }
                    case "byte": { byte r = value; return r.ToString(); }
                    case "int": { int r = value; return r.ToString(); }
                    case "float": { float r = value; return r.ToString(CultureInfo.InvariantCulture); }
                    case "double": { double r = value; return r.ToString(CultureInfo.InvariantCulture); }
                    case "decimal": { decimal r = value; return r.ToString(CultureInfo.InvariantCulture); }
                    case "bool": { bool r = value; return r.ToString(); }
                    case "object": { object r = value; return r?.ToString() ?? "null"; }
                    default: return "неизвестный тип";
                }
            }
            catch (Exception ex)
            {
                // как правило — Microsoft.CSharp.RuntimeBinder.RuntimeBinderException:
                // "Cannot implicitly convert type ..."
                return $"недоступно ({ex.GetType().Name})";
            }
        }

        // ---- Проверка явного преобразования через приведение типа ----
        private static string TryExplicitConvert(dynamic value, string targetType)
        {
            try
            {
                switch (targetType)
                {
                    case "char": { char r = (char)value; return r.ToString(); }
                    case "string": { string r = (string)value; return r ?? "null"; }
                    case "byte": { byte r = (byte)value; return r.ToString(); }
                    case "int": { int r = (int)value; return r.ToString(); }
                    case "float": { float r = (float)value; return r.ToString(CultureInfo.InvariantCulture); }
                    case "double": { double r = (double)value; return r.ToString(CultureInfo.InvariantCulture); }
                    case "decimal": { decimal r = (decimal)value; return r.ToString(CultureInfo.InvariantCulture); }
                    case "bool": { bool r = (bool)value; return r.ToString(); }
                    case "object": { object r = (object)value; return r?.ToString() ?? "null"; }
                    default: return "неизвестный тип";
                }
            }
            catch (Exception ex)
            {
                // RuntimeBinderException — нет такого оператора преобразования вовсе;
                // InvalidCastException / OverflowException / FormatException — преобразование
                // существует, но конкретное значение под него не подходит
                return $"недоступно/ошибка ({ex.GetType().Name}: {ex.Message})";
            }
        }
    }
}