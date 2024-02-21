using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace CCD_DDS
{
    public partial class SetupPage : Page
    {
        private const string FileName = "SetupData.csv";
        //private const string FileName = "GasData.csv";
        private SoundPlayer clickSoundPlayer;
        public SetupPage()
        {
            clickSoundPlayer = new SoundPlayer("Resource\\click.wav");
            InitializeComponent();
            LoadDataFromFile();
        }
        // Method to load data from CSV file
        private void LoadDataFromFile()
        {
            try
            {
                // Check if the file exists
                if (File.Exists(FileName))
                {
                    // Open the CSV file for reading
                    using (var reader = new StreamReader(FileName))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        // Read the records into GasData objects
                        var records = csv.GetRecords<GasData>().ToList();

                        // Populate the UI with the data
                        foreach (var gasData in records)
                        {
                            switch (gasData.Gas)
                            {
                                case "Gas 1":
                                    Gas1ConcentrationTextBox.Text = gasData.Concentration;
                                    Gas1AmountTextBox.Text = gasData.Amount;
                                    Gas1ExpirationDatePicker.SelectedDate = ParseDate(gasData.Expiration);
                                    Gas1CheckBox.IsChecked = ParseBool(gasData.Selected);
                                    break;
                                case "Gas 2":
                                    Gas2ConcentrationTextBox.Text = gasData.Concentration;
                                    Gas2AmountTextBox.Text = gasData.Amount;
                                    Gas2ExpirationDatePicker.SelectedDate = ParseDate(gasData.Expiration);
                                    Gas2CheckBox.IsChecked = ParseBool(gasData.Selected);
                                    break;
                                case "Gas 3":
                                    Gas3ConcentrationTextBox.Text = gasData.Concentration;
                                    Gas3AmountTextBox.Text = gasData.Amount;
                                    Gas3ExpirationDatePicker.SelectedDate = ParseDate(gasData.Expiration);
                                    Gas3CheckBox.IsChecked = ParseBool(gasData.Selected);
                                    break;
                                case "Gas 4":
                                    Gas4ConcentrationTextBox.Text = gasData.Concentration;
                                    Gas4AmountTextBox.Text = gasData.Amount;
                                    Gas4ExpirationDatePicker.SelectedDate = ParseDate(gasData.Expiration);
                                    Gas4CheckBox.IsChecked = ParseBool(gasData.Selected);
                                    break;
                                case "Gas 5":
                                    Gas5ConcentrationTextBox.Text = gasData.Concentration;
                                    Gas5AmountTextBox.Text = gasData.Amount;
                                    Gas5ExpirationDatePicker.SelectedDate = ParseDate(gasData.Expiration);
                                    Gas5CheckBox.IsChecked = ParseBool(gasData.Selected);
                                    break;
                                default:
                                    // Handle unrecognized gas names if necessary
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading data: {ex.Message}");
            }
        }

        private DateTime? ParseDate(string date)
        {
            return string.IsNullOrEmpty(date) ? null : (DateTime?)DateTime.Parse(date);
        }

        private bool? ParseBool(string value)
        {
            if (bool.TryParse(value, out bool result))
            {
                return result;
            }
            return null;
        }
        public void NavigateToHome(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            NavigationService.Navigate(new HomePage());
        }
        /*        private void SaveButton_Click(object sender, RoutedEventArgs e)
                {
                    try
                    {
                        // Create or append to the CSV file
                        using (var writer = new StreamWriter(FileName, append: false))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            // Write headers
                            csv.WriteHeader<GasData>();
                            csv.NextRecord();

                            // Write the records
                            WriteGasData(csv, "Gas 1", Gas1ConcentrationTextBox.Text, Gas1AmountTextBox.Text, Gas1ExpirationDatePicker.SelectedDate, Gas1CheckBox.IsChecked ?? false);
                            WriteGasData(csv, "Gas 2", Gas2ConcentrationTextBox.Text, Gas2AmountTextBox.Text, Gas2ExpirationDatePicker.SelectedDate, Gas2CheckBox.IsChecked ?? false);
                            WriteGasData(csv, "Gas 3", Gas3ConcentrationTextBox.Text, Gas3AmountTextBox.Text, Gas3ExpirationDatePicker.SelectedDate, Gas3CheckBox.IsChecked ?? false);
                            WriteGasData(csv, "Gas 4", Gas4ConcentrationTextBox.Text, Gas4AmountTextBox.Text, Gas4ExpirationDatePicker.SelectedDate, Gas4CheckBox.IsChecked ?? false);
                            WriteGasData(csv, "Gas 5", Gas5ConcentrationTextBox.Text, Gas5AmountTextBox.Text, Gas5ExpirationDatePicker.SelectedDate, Gas5CheckBox.IsChecked ?? false);

                            MessageBox.Show("Data saved successfully!");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while saving data: {ex.Message}");
                    }
                }*/
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create or append to the CSV file
                using (var writer = new StreamWriter(FileName, append: false))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    // Write headers
                    csv.WriteHeader<GasData>();
                    csv.NextRecord();

                    // Write the records
                    WriteGasData(csv, "Gas 1", Gas1ConcentrationTextBox.Text, Gas1AmountTextBox.Text, Gas1ExpirationDatePicker.SelectedDate, Gas1CheckBox.IsChecked ?? false);
                    WriteGasData(csv, "Gas 2", Gas2ConcentrationTextBox.Text, Gas2AmountTextBox.Text, Gas2ExpirationDatePicker.SelectedDate, Gas2CheckBox.IsChecked ?? false);
                    WriteGasData(csv, "Gas 3", Gas3ConcentrationTextBox.Text, Gas3AmountTextBox.Text, Gas3ExpirationDatePicker.SelectedDate, Gas3CheckBox.IsChecked ?? false);
                    WriteGasData(csv, "Gas 4", Gas4ConcentrationTextBox.Text, Gas4AmountTextBox.Text, Gas4ExpirationDatePicker.SelectedDate, Gas4CheckBox.IsChecked ?? false);
                    WriteGasData(csv, "Gas 5", Gas5ConcentrationTextBox.Text, Gas5AmountTextBox.Text, Gas5ExpirationDatePicker.SelectedDate, Gas5CheckBox.IsChecked ?? false);

                    MessageBox.Show("Data saved successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving data: {ex.Message}");
            }
        }

        private void WriteGasData(CsvWriter csv, string gas, string concentration, string amount, DateTime? expiration, bool selected)
        {
            csv.WriteField(gas);
            csv.WriteField(concentration);
            csv.WriteField(amount);
            csv.WriteField(expiration?.ToString() ?? string.Empty);
            csv.WriteField(selected.ToString()); // Convert bool? to string
            csv.NextRecord();
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Check if the input is a digit or a single decimal point
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c) && c != '.')
                {
                    e.Handled = true; // Mark the event as handled to prevent non-numeric input
                    return;
                }
            }

            // Check if the input is a decimal point and if the text is empty
            if (e.Text == "." && string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0."; // Attach "0" and the decimal point
                textBox.CaretIndex = textBox.Text.Length; // Move the caret to the end
                e.Handled = true; // Mark the event as handled
                return;
            }

            // Check if the input is a decimal point and the text already contains one
            if (e.Text == "." && textBox.Text.Contains("."))
            {
                e.Handled = true; // Mark the event as handled to prevent multiple decimal points
                return;
            }

            // Check if the input contains a decimal point, and if so, prevent another one
            if (e.Text == "." && textBox.SelectionLength == 0)
            {
                int caretIndex = textBox.CaretIndex;
                int decimalIndex = textBox.Text.IndexOf('.');
                if (decimalIndex != -1 && caretIndex > decimalIndex)
                {
                    e.Handled = true;
                    return;
                }
            }
        }
    }
}
