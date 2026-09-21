using System;
using System.Windows;
using System.Windows.Media;

namespace XMLTest;

/// <summary>
/// Логика взвимодействия MainWindow.xmal.cs и MainWindow.xmal 
/// </summary>

public partial class MainWindow : Window
{
    /// <summary>
    /// Обьект класса Random для генерации псевдо случайных чисел
    /// </summary>
    public Random random = new Random();

    public MainWindow()
    {
    /// <summary>
    /// Инициализация XAML
    /// </summary>
        InitializeComponent();
    }

    
        /// <summary>
        /// Обработка нажатия кнопки смены Размера
        /// </summary>
        /// <param name="sender"></parram>
        /// <param name="e"></parram>
    private void RandomFormButton_Click(object sender, RoutedEventArgs e)
    {
        /// <summary>
        /// для ширины и высоты Label генерируются два разных числа
        /// </summary>
        int number = Random.Shared.Next(50, 200);
        int number2 = Random.Shared.Next(50, 350);
        Cube.Height = number;
        Cube.Width = number2;
    }

        /// <summary>
        /// Обработка нажатия кнопки смены цвета
        /// </summary>
        /// <param name="sender"></parram>
        /// <param name="e"></parram>
    private void RandomColorButton_Click(object sender, RoutedEventArgs e)
    {
        /// <summary>
        /// Смена цвета текста на случайный в палитре RGB
        /// </summary>
        Cube.Foreground = new SolidColorBrush(Color.FromRgb(
            (byte)random.Next(0, 255),
            (byte)random.Next(0, 255),
            (byte)random.Next(0, 255)));
        /// <summary>
        /// Смена цвета "задней стенки" на случайный в палитре RGB
        /// </summary>
        Cube.Background = new SolidColorBrush(Color.FromRgb(
            (byte)random.Next(0, 255),
            (byte)random.Next(0, 255),
            (byte)random.Next(0, 255)));
    }
}