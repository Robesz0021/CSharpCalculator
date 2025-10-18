using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private double? accumulator = null;
        private string pendingOperator = null;
        private bool isNewEntry = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            var key = btn.Content?.ToString();
            if (string.IsNullOrEmpty(key)) return;

            if (DisplayArea.Text == "Error" || isNewEntry && key != ".")
            {
                DisplayArea.Text = "0";
            }

            // start new entry
            if (isNewEntry)
            {
                DisplayArea.Text = (key == ".") ? "0." : key;
                isNewEntry = false;
                return;
            }

            if (key == ".")
            {
                if (DisplayArea.Text.Contains(".")) return;
                DisplayArea.Text += ".";
            }
            else
            {
                if (DisplayArea.Text == "0")
                    DisplayArea.Text = key;
                else
                    DisplayArea.Text += key;
            }
        }

        private void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            var op = btn.Content?.ToString();
            if (string.IsNullOrEmpty(op)) return;

            double current;
            if (!TryParseDisplay(out current)) return;

            if (accumulator == null)
            {
                accumulator = current;
            }
            else if (!isNewEntry)
            {
                accumulator = Calculate(accumulator.Value, pendingOperator, current);
                if (double.IsNaN(accumulator.Value) || double.IsInfinity(accumulator.Value))
                {
                    DisplayError();
                    return;
                }
                SetDisplay(accumulator.Value);
            }

            pendingOperator = op;
            isNewEntry = true;
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            double current;
            if (!TryParseDisplay(out current)) return;

            if (pendingOperator != null && accumulator != null)
            {
                var result = Calculate(accumulator.Value, pendingOperator, current);
                if (double.IsNaN(result) || double.IsInfinity(result))
                {
                    DisplayError();
                    return;
                }
                SetDisplay(result);
                accumulator = result;
                pendingOperator = null;
                isNewEntry = true;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            accumulator = null;
            pendingOperator = null;
            isNewEntry = true;
            DisplayArea.Text = "0";
        }

        private void ClearEntryButton_Click(object sender, RoutedEventArgs e)
        {
            isNewEntry = true;
            DisplayArea.Text = "0";
        }

        private void PercentButton_Click(object sender, RoutedEventArgs e)
        {
            double current;
            if (!TryParseDisplay(out current)) return;
            var percent = current / 100.0;
            SetDisplay(percent);
            isNewEntry = true;
        }

        // Backspace: remove last character of the current entry
        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayArea.Text == "Error")
            {
                DisplayArea.Text = "0";
                isNewEntry = true;
                return;
            }

            // If a new entry is expected (just pressed operator or result shown), treat backspace as no-op or reset to 0
            if (isNewEntry)
            {
                DisplayArea.Text = "0";
                isNewEntry = true;
                return;
            }

            var text = DisplayArea.Text ?? string.Empty;
            if (text.Length <= 1)
            {
                DisplayArea.Text = "0";
                isNewEntry = true;
                return;
            }

            text = text.Substring(0, text.Length - 1);

            // If result is just a lone negative sign or empty, reset to 0
            if (string.IsNullOrEmpty(text) || text == "-")
            {
                DisplayArea.Text = "0";
                isNewEntry = true;
            }
            else
            {
                DisplayArea.Text = text;
            }
        }

        private bool TryParseDisplay(out double value)
        {
            value = 0;
            var text = DisplayArea.Text;
            if (string.IsNullOrEmpty(text) || text == "Error")
            {
                value = 0;
                return false;
            }
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private double Calculate(double left, string op, double right)
        {
            switch (op)
            {
                case "+": return left + right;
                case "-": return left - right;
                case "*": return left * right;
                case "/":
                    if (Math.Abs(right) < double.Epsilon) return double.NaN;
                    return left / right;
                default: return right;
            }
        }

        private void SetDisplay(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                DisplayError();
                return;
            }

            // show integer without decimal point when appropriate
            if (Math.Abs(value % 1) < 1e-12)
                DisplayArea.Text = ((long)value).ToString(CultureInfo.InvariantCulture);
            else
                DisplayArea.Text = value.ToString("G15", CultureInfo.InvariantCulture);
        }

        private void DisplayError()
        {
            DisplayArea.Text = "Error";
            accumulator = null;
            pendingOperator = null;
            isNewEntry = true;
        }
    }
}