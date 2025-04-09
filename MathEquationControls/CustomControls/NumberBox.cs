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
using MathEquationControls.Converters;

namespace MathEquationControls
{
    [TemplatePart(Name = NumberBox.ElementBorder, Type = typeof(Border))]
    [TemplatePart(Name = NumberBox.ElementTextBox, Type = typeof(TextBox))]
    public class NumberBox : BigRangeBase, INotifyPropertyChanged
    {
        [Bindable(true), Browsable(true), Category("Common")]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
                                                                                    nameof(Text),
                                                                                    typeof(string),
                                                                                    typeof(NumberBox),
                                                                                    new PropertyMetadata(
                                                                                        default(string),
                                                                                        new PropertyChangedCallback(
                                                                                            NumberBox.RaiseTextChanged)
                                                                                        )
                                                                                    );

        public event RoutedPropertyChangedEventHandler<string> TextChanged
        {
            add { base.AddHandler(TextChangedEvent, value); }
            remove { base.RemoveHandler(TextChangedEvent, value); }
        }

        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
                                                                                    nameof(TextChanged),
                                                                                    RoutingStrategy.Bubble,
                                                                                    typeof(RoutedPropertyChangedEventHandler<string>),
                                                                                    typeof(NumberBox));

        protected virtual void RaiseTextChanged(string oldValue, string newValue)
        {
            RoutedPropertyChangedEventArgs<string> e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue);
            e.RoutedEvent = TextChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox element = (NumberBox)d;
            element.RaiseTextChanged((string)e.OldValue, (string)e.NewValue);
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
            this.IsEnabledChanged += NumberBox_IsEnabledChanged;
        }

        private void Control_Unloaded(object sender, RoutedEventArgs e)
        {
            UnRegisterEvents();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            controlBorder = GetTemplateChild(ElementBorder) as Border;
            controlTextBox = GetTemplateChild(ElementTextBox) as TextBox;
            controlTextBox.DataContext = this;

            if (controlTextBox != null)
            {
                RegisterEvents();
            }

            if (IsEnabled)
            {
                AttachInputBehaviors();
            }

            if (controlTextBox != null)
            {
                if (!string.IsNullOrWhiteSpace(Text))
                {
                    controlTextBox.Text = Text;
                }
            }

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                //controlTextBox.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 255));
            }
        }

        private bool EventsRegistered = false;
        private void RegisterEvents()
        {
            if (!EventsRegistered)
            {
                EventsRegistered = true;

                ValueChanged += Control_ValueChanged;
                TextChanged += NumberBox_TextChanged;
            }
        }

        private void UnRegisterEvents()
        {
            ValueChanged -= Control_ValueChanged;
            TextChanged -= NumberBox_TextChanged;
            DetachInputBehaviors();
        }

        private void NumberBox_TextChanged(object sender, RoutedPropertyChangedEventArgs<string> e)
        {
            if (Value.ToString() != e.NewValue)
            {
                if (BigInteger.TryParse(e.NewValue, out BigInteger result))
                {
                    Value = result;
                }
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

            if (text != Text)
            {
                Text = text;
            }
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
