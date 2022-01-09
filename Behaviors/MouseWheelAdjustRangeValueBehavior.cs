using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Input;
using System.Numerics;
using System.Windows.Controls.Primitives;
using MathEquationControl.Primitives;

namespace MathEquationControl.Behaviors
{
	public class MouseWheelAdjustRangeValueBehavior : Behavior<BigRangeBase>
	{
		private DependencyProperty _valueProperty;

		public MouseWheelAdjustRangeValueBehavior(DependencyProperty valueProperty)
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
			AssociatedObject.SetValue(_valueProperty, result);
		}

		private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			BigInteger multiplier = 1;

			if (Keyboard.Modifiers == ModifierKeys.Shift)
			{
				multiplier = AssociatedObject.SmallChange;
			}
			else if (Keyboard.Modifiers == ModifierKeys.Control)
			{
				multiplier = AssociatedObject.MediumChange;
			}
			else if (Keyboard.Modifiers == ModifierKeys.Alt)
			{
				multiplier = AssociatedObject.LargeChange;
			}

			AddValue(((BigInteger)(e.Delta / Mouse.MouseWheelDeltaForOneLine)) * multiplier);

			UIElement dependencyObject = (UIElement)sender;
			DependencyObject focusScope = FocusManager.GetFocusScope(AssociatedObject);
			FocusManager.SetFocusedElement(focusScope, dependencyObject);
		}
	}
}
