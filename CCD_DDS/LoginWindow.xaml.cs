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
using System.Security.Cryptography;
using System.Text;

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

        //FOR TEST ONLY
        //private bool IsValidCredentials(string username, string password)
        //{
        //    // Replace this with your actual authentication logic
        //    return username == "admin" && password == "admin";
        //}

        private bool IsValidCredentials(string username, string password)
        {
            string filePath = "credentials.txt";

            try
            {
                // Read all lines from the credentials file
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    // Split each line into username and hashed password parts
                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        string storedUsername = parts[0];
                        string storedHashedPassword = parts[1];

                        // Compute the hash of the provided password
                        string hashedPassword = ComputeHash(password);

                        // Check if the provided username and hashed password match the stored credentials
                        if (username == storedUsername && hashedPassword == storedHashedPassword)
                        {
                            return true; // Credentials are valid
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading credentials file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return false; // Credentials are invalid
        }

        private string ComputeHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Compute hash from input string
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convert byte array to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
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
