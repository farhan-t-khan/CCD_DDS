using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CCD_DDS
{
    public class ExpiryDateBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int daysUntilExpiry)
            {
                if (daysUntilExpiry < 5)
                {
                    return Brushes.Red;
                }
                else if (daysUntilExpiry < 60)
                {
                    return Brushes.Yellow;
                }
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
