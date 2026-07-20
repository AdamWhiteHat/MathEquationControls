using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ExtendedArithmetic;
using MathEquationControls;
using MathEquationControls.Behaviors;

namespace TestMathEquationControls.CompositeControls
{
    /// <summary>
    /// Interaction logic for PolynomialQuotientField.xaml
    /// </summary>
    public partial class PolynomialQuotientField : Page
    {

        #region Dependency Properties

        public BigInteger Modulus
        {
            get => (BigInteger)GetValue(ModulusProperty);
            set => SetValue(ModulusProperty, value);
        }

        public static readonly DependencyProperty ModulusProperty = DependencyProperty.Register(
                                                                        nameof(Modulus),
                                                                        typeof(BigInteger),
                                                                        typeof(PolynomialQuotientField),
                                                                        new PropertyMetadata(
                                                                            default(BigInteger),
                                                                            new PropertyChangedCallback(PolynomialQuotientField.OnModulusChanged)
                                                                        )
                                                               );


        public event RoutedPropertyChangedEventHandler<BigInteger> ModulusChanged
        {
            add { base.AddHandler(ModulusChangedEvent, value); }
            remove { base.RemoveHandler(ModulusChangedEvent, value); }
        }

        public static readonly RoutedEvent ModulusChangedEvent = EventManager.RegisterRoutedEvent(
                                                                        nameof(ModulusChanged),
                                                                        RoutingStrategy.Bubble,
                                                                        typeof(RoutedPropertyChangedEventHandler<BigInteger>),
                                                                        typeof(PolynomialQuotientField));

        protected virtual void OnModulusChanged(BigInteger oldValue, BigInteger newValue)
        {
            RoutedPropertyChangedEventArgs<BigInteger> e = new RoutedPropertyChangedEventArgs<BigInteger>(oldValue, newValue);
            e.RoutedEvent = ModulusChangedEvent;
            base.RaiseEvent(e);
        }

        private static void OnModulusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialQuotientField element = (PolynomialQuotientField)d;
            element.OnModulusChanged((BigInteger)e.OldValue, (BigInteger)e.NewValue);
        }


        public BigInteger BaseValue
        {
            get => (BigInteger)GetValue(BaseValueProperty);
            set => SetValue(BaseValueProperty, value);
        }

        public static readonly DependencyProperty BaseValueProperty = DependencyProperty.Register(
                                                                        nameof(BaseValue),
                                                                        typeof(BigInteger),
                                                                        typeof(PolynomialQuotientField),
                                                                        new PropertyMetadata(
                                                                            default(BigInteger),
                                                                            new PropertyChangedCallback(PolynomialQuotientField.OnBaseValueChanged)
                                                                        )
                                                               );

        public event RoutedPropertyChangedEventHandler<BigInteger> BaseValueChanged
        {
            add { base.AddHandler(BaseValueChangedEvent, value); }
            remove { base.RemoveHandler(BaseValueChangedEvent, value); }
        }

        public static readonly RoutedEvent BaseValueChangedEvent = EventManager.RegisterRoutedEvent(
                                                                        nameof(BaseValueChanged),
                                                                        RoutingStrategy.Bubble,
                                                                        typeof(RoutedPropertyChangedEventHandler<BigInteger>),
                                                                        typeof(PolynomialQuotientField));

        protected virtual void OnBaseValueChanged(BigInteger oldValue, BigInteger newValue)
        {
            RoutedPropertyChangedEventArgs<BigInteger> e = new RoutedPropertyChangedEventArgs<BigInteger>(oldValue, newValue);
            e.RoutedEvent = BaseValueChangedEvent;
            base.RaiseEvent(e);
        }

        private static void OnBaseValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialQuotientField element = (PolynomialQuotientField)d;
            element.OnBaseValueChanged((BigInteger)e.OldValue, (BigInteger)e.NewValue);
        }

        #endregion



        private Polynomial dividendPoly = null;
        private Polynomial modPoly = null;
        private Polynomial quotientPoly = null;


        public PolynomialQuotientField()
        {
            //InitializeComponent();
            this.Loaded += PageLoaded;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            dividendPolynomialCtrl.Text = "1*X^5 + 2*X^4 + 3*X^3 + 4*X^2 + 5*X + 6";
            modulusPolynomialCtrl.Text = "X^2 - 1";
            Modulus = 8;
            BaseValue = 9;

            dividendPoly = Polynomial.Parse(dividendPolynomialCtrl.Text);
            modPoly = Polynomial.Parse(modulusPolynomialCtrl.Text);

            dividendPolynomialCtrl.TextChanged += dividendPolynomialCtrl_PolynomialChanged;
            modulusPolynomialCtrl.TextChanged += modulusPolynomialCtrl_PolynomialChanged;

            base.DataContext = this;
            modulusInteger.DataContext = this;
            xIntegerValue.DataContext = this;

            Calculate();
        }

        private void dividendPolynomialCtrl_PolynomialChanged(object sender, EventArgs e)
        {
            dividendPoly = Polynomial.Parse(dividendPolynomialCtrl.Text);
            Calculate();
        }

        private void modulusPolynomialCtrl_PolynomialChanged(object sender, EventArgs e)
        {
            modPoly = Polynomial.Parse(modulusPolynomialCtrl.Text);
            Calculate();
        }

        private void numberboxControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger> e)
        {
            Calculate();
        }

        private void Calculate()
        {
            if (dividendPoly != null && modPoly != null)
            {
                bool calcIntegerTotals = false;
                if (modulusInteger.Value == 0 || dividendPoly.Equals(ExtendedArithmetic.Polynomial.Zero))
                {
                    quotientPoly = ExtendedArithmetic.Polynomial.Zero;
                    quotient.Text = quotientPoly.ToString();
                    integerTotal_Dividend.Text = "0";
                    integerTotal_Quotient.Text = "0";
                }
                else if (modPoly.Equals(ExtendedArithmetic.Polynomial.Zero))
                {
                    quotientPoly = Polynomial.Field.Modulus(dividendPoly, modulusInteger.Value);
                    quotient.Text = quotientPoly.ToString();

                    calcIntegerTotals = true;
                }
                else
                {
                    quotientPoly = Polynomial.Field.ModMod(dividendPoly, modPoly, modulusInteger.Value);
                    quotient.Text = quotientPoly.ToString();

                    calcIntegerTotals = true;
                }

                if (calcIntegerTotals)
                {
                    BigInteger x = xIntegerValue.Value;

                    BigInteger integerValue_dividend = dividendPoly.Evaluate(x);
                    integerTotal_Dividend.Text = integerValue_dividend.ToString();

                    BigInteger integerValue_quotient = quotientPoly.Evaluate(x);
                    integerTotal_Quotient.Text = integerValue_quotient.ToString();

                    int sign = integerValue_quotient.Sign;

                    BigInteger integerValue_quotientMod = BigInteger.Abs(integerValue_quotient).Mod(modulusInteger.Value);
                    if (sign == -1)
                    {
                        integerValue_quotientMod = integerValue_quotientMod * sign;
                    }
                    integerTotal_QuotientMod.Text = integerValue_quotientMod.ToString();
                }
            }
        }
    }
}
