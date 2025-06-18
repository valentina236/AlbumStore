using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace AlbumStore.Helpers
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = (bool)value;
            bool isInverse = parameter?.ToString() == "Inverse";
            return (isVisible && !isInverse) || (!isVisible && isInverse) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
