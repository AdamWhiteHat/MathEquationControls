using ExtendedArithmetic;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
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

namespace TestMathEquationControls.Polynomials
{
    /// <summary>
    /// Interaction logic for BaseMExpansion.xaml
    /// </summary>
    public partial class BaseMExpansion : Window, INotifyPropertyChanged
    {
        public BigInteger TargetValue
        {
            get => _targetValue;
            set
            {
                if (value != _targetValue)
                {
                    _targetValue = value;
                    RaisePropertyChanged();
                }
            }
        }
        private BigInteger _targetValue;

        public BigInteger PolynomialBaseM
        {
            get => _polynomialBaseM;
            set
            {
                if (value != _polynomialBaseM)
                {
                    _polynomialBaseM = value;
                    RaisePropertyChanged();
                }
            }
        }
        private BigInteger _polynomialBaseM;

        public BigInteger PolynomialDegree
        {
            get => _polynomialDegree;
            set
            {
                if (value != _polynomialDegree)
                {
                    _polynomialDegree = value;
                    RaisePropertyChanged();
                }
            }
        }
        private BigInteger _polynomialDegree;

        public event PropertyChangedEventHandler PropertyChanged;

        private Polynomial poly = null;
        private Dictionary<int, (bool, Term)> lockedTerms;

        public BaseMExpansion()
        {
            InitializeComponent();
            this.DataContext = this;
            lockedTerms = new Dictionary<int, (bool, Term)>();
            poly = new Polynomial();
            wrappanelLockTerms.Children.Clear();
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            base.DataContext = this;
            nboxBaseMValue.DataContext = this;
            nboxTargetValue.DataContext = this;
            nboxDegree.DataContext = this;

            TargetValue = 3218147;
            PolynomialBaseM = 148;
            PolynomialDegree = 3;

            SetPolynomial();

            polyControl.PolynomialChanged += PolyControl_PolynomialChanged;
            PopulateLockTermCheckboxes();
        }

        private void PolyControl_PolynomialChanged(object sender, EventArgs e)
        {
            if (IsLockInEffect())
            {
                ClampPolynomial();
            }
            else
            {
                PopulateLockTermCheckboxes();
            }
        }

        private void baseMValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger> e)
        {
            SetPolynomial();
        }

        private void nboxTargetValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger> e)
        {
            SetPolynomial();
        }

        private void nboxDegree_ValueChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger> e)
        {
            SetPolynomial();
        }

        private void checkboxSmallCoefficients_Click(object sender, RoutedEventArgs e)
        {
            SetPolynomial();
        }

        private void SetPolynomial()
        {
            poly = new Polynomial(TargetValue, PolynomialBaseM, (int)PolynomialDegree);
            if (checkboxSmallCoefficients.IsChecked.HasValue && checkboxSmallCoefficients.IsChecked.Value == true)
            {
                poly = Polynomial.MakeCoefficientsSmaller(poly, PolynomialBaseM);
            }
            polyControl.Text = poly.ToString();
        }

        private void PopulateLockTermCheckboxes()
        {
            int lockedTerms_Degree = Math.Max(0, lockedTerms.Count - 1);
            if (poly.Degree < lockedTerms_Degree)
            {
                while (poly.Degree < lockedTerms_Degree)
                {
                    if (lockedTerms[lockedTerms_Degree].Item1)
                    {
                        return;
                    }
                    lockedTerms.Remove(lockedTerms_Degree);

                    CheckBox toRemove = wrappanelLockTerms.Children.Cast<CheckBox>().Where(cb => ((int)cb.Tag) == lockedTerms_Degree).First();
                    wrappanelLockTerms.Children.Remove(toRemove);

                    lockedTerms_Degree = Math.Max(0, lockedTerms.Count - 1);
                }
            }
            else if (poly.Degree > lockedTerms_Degree)
            {
                if (poly.Degree > 0)
                {
                    foreach (Term term in poly.Terms.Reverse())
                    {
                        if (!lockedTerms.ContainsKey(term.Exponent))
                        {
                            double cbWidth = polyControl.ActualWidth / polyControl.Polynomial.Terms.Length;
                            CheckBox checkBox = CreateLockTermCheckbox(term, cbWidth);

                            wrappanelLockTerms.Children.Add(checkBox);
                            lockedTerms.Add(term.Exponent, (false, term.Clone()));
                        }
                    }
                }
            }
        }

        private bool IsLockInEffect()
        {
            return (poly.Degree > 0 && lockedTerms.Values.Any(v => v.Item1 == true));
        }

        private void ClampPolynomial()
        {
            if (IsLockInEffect())
            {
                BigInteger remaining = TargetValue;

                var lockedKVPs = lockedTerms.Where(kvp => kvp.Value.Item1 == true).ToList();
                var termsToSkip = lockedKVPs.Select(kvp => kvp.Key).ToList();

                var lockedValue = lockedKVPs.Select(kvp => kvp.Value.Item2.Evaluate(PolynomialBaseM)).Sum();
                remaining -= lockedValue;

                List<Term> newTerms = new List<Term>();

                int d = poly.Degree;
                while (d >= 0 && BigInteger.Abs(remaining) > 0)
                {
                    if (termsToSkip.Contains(d))
                    {
                        newTerms.Add(lockedTerms[d].Item2);
                        d--;
                        continue;
                    }

                    BigInteger placeValue = BigInteger.Pow(PolynomialBaseM, d);
                    if (placeValue == remaining)
                    {
                        newTerms.Add(new Term(1, d));
                        remaining -= placeValue;
                    }
                    else if (placeValue > BigInteger.Abs(remaining))
                    {
                        newTerms.Add(new Term(0, d));
                    }
                    else if (placeValue < BigInteger.Abs(remaining))
                    {
                        BigInteger quotient = BigInteger.Divide(remaining, placeValue);


                        newTerms.Add(new Term(quotient, d));
                        BigInteger toSubtract = BigInteger.Multiply(quotient, placeValue);

                        remaining -= toSubtract;
                    }
                    d--;
                }

                poly = new Polynomial(newTerms.ToArray());
                polyControl.Text = poly.ToString();
            }
        }

        private CheckBox CreateLockTermCheckbox(Term term, double cbWidth)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.Content = "Lock term value?";
            checkBox.Tag = term.Exponent;
            checkBox.Name = $"checkbox_lockTerm{term.Exponent}";
            checkBox.Checked += lockTerm_Checked;
            checkBox.Unchecked += lockTerm_Unchecked;
            checkBox.Unloaded += lockTerm_Unloaded;
            checkBox.Width = Math.Max(100, cbWidth);
            checkBox.Margin = new Thickness(5);

            return checkBox;
        }

        private void lockTerm_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                int exponent = (int)checkBox.Tag;
                Term term = poly.Terms[exponent];
                lockedTerms[exponent] = (true, term);

                ClampPolynomial();
            }
        }
        private void lockTerm_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                int exponent = (int)checkBox.Tag;
                Term term = lockedTerms[exponent].Item2;
                lockedTerms[exponent] = (false, term);

                ClampPolynomial();
            }
        }

        private void lockTerm_Unloaded(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                checkBox.Checked -= lockTerm_Checked;
                checkBox.Unchecked -= lockTerm_Unchecked;
                checkBox.Unloaded -= lockTerm_Unloaded;
            }
        }
    }
}
