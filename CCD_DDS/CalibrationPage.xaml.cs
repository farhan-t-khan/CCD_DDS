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
using System.Windows.Threading;
//using static USBHID.Core.SharedData;
using static USBHID.Core;
using USBHID;

namespace CCD_DDS
{
    /// <summary>
    /// Interaction logic for CalibrationPage.xaml
    /// </summary>
    public partial class CalibrationPage : Page, INotifyPropertyChanged
    {
        private SoundPlayer clickSoundPlayer;
        private SoundPlayer calSoundPlayer;
        public List<string> LeakDefinitionOptions { get; set; }
        public List<string> TankCapacityOptions { get; set; }
        public List<LeakData> LeakDataList { get; set; }
        public List<LeakData> SelectedList { get; set; }
        private CancellationTokenSource source;
        private CancellationToken token;
        private Core? coreWindow;
        private DockingStationController? controller;
        private bool disposed = false;

        public void NavigateToHome(object sender, RoutedEventArgs e)
        {
            coreWindow = null;
            clickSoundPlayer.Play();
            NavigationService.Navigate(new HomePage());
        }



        //////////////////////

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        private Brush _dataGridBackground = Brushes.White;
        public Brush DataGridBackground
        {
            get { return _dataGridBackground; }
            set
            {
                _dataGridBackground = value;
                OnPropertyChanged(nameof(DataGridBackground));
            }
        }

        public CalibrationPage()
        {
            coreWindow = new Core();
            controller = new DockingStationController();
            InitializeComponent();
            clickSoundPlayer = new SoundPlayer("Resource\\click.wav");
            //calSoundPlayer = new SoundPlayer("Resource\\cal.wav");
            LeakDefinitionOptions = new List<string> { "100", "200", "500", "1000", "2000", "5000", "10000", "25000" };
            TankCapacityOptions = new List<string> { "Travel", "Small", "Medium", "Large" };
            DataContext = this;
            // Load data from CSV
            LoadDataFromCsv();
            LoadSelected();
            RefreshDataGrid();
            // Start a timer to update the clock every second
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            

            // Set the initial clock time and date
            ClockTextBlock.Text = DateTime.Now.ToString("HH:mm:ss");
            DateTextBlock.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy");

            //Add Model and Serial from detector
            ModelTextBlock.Text = "Model: " + coreWindow.Model;
            SerialTextBlock.Text = "Serial: " + coreWindow.Serial;

            if(coreWindow.Model is null || coreWindow.Serial is null)
            {
                CalibrateButton.Visibility = Visibility.Collapsed;
            } else
            {
                CalibrateButton.Visibility = Visibility.Visible;
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update the clock every second
            ClockTextBlock.Text = DateTime.Now.ToString("HH:mm:ss");

            // Update the date
            DateTextBlock.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
        }
        private void LoadDataFromCsv()
        {
            string csvFilePath = "TableData.csv";
            LeakDataList = new List<LeakData>();

            try
            {
                // Read all lines from the CSV file
                string[] lines = File.ReadAllLines(csvFilePath);

                // Skip the header row
                for (int i = 1; i < lines.Length; i++)
                {
                    // Split the current line by commas
                    string[] values = lines[i].Split(',');

                    // Create a new LeakData object and populate its properties
                    LeakData leakData = new LeakData
                    {
                        Port = values[0],
                        LeakDefinition = values[1],
                        Concentration = values[2],
                        TankCapacity = values[3],
                        ExpiryDate = string.IsNullOrWhiteSpace(values[4]) ? null : DateTime.Parse(values[4]),
                        LotNumber = values[5],
                        MeasuredConcentration = values[6],
                        Tolerance = values[7],
                        IsSelected = Convert.ToBoolean(values[8]),
                        Status = "",
                        TankLevel = Convert.ToDouble(values[10]),
                        DriftIsSelected = Convert.ToBoolean(values[11]),
                        PrecisionDate = values[12],
                        PrecisionTime = values[13],
                        Measurement1 = values[14],
                        Measurement2 = values[15],
                        Measurement3 = values[16],
                        Precision = values[17],
                        DriftDate1 = values[18],
                        DriftTime1 = values[19],
                        DriftConcentration1 = values[20],
                        DriftDate2 = values[21],
                        DriftTime2 = values[22],
                        DriftConcentration2 = values[23],
                        DriftPercentage = values[24],
                    };

                    // If Port is 0, set the Leak Definition directly to "0"
                    if (values[0] == "0")
                    {
                        leakData.LeakDefinition = "0";
                    }

                    // Add the LeakData object to the list
                    LeakDataList.Add(leakData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data from CSV: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadSelected()
        {
            SelectedList = new List<LeakData>();
            for (int i = 0; i < LeakDataList.Count; i++)
            {
                if (LeakDataList[i].IsSelected)
                {
                    SelectedList.Add(LeakDataList[i]);
                }
            }
        }
        private void LoadCalData()
        {
            string csvFilePath = "CalRecord_03062024_1000.csv";

            try
            {
                // Read all lines from the CSV file
                string[] lines = File.ReadAllLines(csvFilePath);

                // Skip the header row
                for (int i = 1; i < lines.Length; i++)
                {
                    // Split the current line by commas
                    string[] values = lines[i].Split(',');
                    LeakDataList[i - 1].MeasuredConcentration = values[1];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data from CSV: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SaveCalData()
        {
            string csvFilePath = "Calibration.csv";

            try
            {
                // Open the file in append mode, creating it if it doesn't exist
                using (StreamWriter writer = new StreamWriter(csvFilePath, true))
                {
                    // Check if the file is empty to determine if the header needs to be written
                    if (writer.BaseStream.Length == 0)
                    {
                        writer.WriteLine("Calibration Date Time,Port,Leak Definition (ppm),Concentration (ppm),Tank Capacity,Expiry Date,Lot Number,Measured Concentration (ppm)");
                    }

                    // Write data rows
                    foreach (LeakData leakData in SelectedList)
                    {
                        if (leakData.IsSelected)
                        {
                            string expiryDate = leakData.ExpiryDate.HasValue ? leakData.ExpiryDate.Value.ToString("MM/dd/yyyy") : "";
                            string calDateTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

                            writer.WriteLine($"{calDateTime},{leakData.Port},{leakData.LeakDefinition},{leakData.Concentration},{leakData.TankCapacity}," +
                                $"{expiryDate},{leakData.LotNumber},{leakData.MeasuredConcentration}");

                        }
                    }
                    // Write a divider line after each set of records
                    writer.WriteLine("-----------------------------------");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data to CSV: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ToggleButtonVisibility(bool IsReadOnly)
        {
            // Toggle visibility of navigation buttons
            CalibrateButton.Visibility = IsReadOnly ? Visibility.Visible : Visibility.Collapsed;
            // Toggle visibility of Save and Cancel buttons
            CancelButton.Visibility = IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
        }
        private void CalibrationSetupButtonClick(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            //Show login window
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();

            if (loginWindow.IsAuthenticated)
            {
                clickSoundPlayer.Play();
                
                //EditButton.Visibility = Visibility.Collapsed;
                CalibrationBackButton.Visibility = Visibility.Collapsed;
                SaveButton.Visibility = Visibility.Visible;
                
                RefreshDataGrid();
            }
        }
        private async void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            // Perform saving logic here
            SaveDataToCsv();
            // Reload data to refresh the table contents
            RefreshDataGrid();

            //EditButton.Visibility = Visibility.Visible;
            CalibrationBackButton.Visibility = Visibility.Visible;
            SaveButton.Visibility = Visibility.Collapsed;
        }
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            // Perform cancel logic here

            // Reload data to refresh the table contents
            RefreshDataGrid();
            // Toggle back to view mode
            //EditButton.Visibility = Visibility.Visible;
            CalibrationBackButton.Visibility = Visibility.Visible;
            SaveButton.Visibility = Visibility.Collapsed;
        }
        private void SaveDataToCsv()
        {
            string csvFilePath = "TableData.csv";

            try
            {
                using (StreamWriter writer = new StreamWriter(csvFilePath, false))
                {
                    // Write header row
                    writer.WriteLine("Port,Leak Definition (ppm),Concentration (ppm),Tank Capacity,Expiry Date,Lot Number,Measured Concentration (ppm),Calibration Tolerance (%),Selected,Status,Estimated Tank Level (%),Drift Selected,Precision Date,Precision Time,Measurement1,Measurement2,Measurement3,Precision,Drift Date 1,Drift Time 1,Drift Concentration 1,Drift Date 2,Drift Time 2,Drift Concentration 2,Drift Percentage");

                    // Write data rows
                    foreach (LeakData leakData in LeakDataList)
                    {
                        //Selected value is boolean and needs special handling
                        string isSelected = leakData.IsSelected ? "True" : "False";

                        string expiryDate = leakData.ExpiryDate.HasValue ? leakData.ExpiryDate.Value.ToString("MM/dd/yyyy") : "";
                        writer.WriteLine($"{leakData.Port},{leakData.LeakDefinition},{leakData.Concentration},{leakData.TankCapacity}," +
                            $"{expiryDate},{leakData.LotNumber},{leakData.MeasuredConcentration},{leakData.Tolerance},{isSelected},{leakData.Status},{leakData.TankLevel},{leakData.DriftIsSelected}," +
                            $"{leakData.PrecisionDate},{leakData.PrecisionTime},{leakData.Measurement1},{leakData.Measurement2},{leakData.Measurement3},{leakData.Precision},{leakData.DriftDate1},{leakData.DriftTime1},{leakData.DriftConcentration1},{leakData.DriftDate2},{leakData.DriftTime2},{leakData.DriftConcentration2},{leakData.DriftPercentage}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data to CSV: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshDataGrid()
        {
            // Reload data to refresh the table contents
            dataGrid.ItemsSource = null;
            LoadDataFromCsv();
            LoadSelected();
            dataGrid.ItemsSource = SelectedList; 
        }



        private async void StartCalibration(object sender, RoutedEventArgs e)
        {
            //The object coreWindow interracts with the detector
            Core coreWindow = new Core();
            
            clickSoundPlayer.Play();
            //QuitAppButton.Visibility = Visibility.Collapsed;

            bool anyExpired = false;

            foreach(LeakData leakData in SelectedList)
            {
                if(leakData.DaysUntilExpiry == 0)
                {
                    anyExpired = true;
                }
            }

            if (anyExpired)
            {
                MessageBox.Show("One or more gases have expired.", "Expired", MessageBoxButton.OK, MessageBoxImage.Warning);
            } else
            {
                //EditButton.Visibility = Visibility.Collapsed;
                CalibrationBackButton.Visibility = Visibility.Collapsed;
                source = new CancellationTokenSource();
                token = source.Token;
                foreach (LeakData leakData in SelectedList)
                {
                    leakData.Status = "";
                    leakData.MeasuredConcentration = "";
                }
                RefreshColumn(8);
                RefreshColumn(6);
                // Hide the other buttons and show the cancel button
                CalibrateButton.Visibility = Visibility.Collapsed;
                CalibrationCancelButton.Visibility = Visibility.Visible;

                var selectedItems = SelectedList.Where(item => item.Port != "0").ToList();

                //Sort by concentration
                selectedItems = selectedItems.OrderBy(item => int.Parse(item.Concentration)).ToList();

                //Mark gas with highest concentration
                //var highest = selectedItems[selectedItems.Count - 1];

                //Insert at the beginning of the list
                //selectedItems.Insert(0, highest);

                //Remove last element to calibrate the highest gas only once
                //selectedItems.RemoveAt(selectedItems.Count - 1);
                bool detectorCalPassed = true;
                foreach (LeakData leakData in selectedItems)
                {
                    // Check for cancellation before each iteration
                    if (token.IsCancellationRequested)
                    {
                        // Reset UI and exit the method
                        ResetUI();
                        return;
                    }

                    await ReadZeroGas();

                    //Turn detector pump on 
                    detectorPump(coreWindow, true);
                    
                    // Update the status to "Reading Gas"
                    leakData.Status = "Reading Gas";
                    // Refresh the UI to reflect the change
                    RefreshColumn(8);
                    // Wait for a brief moment to simulate the reading process
                    await Task.Delay(3000);

                    // Update the status to "Calibrating..."
                    //calSoundPlayer.Play();
                    //clickSoundPlayer.Play();
                    leakData.Status = "Calibrating...";
                    await Task.Delay(2000);
                    
                    //Turn detector pump off
                    detectorPump(coreWindow, false);
                    
                    // Refresh the UI to reflect the change
                    RefreshColumn(8);

                    //Record Date and Time for use in Drift Check
                    leakData.DriftDate1 = DateTime.Now.ToString("MM/dd/yyyy");
                    leakData.DriftTime1 = DateTime.Now.ToString("HH:mm:ss");

                    //Simulate calibration dummy values
                    Random random = new Random();
                    double percent = random.Next(2, 4);
                    int sign = random.Next(0, 2);

                    if (sign == 0)
                    {
                        double concentration = Convert.ToDouble(leakData.Concentration);
                        double newMeasuredConcentration = concentration + (percent / 100) * concentration;
                        leakData.MeasuredConcentration = ((int)Math.Round(newMeasuredConcentration)).ToString();
                        leakData.DriftConcentration1 = leakData.MeasuredConcentration;

                    }
                    else
                    {
                        double concentration = Convert.ToDouble(leakData.Concentration);
                        double newMeasuredConcentration = concentration - (percent / 100) * concentration;
                        leakData.MeasuredConcentration = ((int)Math.Round(newMeasuredConcentration)).ToString();
                        leakData.DriftConcentration1 = leakData.MeasuredConcentration;
                    }


                    // Wait for a brief moment to simulate the calibration process
                    await Task.Delay(2000);

                    //Calculate Pass or Fail according to Tolerance set
                    double expectedConcentration = Convert.ToDouble(leakData.Concentration);
                    double tolerancePercentage = Convert.ToDouble(leakData.Tolerance);

                    double lowerBound = expectedConcentration - (expectedConcentration * tolerancePercentage / 100);
                    double upperBound = expectedConcentration + (expectedConcentration * tolerancePercentage / 100);

                    double measuredConcentration = Convert.ToDouble(leakData.MeasuredConcentration);

                    if (measuredConcentration >= lowerBound && measuredConcentration <= upperBound)
                    {
                        leakData.Status = "Passed";
                    }
                    else
                    {
                        leakData.Status = "Failed";
                        detectorCalPassed = false;
                    }

                    // Refresh the UI to reflect the changes
                    RefreshColumn(8);

                    // Wait for a brief moment before moving to the next item
                    await Task.Delay(500);
                }
                SaveCalData();
                SaveDataToCsv();
                RefreshAll();

                //Send Calibration passed or Failed to detector
                if (detectorCalPassed)
                {
                    coreWindow.SendPacket(new byte[] { 0x2a });
                } else
                {
                    coreWindow.SendPacket(new byte[] { 0x2b });
                }

                CalibrateButton.Visibility = Visibility.Visible;
                CalibrationCancelButton.Visibility = Visibility.Collapsed;
                CalibrationBackButton.Visibility = Visibility.Visible;
                //EditButton.Visibility = Visibility.Visible;

                //Experiment with commands
                //coreWindow.CalibrationSuccessful();
                //coreWindow.PumpOff();
            }
        }

        private void CalibrationCancelClick(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            // Cancel ongoing calibrations
            if (source != null)
            {
                source.Cancel();
            }
            // Show the other buttons and hide the cancel button
            CalibrateButton.Visibility = Visibility.Visible;
            CalibrationCancelButton.Visibility = Visibility.Collapsed;
            CalibrationBackButton.Visibility = Visibility.Visible;
        }

        private void ResetUI()
        {
            foreach (LeakData leakData in LeakDataList)
            {
                leakData.Status = "";
            }
            RefreshAll();
            // Show the other buttons and hide the cancel button
            CalibrateButton.Visibility = Visibility.Visible;
            CalibrationCancelButton.Visibility = Visibility.Collapsed;
        }

/*        private void RefreshColumn(int columnIndex)
        {
            // Iterate through each row in the DataGrid
            foreach (var item in dataGrid.Items)
            {
                // Get the corresponding property of the item based on the column index
                var property = item.GetType().GetProperty(dataGrid.Columns[columnIndex].SortMemberPath);

                // Update the property value
                property?.SetValue(item, property.GetValue(item));
            }
        }*/
        private void RefreshColumn(int columnIndex)
        {
            if (columnIndex >= 0 && columnIndex < dataGrid.Columns.Count)
            {
                // Iterate through each row in the DataGrid
                foreach (var item in dataGrid.Items)
                {
                    // Get the corresponding property of the item based on the column index
                    var sortMemberPath = dataGrid.Columns[columnIndex].SortMemberPath;
                    var property = item.GetType().GetProperty(sortMemberPath);

                    if (property != null)
                    {
                        // Update the property value
                        property.SetValue(item, property.GetValue(item));
                    }
                }
            }
        }


        private void RefreshAll()
        {
            // This method forces the DataGrid to refresh its items, ensuring the UI reflects the changes immediately
            dataGrid.Items.Refresh();
        }

        private async Task ReadZeroGas()
        {
            // Check for cancellation before starting the operation
            if (token.IsCancellationRequested)
            {
                // Perform any necessary cleanup and exit early
                ResetUI();
                return;
            }
            
            Core coreWindow = new Core();
            
            detectorPump(coreWindow, true);
            LeakDataList[0].Status = "Reading Gas";
            RefreshColumn(8);
            await Task.Delay(3000);
            detectorPump(coreWindow, false);
            LeakDataList[0].Status = "Done";
            RefreshColumn(8);
            await Task.Delay(1000);
            LeakDataList[0].Status = "";
            RefreshColumn(8);
        }
        private void NavigateToSetup(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            ScreenNameTextBlock.Text = "Setup";
            NavigationService.Navigate(new SetupPage());
        }

        //Detector pump ON/OFF
        private void detectorPump(Core coreWindow, bool state)
        {
            if (state)
            {
                coreWindow.SendPacket(new byte[] { 0x20 });
            } else
            {
                coreWindow.SendPacket(new byte[] { 0x21 });
            }
            
        }
    }
}
