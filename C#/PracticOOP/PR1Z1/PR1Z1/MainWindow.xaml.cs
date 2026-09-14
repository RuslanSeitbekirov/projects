using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace App1;

public partial class MainWindow : Window
{
    private int _clickCount;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            button.Content = $"Clicked count: {++_clickCount}";
        }
    }
}
