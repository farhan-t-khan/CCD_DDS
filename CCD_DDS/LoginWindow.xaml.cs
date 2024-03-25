using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;

namespace CCD_DDS
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public bool IsAuthenticated { get; private set; }
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            // Perform authentication (replace this with your actual authentication logic)
            if (IsValidCredentials(username, password))
            {
                IsAuthenticated = true;
                Close(); // Close the login window if authentication succeeds
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.", "Authentication Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidCredentials(string username, string password)
        {
            // Replace this with your actual authentication logic
            return username == "admin" && password == "admin";
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                ShowSystemKeyboard();
            }
        }
        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                ShowSystemKeyboard();
            }
        }

        private void ShowSystemKeyboard()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "osk.exe"),
                    UseShellExecute = true,
                };
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open the on-screen keyboard. Please open it manually using your device's accessibility options.");
                Debug.WriteLine(ex.Message);
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CloseSystemKeyboard();
            }
        }

        private void CloseSystemKeyboard()
        {
            Process[] processes = Process.GetProcessesByName("osk");
            foreach (Process process in processes)
            {
                process.Kill();
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                InputMethod.SetIsInputMethodEnabled(textBox, false);
                CloseSystemKeyboard();
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                InputMethod.SetIsInputMethodEnabled(passwordBox, false);
                CloseSystemKeyboard();
            }
        }
    }
}
