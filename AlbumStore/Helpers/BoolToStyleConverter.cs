using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AlbumStore.Helpers
{
    public class BoolToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isActive = (bool)value;
            string styleType = parameter as string;

            if (styleType == "Active")
            {
                return isActive
                    ? Application.Current.FindResource("SelectedMenuButtonStyle")
                    : Application.Current.FindResource("MenuButtonStyle");
            }

            return Application.Current.FindResource("MenuButtonStyle");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
