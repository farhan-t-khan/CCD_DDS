using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace CCD_DDS
{
    public partial class ConnectionErrorPage : Page
    {
        public ConnectionErrorPage()
        {
            InitializeComponent();
        }

        // Retry Button Click Event Handler
        private void RetryButtonClick(object sender, RoutedEventArgs e)
        {
            // Navigate back to CalibrationPage to retry connection
            NavigationService.Navigate(new CalibrationPage());

/*            this.Loaded += (s, e) =>
            {
                NavigationService.Navigate(new CalibrationPage());
            };*/
        }

        // Cancel Button Click Event Handler
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            // Navigate back to the HomePage
            NavigationService.Navigate(new HomePage());

            /*            this.Loaded += (s, e) =>
                        {
                            this.Loaded += (s, e) => 
                            {
                                NavigationService.Navigate(new HomePage()); 
                            };
                        };*/
        }
    }
}
