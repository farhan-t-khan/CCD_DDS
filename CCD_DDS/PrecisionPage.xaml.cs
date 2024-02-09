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

namespace CCD_DDS
{
    /// <summary>
    /// Interaction logic for PrecisionPage.xaml
    /// </summary>
    public partial class PrecisionPage : Page
    {
        private SoundPlayer clickSoundPlayer;
        public PrecisionPage()
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
