using ExtendedArithmetic;
using MathEquationControls.CustomControls.Polynomial;
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
using MathEquationControls.CustomControls;

namespace TestMathEquationControls.CompositeControls
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
            get { return (BigInteger)GetValue(PolynomialBaseMProperty); }
            set { SetValue(PolynomialBaseMProperty, value); }
        }

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

        #region Dependency Properties

        public static readonly DependencyProperty PolynomialBaseMProperty = DependencyProperty.Register(
                                                                                nameof(PolynomialBaseM),
                                                                                typeof(BigInteger),
                                                                                typeof(BaseMExpansion),
                                                                                new PropertyMetadata(
                                                                                    BigInteger.One,
                                                                                    new PropertyChangedCallback(BaseMExpansion.OnPolynomialBaseMChanged)
                                                                                )
                                                                       );

        public event RoutedPropertyChangedEventHandler<BigInteger> PolynomialBaseMChanged
        {
            add { base.AddHandler(PolynomialBaseMChangedEvent, value); }
            remove { base.RemoveHandler(PolynomialBaseMChangedEvent, value); }
        }

        public static readonly RoutedEvent PolynomialBaseMChangedEvent = EventManager.RegisterRoutedEvent(
                                                                        nameof(PolynomialBaseMChanged),
                                                                        RoutingStrategy.Bubble,
                                                                        typeof(RoutedPropertyChangedEventHandler<BigInteger>),
                                                                        typeof(BaseMExpansion));

        protected virtual void OnPolynomialBaseMChanged(BigInteger oldValue, BigInteger newValue)
        {
            RoutedPropertyChangedEventArgs<BigInteger> e = new RoutedPropertyChangedEventArgs<BigInteger>(oldValue, newValue);
            e.RoutedEvent = PolynomialBaseMChangedEvent;
            base.RaiseEvent(e);
        }

        private static void OnPolynomialBaseMChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseMExpansion element = (BaseMExpansion)d;
            element.OnPolynomialBaseMChanged((BigInteger)e.OldValue, (BigInteger)e.NewValue);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private Polynomial _polynomial = null;
        private Dictionary<int, bool> _indexIsTermLockedDictionary;

        public BaseMExpansion()
        {
            InitializeComponent();
            wrappanelTermLocks.Children.Clear();
            _indexIsTermLockedDictionary = new Dictionary<int, bool>();
            _polynomial = new Polynomial();
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _targetValue = 3218147;
            _polynomialDegree = 3;
            SetValue(PolynomialBaseMProperty, new BigInteger(148));

            ctrlPolynomialBaseM.DataContext = this;
            ctrlTargetValue.DataContext = this;
            ctrlDegree.DataContext = this;

            ctrlTargetValue.ValueChanged += targetValue_ValueChanged;
            ctrlDegree.ValueChanged += polynomialDegree_ValueChanged;
            ctrlPolynomialBaseM.ValueChanged += polynomialBaseM_ValueChanged;
            checkboxSmallCoefficients.Click += checkboxSmallCoefficients_Click;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            checkboxSmallCoefficients.Click -= checkboxSmallCoefficients_Click;
            ctrlPolynomialBaseM.ValueChanged -= polynomialBaseM_ValueChanged;
            ctrlDegree.ValueChanged -= polynomialDegree_ValueChanged;
            ctrlTargetValue.ValueChanged -= targetValue_ValueChanged;
        }

        private void polynomialBaseM_ValueChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger> e)
        {
            ClampPolynomial();
        }

        private void targetValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger> e)
        {
            ClampPolynomial();
        }

        private void polynomialDegree_ValueChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger> e)
        {
            ClampPolynomial();
        }

        private void checkboxSmallCoefficients_Click(object sender, RoutedEventArgs e)
        {
            ClampPolynomial();
        }

        /// <summary>
        /// Returns true if any of the "Lock Term" checkboxes are checked, i.e. any lock is in effect
        /// </summary>
        private bool IsLockInEffect()
        {
            return _indexIsTermLockedDictionary.Values.Any(v => v == true);
        }

        /// <summary>
        /// Like <see cref="SetPolynomial"/> except it takes the locked terms as extra constraints, only modifying the unlocked terms.
        /// </summary>
        private void ClampPolynomial()
        {
            if (PolynomialBaseM == 0)
            {
                return;
            }

            if (!IsLockInEffect())
            {
                SetPolynomial();
            }
            else
            {
                List<KeyValuePair<int, bool>> lockedKVPs = _indexIsTermLockedDictionary.Where(kvp => kvp.Value == true).ToList();
                List<int> lockedIndices = lockedKVPs.Select(kvp => kvp.Key).ToList();

                List<Term> lockedTerms = _polynomial.Terms.Where(term => lockedIndices.Contains(term.Exponent)).ToList();
                BigInteger lockedValue = lockedTerms.Select(term => term.Evaluate(PolynomialBaseM)).Sum();

                BigInteger valueRemaining = TargetValue - lockedValue;

                List<int> degreeRemaining = new List<int>();

                int deg = (int)PolynomialDegree;
                while (deg >= 0)
                {
                    degreeRemaining.Add(deg);
                    deg--;
                }
                degreeRemaining = degreeRemaining.Except(lockedIndices).ToList();

                List<Term> newTerms = new List<Term>();
                newTerms.AddRange(lockedTerms);

                foreach (int d in degreeRemaining)
                {
                    if (BigInteger.Abs(valueRemaining) == 0)
                    {
                        break;
                    }

                    BigInteger placeValue = BigInteger.Pow(PolynomialBaseM, d);
                    if (placeValue == 1)
                    {
                        newTerms.Add(new Term(valueRemaining, d));
                        valueRemaining = 0;
                        break;
                    }
                    else if (placeValue == BigInteger.Abs(valueRemaining))
                    {
                        newTerms.Add(new Term(valueRemaining.Sign, d));
                        valueRemaining -= placeValue;
                        break;
                    }
                    else if (placeValue < BigInteger.Abs(valueRemaining))
                    {
                        BigInteger quotient = BigInteger.Divide(valueRemaining, placeValue);

                        newTerms.Add(new Term(quotient, d));
                        BigInteger toSubtract = BigInteger.Multiply(quotient, placeValue);
                        valueRemaining -= toSubtract;
                    }
                    else if (placeValue > BigInteger.Abs(valueRemaining))
                    {
                        newTerms.Add(new Term(0, d));
                    }
                }

                newTerms = newTerms.OrderBy(trm => trm.Exponent).ToList();

                if (checkboxSmallCoefficients.IsChecked.HasValue && checkboxSmallCoefficients.IsChecked.Value == true)
                {
                    BigInteger maxCoeff = PolynomialBaseM / 2;

                    int i = 0;
                    for (int maxDegree = newTerms.Max(trm => trm.Exponent); i <= maxDegree; i++)
                    {
                        if (newTerms[i].CoEfficient > maxCoeff)
                        {
                            if (lockedIndices.Contains(i) || lockedIndices.Contains(i + 1) || (i + 1) > maxDegree)
                            {
                                continue;
                            }

                            BigInteger newCoeff = -(PolynomialBaseM - newTerms[i].CoEfficient);
                            BigInteger newCoeff2 = newTerms[i + 1].CoEfficient + 1;

                            newTerms[i] = new Term(newCoeff, i);
                            newTerms[i + 1] = new Term(newCoeff2, i + 1);
                        }
                    }
                }

                // Remove terms with zero coefficients. They neither contribute to the evaluated polynomial value nor are displayed.
                // The polynomial class represents a 'sparse' polynomial; it does not store zero coefficient terms.
                // If you pass in zero terms into the constructor anyways, they will be discarded.
                newTerms.RemoveAll((Term t) => t.CoEfficient == 0L);
                if (!newTerms.Any())
                {
                    newTerms = Term.GetTerms(new BigInteger[1] { 0 }).ToList();
                }

                _polynomial = new Polynomial(newTerms.ToArray());
                polyControl.Text = _polynomial.ToString();
            }

            PopulateTermLockCheckboxes();
        }

        /// <summary>
        /// Back-Calculates the polynomial terms given the TargetValue, PolynomialBaseM (Indeterminant value), and PolynomialDegree
        /// </summary>
        private void SetPolynomial()
        {
            _polynomial = new Polynomial(TargetValue, PolynomialBaseM, (int)PolynomialDegree);
            if (checkboxSmallCoefficients.IsChecked.HasValue && checkboxSmallCoefficients.IsChecked.Value == true)
            {
                BigInteger maxCoeff = PolynomialBaseM / 2;

                int maxExp = _polynomial.Terms.Max(trm => trm.Exponent);
                maxExp = Math.Max(maxExp, (int)PolynomialDegree);

                var polyExponents = _polynomial.Terms.Select(t => t.Exponent).ToList();

                List<Term> newTerms = new List<Term>();

                int n = 0;
                while (n <= maxExp)
                {
                    if (polyExponents.Contains(n))
                    {
                        Term fromTerm = _polynomial.Terms.Where(t => n == t.Exponent).Single();
                        newTerms.Add(new Term(fromTerm.CoEfficient, fromTerm.Exponent));
                    }
                    else
                    {
                        newTerms.Add(new Term(0, n));
                    }
                    n++;
                }

                int i = 0;
                for (int maxDegree = (int)PolynomialDegree; i <= maxDegree; i++)
                {
                    if (newTerms[i].CoEfficient > maxCoeff)
                    {
                        if ((i + 1) > maxDegree)
                        {
                            break;
                        }

                        BigInteger newCoeff = -(PolynomialBaseM - newTerms[i].CoEfficient);
                        BigInteger newCoeff2 = newTerms[i + 1].CoEfficient + 1;

                        newTerms[i] = new Term(newCoeff, i);
                        if ((i + 1) > n)
                        {
                            newTerms.Add(new Term(0, i + 1));
                        }
                        newTerms[i + 1] = new Term(newCoeff2, i + 1);
                    }
                }

                _polynomial = new Polynomial(newTerms.ToArray());
            }

            polyControl.Polynomial = _polynomial;
        }

        /// <summary>
        /// Creates or adjusts the "Lock Term" checkboxes to match the number of polynomial terms
        /// </summary>
        private void PopulateTermLockCheckboxes()
        {
            int lockedTerms_Degree = Math.Max(0, _indexIsTermLockedDictionary.Count - 1);
            if (_polynomial.Degree < lockedTerms_Degree)
            {
                while (_polynomial.Degree < lockedTerms_Degree)
                {
                    _indexIsTermLockedDictionary.Remove(lockedTerms_Degree);

                    CheckBox toRemove = wrappanelTermLocks.Children.Cast<CheckBox>().Where(cb => ((int)cb.Tag) == lockedTerms_Degree).Single();
                    wrappanelTermLocks.Children.Remove(toRemove);

                    lockedTerms_Degree = Math.Max(0, _indexIsTermLockedDictionary.Count - 1);
                }
            }
            else if (_polynomial.Degree > lockedTerms_Degree)
            {
                if (_polynomial.Degree > 0)
                {
                    foreach (Term term in _polynomial.Terms.Reverse())
                    {
                        if (!_indexIsTermLockedDictionary.ContainsKey(term.Exponent))
                        {
                            PolynomialTermControl termControl = polyControl.Children.Where(ptc => ptc.Exponent == term.Exponent).Single();
                            double cbWidth = polyControl.ActualWidth / polyControl.Polynomial.Terms.Length;

                            CheckBox checkBox = CreateLockTermCheckbox(termControl, cbWidth);

                            int index = _polynomial.Degree - term.Exponent;

                            wrappanelTermLocks.Children.Insert(index, checkBox);
                            _indexIsTermLockedDictionary.Add(term.Exponent, false);
                        }
                    }
                }
            }
        }

        private CheckBox CreateLockTermCheckbox(PolynomialTermControl termControl, double cbWidth)
        {
            Term term = termControl.Term;
            CheckBox checkBox = new CheckBox();
            checkBox.SetValue(Control.StyleProperty, App.Current.Resources["ToggleLockStyle"]);
            checkBox.Content = term.Exponent.ToString();
            checkBox.Tag = term.Exponent;
            checkBox.Name = $"checkbox_lockTerm{term.Exponent}";
            checkBox.Checked += lockTerm_Checked;
            checkBox.Unchecked += lockTerm_Unchecked;
            checkBox.Unloaded += lockTerm_Unloaded;
            checkBox.Padding = new Thickness(5, 5, 5, 5);

            checkBox.Width = Math.Max(100, cbWidth);

            termControl.SizeChanged += (s, a) =>
            {
                if (a.WidthChanged)
                {
                    checkBox.Width = a.NewSize.Width;
                }
            };

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

                //ClampPolynomial();
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
