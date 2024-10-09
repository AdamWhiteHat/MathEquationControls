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

namespace MathEquationControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:MathEquationControl"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:MathEquationControl;assembly=MathEquationControl"
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
    ///     <MyNamespace:PolynomialTerm/>
    ///
    /// </summary>
    [TemplatePart(Name = PolynomialTermControl.ElementBorder, Type = typeof(Border))]
    [TemplatePart(Name = PolynomialTermControl.ElementTextBlock, Type = typeof(TextBlock))]
    [TemplatePart(Name = PolynomialTermControl.ElementRichTextBox, Type = typeof(RichTextBox))]
    //[TemplatePart(Name = PolynomialTermControl.ElementCoefficient, Type = typeof(Run))]
    //[TemplatePart(Name = PolynomialTermControl.ElementMultiplicationSymbol, Type = typeof(Run))]
    //[TemplatePart(Name = PolynomialTermControl.ElementIndeteminant, Type = typeof(Run))]
    //[TemplatePart(Name = PolynomialTermControl.ElementExponent, Type = typeof(Run))]
    public class PolynomialTermControl : Control
    {
        #region Public Properties

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

        public string Text
        {
            get
            {
                return GetStringRepresentation();
            }
        }

        public Term GetPolynomialTerm()
        {
            return new Term(this.Coefficient, this.Exponent);
        }

        #endregion

        #region Dependency Properties

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

        #endregion

        #region Events

        public event TermUpdatedEventHandler TermUpdated;

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

        private static string IndeteminantSymbolValue = "X";
        private static string MultiplicationSymbolValue = "×";

        private const string ElementBorder = "PART_Border";
        private const string ElementRichTextBox = "PART_RichTextBox";
        private const string ElementTextBlock = "PART_TextBlock";
        //private const string ElementCoefficient = "PART_Coefficient";
        //private const string ElementMultiplicationSymbol = "PART_MultiplicationSymbol";
        //private const string ElementIndeteminant = "PART_Indeteminant";
        //private const string ElementExponent = "PART_Exponent";

        private MouseWheelAdjustValueBehavior mouseBehavior;
        private TextInputSetValueBehavior textinputBehavior;

        private Run controlCoefficient;
        private Run controlMultiplicationSymbol;
        private Run controlIndeteminant;
        private Run controlExponent;

        private RichTextBox controlRichTextBox;
        private TextBlock controlTextBlock;
        private Border controlBorder;


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
        }

        private void PolynomialTermControl_Loaded(object sender, RoutedEventArgs e)
        {
            CoefficientChanged += PolynomialTermControl_CoefficientChanged1;
            ExponentChanged += PolynomialTermControl_ExponentChanged1;
        }

        private void PolynomialTermControl_Unloaded(object sender, RoutedEventArgs e)
        {
            CoefficientChanged -= PolynomialTermControl_CoefficientChanged1;
            ExponentChanged -= PolynomialTermControl_ExponentChanged1;
        }

        #endregion

        #region Set Controls

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            controlBorder = GetTemplateChild(ElementBorder) as Border;
            controlRichTextBox = GetTemplateChild(ElementRichTextBox) as RichTextBox;
            controlTextBlock = GetTemplateChild(ElementTextBlock) as TextBlock;

            /*
			controlIndeteminant = GetTemplateChild(ElementIndeteminant) as Run;
			controlMultiplicationSymbol = GetTemplateChild(ElementMultiplicationSymbol) as Run;

			controlCoefficient = GetTemplateChild(ElementCoefficient) as Run;
			if (controlCoefficient != null)
			{
				CoefficientChanged += PolynomialTermControl_CoefficientChanged;
			}

			controlExponent = GetTemplateChild(ElementExponent) as Run;
			if (controlExponent != null)
			{
				ExponentChanged += PolynomialTermControl_ExponentChanged;
			}
			*/

            mouseBehavior = new MouseWheelAdjustValueBehavior(CoefficientProperty);
            Interaction.GetBehaviors(this).Add(mouseBehavior);

            SetControls();
        }

        private void PolynomialTermControl_CoefficientChanged1(object sender, RoutedPropertyChangedEventArgs<BigInteger> e)
        {
            SetControls();
            OnTermUpdated(new TermUpdatedEventArgs(GetPolynomialTerm()));
        }

        private void PolynomialTermControl_ExponentChanged1(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            SetControls();
            OnTermUpdated(new TermUpdatedEventArgs(GetPolynomialTerm()));
        }

        private void SetControls()
        {
            Term term = GetPolynomialTerm();
            string termString = GetStringRepresentation();

            if (controlTextBlock != null)
            {
                controlTextBlock.Text = termString;
            }

            string measureString = $"_{termString}_";

            Size measuredStringSize = WPFHelper.MeasureString(measureString, this, controlRichTextBox);
            controlRichTextBox.Width = measuredStringSize.Width;
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

        #endregion

    }
}
