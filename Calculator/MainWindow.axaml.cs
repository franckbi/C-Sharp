using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Calculator;

public partial class MainWindow : Window
{
    // Holds the first number in the calculation
    private double resultValue = 0;

    // Stores which operator (+, -, ×, ÷) was clicked
    private string operatorClicked = "";

    // Flag to know if we should start a new entry (e.g., after pressing an operator)
    private bool isNewEntry = false;

    public MainWindow()
    {
        InitializeComponent();
    }

    // Handles digit and dot (.) buttons
    private void onButton_Click(object? sender, RoutedEventArgs e)
    {
        // Safety check: if resultTextBlock doesn't exist, do nothing
        if (resultTextBlock is null) return;

        // If we've just pressed an operator, start a new number
        if (isNewEntry)
        {
            resultTextBlock.Text = "0";
            isNewEntry = false;
        }

        // If the current display is "0", replace it, otherwise append the new character
        if (sender is Button button && button.Content is not null)
        {
            if (resultTextBlock.Text == "0")
                resultTextBlock.Text = button.Content.ToString(); 
            else
                resultTextBlock.Text += button.Content.ToString();
        }
    }

    // Handles operator buttons (+, -, ×, ÷)
    private void operator_Click(object? sender, RoutedEventArgs e)
    {
        if (resultTextBlock is null) return;

        if (sender is Button button && button.Content is not null)
        {
            // Parse whatever is currently in the display
            if (double.TryParse(resultTextBlock.Text, out double parsedValue))
            {
                resultValue = parsedValue;      // e.g., if display was "5", store 5
                operatorClicked = button.Content.ToString(); // e.g., "+"
                isNewEntry = true;             // The next digit typed starts fresh
            }
            else
            {
                Console.WriteLine("Warning: Invalid number format.");
            }
        }
    }

    // Handles the '=' button
    private void equalButton_Click(object? sender, RoutedEventArgs e)
    {
        if (resultTextBlock is null) return;

        if (double.TryParse(resultTextBlock.Text, out double parsedValue))
        {
            double finalResult = 0; // We’ll compute it below
            switch (operatorClicked)
            {
                case "+":
                    finalResult = resultValue + parsedValue;
                    break;
                case "-":
                    finalResult = resultValue - parsedValue;
                    break;
                case "×":
                    finalResult = resultValue * parsedValue;
                    break;
                case "÷":
                    if (parsedValue == 0)
                    {
                        resultTextBlock.Text = "Error"; // can't divide by 0
                        return;
                    }
                    else
                    {
                        finalResult = resultValue / parsedValue;
                    }
                    break;
            }

            // Show the result
            resultTextBlock.Text = finalResult.ToString();
            // Next time we press a digit, it should start a new entry
            isNewEntry = true;
        }
        else
        {
            Console.WriteLine("Warning: Invalid number format.");
        }
    }

    // Handles the "Clear" button
    private void clear_Click(object? sender, RoutedEventArgs e)
    {
        if (resultTextBlock is null) return;

        // Reset everything
        resultTextBlock.Text = "0";
        resultValue = 0;
        operatorClicked = "";
        isNewEntry = false;
    }
}
