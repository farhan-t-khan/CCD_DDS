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


namespace CCD_DDS
{
    /// <summary>
    /// Interaction logic for CalibrationPage.xaml
    /// </summary>
    public partial class CalibrationPage : Page, INotifyPropertyChanged
    {


        public void NavigateToHome(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            NavigationService.Navigate(new HomePage());
        }


        //////////////////////

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private SoundPlayer clickSoundPlayer;
        private SoundPlayer calSoundPlayer;
        public List<string> LeakDefinitionOptions { get; set; }
        public List<string> TankCapacityOptions { get; set; }
        public List<LeakData> LeakDataList { get; set; }
        public List<LeakData> SelectedList { get; set; }
        private CancellationTokenSource source;
        private CancellationToken token;
        private bool _isReadOnly = true;
        public bool _isEditMode = false;

        public bool IsEditMode
        {
            get { return _isEditMode; }
            set
            {
                _isEditMode = value;
                OnPropertyChanged(nameof(IsEditMode));
            }
        }
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                _isReadOnly = value;
                OnPropertyChanged(nameof(IsReadOnly));

                // Update DataGrid background color based on edit mode
                DataGridBackground = value ? Brushes.White : Brushes.LightGray;
            }
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
            InitializeComponent();
            clickSoundPlayer = new SoundPlayer("Resource\\click.wav");
            //calSoundPlayer = new SoundPlayer("Resource\\cal.wav");
            LeakDefinitionOptions = new List<string> { "100", "200", "500", "1000", "2000", "5000", "10000", "25000" };
            TankCapacityOptions = new List<string> { "Travel", "Small", "Medium", "Large" };
            DataContext = this;
            IsReadOnly = true;
            // Load data from CSV
            LoadDataFromCsv();
            LoadSelected();
            RefreshDataGrid();
            //LoadCalData();
            //SaveDataToCsv();
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
                        Status = ""
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


        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            IsEditMode = !IsEditMode;
            // Perform cancel logic here

            // Reload data to refresh the table contents
            RefreshDataGrid();
            // Toggle back to view mode
            IsReadOnly = true;
            ToggleButtonVisibility(IsReadOnly);
        }
        private void SaveDataToCsv()
        {
            string csvFilePath = "TableData.csv";

            try
            {
                using (StreamWriter writer = new StreamWriter(csvFilePath, false))
                {
                    // Write header row
                    writer.WriteLine("Port,Leak Definition (ppm),Concentration (ppm),Tank Capacity,Expiry Date,Lot Number,Measured Concentration (ppm),Calibration Tolerance (%),Selected,Status");

                    // Write data rows
                    foreach (LeakData leakData in LeakDataList)
                    {
                        //Selected value is boolean and needs special handling
                        string isSelected = leakData.IsSelected ? "True" : "False";

                        string expiryDate = leakData.ExpiryDate.HasValue ? leakData.ExpiryDate.Value.ToString("MM/dd/yyyy") : "";
                        writer.WriteLine($"{leakData.Port},{leakData.LeakDefinition},{leakData.Concentration},{leakData.TankCapacity}," +
                            $"{expiryDate},{leakData.LotNumber},{leakData.MeasuredConcentration},{leakData.Tolerance},{isSelected},{leakData.Status}");
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
            //dataGrid.ItemsSource = LeakDataList;
            if (IsEditMode)
            {
                dataGrid.ItemsSource = LeakDataList;
            }
            else
            {
                dataGrid.ItemsSource = SelectedList;
            }
        }

        private void DataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject depObj = (DependencyObject)e.OriginalSource;

            // Traverse the visual tree to find the DataGridCell
            while (depObj != null && !(depObj is DataGridCell))
            {
                depObj = VisualTreeHelper.GetParent(depObj);
            }

            if (depObj is DataGridCell cell)
            {
                // Check if the cell corresponds to the "Selected" column and the row is for Port 0
                if (cell.Column.Header.ToString() == "Selected" && cell.DataContext is LeakData leakData && leakData.Port == "0")
                {
                    // Prevent the checkbox from being unchecked
                    e.Handled = true;
                }
                if (cell.Column.Header.ToString() == "Selected" && IsReadOnly)
                {
                    // Prevent the checkbox from being modified
                    e.Handled = true;
                }
            }
        }

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e.Column.Header.ToString() == "Leak Definition (ppm)" || e.Column.Header.ToString() == "Certified Conc (ppm)" || e.Column.Header.ToString() == "Calibration Tolerance (%)" || e.Column.Header.ToString() == "Measured Concentration (ppm)")
            {
                if (e.Row.Item is LeakData item && item.Port == "0")
                {
                    e.Cancel = true;
                }
            }
        }
        private void DataGrid_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Validate input to allow only numeric values and a single decimal point
            TextBox textBox = sender as TextBox;
            if (!string.IsNullOrEmpty(e.Text) && !char.IsDigit(e.Text[0]) && e.Text[0] != '.')
            {
                e.Handled = true;
            }
            else if (e.Text[0] == '.' && textBox.Text.Contains("."))
            {
                // Allowing only one decimal point
                e.Handled = true;
            }
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Validate input to allow only numeric values and a single decimal point
            if (!string.IsNullOrEmpty(e.Text) && !char.IsDigit(e.Text[0]) && e.Text[0] != '.')
            {
                e.Handled = true;
            }
            else if (e.Text[0] == '.' && ((TextBox)sender).Text.Contains("."))
            {
                // Allowing only one decimal point
                e.Handled = true;
            }
        }
        private async void StartCalibration(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            //QuitAppButton.Visibility = Visibility.Collapsed;
            CalibrationBackButton.Visibility = Visibility.Collapsed;
            source = new CancellationTokenSource();
            token = source.Token;
            foreach (LeakData leakData in LeakDataList)
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
            var highest = selectedItems[selectedItems.Count - 1];

            //Insert at the beginning of the list
            selectedItems.Insert(0, highest);

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
                // Refresh the UI to reflect the change
                RefreshColumn(8);

                //Simulate calibration dummy values

                Random random = new Random();
                double percent = random.Next(2, 4);
                int sign = random.Next(0, 2);

                if (sign == 0)
                {
                    double concentration = Convert.ToDouble(leakData.Concentration);
                    double newMeasuredConcentration = concentration + (percent / 100) * concentration;
                    leakData.MeasuredConcentration = ((int)Math.Round(newMeasuredConcentration)).ToString();

                }
                else
                {
                    double concentration = Convert.ToDouble(leakData.Concentration);
                    double newMeasuredConcentration = concentration - (percent / 100) * concentration;
                    leakData.MeasuredConcentration = ((int)Math.Round(newMeasuredConcentration)).ToString();
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
                }

                // Refresh the UI to reflect the changes
                RefreshColumn(8);

                // Wait for a brief moment before moving to the next item
                await Task.Delay(500);
            }
            SaveCalData();
            SaveDataToCsv();
            RefreshAll();

            CalibrateButton.Visibility = Visibility.Visible;
            CalibrationCancelButton.Visibility = Visibility.Collapsed;
            CalibrationBackButton.Visibility = Visibility.Visible;

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

        private void RefreshColumn(int columnIndex)
        {
            // Iterate through each row in the DataGrid
            foreach (var item in dataGrid.Items)
            {
                // Get the corresponding property of the item based on the column index
                var property = item.GetType().GetProperty(dataGrid.Columns[columnIndex].SortMemberPath);

                // Update the property value
                property?.SetValue(item, property.GetValue(item));
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

            LeakDataList[0].Status = "Reading Gas";
            RefreshColumn(8);
            await Task.Delay(3000);
            LeakDataList[0].Status = "Done";
            RefreshColumn(8);
            await Task.Delay(1000);
            LeakDataList[0].Status = "";
            RefreshColumn(8);
        }
        private void QuitApplication_Click(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

    }
}
