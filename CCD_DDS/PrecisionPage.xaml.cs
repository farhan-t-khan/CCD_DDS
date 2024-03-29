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
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Windows.Threading;

namespace CCD_DDS
{
    /// <summary>
    /// Interaction logic for PrecisionPage.xaml
    /// </summary>
    public partial class PrecisionPage : Page, INotifyPropertyChanged
    {
        private SoundPlayer clickSoundPlayer;
        public List<string> LeakDefinitionOptions { get; set; }
        public List<LeakData> LeakDataList { get; set; }
        public List<LeakData> PrecisionList { get; set; }
        private CancellationTokenSource source;
        private CancellationToken token;
        private bool _isReadOnly = true;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public PrecisionPage()
        {
            clickSoundPlayer = new SoundPlayer("Resource\\click.wav");
            InitializeComponent();
            LeakDefinitionOptions = new List<string> { "100", "200", "500", "1000", "2000", "5000", "10000", "25000" };
            DataContext = this;
            IsReadOnly = true;
            LoadDataFromCsv();
            LoadPrecisionData();
            // Start a timer to update the clock every second
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            // Set the initial clock time and date
            ClockTextBlock.Text = DateTime.Now.ToString("HH:mm:ss");
            DateTextBlock.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update the clock every second
            ClockTextBlock.Text = DateTime.Now.ToString("HH:mm:ss");

            // Update the date
            DateTextBlock.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
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
        private void LoadPrecisionData()
        {
           
            PrecisionList = new List<LeakData>();
            foreach (LeakData leakData in LeakDataList)
            {
                if (leakData.IsSelected == true && Convert.ToInt64(leakData.Port) != 0)
                {
                    PrecisionList.Add(leakData);
                }
            }
        }
        private void SavePrecisionData()
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
            IsReadOnly = true;
            PrecisionStartButton.Visibility = Visibility.Visible;
            PrecisionBackButton.Visibility = Visibility.Visible;
            PrecisionCancelButton.Visibility = Visibility.Collapsed;
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
            dataGridPrecision.ItemsSource = null;
            LoadDataFromCsv();
            dataGridPrecision.ItemsSource = PrecisionList;
        }


        private void ResetUI()
        {

        }

        private void RefreshColumn(int columnIndex)
        {
            if (columnIndex >= 0 && columnIndex < dataGridPrecision.Columns.Count)
            {
                // Iterate through each row in the DataGrid
                foreach (var item in dataGridPrecision.Items)
                {
                    // Get the corresponding property of the item based on the column index
                    var sortMemberPath = dataGridPrecision.Columns[columnIndex].SortMemberPath;
                    var property = item.GetType().GetProperty(sortMemberPath);

                    if (property != null)
                    {
                        // Update the property value
                        property.SetValue(item, property.GetValue(item));
                    }
                }
            }
        }

        private async void PrecisionCheck(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            //QuitAppButton.Visibility = Visibility.Collapsed;
            source = new CancellationTokenSource();
            token = source.Token;
            foreach (LeakData leakData in PrecisionList)
            {
                leakData.PrecisionDate = "";
                leakData.PrecisionTime = "";
                leakData.Measurement1 = "";
                leakData.Measurement2 = "";
                leakData.Measurement3 = "";
                leakData.Precision = "";
            }
            RefreshColumn(3);
            RefreshColumn(4);
            RefreshColumn(5);
            RefreshColumn(6);
            RefreshColumn(7);
            RefreshColumn(8);
            // Hide the other buttons and show the cancel button
            PrecisionStartButton.Visibility = Visibility.Collapsed;
            PrecisionBackButton.Visibility = Visibility.Collapsed;
            PrecisionCancelButton.Visibility = Visibility.Visible;

            var selectedItems = PrecisionList.Where(item => item.Port != "0").ToList();


            foreach (LeakData leakData in selectedItems)
            {
                leakData.PrecisionDate = DateTime.Now.ToString("MM/dd/yyyy");
                leakData.PrecisionTime = DateTime.Now.ToString("HH:mm:ss");
                for (int i = 0; i < 3; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        // Reset UI and exit the method
                        ResetUI();
                        return;
                    }

                    // Update the status to "Reading"
                    var property = typeof(LeakData).GetProperty($"Measurement{i + 1}");
                    if (property != null)
                    {
                        // Update the status to "Reading"
                        property.SetValue(leakData, "Reading...");
                    }
                    // Refresh the UI to reflect the change
                    RefreshColumn(5);
                    // Wait for a brief moment to simulate the reading process
                    await Task.Delay(3000);


                    //Simulate precision dummy values

                    Random random = new Random();
                    double percent = random.Next(2, 4);
                    int sign = random.Next(0, 2);

                    double concentration = Convert.ToDouble(leakData.Concentration);

                    if (sign == 0)
                    { 
                        double newMeasuredConcentration = concentration + (percent / 100) * concentration;
                        if (property != null && property.PropertyType == typeof(string))
                        {
                            property.SetValue(leakData, ((int)Math.Round(newMeasuredConcentration)).ToString());
                        }

                    }
                    else
                    {
                        double newMeasuredConcentration = concentration - (percent / 100) * concentration;
                        if (property != null && property.PropertyType == typeof(string))
                        {
                            property.SetValue(leakData, ((int)Math.Round(newMeasuredConcentration)).ToString());
                        }
                    }

                    // Calculate and update Precision property

                    
                    // Wait for a brief moment
                    await Task.Delay(2000);


                    // Refresh the UI to reflect the changes
                    RefreshColumn(5);
                    RefreshColumn(6);
                    RefreshColumn(7);

                    // Wait for a brief moment before moving to the next item
                    await Task.Delay(500);
                }
                double precision = CalculatePrecision(leakData);
                leakData.Precision = precision.ToString("0.00");
                RefreshColumn(8);
            }

            RefreshAll();
            SaveDataToCsv();
            PrecisionStartButton.Visibility = Visibility.Visible;
            PrecisionBackButton.Visibility = Visibility.Visible;
            PrecisionCancelButton.Visibility = Visibility.Collapsed;

        }
        private double CalculatePrecision(LeakData leakData)
        {
            double meas1 = double.Parse(leakData.Measurement1);
            double meas2 = double.Parse(leakData.Measurement2);
            double meas3 = double.Parse(leakData.Measurement3);
            double concentration = double.Parse(leakData.Concentration);

            double averageDifference = (Math.Abs(meas1 - concentration) + Math.Abs(meas2 - concentration) + Math.Abs(meas3 - concentration)) / 3;
            double precision = averageDifference / concentration * 100;

            return precision;
        }

        private void RefreshAll()
        {
            // This method forces the DataGrid to refresh its items, ensuring the UI reflects the changes immediately
            dataGridPrecision.Items.Refresh();
        }

    }
}
