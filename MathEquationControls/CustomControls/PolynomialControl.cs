using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Numerics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;
using ExtendedArithmetic;

namespace MathEquationControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:MathEquationControl.CustomControls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:MathEquationControl.CustomControls;assembly=MathEquationControl.CustomControls"
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
    ///     <MyNamespace:PolynomialControl/>
    ///
    /// </summary>

    [TemplatePart(Name = PolynomialControl.ElementBorder, Type = typeof(Border))]
    [TemplatePart(Name = PolynomialControl.ElementContentsPanel, Type = typeof(StackPanel))]
    public class PolynomialControl : Control
    {

        #region Public Properties
        public ExtendedArithmetic.Polynomial Polynomial
        {
            get
            {
                return _polynomial;
            }
            set
            {
                if (_polynomial == null || !_polynomial.Equals(value))
                {
                    _polynomial = value;
                    if (_polynomial == null)
                    {
                        _polynomial = ExtendedArithmetic.Polynomial.Zero;
                    }
                    ConstructTermControlsFromPolynomial(_polynomial);
                    RaisePolynomialChanged();
                }
            }
        }
        private ExtendedArithmetic.Polynomial _polynomial = null;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    RaiseTextChanged();
                }
            }
        }
        private string _text = null;

        private bool SuppressUpdateEvents = false;

        public bool DockToParent
        {
            get => (bool)GetValue(DockToParentProperty);
            set => SetValue(DockToParentProperty, value);
        }

        public IReadOnlyList<PolynomialTermControl> Children
        {
            get
            {
                return controlContentsPanel.Children.OfType<PolynomialTermControl>().ToList();
            }
        }

        #endregion

        #region Dependency Properties
        /*
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
                                                                                nameof(Text),
                                                                                typeof(string),
                                                                                typeof(PolynomialControl),
                                                                                new PropertyMetadata(
                                                                                    default(string),
                                                                                    new PropertyChangedCallback(PolynomialControl.OnTextChanged)
                                                                                )
                                                                       );
        */
        public static readonly DependencyProperty DockToParentProperty = DependencyProperty.Register(nameof(DockToParent), typeof(bool), typeof(PolynomialControl));

        #endregion

        #region Events

        public event EventHandler TextChanged;
        public event EventHandler PolynomialChanged;

        /*
        public event RoutedPropertyChangedEventHandler<string> TextChanged
        {
            add { base.AddHandler(TextChangedEvent, value); }
            remove { base.RemoveHandler(TextChangedEvent, value); }
        }

        #region RoutedEvents

        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
                                                                        nameof(TextChanged),
                                                                        RoutingStrategy.Bubble,
                                                                        typeof(RoutedPropertyChangedEventHandler<string>),
                                                                        typeof(PolynomialControl));

        #endregion
        */
        #region Raise Event Methods

        /*
        protected virtual void OnTextChanged(string oldValue, string newValue)
        {
            RoutedPropertyChangedEventArgs<string> e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue);
            e.RoutedEvent = TextChangedEvent;
            base.RaiseEvent(e);
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialControl element = (PolynomialControl)d;
            element.OnTextChanged((string)e.OldValue, (string)e.NewValue);
        }
        */

        private void RaisePolynomialChanged()
        {
            if (!SuppressUpdateEvents)
            {
                PolynomialChanged?.Invoke(this, new EventArgs());
            }
        }

        private void RaiseTextChanged()
        {
            if (!SuppressUpdateEvents)
            {
                TextChanged?.Invoke(this, new EventArgs());
            }
        }

        #endregion

        #endregion

        #region Template Constants & Private Controls

        private const string ElementBorder = "PART_Border";
        private const string ElementContentsPanel = "PART_ContentsPanel";

        private Border controlBorder;
        private StackPanel controlContentsPanel;

        private Dictionary<int, PolynomialTermControl> _controlCache_Terms;
        private Dictionary<int, TextBlock> _controlCache_TextBlock;

        #endregion

        #region Constructors

        static PolynomialControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PolynomialControl), new FrameworkPropertyMetadata(typeof(PolynomialControl)));
        }

        public PolynomialControl()
        {
            this.IsHitTestVisible = true;
            this.Loaded += PolynomialControl_Loaded;
            this.Unloaded += PolynomialControl_Unloaded;
            _controlCache_Terms = new Dictionary<int, PolynomialTermControl>();
            _controlCache_TextBlock = new Dictionary<int, TextBlock>();
            SuppressUpdateEvents = false;
        }

        private void PolynomialControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DockToParent)
            {
                FrameworkElement parent = (FrameworkElement)WPFHelper.GetParent(this);

                double parentActualHeight = parent.ActualHeight;
                this.Height = parentActualHeight;
            }

            this.PolynomialChanged += PolynomialControl_PolynomialChanged;
            this.TextChanged += PolynomialControl_TextChanged;
        }

        private void PolynomialControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.PolynomialChanged -= PolynomialControl_PolynomialChanged;
            this.TextChanged -= PolynomialControl_TextChanged;
        }

        private void PolynomialControl_TextChanged(object? sender, EventArgs e)
        {
            if (Polynomial != null)
            {
                string tempS = Polynomial.ToString();
                if (Text.Equals(tempS, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            Polynomial tempP = null;
            try
            {
                tempP = ExtendedArithmetic.Polynomial.Parse(Text);
            }
            catch
            {
                return;
            }

            if (tempP != null)
            {
                Polynomial = tempP;
            }
        }

        private void PolynomialControl_PolynomialChanged(object? sender, EventArgs e)
        {
            if (Polynomial == null)
            {
                Text = string.Empty;
                return;
            }
            string temp = Polynomial.ToString();
            if (!temp.Equals(Text, StringComparison.OrdinalIgnoreCase))
            {
                Text = temp;
            }
        }

        #endregion

        #region Set Controls

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            controlBorder = GetTemplateChild(ElementBorder) as Border;
            controlContentsPanel = GetTemplateChild(ElementContentsPanel) as StackPanel;
        }

        private void ConstructTermControlsFromPolynomial(ExtendedArithmetic.Polynomial poly)
        {
            SuppressUpdateEvents = true;

            controlContentsPanel.Children.Clear();

            bool firstPass = true;
            foreach (ExtendedArithmetic.Term term in poly.Terms.Reverse())
            {
                TextBlock additiveSymbol = GetTextBlockControl(term);
                if (firstPass)
                {
                    if (term.CoEfficient.Sign == -1)
                    {
                        controlContentsPanel.Children.Add(additiveSymbol);
                    }
                }
                else
                {
                    controlContentsPanel.Children.Add(additiveSymbol);
                }

                PolynomialTermControl termCtrl = GetTermControl(term);
                controlContentsPanel.Children.Add(termCtrl);

                if (firstPass)
                {
                    firstPass = false;
                }
            }

            SuppressUpdateEvents = false;
        }

        private PolynomialTermControl GetTermControl(Term term)
        {
            PolynomialTermControl result = null;
            if (_controlCache_Terms.ContainsKey(term.Exponent))
            {
                result = _controlCache_Terms[term.Exponent];
                result.Term = term;
            }
            else
            {
                result = new PolynomialTermControl(term);
                result.Style = (Style)FindResource("PolynomialTermStyle");
                result.Height = this.Height;
                result.TermUpdated += TermCtrl_TermUpdated;
                _controlCache_Terms[term.Exponent] = result;
            }
            return result;
        }

        private TextBlock GetTextBlockControl(Term term)
        {
            TextBlock result = null;
            if (_controlCache_TextBlock.ContainsKey(term.Exponent))
            {
                result = _controlCache_TextBlock[term.Exponent];
            }
            else
            {
                result = new TextBlock();
                result.Style = (Style)FindResource("TextBlockStyle");
                _controlCache_TextBlock[term.Exponent] = result;
            }

            bool skip = false;
            if (term.CoEfficient.Sign == -1)
            {
                result.Text = " - ";
            }
            else if (term.CoEfficient.Sign == 1)
            {
                result.Text = " + ";
            }
            else if (term.CoEfficient.Sign == 0)
            {
                skip = true;
            }

            if (skip)
            {
                result.Visibility = Visibility.Hidden;
            }
            else
            {
                result.Visibility = Visibility.Visible;
            }

            return result;
        }

        private void TermCtrl_TermUpdated(object sender, TermUpdatedEventArgs e)
        {
            if (!SuppressUpdateEvents)
            {
                UpdatePolynomialFromTerms();
            }
        }

        private void UpdatePolynomialFromTerms()
        {
            var terms = controlContentsPanel.Children.OfType<PolynomialTermControl>().Select(ctrl => ctrl.Term).ToArray();
            Polynomial = new ExtendedArithmetic.Polynomial(terms);
        }

        #endregion

    }
}
