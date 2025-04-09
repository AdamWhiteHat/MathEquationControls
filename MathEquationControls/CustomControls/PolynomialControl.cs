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
using MathEquationControls.Converters;

namespace MathEquationControls
{
    [TemplatePart(Name = PolynomialControl.ElementBorder, Type = typeof(Border))]
    [TemplatePart(Name = PolynomialControl.ElementContentsPanel, Type = typeof(StackPanel))]
    public class PolynomialControl : Control
    {

        #region Public Properties

        [Bindable(true), Browsable(true), Category("Common")]
        [TypeConverter(typeof(PolynomialConverter))]
        public ExtendedArithmetic.Polynomial Polynomial
        {
            get { return (ExtendedArithmetic.Polynomial)GetValue(PolynomialProperty); }
            set { SetValue(PolynomialProperty, value); }
        }

        [Bindable(true), Browsable(true), Category("Common")]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

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

        public static readonly DependencyProperty DockToParentProperty = DependencyProperty.Register(nameof(DockToParent), typeof(bool), typeof(PolynomialControl));

        public static readonly DependencyProperty PolynomialProperty = DependencyProperty.Register(
                                                                            nameof(Polynomial),
                                                                            typeof(ExtendedArithmetic.Polynomial),
                                                                            typeof(PolynomialControl),
                                                                            new PropertyMetadata(
                                                                                ExtendedArithmetic.Polynomial.Zero,
                                                                                new PropertyChangedCallback(
                                                                                    PolynomialControl.RaisePolynomialChanged)
                                                                                )
                                                                            );

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
                                                                            nameof(Text),
                                                                            typeof(string),
                                                                            typeof(PolynomialControl),
                                                                            new PropertyMetadata(
                                                                                "",
                                                                                new PropertyChangedCallback(
                                                                                    PolynomialControl.RaiseTextChanged)
                                                                                )
                                                                            );

        #endregion

        #region Events

        public event RoutedPropertyChangedEventHandler<ExtendedArithmetic.Polynomial> PolynomialChanged
        {
            add { base.AddHandler(PolynomialChangedEvent, value); }
            remove { base.RemoveHandler(PolynomialChangedEvent, value); }
        }

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

        public static readonly RoutedEvent PolynomialChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(PolynomialChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<ExtendedArithmetic.Polynomial>),
                                                                            typeof(PolynomialControl));

        #endregion

        #region Raise Event Methods

        protected virtual void RaisePolynomialChanged(ExtendedArithmetic.Polynomial oldValue, ExtendedArithmetic.Polynomial newValue)
        {
            RoutedPropertyChangedEventArgs<ExtendedArithmetic.Polynomial> e = new RoutedPropertyChangedEventArgs<ExtendedArithmetic.Polynomial>(oldValue, newValue);
            e.RoutedEvent = PolynomialChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaisePolynomialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialControl element = (PolynomialControl)d;
            element.RaisePolynomialChanged((ExtendedArithmetic.Polynomial)e.OldValue, (ExtendedArithmetic.Polynomial)e.NewValue);
        }

        protected virtual void RaiseTextChanged(string oldValue, string newValue)
        {
            RoutedPropertyChangedEventArgs<string> e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue);
            e.RoutedEvent = TextChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialControl element = (PolynomialControl)d;
            element.RaiseTextChanged((string)e.OldValue, (string)e.NewValue);
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
            this.Unloaded += PolynomialControl_Unloaded;
            _controlCache_Terms = new Dictionary<int, PolynomialTermControl>();
            _controlCache_TextBlock = new Dictionary<int, TextBlock>();
            SuppressUpdateEvents = false;

            //if (DesignerProperties.GetIsInDesignMode(this))
            //{
            //    Term trm = new Term(42, 0);
            //    PolynomialTermControl termCtrl = new PolynomialTermControl(trm);
            //    termCtrl.Style = (Style)FindResource("PolynomialTermStyle");

            //    if (controlContentsPanel == null)
            //    {
            //        controlContentsPanel = GetTemplateChild(ElementContentsPanel) as StackPanel;
            //        //new StackPanel()
            //        //{
            //        //    Name = "PART_ContentsPanel",
            //        //    Orientation = Orientation.Horizontal,
            //        //    HorizontalAlignment = HorizontalAlignment.Center,
            //        //    VerticalAlignment = VerticalAlignment.Center
            //        //};
            //    }
            //    controlContentsPanel.Children.Add(termCtrl);
            //}
        }

        private void PolynomialControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.PolynomialChanged -= PolynomialControl_PolynomialChanged;
            this.TextChanged -= PolynomialControl_TextChanged;
        }

        #endregion

        #region Set Controls

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            controlBorder = GetTemplateChild(ElementBorder) as Border;
            controlContentsPanel = GetTemplateChild(ElementContentsPanel) as StackPanel;


            if (controlBorder != null && controlContentsPanel != null)
            {
                RegisterEvents();

                if (!string.IsNullOrWhiteSpace(Text))
                {
                    TextToPolynomial();
                }
            }

            //if(DesignerProperties.GetIsInDesignMode(this))
            //{
            //    if (controlBorder != null && controlContentsPanel != null)
            //    {
            //        this.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 255));
            //    }
            //}
        }

        private void RegisterEvents()
        {
            if (DockToParent)
            {
                FrameworkElement parent = (FrameworkElement)WPFHelper.GetParent(this);

                double parentActualHeight = parent.ActualHeight;
                this.Height = parentActualHeight;
            }

            this.TextChanged += PolynomialControl_TextChanged;
            this.PolynomialChanged += PolynomialControl_PolynomialChanged;
        }

        private void PolynomialControl_TextChanged(object sender, RoutedPropertyChangedEventArgs<string> e)
        {
            if (e.OldValue == e.NewValue)
            {
                return;
            }

            TextToPolynomial();
        }

        private void PolynomialControl_PolynomialChanged(object sender, RoutedPropertyChangedEventArgs<Polynomial> e)
        {
            if (e.OldValue.ToString().Equals(e.NewValue.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (Polynomial == null)
            {
                Polynomial = ExtendedArithmetic.Polynomial.Zero;
                return;
            }

            string temp = Polynomial.ToString();
            if (!temp.Equals(Text, StringComparison.OrdinalIgnoreCase))
            {
                Text = temp;
            }

            ConstructTermControlsFromPolynomial(Polynomial);
        }

        private void TextToPolynomial()
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

        private void ConstructTermControlsFromPolynomial(ExtendedArithmetic.Polynomial poly)
        {
            SuppressUpdateEvents = true;

            controlContentsPanel.Children.Clear();

            bool firstPass = true;
            List<Term> termsToIterate = poly.Terms.Reverse().ToList();
            //foreach (ExtendedArithmetic.Term term in termsToIterate)

            int deg = poly.Degree;
            int index = deg;

            while (index >= 0)
            {
                Term term = termsToIterate.Where(t => t.Exponent == index).FirstOrDefault();
                if (term == null)
                {
                    term = new Term(0, index);
                }

                PolynomialTermControl termCtrl = GetTermControl(term);
                if (firstPass)
                {
                    firstPass = false;
                    termCtrl.IsLeadingTerm = true;
                }
                else
                {
                    termCtrl.IsLeadingTerm = false;
                }

                controlContentsPanel.Children.Add(termCtrl);
                index--;
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

        public override string ToString()
        {
            return Text;
        }

        #endregion

    }
}
