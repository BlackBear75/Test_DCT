using System.Globalization;
using System.Windows.Data;

namespace Test_DCT.Helpers;

    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return int.Parse(parameter.ToString());

            return Binding.DoNothing;
        }
    }


