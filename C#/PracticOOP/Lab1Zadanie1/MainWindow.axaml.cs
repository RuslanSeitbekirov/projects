using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Lab1Zadanie1;  

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Находим кнопку и вешаем обработчик
        var myButton = this.Get<Button>("myButton");
        var myText = this.Get<TextBlock>("myText");

        myButton.Click += (sender, e) => 
        {
            myText.Text = "Кнопка нажата! 🎉";
        };
    }
}