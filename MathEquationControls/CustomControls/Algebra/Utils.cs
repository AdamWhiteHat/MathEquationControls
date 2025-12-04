using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathEquationControls.CustomControls.Algebra
{
    public static class Utils
    {
        public static char[] OperationSymbols = new char[] { '+','-','*','/' };

        public static void SetOperand<T>(T target, DependencyProperty property, string text) where T : DependencyObject
        {
            if (text.Any(c => OperationSymbols.Contains(c)))
            {
                BinaryOperation bin = new BinaryOperation();
                bin.Text = text;
                target.SetValue(property, bin);
            }
            else if (text.Any(c => char.IsNumber(c)))
            {
                Number num = new Number(text);
                target.SetValue(property, num);
            }
            else if (text.Any(c => char.IsAsciiLetter(c)))
            {
                if (text.Length != 1)
                {
                    throw new FormatException();
                }
                Variable vari = new Variable(text);
                target.SetValue(property, vari);
            }
            else
            {
                target.SetValue(property, Expression.Empty);
            }
        }
    }
}
