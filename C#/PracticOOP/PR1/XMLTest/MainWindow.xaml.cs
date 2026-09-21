using System;
using System.Windows;
using System.Windows.Media;

namespace XMLTest;

public partial class MainWindow : Window
{
    public Random random = new Random();

    public MainWindow()
    {
        InitializeComponent();
    }

    private void RandomFormButton_Click(object sender, RoutedEventArgs e)
    {
        int number = Random.Shared.Next(50, 200);
        int number2 = Random.Shared.Next(50, 350);
        Cube.Height = number;
        Cube.Width = number2;
    }

    private void RandomColorButton_Click(object sender, RoutedEventArgs e)
    {

        Cube.Foreground = new SolidColorBrush(Color.FromRgb(
            (byte)random.Next(0, 255),
            (byte)random.Next(0, 255),
            (byte)random.Next(0, 255)));

        Cube.Background = new SolidColorBrush(Color.FromRgb(
            (byte)random.Next(0, 255),
            (byte)random.Next(0, 255),
            (byte)random.Next(0, 255)));
    }
}