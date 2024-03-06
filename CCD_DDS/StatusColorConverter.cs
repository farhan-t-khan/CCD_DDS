using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CCD_DDS
{
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Assuming value is a string representing the status
            string status = value as string;

            // Set default color
            Brush defaultColor = Brushes.Black;

            // Define colors for different status values
            switch (status)
            {
                case "Reading Gas":
                    return Brushes.Red;
                case "Calibrating...":
                    return Brushes.Blue; // Set color for Calibrating...
                case "Done":
                    return Brushes.Green; // Set color for Done
                // Add more cases for other status values as needed
                default:
                    return defaultColor;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
