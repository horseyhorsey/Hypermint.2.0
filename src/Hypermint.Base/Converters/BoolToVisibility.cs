using System;
using System.Globalization;
using System.Windows.Data;

namespace Hypermint.Base.Converters
{
    public class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visible = System.Windows.Visibility.Visible;

            try
            {
                var parseBool =
                    bool.Parse(value.ToString());
                if (parseBool)
                    visible = System.Windows.Visibility.Collapsed;
                else
                    visible = System.Windows.Visibility.Visible;
            }
            catch (Exception) { }

            return visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
