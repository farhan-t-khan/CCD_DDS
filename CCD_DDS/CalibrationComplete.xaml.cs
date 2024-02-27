using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for CalibrationComplete.xaml
    /// </summary>
    public partial class CalibrationComplete : Page
    {
        private SoundPlayer clickSoundPlayer;
        public CalibrationComplete()
        {
            InitializeComponent();
            clickSoundPlayer = new SoundPlayer("Resource\\click.wav");
            LoadGasData();
            LoadCalibrationData();
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

        //Non OOP design
        private Dictionary<string, TextBox> gasTextBoxes = new Dictionary<string, TextBox>();
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

                        //Add the gas TextBox to the dictionary
                        gasTextBoxes.Add(gasName, gasTextBox);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading data: {ex.Message}");
            }
        }

        private void LoadCalibrationData()
        {
            try
            {
                // Check if the CalData.csv file exists
                if (!File.Exists("CalData.csv"))
                {
                    MessageBox.Show("Calibration data file not found.");
                    return;
                }
                // Read all lines from the CalData.csv file
                List<string> calLines = File.ReadAllLines("CalData.csv").ToList();

                // Process each line of the calibration data
                foreach (string line in calLines.Skip(1))
                {
                    string[] values = line.Split(',');

                    // Extract the gas name and calibration PPM value
                    string gasName = values[0];
                    string calPPM = values[1];

                    // Check if the gas TextBox exists in the dictionary
                    if (gasTextBoxes.ContainsKey(gasName))
                    {
                        // Get the TextBox associated with the gas name
                        TextBox gasTextBox = gasTextBoxes[gasName];

                        // Set the text of the TextBox to the calibration PPM value
                        gasTextBox.Text = calPPM;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading calibration data: {ex.Message}");
            }
        }

        //OOP design
        /*        private void LoadGasData()
                {
                    try
                    {
                        List<GasData> gasDataList = new List<GasData>();

                        List<string> gasLines = File.ReadAllLines("SetupData.csv").ToList();

                        // Skip the header line and process each data line
                        foreach (string line in gasLines.Skip(1))
                        {
                            string[] values = line.Split(',');

                            // Extract the values
                            GasData gasData = new GasData
                            {
                                Gas = values[0],
                                Concentration = values[1],
                                Amount = values[2],
                                Expiration = values[3],
                                Selected = values[4]
                            };

                            gasDataList.Add(gasData);
                        }

                        // Display gas data
                        foreach (GasData gasData in gasDataList.Where(g => g.Selected == "True"))
                        {
                            // Construct the gas information string
                            string gasInfo = $"{gasData.Gas}: {gasData.Concentration} ppm";

                            // Create a TextBlock to display the gas information
                            TextBlock gasTextBlock = new TextBlock();
                            gasTextBlock.Text = gasInfo;

                            // Create TextBox
                            TextBox gasTextBox = new TextBox();

                            // Add the TextBlock and TextBox to the StackPanel
                            GasStackPanel.Children.Add(gasTextBlock);
                            GasStackPanel.Children.Add(gasTextBox);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while loading data: {ex.Message}");
                    }
                }*/
    }
}
