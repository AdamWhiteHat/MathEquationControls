using ExtendedArithmetic;
using MathEquationControls.Behaviors;
using MathEquationControls.Primitives;
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
using Microsoft.Xaml.Behaviors;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MathEquationControls.ValueConverters;

namespace MathEquationControls
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
    ///     <MyNamespace:Coefficient/>
    ///
    /// </summary>
    [TemplatePart(Name = NumberBox.ElementBorder, Type = typeof(Border))]
    [TemplatePart(Name = NumberBox.ElementTextBox, Type = typeof(TextBox))]
    public class NumberBox : BigRangeBase, INotifyPropertyChanged
    {
        public string Text
        {
            get => controlTextBox.Text;
            set
            {
                if (controlTextBox.Text != value)
                {
                    controlTextBox.Text = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected const string ElementBorder = "PART_Border";
        protected const string ElementTextBox = "PART_TextBox";

        private Border controlBorder;
        private TextBox controlTextBox;
        private MouseWheelAdjustRangeValueBehavior mouseWheelBehavior;
        private DragUpDownAdjustValueBehavior mouseDragBehavior;

        static NumberBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberBox), new FrameworkPropertyMetadata(typeof(NumberBox)));
        }

        public NumberBox()
        {
            this.Unloaded += Control_Unloaded;
            this.Loaded += Control_Loaded;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            // controlTextBox.TextChanged += ControlTextBox_TextChanged;
            // controlTextBox.KeyUp += ControlTextBox_KeyUp;
            ValueChanged += Control_ValueChanged;
        }

        private void Control_Unloaded(object sender, RoutedEventArgs e)
        {
            // controlTextBox.TextChanged -= ControlTextBox_TextChanged;
            // controlTextBox.KeyUp -= ControlTextBox_KeyUp;
            ValueChanged -= Control_ValueChanged;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            controlBorder = GetTemplateChild(ElementBorder) as Border;
            controlTextBox = GetTemplateChild(ElementTextBox) as TextBox;

            controlTextBox.DataContext = this;

            mouseWheelBehavior = new MouseWheelAdjustRangeValueBehavior(ValueProperty);
            Interaction.GetBehaviors(this).Add(mouseWheelBehavior);

            mouseDragBehavior = new DragUpDownAdjustValueBehavior(ValueProperty);
            Interaction.GetBehaviors(this).Add(mouseDragBehavior);

        }


        private void Control_ValueChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger> e)
        {
            string text = e.NewValue.ToString();
            Size measuredStringSize = WPFHelper.MeasureString($" {text} ", this, controlTextBox);
            controlTextBox.Width = measuredStringSize.Width;
        }

        private void ControlTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
            {
                return;
            }

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void ControlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
            {
                return;
            }

            if (textBox.Text != Value.ToString())
            {
                BigInteger temp = new BigInteger();
                if (BigInteger.TryParse(textBox.Text, out temp))
                {
                    this.SetValue(ValueProperty, temp);
                }

            }
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
