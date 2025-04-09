using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MathEquationControls.Converters
{
    public class AbsoluteBigIntegerToStringConverter : IValueConverter
    {
        // This converts the BigInteger object to the string to display.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(BigInteger))
            {
                //throw new Exception($"Could not convert {typeof(BigInteger)} to string.");
                //return DependencyProperty.UnsetValue;
                return string.Empty;
            }
            return BigInteger.Abs((BigInteger)value).ToString();
        }

        // This converts the string into a BigInteger object to store.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BigInteger result;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()) || !BigInteger.TryParse(value.ToString(), out result))
            {
                //throw new Exception($"Could not convert string to {typeof(BigInteger)}.");
                //return DependencyProperty.UnsetValue;
                return BigInteger.MinusOne; // Zero not safe, in case of division. Return -1 instead.
            }
            return result;
        }
    }
}
