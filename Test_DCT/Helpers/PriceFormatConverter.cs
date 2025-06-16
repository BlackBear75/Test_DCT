using System.Globalization;
using System.Windows.Data;

namespace Test_DCT.Helpers;

    public class PriceFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal price)
            {
                if (price >= 1m)
                    return price.ToString("N2");
                else if (price >= 0.01m)
                    return price.ToString("N4");
                else
                    return price.ToString("N6");
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

