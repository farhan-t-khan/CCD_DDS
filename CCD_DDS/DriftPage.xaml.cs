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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CCD_DDS
{
    /// <summary>
    /// Interaction logic for DriftPage.xaml
    /// </summary>
    public partial class DriftPage : Page
    {
        public DriftPage()
        {
            InitializeComponent();
        }
        public void NavigateToHome(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePage());
        }
    }
}
