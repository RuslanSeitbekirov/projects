using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace MatrixMultiplication;

public partial class MainWindow : Window
{
    private const int MaxSize = 20; // ограничение размера в 20 на 20

    // Task 2: двумерные массивы. Как в методичке, первое измерение — столбцы,
    // второе — строки: matrix[column, row].
    private double[,] matrix1 = new double[0, 0];
    private double[,] matrix2 = new double[0, 0];
    private double[,] result = new double[0, 0];

    // Защита от SelectionChanged во время начальной настройки ComboBox
    private bool _ready; //Чтобы SelectionChanged не пытался пересчитать матрицы до
    // того, как все ComboBox'ы настроены, мы ждем, пока конструктор завершится, и только потом ставим _ready = true.

    public MainWindow()
    {
        InitializeComponent();

        var sizes = Enumerable.Range(1, MaxSize).ToList(); // создает послед. от 1 до 20 чтобы заполнить выпадающие списки без цикла for
        m1RowsComboBox.ItemsSource = sizes;
        m1ColumnsComboBox.ItemsSource = sizes;
        m2ColumnsComboBox.ItemsSource = sizes;

        // Начальные размеры: Matrix 1 — 2×3, Matrix 2 — 3×2
        m1RowsComboBox.SelectedIndex = 1;       // 2 строки (считаем с нуля) 
        m1ColumnsComboBox.SelectedIndex = 2;    // 3 столбца
        m2ColumnsComboBox.SelectedIndex = 1;    // 2 столбца

        _ready = true;
        rebuildInputMatrices();
    }

    // ===================================================================
    // Изменение размеров или режима заполнения -> пересоздаём входные матрицы
    // ===================================================================
    private void dimension_Changed(object sender, RoutedEventArgs e)
    {
        if (!_ready) return;
        rebuildInputMatrices();
    }

    private void randomizeButton_Click(object sender, RoutedEventArgs e)
    {
        rebuildInputMatrices();
    }

    private void rebuildInputMatrices()
    {
        if (m1RowsComboBox.SelectedItem is not int m1rows || // проверяет, что в SelectedItem лежит именно int (а не null), и сразу присваивает его переменной m1rows
            m1ColumnsComboBox.SelectedItem is not int m1columns ||
            m2ColumnsComboBox.SelectedItem is not int m2columns)
        {
            return;
        }

        // Число строк Matrix 2 всегда равно числу столбцов Matrix 1
        int m2rows = m1columns;
        m2RowsText.Text = m2rows.ToString();

        matrix1 = new double[m1columns, m1rows];
        matrix2 = new double[m2columns, m2rows];

        // Замечание 1: заполняем ячейки случайными числами
        bool real = realNumbersCheckBox.IsChecked == true;
        fillRandom(matrix1, real);
        fillRandom(matrix2, real);

        initializeGrid(grid1, matrix1);
        initializeGrid(grid2, matrix2);
        clearGrid(grid3);

        matrix1Group.Header = string.Format("Matrix 1 ({0} × {1})", m1rows, m1columns); // обновление заголовков для отображения размеров матриц
        matrix2Group.Header = string.Format("Matrix 2 ({0} × {1})", m2rows, m2columns);
        resultGroup.Header = "Result Matrix";
    }

    private static void fillRandom(double[,] matrix, bool real)
    {
        for (int x = 0; x < matrix.GetLength(0); x++)
        {
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                matrix[x, y] = real
                    ? Math.Round(Random.Shared.NextDouble() * 20 - 10, 2)   // вещественные: -10..10
                    : Random.Shared.Next(-9, 10);                           // целые: -9..9
            }
        }
    }

    // ===================================================================
    // Задания 2-4: чтение матриц, умножение, вывод результата
    // ===================================================================
    private void buttonCalculate_Click(object sender, RoutedEventArgs e)
    {
        // Task 2: копируем данные из Grid в массивы
        if (!getValuesFromGrid(grid1, matrix1, "Matrix 1")) return;
        if (!getValuesFromGrid(grid2, matrix2, "Matrix 2")) return;

        // Task 2: размерности
        int m1columns_m2rows = matrix1.GetLength(0);   // столбцы Matrix 1 = строки Matrix 2
        int m1rows = matrix1.GetLength(1);
        int m2columns = matrix2.GetLength(0);

        result = new double[m2columns, m1rows];

        // Task 3: вычисление произведения
        for (int row = 0; row < m1rows; row++)
        {
            for (int column = 0; column < m2columns; column++)
            {
                double accumulator = 0;

                // Сумма произведений элементов строки Matrix 1 на столбец Matrix 2
                for (int cell = 0; cell < m1columns_m2rows; cell++)
                {
                    accumulator += matrix1[cell, row] * matrix2[column, cell];
                }

                result[column, row] = accumulator;
            }
        }

        // Task 4: показываем результат (только для чтения)
        initializeGrid(grid3, result, readOnly: true);
        resultGroup.Header = string.Format("Result Matrix ({0} × {1})", m1rows, m2columns);
    }

    // ===================================================================
    // Методы из методички: отображение массива в Grid и чтение обратно
    // ===================================================================

    // Отображение двумерного массива в Grid: каждая ячейка — TextBox.
    // Отличие от методички: столбцы и строки имеют размер Auto (а не Star),
    // иначе при большой матрице ячейки сжимаются, а полосы прокрутки не появляются.
    private void initializeGrid(Grid grid, double[,] matrix, bool readOnly = false)
    {
        // Сброс сетки
        clearGrid(grid);

        int columns = matrix.GetLength(0);
        int rows = matrix.GetLength(1);

        for (int x = 0; x < columns; x++)
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        for (int y = 0; y < rows; y++)
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Заполняем каждую ячейку редактируемым TextBox со значением из матрицы
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                var t = new TextBox
                {
                    Text = formatNumber(matrix[x, y]),
                    MinWidth = 60,
                    Margin = new Thickness(2),
                    Padding = new Thickness(3),
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    IsReadOnly = readOnly
                };

                Grid.SetRow(t, y);
                Grid.SetColumn(t, x);
                grid.Children.Add(t);
            }
        }
    }

    // Чтение значений из TextBox'ов сетки в двумерный массив.
    // Вместо double.Parse используется TryParse, чтобы неверный ввод
    // не приводил к исключению.
    private bool getValuesFromGrid(Grid grid, double[,] matrix, string name)
    {
        foreach (TextBox t in grid.Children.OfType<TextBox>())
        {
            int row = Grid.GetRow(t);
            int column = Grid.GetColumn(t);

            if (!tryParseNumber(t.Text, out double value))
            {
                MessageBox.Show(
                    string.Format("{0}: ячейка (строка {1}, столбец {2}) не содержит числа",
                        name, row + 1, column + 1),
                    "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                t.Focus();
                t.SelectAll();
                return false;
            }

            matrix[column, row] = value;
        }

        return true;
    }

    private static void clearGrid(Grid grid)
    {
        grid.Children.Clear();
        grid.ColumnDefinitions.Clear();
        grid.RowDefinitions.Clear();
    }

    // Принимаем и «1,5», и «1.5» независимо от региональных настроек
    private static bool tryParseNumber(string text, out double value)
    {
        return double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value)
            || double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    // Не показываем длинные «хвосты» вроде 0,30000000000000004
    private static string formatNumber(double value)
    {
        return value.ToString("0.######", CultureInfo.CurrentCulture);
    }
}