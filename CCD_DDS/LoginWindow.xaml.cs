using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
    }
}
