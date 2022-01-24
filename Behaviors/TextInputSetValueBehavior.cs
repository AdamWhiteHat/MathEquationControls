using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Input;
using System.Numerics;
using System.Windows.Documents;
using System.Windows.Controls;

namespace MathEquationControl.Behaviors
{
	public class TextInputSetValueBehavior : Behavior<UIElement>
	{
		private DependencyProperty _valueProperty;

		public TextInputSetValueBehavior(DependencyProperty valueProperty)
		{
			_valueProperty = valueProperty;
		}

		protected override void OnAttached()
		{
			AssociatedObject.KeyDown += AssociatedObject_KeyDown;
			base.OnAttached();
		}

		protected override void OnDetaching()
		{
			AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
			base.OnDetaching();
		}

		protected void SetValue(string value)
		{
			BigInteger numericValue;
			if (BigInteger.TryParse(value, out numericValue))
			{
				AssociatedObject.SetCurrentValue(_valueProperty, numericValue);
			}
		}

		private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				RichTextBox richTextBox = AssociatedObject as RichTextBox;
				if (richTextBox != null)
				{
					string plainText = WPFHelper.ExtractPlainTextContent(richTextBox);
					SetValue(plainText);
				}

				TextBlock textBlock = AssociatedObject as TextBlock;
				if (textBlock != null)
				{
					string text = textBlock.Text;
					SetValue(text);
				}

				e.Handled = true;
			}
		}
	}
}
