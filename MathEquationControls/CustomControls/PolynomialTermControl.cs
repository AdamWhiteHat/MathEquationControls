using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Numerics;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using Microsoft.Xaml.Behaviors;

using ExtendedArithmetic;
using MathEquationControls.Behaviors;
using System.ComponentModel;

namespace MathEquationControls
{
    [TemplatePart(Name = PolynomialTermControl.ElementBorder, Type = typeof(Border))]
    [TemplatePart(Name = PolynomialTermControl.ElementWrapPanel, Type = typeof(WrapPanel))]
    [TemplatePart(Name = PolynomialTermControl.ElementCoefficient, Type = typeof(Coefficient))]
    [TemplatePart(Name = PolynomialTermControl.ElementMultiplicationSymbol, Type = typeof(TextBlock))]
    [TemplatePart(Name = PolynomialTermControl.ElementIndeteminant, Type = typeof(TextBlock))]
    [TemplatePart(Name = PolynomialTermControl.ElementExponent, Type = typeof(Run))]
    public class PolynomialTermControl : Control
    {
        #region Public Properties

        [Bindable(true), Browsable(true), Category("Behavior")]
        public int Sign
        {
            get { return (int)GetValue(SignProperty); }
            set { SetValue(SignProperty, value); }
        }


        public BigInteger Coefficient
        {
            get => (BigInteger)GetValue(CoefficientProperty);
            set => SetValue(CoefficientProperty, value);
        }

        public int Exponent
        {
            get => (int)GetValue(ExponentProperty);
            set => SetValue(ExponentProperty, value);
        }

        public Term Term
        {
            get
            {
                return GetPolynomialTerm();
            }
            set
            {
                bool termUpdated = false;
                SuppressTermUpdateEvent = true;
                if (value.CoEfficient != this.Coefficient)
                {
                    this.Coefficient = value.CoEfficient;
                    termUpdated = true;
                }
                if (value.Exponent != this.Exponent)
                {
                    this.Exponent = value.Exponent;
                    termUpdated = true;
                }
                SuppressTermUpdateEvent = false;
                if (termUpdated)
                {
                    SetControls();
                    OnTermUpdated(new TermUpdatedEventArgs(GetPolynomialTerm()));
                }
            }
        }

        [Browsable(true), Category("Behavior")]
        public bool IsLeadingTerm
        {
            get { return (bool)GetValue(IsLeadingTermProperty); }
            set { SetValue(IsLeadingTermProperty, value); }
        }

        //public string Text
        //{
        //    get
        //    {
        //        return GetStringRepresentation();
        //    }
        //}

        public Term GetPolynomialTerm()
        {
            return new Term(this.Coefficient, this.Exponent);
        }

        #endregion

        #region Private Symbols

        private static string IndeteminantSymbolValue = "X";
        private static string MultiplicationSymbolValue = "•"; // 

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty SignProperty = DependencyProperty.Register(
                                                                                nameof(Sign),
                                                                                typeof(int),
                                                                                typeof(PolynomialTermControl),
                                                                                new PropertyMetadata(
                                                                                    0,
                                                                                    new PropertyChangedCallback(PolynomialTermControl.OnSignChanged)));

        public static readonly DependencyProperty CoefficientProperty = DependencyProperty.Register(
                                                                                nameof(Coefficient),
                                                                                typeof(BigInteger),
                                                                                typeof(PolynomialTermControl),
                                                                                new PropertyMetadata(
                                                                                    default(BigInteger),
                                                                                    new PropertyChangedCallback(PolynomialTermControl.OnCoefficientChanged)
                                                                                )
                                                                       );

        public static readonly DependencyProperty ExponentProperty = DependencyProperty.Register(
                                                                                nameof(Exponent),
                                                                                typeof(int),
                                                                                typeof(PolynomialTermControl),
                                                                                new PropertyMetadata(
                                                                                    default(int),
                                                                                    new PropertyChangedCallback(PolynomialTermControl.OnExponentChanged)
                                                                                )
                                                                        );

        public static readonly DependencyProperty IsLeadingTermProperty = DependencyProperty.Register(
                                                                                      nameof(IsLeadingTerm),
                                                                                      typeof(bool),
                                                                                      typeof(PolynomialTermControl),
                                                                                      new PropertyMetadata(false));
        #endregion

        #region Events

        public event TermUpdatedEventHandler TermUpdated;

        public event RoutedPropertyChangedEventHandler<int> SignChanged
        {
            add { base.AddHandler(SignChangedEvent, value); }
            remove { base.RemoveHandler(SignChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<BigInteger> CoefficientChanged
        {
            add { base.AddHandler(CoefficientChangedEvent, value); }
            remove { base.RemoveHandler(CoefficientChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<int> ExponentChanged
        {
            add { base.AddHandler(ExponentChangedEvent, value); }
            remove { base.RemoveHandler(ExponentChangedEvent, value); }
        }

        #region RoutedEvents

        public static readonly RoutedEvent SignChangedEvent = EventManager.RegisterRoutedEvent(
                                                                                nameof(SignChanged),
                                                                                RoutingStrategy.Bubble,
                                                                                typeof(RoutedPropertyChangedEventHandler<int>),
                                                                                typeof(PolynomialTermControl));

        public static readonly RoutedEvent CoefficientChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(CoefficientChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<BigInteger>),
                                                                            typeof(PolynomialTermControl));

        public static readonly RoutedEvent ExponentChangedEvent = EventManager.RegisterRoutedEvent(
                                                                        nameof(ExponentChanged),
                                                                        RoutingStrategy.Bubble,
                                                                        typeof(RoutedPropertyChangedEventHandler<int>),
                                                                        typeof(PolynomialTermControl));

        #endregion

        #region Raise Event Methods

        protected virtual void OnSignChanged(int oldValue, int newValue)
        {
            RoutedPropertyChangedEventArgs<int> e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue);
            e.RoutedEvent = SignChangedEvent;
            base.RaiseEvent(e);
        }

        private static void OnSignChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialTermControl element = (PolynomialTermControl)d;
            element.OnSignChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual void OnTermUpdated(TermUpdatedEventArgs e)
        {
            if (SuppressTermUpdateEvent == false)
            {
                TermUpdatedEventHandler handler = TermUpdated;
                handler?.Invoke(this, e);
            }
        }

        protected virtual void OnCoefficientChanged(BigInteger oldValue, BigInteger newValue)
        {
            RoutedPropertyChangedEventArgs<BigInteger> e = new RoutedPropertyChangedEventArgs<BigInteger>(oldValue, newValue);
            e.RoutedEvent = CoefficientChangedEvent;
            base.RaiseEvent(e);
        }

        private static void OnCoefficientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialTermControl element = (PolynomialTermControl)d;
            element.OnCoefficientChanged((BigInteger)e.OldValue, (BigInteger)e.NewValue);
        }

        protected virtual void OnExponentChanged(int oldValue, int newValue)
        {
            RoutedPropertyChangedEventArgs<int> e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue);
            e.RoutedEvent = ExponentChangedEvent;
            base.RaiseEvent(e);
        }

        private static void OnExponentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialTermControl element = (PolynomialTermControl)d;
            element.OnExponentChanged((int)e.OldValue, (int)e.NewValue);
        }

        #endregion

        #endregion

        #region Template Constants & Private Controls

        private const string ElementBorder = "PART_Border";
        private const string ElementWrapPanel = "PART_WrapPanel";
        private const string ElementCoefficient = "PART_Coefficient";
        private const string ElementMultiplicationSymbol = "PART_MultiplicationSymbol";
        private const string ElementIndeteminant = "PART_Indeteminant";
        private const string ElementExponent = "PART_Exponent";

        //private MouseWheelAdjustValueBehavior mouseBehavior;
        //private TextInputSetValueBehavior textinputBehavior;

        private Border controlBorder;
        private WrapPanel controlWrapPanel;
        private Coefficient controlCoefficient;
        private TextBlock controlMultiplicationSymbol;
        private TextBlock controlIndeteminant;
        private Run controlExponent;

        private bool SuppressTermUpdateEvent = false;

        #endregion

        #region Constructors

        static PolynomialTermControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PolynomialTermControl), new FrameworkPropertyMetadata(typeof(PolynomialTermControl)));
        }

        public PolynomialTermControl(Term polynomalTerm)
        {
            SuppressTermUpdateEvent = false;
            this.Coefficient = polynomalTerm.CoEfficient;
            this.Exponent = polynomalTerm.Exponent;
            this.DataContext = this;
            this.Loaded += PolynomialTermControl_Loaded;
            this.Unloaded += PolynomialTermControl_Unloaded;

            SetBindings();
        }

        private void PolynomialTermControl_Loaded(object sender, RoutedEventArgs e)
        {
            CoefficientChanged += PolynomialTermControl_CoefficientChanged;
            ExponentChanged += PolynomialTermControl_ExponentChanged;
        }

        private void PolynomialTermControl_Unloaded(object sender, RoutedEventArgs e)
        {
            CoefficientChanged -= PolynomialTermControl_CoefficientChanged;
            ExponentChanged -= PolynomialTermControl_ExponentChanged;
        }


        private void SetBindings()
        {
            Binding signBinding = new Binding()
            {
                Mode = BindingMode.OneWay,
                Source = this,
                Path = new PropertyPath($"Term.CoEfficient.Sign")
            };
            BindingExpressionBase coEfficientSign_Sign_BindingExpressionBase = this.SetBinding(SignProperty, signBinding);
        }

        #endregion

        #region Set Controls

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            controlBorder = GetTemplateChild(ElementBorder) as Border;

            controlCoefficient = GetTemplateChild(ElementCoefficient) as Coefficient;
            if (controlCoefficient != null)
            {
                controlCoefficient.ValueChanged += PolynomialTermControl_CoefficientChanged;
            }

            controlMultiplicationSymbol = GetTemplateChild(ElementMultiplicationSymbol) as TextBlock;
            controlMultiplicationSymbol.Text = MultiplicationSymbolValue;

            controlIndeteminant = GetTemplateChild(ElementIndeteminant) as TextBlock;
            controlIndeteminant.Text = IndeteminantSymbolValue;

            controlExponent = GetTemplateChild(ElementExponent) as Run;

            //mouseBehavior = new MouseWheelAdjustValueBehavior(CoefficientProperty);
            // new DragUpDownAdjustValueBehavior(
            //Interaction.GetBehaviors(this).Add(mouseBehavior);

            SetControls();
        }

        private void PolynomialTermControl_CoefficientChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger> e)
        {
            SetControls();
            OnTermUpdated(new TermUpdatedEventArgs(GetPolynomialTerm()));
        }

        private void PolynomialTermControl_ExponentChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            SetControls();
            OnTermUpdated(new TermUpdatedEventArgs(GetPolynomialTerm()));
        }

        private void SetControls()
        {
            Term term = GetPolynomialTerm();

            if (term.CoEfficient != controlCoefficient.Value)
            {
                controlCoefficient.Value = term.CoEfficient;
            }

            if (term.Exponent.ToString() != controlExponent.Text)
            {
                controlExponent.Text = term.Exponent.ToString();//ConvertToSuperScript(term.Exponent.ToString());
            }

            if (Sign != term.CoEfficient.Sign)
            {
                Sign = term.CoEfficient.Sign;
            }

            //string termString = GetStringRepresentation();

            //if (controlTextBlock != null)
            //{
            //    controlTextBlock.Text = termString;
            //}

            //string measureString = $"_{termString}_";

            //Size measuredStringSize = WPFHelper.MeasureString(measureString, this, controlRichTextBox);
            //controlRichTextBox.Width = measuredStringSize.Width;
        }

        #endregion

        #region Misc

        private string GetStringRepresentation()
        {
            if (Coefficient == 0)
            {
                return "0";
            }

            string signString = string.Empty;
            string coefficientString = Coefficient.ToString();
            string variableString = $"{IndeteminantSymbolValue}{ConvertToSuperScript(Exponent.ToString())}";
            string multiplyString = "*";

            if (Exponent == 0)
            {
                multiplyString = "";
                variableString = "";
            }
            else if (Exponent == 1)
            {
                variableString = IndeteminantSymbolValue;
            }

            if (BigInteger.Abs(Coefficient) == 1)
            {
                coefficientString = "";
                multiplyString = "";

                if (Coefficient.Sign == -1)
                {
                    signString = "-";
                }

                if (Exponent == 0)
                {
                    variableString = IndeteminantSymbolValue;
                }
            }

            return $"{signString}{coefficientString}{multiplyString}{variableString}";
        }

        private string ConvertToSuperScript(string numbers)
        {
            return FormatFromIndex(numbers, superscriptDigits);
        }

        private static string FormatFromIndex(string input, string dictionary)
        {
            return new string(
                input.ToCharArray()
                    .Where(c => char.IsDigit(c))
                    .Select(d => int.Parse(d.ToString()))
                    .Select(i => dictionary[i])
                    .ToArray()
                );
        }

        private static string superscriptDigits = "⁰¹²³⁴⁵⁶⁷⁸⁹";

        public override string ToString()
        {
            return GetStringRepresentation();
        }

        #endregion

    }
}
