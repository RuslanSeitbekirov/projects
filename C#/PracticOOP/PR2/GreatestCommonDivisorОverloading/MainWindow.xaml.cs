using System.Drawing;
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

namespace GreatestCommonDivisorОverloading;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
   public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnFindGcd_Click(object sender, RoutedEventArgs e)
        {
            // Пытаемся преобразовать текст из полей в числа
            if (int.TryParse(InputA.Text, out int a) && int.TryParse(InputB.Text, out int b) && 
                int.TryParse(InputC.Text, out int c) && int.TryParse(InputD.Text, out int d) && 
                int.TryParse(InputE.Text, out int l))
            {
                // Считаем НОД (алгоритм Евклида), берем модули для отрицательных чисел
                int gcd = CalculateGCD(Math.Abs(a), Math.Abs(b), Math.Abs(c), Math.Abs(d), Math.Abs(l));
                OutputResult.Text = gcd.ToString();
            }
            else
            {
                OutputResult.Text = "Введите целые числа!";
            }
        }

        // Метод вычисления НОД
        public static int CalculateGCD(int a, int b)
        {
            if (a == 0) return b;
            while (b != 0)
            {
                if (a > b)
                {
                    a -= b;
                }
                else
                {
                    b -= a;
                }
            }
            return a;
        }

        public static int CalculateGCD(int a, int b, int c)
        {
          int g = CalculateGCD(a, b);
          int f = CalculateGCD(g, c);
          return f;  
        }
        public static int CalculateGCD(int a, int b, int c, int d)
        {
          return CalculateGCD(a, CalculateGCD(b, c, d));  
        }
        public static int CalculateGCD(int a, int b, int c, int d, int e)
        {
          return CalculateGCD(a, CalculateGCD(b, c, d, e));
        }

        private void BtnFindGcd2_Click(object sender, RoutedEventArgs e)
        {
            string[] NumLine = Input2.Text.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries); //  — это значение перечисления в 
                                                                                                                // C# (.NET), которое используется в 
                                                                                                              // методе string.Split() для исключения 
                                                                                                            // пустых строк из результирующего массива.
            int len = NumLine.Length;
            int[] num = new int[NumLine.Length];
            for (int i = 0; i < len; i++)
            {
                if(!int.TryParse(NumLine[i], out int f))
                {
                    OutputResult2.Text = "Введённая строка содержит не число!";
                    return;
                }
                num[i] = Math.Abs(f);
            }
            // Пытаемся преобразовать текст из полей в число
            // Считаем НОД (алгоритм Евклида), берем модули для отрицательных чисел
            int gcd = CalculateGCDline(num);
            OutputResult2.Text = gcd.ToString();
 
        }

        public static int CalculateGCDline(int[] num)
        {
            if (num.Length == 0) return 0;
            
            int result = num[0];
            for (int i = 1; i < num.Length; i++)
            {
                result = CalculateGCD(result, num[i]);
            }
            return result;
        }
    }
