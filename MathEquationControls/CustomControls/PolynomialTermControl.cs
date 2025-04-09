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
using MathEquationControls.Converters;

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

        [Bindable(true), Browsable(true), Category("Common")]
        public int Sign
        {
            get { return (int)GetValue(SignProperty); }
            set { SetValue(SignProperty, value); }
        }

        [Bindable(true), Browsable(true), Category("Common")]
        [TypeConverter(typeof(BigIntegerConverter))]
        public BigInteger Coefficient
        {
            get => (BigInteger)GetValue(CoefficientProperty);
            set => SetValue(CoefficientProperty, value);
        }

        [Bindable(true), Browsable(true), Category("Common")]
        public int Exponent
        {
            get => (int)GetValue(ExponentProperty);
            set => SetValue(ExponentProperty, value);
        }

        [Bindable(true), Browsable(true), Category("Common")]
        [TypeConverter(typeof(PolynomialTermConverter))]
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
                    SetExponentControlText();
                    RaiseTermUpdated(new TermUpdatedEventArgs(GetPolynomialTerm()));
                }
            }
        }

        [Bindable(true), Browsable(true), Category("Common")]
        public bool IsLeadingTerm
        {
            get { return (bool)GetValue(IsLeadingTermProperty); }
            set { SetValue(IsLeadingTermProperty, value); }
        }

        [Bindable(true), Browsable(true), Category("Common")]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

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
                                                                                    new PropertyChangedCallback(PolynomialTermControl.RaiseSignChanged)));

        public static readonly DependencyProperty CoefficientProperty = DependencyProperty.Register(
                                                                                nameof(Coefficient),
                                                                                typeof(BigInteger),
                                                                                typeof(PolynomialTermControl),
                                                                                new PropertyMetadata(
                                                                                    default(BigInteger),
                                                                                    new PropertyChangedCallback(PolynomialTermControl.RaiseCoefficientChanged)
                                                                                )
                                                                       );

        public static readonly DependencyProperty ExponentProperty = DependencyProperty.Register(
                                                                                nameof(Exponent),
                                                                                typeof(int),
                                                                                typeof(PolynomialTermControl),
                                                                                new PropertyMetadata(
                                                                                    default(int),
                                                                                    new PropertyChangedCallback(PolynomialTermControl.RaiseExponentChanged)
                                                                                )
                                                                        );

        public static readonly DependencyProperty IsLeadingTermProperty = DependencyProperty.Register(
                                                                                nameof(IsLeadingTerm),
                                                                                typeof(bool),
                                                                                typeof(PolynomialTermControl),
                                                                                new PropertyMetadata(
                                                                                    false,
                                                                                    new PropertyChangedCallback(PolynomialTermControl.RaiseIsLeadingTermChanged)
                                                                                )
                                                                        );

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
                                                                            nameof(Text),
                                                                            typeof(string),
                                                                            typeof(PolynomialTermControl),
                                                                            new PropertyMetadata(
                                                                                default(string),
                                                                                new PropertyChangedCallback(PolynomialTermControl.RaiseTextChanged)
                                                                            )
                                                                    );


        #endregion

        #region Events

        #region Event Member Definition

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

        public event RoutedPropertyChangedEventHandler<bool> IsLeadingTermChanged
        {
            add { base.AddHandler(IsLeadingTermChangedEvent, value); }
            remove { base.RemoveHandler(IsLeadingTermChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<string> TextChanged
        {
            add { base.AddHandler(TextChangedEvent, value); }
            remove { base.RemoveHandler(TextChangedEvent, value); }
        }

        #endregion

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

        public static readonly RoutedEvent IsLeadingTermChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(IsLeadingTermChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<bool>),
                                                                            typeof(PolynomialTermControl));

        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(TextChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<string>),
                                                                            typeof(PolynomialTermControl));

        #endregion

        #region Raise Event Methods

        protected virtual void RaiseSignChanged(int oldValue, int newValue)
        {
            RoutedPropertyChangedEventArgs<int> e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue);
            e.RoutedEvent = SignChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseSignChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialTermControl element = (PolynomialTermControl)d;
            element.RaiseSignChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual void RaiseCoefficientChanged(BigInteger oldValue, BigInteger newValue)
        {
            RoutedPropertyChangedEventArgs<BigInteger> e = new RoutedPropertyChangedEventArgs<BigInteger>(oldValue, newValue);
            e.RoutedEvent = CoefficientChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseCoefficientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialTermControl element = (PolynomialTermControl)d;
            element.RaiseCoefficientChanged((BigInteger)e.OldValue, (BigInteger)e.NewValue);
        }

        protected virtual void RaiseExponentChanged(int oldValue, int newValue)
        {
            RoutedPropertyChangedEventArgs<int> e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue);
            e.RoutedEvent = ExponentChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseExponentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialTermControl element = (PolynomialTermControl)d;
            element.RaiseExponentChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual void RaiseIsLeadingTermChanged(bool oldValue, bool newValue)
        {
            RoutedPropertyChangedEventArgs<bool> e = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue);
            e.RoutedEvent = IsLeadingTermChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseIsLeadingTermChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialTermControl element = (PolynomialTermControl)d;
            element.RaiseIsLeadingTermChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void RaiseTermUpdated(TermUpdatedEventArgs e)
        {
            if (SuppressTermUpdateEvent == false)
            {
                TermUpdatedEventHandler handler = TermUpdated;
                handler?.Invoke(this, e);
            }
        }

        protected virtual void RaiseTextChanged(string oldValue, string newValue)
        {
            RoutedPropertyChangedEventArgs<string> e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue);
            e.RoutedEvent = TextChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialTermControl element = (PolynomialTermControl)d;
            element.RaiseTextChanged((string)e.OldValue, (string)e.NewValue);
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

        public PolynomialTermControl()
        {
            SuppressTermUpdateEvent = false;
            this.DataContext = this;
            this.Loaded += PolynomialTermControl_Loaded;
            this.Unloaded += PolynomialTermControl_Unloaded;
        }
        public PolynomialTermControl(Term polynomialTerm)
        {
            SuppressTermUpdateEvent = false;
            this.Coefficient = polynomialTerm.CoEfficient;
            this.Exponent = polynomialTerm.Exponent;
            this.DataContext = this;
            this.Loaded += PolynomialTermControl_Loaded;
            this.Unloaded += PolynomialTermControl_Unloaded;
        }

        private void PolynomialTermControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (controlCoefficient != null)
            {
                RegisterEvents();
            }
        }

        private void PolynomialTermControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (EventsRegistered)
            {
                CoefficientChanged -= PolynomialTermControl_CoefficientChanged;
                ExponentChanged -= PolynomialTermControl_ExponentChanged;
                TextChanged -= PolynomialTermControl_TextChanged;
                EventsRegistered = false;
            }
        }

        #endregion

        #region Set Controls

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            controlBorder = GetTemplateChild(ElementBorder) as Border;

            controlCoefficient = GetTemplateChild(ElementCoefficient) as Coefficient;

            controlMultiplicationSymbol = GetTemplateChild(ElementMultiplicationSymbol) as TextBlock;
            controlMultiplicationSymbol.Text = MultiplicationSymbolValue;

            controlIndeteminant = GetTemplateChild(ElementIndeteminant) as TextBlock;
            controlIndeteminant.Text = IndeteminantSymbolValue;

            controlExponent = GetTemplateChild(ElementExponent) as Run;

            //mouseBehavior = new MouseWheelAdjustValueBehavior(CoefficientProperty);
            // new DragUpDownAdjustValueBehavior(
            //Interaction.GetBehaviors(this).Add(mouseBehavior);

            SetExponentControlText();

            if (controlCoefficient != null && controlExponent != null)
            {
                RegisterEvents();

                if (!string.IsNullOrWhiteSpace(Text))
                {
                    Term temp = null;
                    try
                    {
                        temp = Term.Parse(Text);
                    }
                    catch
                    {
                        return;
                    }

                    //this.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 255));

                    if (temp != null)
                    {
                        //this.Term = temp;

                        controlCoefficient.Value = temp.CoEfficient;
                        controlExponent.Text = temp.Exponent.ToString();
                    }
                }
            }
        }

        private bool EventsRegistered = false;
        private void RegisterEvents()
        {
            if (!EventsRegistered)
            {
                EventsRegistered = true;

                CoefficientChanged += PolynomialTermControl_CoefficientChanged;
                ExponentChanged += PolynomialTermControl_ExponentChanged;
                TextChanged += PolynomialTermControl_TextChanged;
            }
        }

        private void PolynomialTermControl_CoefficientChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger> e)
        {
            if (e.OldValue.Sign != e.NewValue.Sign)
            {
                this.Sign = e.NewValue.Sign;
            }
            RaiseTermUpdated(new TermUpdatedEventArgs(GetPolynomialTerm()));
        }

        private void PolynomialTermControl_ExponentChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            SetExponentControlText();
            RaiseTermUpdated(new TermUpdatedEventArgs(GetPolynomialTerm()));
        }

        private void SetExponentControlText()
        {
            if (this.Exponent.ToString() != controlExponent.Text)
            {
                controlExponent.Text = this.Exponent.ToString();//ConvertToSuperScript(term.Exponent.ToString());
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

        private void PolynomialTermControl_TextChanged(object sender, RoutedPropertyChangedEventArgs<string> e)
        {
            if (e.OldValue != e.NewValue)
            {
                Term temp = null;
                try
                {
                    temp = Term.Parse(e.NewValue);
                }
                catch
                {
                    return;
                }

                if (temp != null)
                {
                    Term = temp;
                }
            }
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
