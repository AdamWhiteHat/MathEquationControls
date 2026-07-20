using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MathEquationControls.CustomControls.Algebra
{
    public partial class Fraction : UserControl, IExpression
    {
        #region Public Properties

        #region Numerator

        public int Numerator
        {
            get => (int)GetValue(NumeratorProperty);
            set => SetValue(NumeratorProperty, value);
        }


        public static readonly DependencyProperty NumeratorProperty = DependencyProperty.Register(
                                                                                nameof(Numerator),
                                                                                typeof(int),
                                                                                typeof(Fraction),
                                                                                new PropertyMetadata(
                                                                                    default(int),
                                                                                    new PropertyChangedCallback(OnNumeratorChanged)
                                                                                )
                                                                      );

        public event RoutedPropertyChangedEventHandler<int> NumeratorChanged
        {
            add { AddHandler(NumeratorChangedEvent, value); }
            remove { RemoveHandler(NumeratorChangedEvent, value); }
        }

        public static readonly RoutedEvent NumeratorChangedEvent = EventManager.RegisterRoutedEvent(
                                                                                nameof(NumeratorChanged),
                                                                                RoutingStrategy.Bubble,
                                                                                typeof(RoutedPropertyChangedEventHandler<int>),
                                                                                typeof(Fraction));

        protected virtual void OnNumeratorChanged(int oldValue, int newValue)
        {
            RoutedPropertyChangedEventArgs<int> e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue);
            e.RoutedEvent = NumeratorChangedEvent;
            RaiseEvent(e);
        }

        private static void OnNumeratorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Fraction element = (Fraction)d;
            element.OnNumeratorChanged((int)e.OldValue, (int)e.NewValue);
        }

        #endregion

        public OperationType Operation { get { return OperationType.Divide; } }

        #region Denominator

        public int Denominator
        {
            get => (int)GetValue(DenominatorProperty);
            set => SetValue(DenominatorProperty, value);
        }

        public static readonly DependencyProperty DenominatorProperty = DependencyProperty.Register(
                                                                                    nameof(Denominator),
                                                                                    typeof(int),
                                                                                    typeof(Fraction),
                                                                                    new PropertyMetadata(
                                                                                        default(int),
                                                                                        new PropertyChangedCallback(OnDenominatorChanged)
                                                                                    )
                                                                            );

        public event RoutedPropertyChangedEventHandler<int> DenominatorChanged
        {
            add { AddHandler(DenominatorChangedEvent, value); }
            remove { RemoveHandler(DenominatorChangedEvent, value); }
        }

        public static readonly RoutedEvent DenominatorChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(DenominatorChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<int>),
                                                                            typeof(Fraction));
        protected virtual void OnDenominatorChanged(int oldValue, int newValue)
        {
            RoutedPropertyChangedEventArgs<int> e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue);
            e.RoutedEvent = DenominatorChangedEvent;
            RaiseEvent(e);
        }

        private static void OnDenominatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Fraction element = (Fraction)d;
            element.OnDenominatorChanged((int)e.OldValue, (int)e.NewValue);
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
                                                                            typeof(Fraction),
                                                                            new FrameworkPropertyMetadata(
                                                                                default(string),
                                                                                FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                new PropertyChangedCallback(
                                                                                    Fraction.RaiseTextChanged)
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
                                                                            typeof(Fraction));

        protected virtual void RaiseTextChanged(string oldValue, string newValue)
        {
            RoutedPropertyChangedEventArgs<string> e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue);
            e.RoutedEvent = TextChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Fraction element = (Fraction)d;
            element.RaiseTextChanged((string)e.OldValue, (string)e.NewValue);
        }

        #endregion

        #endregion

        #region Template Constants & Private Controls

        #endregion

        public Fraction()
        {
            InitializeComponent();
            Loaded += Fraction_Loaded;
            Unloaded += Fraction_Unloaded;
        }

        private void Fraction_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Fraction_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        public Fraction(int numerator, int denominator)
            : this()
        {
            SetCurrentValue(NumeratorProperty, numerator);
            SetCurrentValue(DenominatorProperty, denominator);
        }

        static Fraction()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Fraction), new FrameworkPropertyMetadata(typeof(Fraction)));
        }
    }
}
