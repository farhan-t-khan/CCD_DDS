using System;
using System.Globalization;
using System.Windows.Data;

namespace CCD_DDS
{
    public class LeakDefinitionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Implement your conversion logic here
            // Convert the value to the desired format
            return value.ToString(); // Example: return value.ToString().ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Implement conversion back if needed
            throw new NotImplementedException();
        }
    }
}
