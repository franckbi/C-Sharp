using Avalonia.Controls;

namespace MyAvaloniaApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var textBox = this.FindControl<TextBox>("inputBox");
        var outputText = this.FindControl<TextBlock>("outputText");

        outputText.Text = $"You typed: {textBox.Text}";
    }
}