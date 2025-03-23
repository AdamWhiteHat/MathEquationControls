using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MathEquationControls.ValueConverters
{
    public class SignToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(Int32))
            {
                return "";
            }
            Int32 sign = (Int32)value;

            if (sign == -1)
            {
                return " - ";
            }
            else if (sign == 1)
            {
                return " + ";
            }
            else if ((sign == 0))
            {
                return " + ";
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
