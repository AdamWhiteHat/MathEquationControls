using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Input;
using System.Numerics;

namespace MathEquationControl.Behaviors
{
	public class MouseWheelAdjustValueBehavior : Behavior<UIElement>
	{
		private DependencyProperty _valueProperty;

		public MouseWheelAdjustValueBehavior(DependencyProperty valueProperty)
		{
			_valueProperty = valueProperty;
		}

		protected override void OnAttached()
		{
			AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
			AssociatedObject.KeyDown += AssociatedObject_KeyDown;
			base.OnAttached();
		}

		protected override void OnDetaching()
		{
			AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
			base.OnDetaching();
		}

		protected void AddValue(BigInteger value)
		{
			BigInteger currentValue = (BigInteger)AssociatedObject.GetValue(_valueProperty);
			BigInteger result = currentValue + value;
			AssociatedObject.SetValue(_valueProperty, result);
		}

		private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			BigInteger multiplier = 1;

			if (Keyboard.Modifiers == ModifierKeys.Control)
			{
				multiplier = 10;
			}
			else if (Keyboard.Modifiers == ModifierKeys.Shift)
			{
				multiplier = 100;

			}

			AddValue(((BigInteger)(e.Delta / Mouse.MouseWheelDeltaForOneLine)) * multiplier);
		}

		private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Add || e.Key == Key.Up)
			{
				AddValue(1);
				e.Handled = true;
			}
			else if (e.Key == Key.Subtract || e.Key == Key.Down)
			{
				AddValue(-1);
				e.Handled = true;
			}
			else if (e.Key == Key.Enter || e.Key == Key.Down)
			{
				AddValue(-1);
				e.Handled = true;
			}
		}
	}
}
