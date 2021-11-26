using ExtendedArithmetic;
using MathEquationControl.Behaviors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MathEquationControl
{
	/// <summary>
	/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
	///
	/// Step 1a) Using this custom control in a XAML file that exists in the current project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:MathEquationControl.CustomControls"
	///
	///
	/// Step 1b) Using this custom control in a XAML file that exists in a different project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:MathEquationControl.CustomControls;assembly=MathEquationControl.CustomControls"
	///
	/// You will also need to add a project reference from the project where the XAML file lives
	/// to this project and Rebuild to avoid compilation errors:
	///
	///     Right click on the target project in the Solution Explorer and
	///     "Add Reference"->"Projects"->[Browse to and select this project]
	///
	///
	/// Step 2)
	/// Go ahead and use your control in the XAML file.
	///
	///     <MyNamespace:NumberBox/>
	///
	/// </summary>
	[TemplatePart(Name = NumberBox.ElementBorder, Type = typeof(Border))]
	[TemplatePart(Name = NumberBox.ElementTextBox, Type = typeof(TextBox))]
	public class NumberBox : Control, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public BigInteger Value
		{
			get => (BigInteger)GetValue(ValueProperty);
			set
			{
				SetValue(ValueProperty, value);
				RaisePropertyChanged(nameof(Value));
			}
		}

		public string Text
		{
			get { return controlTextBox.Text; }
			set
			{
				if (controlTextBox.Text != value)
				{
					controlTextBox.Text = value;
				}
			}
		}

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
																				nameof(Value),
																				typeof(BigInteger),
																				typeof(NumberBox),
																				new PropertyMetadata(
																					default(BigInteger),
																					new PropertyChangedCallback(NumberBox.OnValueChanged)
																				)
																	   );

		public event RoutedPropertyChangedEventHandler<BigInteger> ValueChanged
		{
			add { base.AddHandler(ValueChangedEvent, value); }
			remove { base.RemoveHandler(ValueChangedEvent, value); }
		}

		public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
																			nameof(ValueChanged),
																			RoutingStrategy.Bubble,
																			typeof(RoutedPropertyChangedEventHandler<BigInteger>),
																			typeof(NumberBox));

		private const string ElementTextBox = "PART_TextBox";
		private const string ElementBorder = "PART_Border";
		private TextBox controlTextBox;
		private Border controlBorder;
		private MouseWheelAdjustValueBehavior behavior;

		static NumberBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberBox), new FrameworkPropertyMetadata(typeof(NumberBox)));
		}

		public NumberBox()
		{
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			controlTextBox = GetTemplateChild(ElementTextBox) as TextBox;
			controlBorder = GetTemplateChild(ElementBorder) as Border;

			controlTextBox.DataContext = this;

			ValueChanged += NumberBox_ValueChanged;

			behavior = new MouseWheelAdjustValueBehavior(ValueProperty);
			Interaction.GetBehaviors(this).Add(behavior);
		}

		private void NumberBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger> e)
		{
			Text = Value.ToString();
			Size measuredStringSize = WPFHelper.MeasureString($" {Text} ", this, controlTextBox);
			controlTextBox.Width = measuredStringSize.Width;
		}

		protected virtual void OnValueChanged(BigInteger oldValue, BigInteger newValue)
		{
			if (oldValue.ToString() != newValue.ToString())
			{
				RoutedPropertyChangedEventArgs<BigInteger> e = new RoutedPropertyChangedEventArgs<BigInteger>(oldValue, newValue);
				e.RoutedEvent = ValueChangedEvent;
				base.RaiseEvent(e);
			}
		}

		private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			NumberBox element = (NumberBox)d;
			element.OnValueChanged((BigInteger)e.OldValue, (BigInteger)e.NewValue);
		}

		protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
