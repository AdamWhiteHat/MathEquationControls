using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Xaml.Behaviors;
using Microsoft.Xaml.Behaviors.Core;

namespace MathEquationControls.CustomControls.Algebra
{
    /// <summary>
    /// Interaction logic for Number.xaml
    /// </summary>
    public partial class Number : UserControl, IToken<int>
    {

        #region Value

        [Bindable(true), Browsable(true), Category("Common")]
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
                                                                            nameof(Value),
                                                                            typeof(int),
                                                                            typeof(Number),
                                                                            new FrameworkPropertyMetadata(
                                                                                default(int),
                                                                                FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                new PropertyChangedCallback(
                                                                                    Number.RaiseValueChanged)
                                                                                )
                                                                            );


        public event RoutedPropertyChangedEventHandler<int> ValueChanged
        {
            add { base.AddHandler(ValueChangedEvent, value); }
            remove { base.RemoveHandler(ValueChangedEvent, value); }
        }

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(ValueChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<int>),
                                                                            typeof(Number));

        protected virtual void RaiseValueChanged(int oldValue, int newValue)
        {
            RoutedPropertyChangedEventArgs<int> e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue);
            e.RoutedEvent = ValueChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Number element = (Number)d;
            element.RaiseValueChanged((int)e.OldValue, (int)e.NewValue);
        }

        #endregion

        #region Text

        [RefreshProperties(RefreshProperties.All)]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
                                                                            nameof(Text),
                                                                            typeof(string),
                                                                            typeof(Number),
                                                                            new FrameworkPropertyMetadata(
                                                                                default(string),
                                                                                FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                new PropertyChangedCallback(
                                                                                    Number.RaiseTextChanged)
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
                                                                            typeof(Number));

        protected virtual void RaiseTextChanged(string oldValue, string newValue)
        {
            RoutedPropertyChangedEventArgs<string> e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue);
            e.RoutedEvent = TextChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Number element = (Number)d;
            element.RaiseTextChanged((string)e.OldValue, (string)e.NewValue);
        }

        #endregion

        static Number()
        {
            FocusableProperty.OverrideMetadata(typeof(Number), new FrameworkPropertyMetadata(false));
        }

        public Number()
        {
            InitializeComponent();
            this.Loaded += Control_Loaded;
            this.Unloaded += Control_Unloaded;
            RegisterEvents();
        }

        public Number(string text)
            : this()
        {
            Text = text;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Control_Unloaded(object sender, RoutedEventArgs e)
        {
            UnRegisterEvents();
        }

        private bool EventsRegistered = false;
        private void RegisterEvents()
        {
            if (!EventsRegistered)
            {
                EventsRegistered = true;

                ValueChanged += Control_ValueChanged;
                TextChanged += Control_TextChanged;
                textBox.KeyUp += TextBox_KeyUp;
            }
        }

        private void UnRegisterEvents()
        {
            if (EventsRegistered)
            {
                ValueChanged -= Control_ValueChanged;
                TextChanged -= Control_TextChanged;
                textBox.KeyUp -= TextBox_KeyUp;

                EventsRegistered = false;
            }
        }

        private void Control_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            string newText = e.NewValue.ToString();
            string oldText = (string)GetValue(TextProperty);
            if (oldText != newText)
            {
                SetValue(TextProperty, newText);
            }
        }

        VisualBrush _dashedBrush = null;
        private void Control_TextChanged(object sender, RoutedPropertyChangedEventArgs<string> e)
        {
            if (string.IsNullOrWhiteSpace(e.NewValue))
            {
                if (_dashedBrush == null)
                {
                    Rectangle rect = new Rectangle();
                    rect.Width = this.ActualWidth;
                    rect.Height = this.ActualHeight;
                    rect.Stroke = Brushes.LightGray;
                    rect.StrokeThickness = 2;
                    rect.StrokeDashArray = new DoubleCollection(new double[] { 4.0d, 2.0d });
                    rect.Opacity = 1.0d;
                    _dashedBrush = new VisualBrush(rect);
                }

                textBox.BorderBrush = _dashedBrush;

                return;
            }
            else
            {
                textBox.BorderBrush = Brushes.White;
            }

            if (!int.TryParse(e.NewValue, out int newValue))
            {
                return;
            }
            int oldValue = (int)GetValue(ValueProperty);
            if (oldValue != newValue)
            {
                SetValue(ValueProperty, newValue);
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UIElement focusedElmnt = (UIElement)Keyboard.FocusedElement;
                focusedElmnt.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                e.Handled = true;
            }
        }

        private void textBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!textBox.IsMouseOver && e.OldFocus != textBox && e.NewFocus == textBox)
            {
                textBox.SelectAll();
            }
        }

        private void textBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (textBox.SelectionStart == 0 && textBox.SelectionLength == textBox.Text.Length)
            {
                textBox.SelectionLength = 0;
            }
        }
    }
}
