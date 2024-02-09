using System;
using System.Media;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CCD_DDS
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private SoundPlayer clickSoundPlayer;
        public HomePage()
        {
            InitializeComponent();
            clickSoundPlayer = new SoundPlayer("Resource\\click.wav");
        }
        private void NavigateToCalibration(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            ScreenNameTextBlock.Text = "Calibration";
            NavigationService.Navigate(new CalibrationPage());
        }
        private void NavigateToDrift(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            ScreenNameTextBlock.Text = "Drift";
            NavigationService.Navigate(new DriftPage());
        }
        private void NavigateToPrecision(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            ScreenNameTextBlock.Text = "Precision";
            NavigationService.Navigate(new PrecisionPage());
        }
        private void NavigateToSetup(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            ScreenNameTextBlock.Text = "Setup";
            NavigationService.Navigate(new SetupPage());
        }
    }
}
