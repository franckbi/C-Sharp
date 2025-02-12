using Avalonia.Controls;
using System;
using System.Collections.Generic;

namespace volumeConverter
{
    public partial class MainWindow : Window
    {
        private readonly List<string> Units = new() { "liters", "gallons", "cubic meters" };

        public MainWindow()
        {
            InitializeComponent();

            // Populate ComboBoxes with ItemsSource
            from.ItemsSource = Units;
            to.ItemsSource = Units;

            // Set default selections
            from.SelectedIndex = 0;
            to.SelectedIndex = 1;
        }

        private void Convert(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                // Ensure input is not empty
                if (string.IsNullOrWhiteSpace(input.Text))
                {
                    output.Content = "Please enter a value.";
                    return;
                }

                // Safely parse the input number
                if (!double.TryParse(input.Text, out double value))
                {
                    output.Content = "Invalid number format.";
                    return;
                }

                // Ensure valid selections
                if (from.SelectedItem is string fromUnit && to.SelectedItem is string toUnit)
                {
                    // Convert volume
                    double convertedValue = ConvertVolume(value, fromUnit, toUnit);

                    // Display result
                    output.Content = $"{value} {fromUnit} = {convertedValue:F2} {toUnit}";
                }
                else
                {
                    output.Content = "Please select valid units.";
                }
            }
            catch (Exception ex)
            {
                output.Content = "Error: " + ex.Message;
            }
        }

        private double ConvertVolume(double value, string from, string to)
        {
            // Conversion factors
            double litersToGallons = 0.264172;
            double litersToCubicMeters = 0.001;
            double gallonsToLiters = 3.78541;
            double cubicMetersToLiters = 1000;

            // Convert input to liters
            double inLiters = from switch
            {
                "liters" => value,
                "gallons" => value * gallonsToLiters,
                "cubic meters" => value * cubicMetersToLiters,
                _ => throw new ArgumentException("Invalid unit")
            };

            // Convert from liters to target unit
            return to switch
            {
                "liters" => inLiters,
                "gallons" => inLiters * litersToGallons,
                "cubic meters" => inLiters * litersToCubicMeters,
                _ => throw new ArgumentException("Invalid unit")
            };
        }
    }
}
