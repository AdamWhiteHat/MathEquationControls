using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Numerics;
using System.Windows.Controls.Primitives;
using MathEquationControls.Primitives;
using Microsoft.Xaml.Behaviors;

namespace MathEquationControls.Behaviors
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
			AssociatedObject.SetCurrentValue(_valueProperty, result);
		}

		private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			BigInteger multiplier = 1;

			if (Keyboard.Modifiers == ModifierKeys.Shift)
			{
				multiplier = 10;
			}
			else if (Keyboard.Modifiers == ModifierKeys.Control)
			{
				multiplier = 100;
			}
			else if (Keyboard.Modifiers == ModifierKeys.Alt)
			{
				multiplier = 1000;
			}

			AddValue(((BigInteger)(e.Delta / Mouse.MouseWheelDeltaForOneLine)) * multiplier);

			UIElement dependencyObject = (UIElement)sender;
			DependencyObject focusScope = FocusManager.GetFocusScope(AssociatedObject);
			FocusManager.SetFocusedElement(focusScope, dependencyObject);
		}
	}
}
