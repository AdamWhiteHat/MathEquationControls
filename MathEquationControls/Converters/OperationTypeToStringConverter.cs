using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ExtendedArithmetic;
using MathEquationControls.CustomControls.Algebra;

namespace MathEquationControls.Converters
{
    public class OperationTypeToStringConverter : IValueConverter
    {
        // This converts the OperationType object to the string to display.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(OperationType))
            {
                //return DependencyProperty.UnsetValue;
                return string.Empty;
            }

            OperationType operationType = (OperationType)value;
            return OperationTypeHelper.OperationType2SymbolDictionary[operationType];
        }

        // This converts the string into an OperationType object to store.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            OperationType result;
            if (value == null || value.GetType() != typeof(string) || !OperationType.TryParse(value.ToString(), out result))
            {
                return DependencyProperty.UnsetValue;
            }
            return result;
        }
    }
}
