using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.Reflection.Metadata;

namespace ValueConvertation;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
/// 
public class Value{
    private string type = "";
    private int data;
    

}



public partial class MainWindow : Window
{
    private static readonly string[] TypeNames =
            { "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16" };
            
    public MainWindow()
    {
        InitializeComponent();
        sourceTypeCombo.ItemsSource = TypeNames;
        targetTypeCombo.ItemsSource = TypeNames;
        sourceTypeCombo.SelectedIndex = 8; // 10
        targetTypeCombo.SelectedIndex = 0; // 2
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

        }

        private void Log(string message) => resultsListBox.Items.Add(message);

        // ---- Создание значения нужного типа из введённой строки ----
        private static dynamic CreateValue(string typeName, string text)
        {
            switch (typeName)
            {
                case "2":
                    return text;
                case "3":
                    return byte.Parse(text, CultureInfo.InvariantCulture);
                case "4":
                    return int.Parse(text, CultureInfo.InvariantCulture);
                case "5":
                    return float.Parse(text, CultureInfo.InvariantCulture);
                case "6":
                    return double.Parse(text, CultureInfo.InvariantCulture);
                case "7":
                    return decimal.Parse(text, CultureInfo.InvariantCulture);
                case "8":
                    return bool.Parse(text);
                case "9":
                    return (object)text; // произвольный объект — здесь просто строка в обёртке object
                case "10":
                    return (object)text; // произвольный объект — здесь просто строка в обёртке object
                case "11":
                    return (object)text; // произвольный объект — здесь просто строка в обёртке object
                case "12":
                    return (object)text; // произвольный объект — здесь просто строка в обёртке object
                case "13":
                    return (object)text; // произвольный объект — здесь просто строка в обёртке object
                case "14":
                    return (object)text; // произвольный объект — здесь просто строка в обёртке object
                case "15":
                    return (object)text; // произвольный объект — здесь просто строка в обёртке object
                case "16":
                    return (object)text; // произвольный объект — здесь просто строка в обёртке object
                default:
                    throw new NotSupportedException(typeName);
            }
        }
       
}