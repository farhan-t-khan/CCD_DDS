using System.Windows;

namespace CCD_DDS
{
    public partial class ConnectionPopup : Window
    {
        public bool ShouldRetry { get; private set; }

        public ConnectionPopup()
        {
            InitializeComponent();
            ShouldRetry = false; // Initialize with no retry
        }

        // Retry Button Click Event Handler
        private void RetryButtonClick(object sender, RoutedEventArgs e)
        {
            ShouldRetry = true;
            this.DialogResult = true; // Close the dialog and return true
        }

        // Cancel Button Click Event Handler
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            ShouldRetry = false;
            this.DialogResult = false; // Close the dialog and return false
        }
    }
}