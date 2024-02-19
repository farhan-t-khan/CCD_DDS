using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
    /// Interaction logic for CalibrationStart.xaml
    /// </summary>
    public partial class CalibrationStart : Page
    {
        private SoundPlayer clickSoundPlayer;
        public CalibrationStart()
        {
            clickSoundPlayer = new SoundPlayer("Resource\\click.wav");
            InitializeComponent();
        }
        public void NavigateToHome(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            NavigationService.Navigate(new HomePage());
        }
    }
}
