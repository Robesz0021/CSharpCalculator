using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

            // keyboard handlers
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            this.PreviewTextInput += MainWindow_PreviewTextInput;
        }

        // Textual input (gives the actual typed characters, good for + - * / % . , digits)
        private void MainWindow_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Text)) return;
            var ch = e.Text[0];

            if (ch >= '0' && ch <= '9')
            {
                ProcessNumberInput(ch.ToString());
                e.Handled = true;
                return;
            }

            if (ch == '.' || ch == ',')
            {
                ProcessNumberInput(".");
                e.Handled = true;
                return;
            }

            switch (ch)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '%':
                    ProcessOperatorInput(ch.ToString());
                    e.Handled = true;
                    break;
                case '=':
                    ProcessEquals();
                    e.Handled = true;
                    break;
            }
        }

        // Key strokes for non-text keys (Enter, Backspace, Delete, Escape, numpad operators)
        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    ProcessEquals();
                    e.Handled = true;
                    break;
                case Key.Back:
                    ProcessBackspace();
                    e.Handled = true;
                    break;
                case Key.Escape:
                    ProcessClear();
                    e.Handled = true;
                    break;
                case Key.Delete:
                    ProcessClearEntry();
                    e.Handled = true;
                    break;
                // numpad digits handled by PreviewTextInput as text in many cases, but map explicit keys too
                case Key.Add:
                    ProcessOperatorInput("+");
                    e.Handled = true;
                    break;
                case Key.Subtract:
                    ProcessOperatorInput("-");
                    e.Handled = true;
                    break;
                case Key.Multiply:
                    ProcessOperatorInput("*");
                    e.Handled = true;
                    break;
                case Key.Divide:
                    ProcessOperatorInput("/");
                    e.Handled = true;
                    break;
                default:
                    // handle main-row digits when PreviewTextInput not fired (rare)
                    if (e.Key >= Key.D0 && e.Key <= Key.D9)
                    {
                        var num = (char)('0' + (e.Key - Key.D0));
                        ProcessNumberInput(num.ToString());
                        e.Handled = true;
                    }
                    else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                    {
                        var num = (char)('0' + (e.Key - Key.NumPad0));
                        ProcessNumberInput(num.ToString());
                        e.Handled = true;
                    }
                    break;
            }
        }

        // Helpers used by both keyboard and click handlers
        private void ProcessNumberInput(string key)
        {
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

        private void ProcessOperatorInput(string op)
        {
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

        private void ProcessEquals()
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

        private void ProcessClear()
        {
            accumulator = null;
            pendingOperator = null;
            isNewEntry = true;
            DisplayArea.Text = "0";
        }

        private void ProcessClearEntry()
        {
            isNewEntry = true;
            DisplayArea.Text = "0";
        }

        private void ProcessBackspace()
        {
            if (DisplayArea.Text == "Error")
            {
                DisplayArea.Text = "0";
                isNewEntry = true;
                return;
            }

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

        // Existing click handlers now delegate to helpers (keep for XAML wiring)
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            var key = btn.Content?.ToString();
            if (string.IsNullOrEmpty(key)) return;
            ProcessNumberInput(key);
        }

        private void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            var op = btn.Content?.ToString();
            if (string.IsNullOrEmpty(op)) return;
            ProcessOperatorInput(op);
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessEquals();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessClear();
        }

        private void ClearEntryButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessClearEntry();
        }

        private void PercentButton_Click(object sender, RoutedEventArgs e)
        {
            double current;
            if (!TryParseDisplay(out current)) return;
            var percent = current / 100.0;
            SetDisplay(percent);
            isNewEntry = true;
        }

        // Backspace click still delegates
        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessBackspace();
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