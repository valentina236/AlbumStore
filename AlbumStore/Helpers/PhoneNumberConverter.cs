using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AlbumStore.Helpers
{
    public class PhoneNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string phoneNumber && !string.IsNullOrWhiteSpace(phoneNumber))
            {
                string digits = new string(phoneNumber.Where(char.IsDigit).ToArray());

                if (digits.Length >= 11 && digits.StartsWith("373"))
                {
                    string localNumber = digits.Substring(3);
                    if (localNumber.Length == 8)
                    {
                        return $"+373 {localNumber.Substring(0, 2)} {localNumber.Substring(2, 3)} {localNumber.Substring(5, 3)}";
                    }
                }

                return phoneNumber;
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value is string formattedNumber)
            {
                return new string(formattedNumber.Where(char.IsDigit).ToArray());
            }
            return value;
        }
    }
}
