using System;
using System.Windows.Data;

namespace AlbumStore.Helpers
{
    public class BooleanToAddEditConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool isAddMode && parameter is string param)
            {
                string[] parts = param.Split('|');
                return isAddMode ? parts[0] : parts[1];
            }
            return "Альбом";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
