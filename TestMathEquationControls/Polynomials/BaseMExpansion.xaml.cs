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
            get => ctrlPolynomialBaseM.Value;
            set
            {
                if (ctrlPolynomialBaseM.Value != value)
                {
                    ctrlPolynomialBaseM.Value = value;
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

        private Polynomial _poynomial = null;
        private Dictionary<int, bool> _indexIsTermLockedDictionary;
        //private Dictionary<int, (bool, Term)> _indexLockTermDictionary;

        public BaseMExpansion()
        {
            InitializeComponent();
            this.DataContext = this;
            _indexIsTermLockedDictionary = new Dictionary<int, bool>();
            _poynomial = new Polynomial();
            wrappanelTermLocks.Children.Clear();
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.DataContext = this;
            ctrlPolynomialBaseM.DataContext = this;
            ctrlTargetValue.DataContext = this;
            ctrlDegree.DataContext = this;

            TargetValue = 3218147;
            PolynomialBaseM = 148;
            PolynomialDegree = 3;

            SetPolynomial();
            //PopulateTermLockCheckboxes();

            ctrlTargetValue.ValueChanged += targetValue_ValueChanged;
            ctrlDegree.ValueChanged += polynomialDegree_ValueChanged;
            ctrlPolynomialBaseM.ValueChanged += polynomialBaseM_ValueChanged;
            polyControl.PolynomialChanged += PolyControl_PolynomialChanged;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            polyControl.PolynomialChanged -= PolyControl_PolynomialChanged;
            ctrlPolynomialBaseM.ValueChanged -= polynomialBaseM_ValueChanged;
            ctrlDegree.ValueChanged -= polynomialDegree_ValueChanged;
            ctrlTargetValue.ValueChanged -= targetValue_ValueChanged;
        }

        private void PolyControl_PolynomialChanged(object sender, EventArgs e)
        {
            //if (IsLockInEffect())
            //{
            //    ClampPolynomial();
            //}
        }

        private void polynomialBaseM_ValueChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger> e)
        {
            SetPolynomial();
        }

        private void targetValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger> e)
        {
            SetPolynomial();
        }

        private void polynomialDegree_ValueChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger> e)
        {
            SetPolynomial();
            PopulateTermLockCheckboxes();
        }

        private void checkboxSmallCoefficients_Click(object sender, RoutedEventArgs e)
        {
            SetPolynomial();
        }

        /// <summary>
        /// Back-Calculates the polynomial terms given the TargetValue, PolynomialBaseM (Indeterminant value), and PolynomialDegree
        /// </summary>
        private void SetPolynomial()
        {
            _poynomial = new Polynomial(TargetValue, PolynomialBaseM, (int)PolynomialDegree);
            if (checkboxSmallCoefficients.IsChecked.HasValue && checkboxSmallCoefficients.IsChecked.Value == true)
            {
                _poynomial = Polynomial.MakeCoefficientsSmaller(_poynomial, PolynomialBaseM);
            }
            polyControl.Polynomial = _poynomial;
        }

        /// <summary>
        /// Creates or adjusts the "Lock Term" checkboxes to match the number of polynomial terms
        /// </summary>
        private void PopulateTermLockCheckboxes()
        {
            int lockedTerms_Degree = Math.Max(0, _indexIsTermLockedDictionary.Count - 1);
            if (_poynomial.Degree < lockedTerms_Degree)
            {
                while (_poynomial.Degree < lockedTerms_Degree)
                {
                    _indexIsTermLockedDictionary.Remove(lockedTerms_Degree);

                    CheckBox toRemove = wrappanelTermLocks.Children.Cast<CheckBox>().Where(cb => ((int)cb.Tag) == lockedTerms_Degree).Single();
                    wrappanelTermLocks.Children.Remove(toRemove);

                    lockedTerms_Degree = Math.Max(0, _indexIsTermLockedDictionary.Count - 1);
                }
            }
            else if (_poynomial.Degree > lockedTerms_Degree)
            {
                if (_poynomial.Degree > 0)
                {
                    foreach (Term term in _poynomial.Terms.Reverse())
                    {
                        if (!_indexIsTermLockedDictionary.ContainsKey(term.Exponent))
                        {
                            double cbWidth = polyControl.ActualWidth / polyControl.Polynomial.Terms.Length;
                            CheckBox checkBox = CreateLockTermCheckbox(term, cbWidth);

                            wrappanelTermLocks.Children.Add(checkBox);
                            _indexIsTermLockedDictionary.Add(term.Exponent, false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if any of the "Lock Term" checkboxes are checked, i.e. any lock is in effect
        /// </summary>
        private bool IsLockInEffect()
        {
            return ((_poynomial.Degree > 0) && (_indexIsTermLockedDictionary.Values.Any(v => v == true)));
        }

        /// <summary>
        /// Like <see cref="SetPolynomial"/> except it takes the locked terms as extra constraints, only modifying the unlocked term.
        /// </summary>
        private void ClampPolynomial()
        {
            if (IsLockInEffect())
            {
                List<KeyValuePair<int, bool>> lockedKVPs = _indexIsTermLockedDictionary.Where(kvp => kvp.Value == true).ToList();
                List<int> lockedIndices = lockedKVPs.Select(kvp => kvp.Key).ToList();

                List<Term> lockedTerms = _poynomial.Terms.Where(term => lockedIndices.Contains(term.Exponent)).ToList();
                BigInteger lockedValue = lockedTerms.Select(term => term.Evaluate(PolynomialBaseM)).Sum();

                BigInteger remaining = TargetValue - lockedValue;

                int d = _poynomial.Degree;

                List<Term> newTerms = new List<Term>();


                while ((d >= 0) && (BigInteger.Abs(remaining) > 0))
                {
                    if (lockedIndices.Contains(d))
                    {
                        newTerms.Add(_poynomial.Terms.Where(term => term.Exponent == d).Single());
                        d--;
                        continue;
                    }

                    BigInteger placeValue = BigInteger.Pow(PolynomialBaseM, d);
                    if (placeValue == BigInteger.Abs(remaining))
                    {
                        newTerms.Add(new Term(remaining.Sign, d));
                        remaining = 0;
                    }
                    else if (placeValue < BigInteger.Abs(remaining))
                    {
                        BigInteger quotient = BigInteger.Divide(remaining, placeValue);


                        newTerms.Add(new Term(quotient, d));
                        BigInteger toSubtract = BigInteger.Multiply(quotient, placeValue);

                        if (remaining.Sign == -1)
                        {
                            remaining += toSubtract;
                        }
                        else
                        {
                            remaining -= toSubtract;
                        }

                    }
                    else if (placeValue > BigInteger.Abs(remaining))
                    {
                        newTerms.Add(new Term(0, d));
                    }
                    d--;
                }

                _poynomial = new Polynomial(newTerms.ToArray());
                polyControl.Text = _poynomial.ToString();
            }
        }


        private List<Term> Recursive_ClampPolynomial(int d, BigInteger remaining, List<int> lockedIndices)
        {
            List<Term> results = new List<Term>();

            while ((d >= 0) && (BigInteger.Abs(remaining) > 0))
            {
                if (lockedIndices.Contains(d))
                {
                    results.Add(_poynomial.Terms.Where(term => term.Exponent == d).Single());
                    d--;
                    continue;
                }

                BigInteger placeValue = BigInteger.Pow(PolynomialBaseM, d);
                if (placeValue == BigInteger.Abs(remaining))
                {
                    results.Add(new Term(remaining.Sign, d));
                    remaining = 0;
                }
                else if (placeValue < BigInteger.Abs(remaining))
                {
                    BigInteger quotient = BigInteger.Divide(remaining, placeValue);

                    results.Add(new Term(quotient, d));
                    BigInteger toSubtract = BigInteger.Multiply(quotient, placeValue);

                    if (remaining.Sign == -1)
                    {
                        remaining += toSubtract;
                    }
                    else
                    {
                        remaining -= toSubtract;
                    }
                }
                else if (placeValue > BigInteger.Abs(remaining))
                {
                    //newTerms.Add(new Term(0, d));

                    BigInteger coefficient = -1;
                    List<Term> newTerms = new List<Term>();

                    do
                    {
                        coefficient++;

                        Term thisTerm = new Term(coefficient, d);
                        BigInteger thisContribution = thisTerm.Evaluate(PolynomialBaseM);
                        BigInteger proposedRemaining = remaining - thisContribution;

                        newTerms = Recursive_ClampPolynomial(d - 1, proposedRemaining, lockedIndices);

                        if (newTerms.Any())
                        {
                            results.Add(thisTerm);
                            results.AddRange(newTerms);

                            BigInteger newContribution = newTerms.Select(term => term.Evaluate(PolynomialBaseM)).Sum();
                            BigInteger newRemaining = proposedRemaining - newContribution;

                            if (newRemaining != 0)
                            {
                                throw new ArithmeticException($"There is a bug in the logic. NewRemaining should be zero, but was found to be: {newRemaining}");
                            }

                            return results;
                        }
                    }
                    while (!newTerms.Any() && (coefficient < PolynomialBaseM));
                }
                d--;
            }

            return results;
        }


        private CheckBox CreateLockTermCheckbox(Term term, double cbWidth)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.SetValue(Control.StyleProperty, App.Current.Resources["ToggleLockStyle"]);
            checkBox.Content = term.Exponent.ToString();
            checkBox.Tag = term.Exponent;
            checkBox.Name = $"checkbox_lockTerm{term.Exponent}";
            checkBox.Checked += lockTerm_Checked;
            checkBox.Unchecked += lockTerm_Unchecked;
            checkBox.Unloaded += lockTerm_Unloaded;
            checkBox.Width = Math.Max(100, cbWidth);
            checkBox.Margin = new Thickness(5);

            return checkBox;
        }

        private void lockTerm_Unloaded(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                checkBox.Checked -= lockTerm_Checked;
                checkBox.Unchecked -= lockTerm_Unchecked;
                checkBox.Unloaded -= lockTerm_Unloaded;

                if (checkBox.IsChecked.HasValue && checkBox.IsChecked.Value == true)
                {
                    int key = (int)checkBox.Tag;
                    _indexIsTermLockedDictionary[key] = false;
                }
            }
        }

        private void lockTerm_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null)
            {

                int key = (int)checkBox.Tag;
                _indexIsTermLockedDictionary[key] = true;

                ClampPolynomial();
            }
        }

        private void lockTerm_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                int key = (int)checkBox.Tag;
                _indexIsTermLockedDictionary[key] = false;

                ClampPolynomial();
            }
        }
    }
}
