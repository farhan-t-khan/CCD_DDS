﻿using System;
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

namespace CCD_DDS
{
    /// <summary>
    /// Interaction logic for SetupPage.xaml
    /// </summary>
    public partial class SetupPage : Page
    {
        private SoundPlayer clickSoundPlayer;
        public SetupPage()
        {
            clickSoundPlayer = new SoundPlayer("Resource\\click.wav");
            InitializeComponent();
        }
        public void NavigateToHome(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            NavigationService.Navigate(new HomePage());
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Save the data entered by the user for each gas item
            // You can access the input fields like Gas1ConcentrationTextBox.Text, Gas1AmountTextBox.Text, etc.
            // Save the data to a text file or a database
            // Example:
            string gas1Concentration = Gas1ConcentrationTextBox.Text;
            string gas1Amount = Gas1AmountTextBox.Text;
            DateTime gas1Expiration = Gas1ExpirationDatePicker.SelectedDate ?? DateTime.MinValue;
            bool gas1Selected = Gas1CheckBox.IsChecked ?? false;

            string gas2Concentration = Gas2ConcentrationTextBox.Text;
            string gas2Amount = Gas2AmountTextBox.Text;
            DateTime gas2Expiration = Gas2ExpirationDatePicker.SelectedDate ?? DateTime.MinValue;
            bool gas2Selected = Gas2CheckBox.IsChecked ?? false;

            string gas3Concentration = Gas3ConcentrationTextBox.Text;
            string gas3Amount = Gas3AmountTextBox.Text;
            DateTime gas3Expiration = Gas3ExpirationDatePicker.SelectedDate ?? DateTime.MinValue;
            bool gas3Selected = Gas3CheckBox.IsChecked ?? false;

            string gas4Concentration = Gas4ConcentrationTextBox.Text;
            string gas4Amount = Gas4AmountTextBox.Text;
            DateTime gas4Expiration = Gas4ExpirationDatePicker.SelectedDate ?? DateTime.MinValue;
            bool gas4Selected = Gas4CheckBox.IsChecked ?? false;

            string gas5Concentration = Gas5ConcentrationTextBox.Text;
            string gas5Amount = Gas5AmountTextBox.Text;
            DateTime gas5Expiration = Gas5ExpirationDatePicker.SelectedDate ?? DateTime.MinValue;
            bool gas5Selected = Gas5CheckBox.IsChecked ?? false;

            MessageBox.Show("Data saved successfully!");
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
