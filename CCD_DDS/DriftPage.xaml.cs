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

        private void ToggleButtonVisibility(bool IsReadOnly)
        {
            // Toggle visibility of navigation buttons
            DriftBackButton.Visibility = IsReadOnly ? Visibility.Visible : Visibility.Collapsed;

            // Toggle visibility of Save and Cancel buttons
            SaveButton.Visibility = IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
            CancelButton.Visibility = IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            clickSoundPlayer.Play();
            IsReadOnly = !IsReadOnly;
            DriftEditButton.Visibility = Visibility.Collapsed;
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
            DriftEditButton.Visibility = Visibility.Visible;
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
            DriftEditButton.Visibility = Visibility.Visible;
        }
        private void SaveDataToCsv()
        {
            string csvFilePath = "TableData.csv";

            try
            {
                using (StreamWriter writer = new StreamWriter(csvFilePath, false))
                {
                    // Write header row
                    writer.WriteLine("Port,Leak Definition (ppm),Concentration (ppm),Tank Capacity,Expiry Date,Lot Number,Measured Concentration (ppm),Calibration Tolerance (%),Selected,Status,Estimated Tank Level (%)");

                    // Write data rows
                    foreach (LeakData leakData in LeakDataList)
                    {
                        //Selected value is boolean and needs special handling
                        string isSelected = leakData.IsSelected ? "True" : "False";

                        string expiryDate = leakData.ExpiryDate.HasValue ? leakData.ExpiryDate.Value.ToString("MM/dd/yyyy") : "";
                        writer.WriteLine($"{leakData.Port},{leakData.LeakDefinition},{leakData.Concentration},{leakData.TankCapacity}," +
                            $"{expiryDate},{leakData.LotNumber},{leakData.MeasuredConcentration},{leakData.Tolerance},{isSelected},{leakData.Status},{leakData.TankLevel}");
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
            dataGridDrift.ItemsSource = LeakDataList;
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
