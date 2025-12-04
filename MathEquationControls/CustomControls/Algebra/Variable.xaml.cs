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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Xaml.Behaviors;

namespace MathEquationControls.CustomControls.Algebra
{
    /// <summary>
    /// A Variable.  An alpha character used as a stand-in for a numeric value, which is solved for.
    /// </summary>
    public partial class Variable : UserControl, IToken<char>
    {

        #region Value

        [Bindable(true), Browsable(true), Category("Common")]
        public char Value
        {
            get { return (char)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
                                                                            nameof(Value),
                                                                            typeof(char),
                                                                            typeof(Variable),
                                                                            new FrameworkPropertyMetadata(
                                                                                default(char),
                                                                                FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                new PropertyChangedCallback(
                                                                                    Variable.RaiseValueChanged)
                                                                                )
                                                                            );


        public event RoutedPropertyChangedEventHandler<char> ValueChanged
        {
            add { base.AddHandler(ValueChangedEvent, value); }
            remove { base.RemoveHandler(ValueChangedEvent, value); }
        }

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(ValueChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<char>),
                                                                            typeof(Variable));

        protected virtual void RaiseValueChanged(char oldValue, char newValue)
        {
            RoutedPropertyChangedEventArgs<char> e = new RoutedPropertyChangedEventArgs<char>(oldValue, newValue);
            e.RoutedEvent = ValueChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Variable element = (Variable)d;
            element.RaiseValueChanged((char)e.OldValue, (char)e.NewValue);
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
                                                                            typeof(Variable),
                                                                            new FrameworkPropertyMetadata(
                                                                                default(string),
                                                                                FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                new PropertyChangedCallback(
                                                                                    Variable.RaiseTextChanged)
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
                                                                            typeof(Variable));

        protected virtual void RaiseTextChanged(string oldValue, string newValue)
        {
            RoutedPropertyChangedEventArgs<string> e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue);
            e.RoutedEvent = TextChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Variable element = (Variable)d;
            element.RaiseTextChanged((string)e.OldValue, (string)e.NewValue);
        }

        #endregion

        public Variable()
        {
            InitializeComponent();
            this.Loaded += Control_Loaded;
            this.Unloaded += Control_Unloaded;
            RegisterEvents();
        }

        public Variable(string text)
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
            }
        }

        private void UnRegisterEvents()
        {
            if (EventsRegistered)
            {
                ValueChanged -= Control_ValueChanged;
                TextChanged -= Control_TextChanged;

                EventsRegistered = false;
            }
        }

        private void Control_ValueChanged(object sender, RoutedPropertyChangedEventArgs<char> e)
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

            if (!char.TryParse(e.NewValue, out char newValue))
            {
                return;
            }
            char oldValue = (char)GetValue(ValueProperty);
            if (oldValue != newValue)
            {
                SetValue(ValueProperty, newValue);
            }
        }
    }
}
