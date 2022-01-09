using ExtendedArithmetic;
using MathEquationControl.Behaviors;
using MathEquationControl.Primitives;
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
using System.Windows.Controls.Primitives;
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
	public class NumberBox : BigRangeBase, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private const string ElementTextBox = "PART_TextBox";
		private const string ElementBorder = "PART_Border";
		private TextBox controlTextBox;
		private Border controlBorder;
		private MouseWheelAdjustRangeValueBehavior behavior;

		static NumberBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberBox), new FrameworkPropertyMetadata(typeof(NumberBox)));
		}

		public NumberBox()
		{
			this.Minimum = null;
			this.Maximum = null;
			this.UnitaryChange = 1;
			this.SmallChange = 10;
			this.MediumChange = 100;
			this.LargeChange = 1000;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			controlTextBox = GetTemplateChild(ElementTextBox) as TextBox;
			controlBorder = GetTemplateChild(ElementBorder) as Border;

			controlTextBox.DataContext = this;

			ValueChanged += NumberBox_ValueChanged; ;

			behavior = new MouseWheelAdjustRangeValueBehavior(ValueProperty);
			Interaction.GetBehaviors(this).Add(behavior);
		}

		private void NumberBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger> e)
		{
			string text = e.NewValue.ToString();
			Size measuredStringSize = WPFHelper.MeasureString($" {text} ", this, controlTextBox);
			controlTextBox.Width = measuredStringSize.Width;
		}

		protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
