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
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace CCD_DDS
{
    /// <summary>
    /// Interaction logic for DriftPage.xaml
    /// </summary>
    public partial class DriftPage : Page, INotifyPropertyChanged
    {
        private SoundPlayer clickSoundPlayer;
        public List<string> LeakDefinitionOptions { get; set; }
        public List<LeakData> LeakDataList { get; set; }
        public List<LeakData> DriftDataList { get; set; }
        private CancellationTokenSource source;
        private CancellationToken token;
        private bool _isReadOnly = true;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public DriftPage()
        {
            clickSoundPlayer = new SoundPlayer("Resource\\click.wav");
            InitializeComponent();
            LeakDefinitionOptions = new List<string> { "100", "200", "500", "1000", "2000", "5000", "10000", "25000" };
            DataContext = this;
            IsReadOnly = true;
            LoadDataFromCsv();
            LoadDriftDataFromCsv();
        }
        public void NavigateToHome(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            NavigationService.Navigate(new HomePage());
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
        private void LoadDriftDataFromCsv()
        {
            string csvFilePath = "TableData.csv";
            DriftDataList = new List<LeakData>();
            foreach (LeakData leakData in LeakDataList)
            {
                if (leakData.DriftIsSelected == true && Convert.ToInt64(leakData.Port) != 0)
                {
                    DriftDataList.Add(leakData);
                }
            }
        }
        private void SaveDriftData()
        {
            // Generate file name based on current date and time
            string dateTimeString = DateTime.Now.ToString("MMddyyyy_HHmmss");
            string csvFilePath = $"CalRecord_{dateTimeString}.csv";
            try
            {
                using (StreamWriter writer = new StreamWriter(csvFilePath, false))
                {
                    // Write header row
                    writer.WriteLine("Port,Leak Definition (ppm),Concentration (ppm),Tank Capacity,Expiry Date,Lot Number,Measured Concentration (ppm)");

                    // Write data rows
                    foreach (LeakData leakData in LeakDataList)
                    {
                        if (leakData.IsSelected)
                        {
                            string expiryDate = leakData.ExpiryDate.HasValue ? leakData.ExpiryDate.Value.ToString("MM/dd/yyyy") : "";
                            writer.WriteLine($"{leakData.Port},{leakData.LeakDefinition},{leakData.Concentration},{leakData.TankCapacity}," +
                                $"{expiryDate},{leakData.LotNumber},{leakData.MeasuredConcentration}");

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data to CSV: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void DriftCheck(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            //QuitAppButton.Visibility = Visibility.Collapsed;
            source = new CancellationTokenSource();
            token = source.Token;
            foreach (LeakData leakData in DriftDataList)
            {
                leakData.DriftDate2 = "";
                leakData.DriftTime2 = "";
                leakData.DriftConcentration2 = "";
                leakData.DriftPercentage = "";
            }
            RefreshColumn(3);
            RefreshColumn(4);
            RefreshColumn(5);
            RefreshColumn(6);
            RefreshColumn(7);
            RefreshColumn(8);
            RefreshColumn(9);
            // Hide the other buttons and show the cancel button
            DriftStartButton.Visibility = Visibility.Collapsed;
            DriftBackButton.Visibility = Visibility.Collapsed;
            DriftCancelButton.Visibility = Visibility.Visible;

            var selectedItems = DriftDataList.Where(item => item.Port != "0").ToList();


            foreach (LeakData leakData in selectedItems)
            {

                if (token.IsCancellationRequested)
                {
                    // Reset UI and exit the method
                    ResetUI();
                    return;
                }

                if (leakData.DriftConcentration1 == null)
                {
                    leakData.DriftPercentage = "Calibrate First";
                    continue;
                }

                //Set DriftDate2 and DriftTime2 to Current time
                leakData.DriftDate2 = DateTime.Now.ToString("MM/dd/yyyy");
                leakData.DriftTime2 = DateTime.Now.ToString("HH:mm:ss");

                // Update the status to "Reading"
                leakData.DriftConcentration2 = "Reading";
                // Refresh the UI to reflect the change
                RefreshColumn(6);
                RefreshColumn(7);
                RefreshColumn(8);
                // Wait for a brief moment to simulate the reading process
                await Task.Delay(3000);

                //Simulate Measured now Concentration
                Random random = new Random();
                double percent = random.Next(0, 5);
                int sign = random.Next(0, 2);

                if (sign == 0)
                {
                    double concentration = Convert.ToDouble(leakData.Concentration);
                    double newMeasuredConcentration = concentration + (percent / 100) * concentration;
                    leakData.DriftConcentration2 = ((int)Math.Round(newMeasuredConcentration)).ToString();
                    

                }
                else
                {
                    double concentration = Convert.ToDouble(leakData.Concentration);
                    double newMeasuredConcentration = concentration - (percent / 100) * concentration;
                    leakData.DriftConcentration2 = ((int)Math.Round(newMeasuredConcentration)).ToString();
                    
                }
                RefreshColumn(8);


             
                //Calculate Drift

                double measured = double.Parse(leakData.DriftConcentration2);
                double calibrated = double.Parse(leakData.DriftConcentration1);
                double drift = ((measured - calibrated) / calibrated) * 100;
                leakData.DriftPercentage = drift.ToString("0.00");


                // Refresh the UI to reflect the changes
                await Task.Delay(1000);
                RefreshColumn(9);

                // Wait for a brief moment before moving to the next item
                await Task.Delay(500);
            }

            RefreshAll();
            SaveDataToCsv();
            DriftStartButton.Visibility = Visibility.Visible;
            DriftBackButton.Visibility = Visibility.Visible;
            DriftCancelButton.Visibility = Visibility.Collapsed;

        }



        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            // Perform cancel logic here
            if (source != null)
            {
                source.Cancel();
            }
            // Reload data to refresh the table contents
            RefreshDataGrid();
            // Toggle back to view mode
            DriftStartButton.Visibility = Visibility.Visible;
            DriftBackButton.Visibility = Visibility.Visible;
            DriftCancelButton.Visibility = Visibility.Collapsed;

            //DriftEditButton.Visibility = Visibility.Visible;
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
            dataGridDrift.ItemsSource = null;
            LoadDataFromCsv();
            dataGridDrift.ItemsSource = DriftDataList;
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
                if (cell.Column.Header.ToString() == "Drift Selected" && cell.DataContext is LeakData leakData && leakData.Port == "0")
                {
                    // Prevent the checkbox from being unchecked
                    e.Handled = true;
                }
                if (cell.Column.Header.ToString() == "Drift Selected" && IsReadOnly)
                {
                    // Prevent the checkbox from being modified
                    e.Handled = true;
                }
            }
        }

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e.Column.Header.ToString() == "Leak Definition" || e.Column.Header.ToString() == "Concentration (ppm)" || e.Column.Header.ToString() == "Measured Concentration (ppm)")
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


        private void ResetUI()
        {

        }

        private void RefreshColumn(int columnIndex)
        {
            // Iterate through each row in the DataGrid
            foreach (var item in dataGridDrift.Items)
            {
                // Get the corresponding property of the item based on the column index
                var property = item.GetType().GetProperty(dataGridDrift.Columns[columnIndex].SortMemberPath);

                // Update the property value
                property?.SetValue(item, property.GetValue(item));
            }
        }


        private void RefreshAll()
        {
            // This method forces the DataGrid to refresh its items, ensuring the UI reflects the changes immediately
            dataGridDrift.Items.Refresh();
        }
    }
}
