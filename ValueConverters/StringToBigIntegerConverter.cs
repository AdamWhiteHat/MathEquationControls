using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MathEquationControl.ValueConverters
{
	public class StringToBigIntegerConverter : IValueConverter
	{
		// This converts the BigInteger object to the string to display.
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || value.GetType() != typeof(BigInteger))
			{
				return DependencyProperty.UnsetValue;
			}
			return value.ToString();
		}

		// This converts the string into a BigInteger object to store.
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			BigInteger result;
			if (value == null || !BigInteger.TryParse(value.ToString(), out result))
			{
				return DependencyProperty.UnsetValue;
			}
			return result;
		}
	}
}
