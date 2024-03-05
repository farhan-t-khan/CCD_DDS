﻿/*using System;
using System.Media;
using System.Collections.Generic;
using System.IO;
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
using System.ComponentModel;

namespace CCD_DDS
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class HomePage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private SoundPlayer clickSoundPlayer;
        public List<string> LeakDefinitionOptions { get; set; }
        public List<LeakData> LeakDataList { get; set; }

        public bool _isReadOnly = true;
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                _isReadOnly = value;
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }
        public HomePage()
        {
            InitializeComponent();
            clickSoundPlayer = new SoundPlayer("Resource\\click.wav");
            LeakDefinitionOptions = new List<string> { "100", "200", "500", "1000", "2000", "5000", "10000", "25000" };
            DataContext = this;
            IsReadOnly = true;
            // Load data from CSV
            LoadDataFromCsv();
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
                        IsSelected = false, // Assuming checkboxes are initially unchecked
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
        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            if(IsReadOnly)
            {
                ScreenNameTextBlock.Text = "Edit";
            } else
            {
                ScreenNameTextBlock.Text = "Home";
            }
            // Toggle edit mode
            IsReadOnly = !IsReadOnly;
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
    }
}
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CCD_DDS
{
    public partial class HomePage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private SoundPlayer clickSoundPlayer;
        public List<string> LeakDefinitionOptions { get; set; }
        public List<LeakData> LeakDataList { get; set; }

        private bool _isReadOnly = true;
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

        public HomePage()
        {
            InitializeComponent();
            clickSoundPlayer = new SoundPlayer("Resource\\click.wav");
            LeakDefinitionOptions = new List<string> { "100", "200", "500", "1000", "2000", "5000", "10000", "25000" };
            DataContext = this;
            IsReadOnly = true;
            // Load data from CSV
            LoadDataFromCsv();
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
                        IsSelected = Convert.ToBoolean(values[7]),
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

        private void ToggleButtonVisibility(bool IsReadOnly)
        {
            // Toggle visibility of navigation buttons
            CalibrateButton.Visibility = IsReadOnly ? Visibility.Visible : Visibility.Collapsed;
            DriftButton.Visibility = IsReadOnly ? Visibility.Visible : Visibility.Collapsed;
            PrecisionButton.Visibility = IsReadOnly ? Visibility.Visible : Visibility.Collapsed;

            // Toggle visibility of Save and Cancel buttons
            SaveButton.Visibility = IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
            CancelButton.Visibility = IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            IsReadOnly = !IsReadOnly;
            EditButton.Visibility = Visibility.Collapsed;
            ToggleButtonVisibility(IsReadOnly);
        }

        private async void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            // Perform saving logic here
            SaveDataToCsv();
            // Reload data to refresh the table contents
            RefreshDataGrid();
            // Toggle back to view mode
            IsReadOnly = true;
            ToggleButtonVisibility(IsReadOnly);
            EditButton.Visibility = Visibility.Visible;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            // Perform cancel logic here

            // Reload data to refresh the table contents
            RefreshDataGrid();
            // Toggle back to view mode
            IsReadOnly = true;
            ToggleButtonVisibility(IsReadOnly);
            EditButton.Visibility = Visibility.Visible;
        }
        private void SaveDataToCsv()
        {
            string csvFilePath = "TableData.csv";

            try
            {
                using (StreamWriter writer = new StreamWriter(csvFilePath, false))
                {
                    // Write header row
                    writer.WriteLine("Port,Leak Definition (ppm),Concentration (ppm),Tank Capacity,Expiry Date,Lot Number,Measured Concentration (ppm),Selected,Status");

                    // Write data rows
                    foreach (LeakData leakData in LeakDataList)
                    {
                        //Selected value is boolean and needs special handling
                        string isSelected = leakData.IsSelected ? "True" : "False";

                        string expiryDate = leakData.ExpiryDate.HasValue ? leakData.ExpiryDate.Value.ToString("MM/dd/yyyy") : "";
                        writer.WriteLine($"{leakData.Port},{leakData.LeakDefinition},{leakData.Concentration},{leakData.TankCapacity}," +
                            $"{expiryDate},{leakData.LotNumber},{leakData.MeasuredConcentration},{isSelected},{leakData.Status}");
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
            dataGrid.ItemsSource = LeakDataList;
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
            if (e.Column.Header.ToString() == "Leak Definition" || e.Column.Header.ToString() == "Concentration (ppm)" || e.Column.Header.ToString() == "Measured Concentration (ppm)")
            {
                if (e.Row.Item is LeakData item && item.Port == "0")
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
