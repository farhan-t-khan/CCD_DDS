using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
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
    /// Interaction logic for CalibrationPage.xaml
    /// </summary>
    public partial class CalibrationPage : Page
    {
        private SoundPlayer clickSoundPlayer;
        public CalibrationPage()
        {
            InitializeComponent();
            clickSoundPlayer = new SoundPlayer("Resource\\click.wav");
            LoadGasData();
        }
        public void NavigateToHome(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            NavigationService.Navigate(new HomePage());
        }

        public void NavigateToCalibrationStart(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            NavigationService.Navigate(new CalibrationStart());
        }

        private void LoadGasData()
        {
            try
            {
                List<string> gasLines = File.ReadAllLines("SetupData.csv").ToList();

                // Skip the header line and process each data line
                foreach (string line in gasLines.Skip(1))
                {
                    string[] values = line.Split(',');

                    // Extract the values
                    string gasName = values[0];
                    string concentration = values[1];
                    bool selected = bool.Parse(values[4]); // Parse the 'Selected' value as boolean

                    // If the gas is selected, create and display the gas information
                    if (selected)
                    {
                        // Construct the gas information string
                        string gasInfo = $"{gasName}: {concentration} ppm";

                        // Create a TextBlock to display the gas information
                        TextBlock gasTextBlock = new TextBlock();
                        gasTextBlock.Text = gasInfo;

                        //Create TextBox
                        TextBox gasTextBox = new TextBox();

                        // Add the TextBlock to the StackPanel
                        GasStackPanel.Children.Add(gasTextBlock);
                        GasStackPanel.Children.Add(gasTextBox);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading data: {ex.Message}");
            }
        }

    }
}
