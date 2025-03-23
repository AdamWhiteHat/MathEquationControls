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
        private bool isBehaviorsAttached = false;
        private KeyInputSetValueBehavior keyInputBehavior;
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
            this.IsEnabledChanged += NumberBox_IsEnabledChanged;
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
            DetachInputBehaviors();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            controlBorder = GetTemplateChild(ElementBorder) as Border;
            controlTextBox = GetTemplateChild(ElementTextBox) as TextBox;
            controlTextBox.DataContext = this;

            if (IsEnabled)
            {
                AttachInputBehaviors();
            }
        }

        private void NumberBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == e.NewValue) { return; }

            bool isEnabled = (bool)e.NewValue;
            if (isEnabled)
            {
                AttachInputBehaviors();
            }
            else
            {
                DetachInputBehaviors();
            }
        }

        private void AttachInputBehaviors()
        {
            if (!isBehaviorsAttached)
            {
                isBehaviorsAttached = true;

                mouseWheelBehavior = new MouseWheelAdjustRangeValueBehavior(ValueProperty);
                Interaction.GetBehaviors(this).Add(mouseWheelBehavior);

                mouseDragBehavior = new DragUpDownAdjustValueBehavior(ValueProperty);
                Interaction.GetBehaviors(this).Add(mouseDragBehavior);

                if (controlTextBox != null)
                {
                    keyInputBehavior = new KeyInputSetValueBehavior(controlTextBox, ValueProperty);
                    Interaction.GetBehaviors(this).Add(keyInputBehavior);
                }
            }
        }

        private void DetachInputBehaviors()
        {
            if (isBehaviorsAttached)
            {
                if (mouseWheelBehavior != null)
                {
                    Interaction.GetBehaviors(this).Remove(mouseWheelBehavior);
                    mouseWheelBehavior.Detach();
                    mouseWheelBehavior = null;
                }

                if (mouseDragBehavior != null)
                {
                    Interaction.GetBehaviors(this).Remove(mouseDragBehavior);
                    mouseDragBehavior.Detach();
                    mouseDragBehavior = null;
                }

                if (controlTextBox != null && keyInputBehavior != null)
                {
                    Interaction.GetBehaviors(this).Remove(keyInputBehavior);
                    keyInputBehavior.Detach();
                    keyInputBehavior = null;
                }
                isBehaviorsAttached = false;
            }
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
