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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ExtendedArithmetic;
using MathEquationControls.Converters;
using static System.Net.Mime.MediaTypeNames;
using Polynomial = ExtendedArithmetic.Polynomial;

namespace MathEquationControls.CustomControls.Polynomial
{
    [TemplatePart(Name = ElementBorder, Type = typeof(Border))]
    [TemplatePart(Name = ElementContentsPanel, Type = typeof(StackPanel))]
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

        [Bindable(true), Browsable(true), Category("Common")]
        public bool IsLockingEnabled
        {
            get { return (bool)GetValue(IsLockingEnabledProperty); }
            set { SetValue(IsLockingEnabledProperty, value); }
        }

        [Bindable(true), Browsable(true), Category("Common")]
        public bool AllowNegativeCoefficients
        {
            get { return (bool)GetValue(AllowNegativeCoefficientsProperty); }
            set { SetValue(AllowNegativeCoefficientsProperty, value); }
        }

        [Bindable(true), Browsable(true), Category("Common")]
        public BigInteger Degree
        {
            get { return (BigInteger)GetValue(DegreeProperty); }
            set { SetValue(DegreeProperty, value); }
        }

        [Bindable(true), Browsable(true), Category("Common")]
        public BigInteger? IndeterminateValue
        {
            get { return (BigInteger?)GetValue(IndeterminateValueProperty); }
            set { SetValue(IndeterminateValueProperty, value); }
        }

        public BigInteger? Value
        {
            get { return (BigInteger?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        [Bindable(true), Browsable(true), Category("Common")]
        public BigInteger? TargetValue
        {
            get { return (BigInteger?)GetValue(TargetValueProperty); }
            set { SetValue(TargetValueProperty, value); }
        }


        [Bindable(true), Browsable(true), Category("Common")]
        public bool DockToParent
        {
            get => (bool)GetValue(DockToParentProperty);
            set => SetValue(DockToParentProperty, value);
        }

        public PolynomialTermControl this[int degree]
        {
            get
            {
                var result = Children.Where(ctrl => ctrl.Exponent == degree);
                if (result.Any())
                {
                    return result.First();
                }

                var newTermCtrl = ConstructTermControl(new Term(0, degree));

                int index = 0;
                foreach (var child in controlContentsPanel.Children.OfType<PolynomialTermControl>())
                {
                    if (child.Exponent < degree)
                    {
                        break;
                    }
                    index++;
                }

                controlContentsPanel.Children.Insert(index, newTermCtrl);

                return newTermCtrl;
            }
        }

        public IReadOnlyList<PolynomialTermControl> Children
        {
            get
            {
                return controlContentsPanel.Children.OfType<PolynomialTermControl>().OrderBy(ctrl => ctrl.Exponent).ToList();
            }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty PolynomialProperty = DependencyProperty.Register(
                                                                            nameof(Polynomial),
                                                                            typeof(ExtendedArithmetic.Polynomial),
                                                                            typeof(PolynomialControl),
                                                                            new PropertyMetadata(
                                                                                ExtendedArithmetic.Polynomial.Zero,
                                                                                new PropertyChangedCallback(
                                                                                    RaisePolynomialChanged)
                                                                                )
                                                                            );

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
                                                                            nameof(Text),
                                                                            typeof(string),
                                                                            typeof(PolynomialControl),
                                                                            new PropertyMetadata(
                                                                                "",
                                                                                new PropertyChangedCallback(
                                                                                    RaiseTextChanged)
                                                                                )
                                                                            );

        public static readonly DependencyProperty IsLockingEnabledProperty = DependencyProperty.Register(
                                                                            nameof(IsLockingEnabled),
                                                                            typeof(bool),
                                                                            typeof(PolynomialControl),
                                                                            new FrameworkPropertyMetadata(
                                                                                false,
                                                                                FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                new PropertyChangedCallback(
                                                                                    PolynomialControl.RaiseIsLockingEnabledChanged)
                                                                                )
                                                                            );

        public static readonly DependencyProperty AllowNegativeCoefficientsProperty = DependencyProperty.Register(
                                                                            nameof(AllowNegativeCoefficients),
                                                                            typeof(bool),
                                                                            typeof(PolynomialControl),
                                                                            new FrameworkPropertyMetadata(
                                                                                false,
                                                                                FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                new PropertyChangedCallback(
                                                                                    PolynomialControl.RaiseAllowNegativeCoefficientsChanged)
                                                                                )
                                                                            );

        public static readonly DependencyProperty DegreeProperty = DependencyProperty.Register(
                                                                            nameof(Degree),
                                                                            typeof(BigInteger),
                                                                            typeof(PolynomialControl),
                                                                            new PropertyMetadata(
                                                                                default(BigInteger),
                                                                                new PropertyChangedCallback(
                                                                                    PolynomialControl.RaiseDegreeChanged)
                                                                                )
                                                                            );

        public static readonly DependencyProperty IndeterminateValueProperty = DependencyProperty.Register(
                                                                            nameof(IndeterminateValue),
                                                                            typeof(BigInteger?),
                                                                            typeof(PolynomialControl),
                                                                            new PropertyMetadata(
                                                                                null,
                                                                                new PropertyChangedCallback(
                                                                                    PolynomialControl.RaiseIndeterminateValueChanged)
                                                                                )
                                                                            );

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
                                                                            nameof(Value),
                                                                            typeof(BigInteger?),
                                                                            typeof(PolynomialControl),
                                                                            new PropertyMetadata(
                                                                                null,
                                                                                new PropertyChangedCallback(
                                                                                    PolynomialControl.RaiseValueChanged)
                                                                                )
                                                                            );

        public static readonly DependencyProperty TargetValueProperty = DependencyProperty.Register(
                                                                            nameof(TargetValue),
                                                                            typeof(BigInteger?),
                                                                            typeof(PolynomialControl),
                                                                            new PropertyMetadata(
                                                                                null,
                                                                                new PropertyChangedCallback(
                                                                                    PolynomialControl.RaiseTargetValueChanged)
                                                                                )
                                                                            );

        public static readonly DependencyProperty DockToParentProperty = DependencyProperty.Register(nameof(DockToParent), typeof(bool), typeof(PolynomialControl));

        #endregion

        #region Events

        public event RoutedPropertyChangedEventHandler<ExtendedArithmetic.Polynomial> PolynomialChanged
        {
            add { AddHandler(PolynomialChangedEvent, value); }
            remove { RemoveHandler(PolynomialChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<string> TextChanged
        {
            add { AddHandler(TextChangedEvent, value); }
            remove { RemoveHandler(TextChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<bool> IsLockingEnabledChanged
        {
            add { base.AddHandler(IsLockingEnabledChangedEvent, value); }
            remove { base.RemoveHandler(IsLockingEnabledChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<bool> AllowNegativeCoefficientsChanged
        {
            add { base.AddHandler(AllowNegativeCoefficientsChangedEvent, value); }
            remove { base.RemoveHandler(AllowNegativeCoefficientsChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<BigInteger> DegreeChanged
        {
            add { base.AddHandler(DegreeChangedEvent, value); }
            remove { base.RemoveHandler(DegreeChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<BigInteger?> IndeterminateValueChanged
        {
            add { base.AddHandler(IndeterminateValueChangedEvent, value); }
            remove { base.RemoveHandler(IndeterminateValueChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<BigInteger?> ValueChanged
        {
            add { base.AddHandler(ValueChangedEvent, value); }
            remove { base.RemoveHandler(ValueChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<BigInteger?> TargetValueChanged
        {
            add { base.AddHandler(TargetValueChangedEvent, value); }
            remove { base.RemoveHandler(TargetValueChangedEvent, value); }
        }

        #region RoutedEvents

        public static readonly RoutedEvent PolynomialChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(PolynomialChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<ExtendedArithmetic.Polynomial>),
                                                                            typeof(PolynomialControl));

        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(TextChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<string>),
                                                                            typeof(PolynomialControl));

        public static readonly RoutedEvent IsLockingEnabledChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(IsLockingEnabledChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<bool>),
                                                                            typeof(PolynomialControl));

        public static readonly RoutedEvent AllowNegativeCoefficientsChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(AllowNegativeCoefficientsChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<bool>),
                                                                            typeof(PolynomialControl));

        public static readonly RoutedEvent DegreeChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(DegreeChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<BigInteger>),
                                                                            typeof(PolynomialControl));

        public static readonly RoutedEvent IndeterminateValueChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(IndeterminateValueChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<BigInteger?>),
                                                                            typeof(PolynomialControl));

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(ValueChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<BigInteger?>),
                                                                            typeof(PolynomialControl));

        public static readonly RoutedEvent TargetValueChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(TargetValueChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<BigInteger?>),
                                                                            typeof(PolynomialControl));

        #endregion

        #region Raise Event Methods

        protected virtual void RaisePolynomialChanged(ExtendedArithmetic.Polynomial oldValue, ExtendedArithmetic.Polynomial newValue)
        {
            RoutedPropertyChangedEventArgs<ExtendedArithmetic.Polynomial> e = new RoutedPropertyChangedEventArgs<ExtendedArithmetic.Polynomial>(oldValue, newValue);
            e.RoutedEvent = PolynomialChangedEvent;
            RaiseEvent(e);
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
            RaiseEvent(e);
        }

        private static void RaiseTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialControl element = (PolynomialControl)d;
            element.RaiseTextChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void RaiseIsLockingEnabledChanged(bool oldValue, bool newValue)
        {
            RoutedPropertyChangedEventArgs<bool> e = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue);
            e.RoutedEvent = IsLockingEnabledChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseIsLockingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialControl element = (PolynomialControl)d;
            element.RaiseIsLockingEnabledChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void RaiseAllowNegativeCoefficientsChanged(bool oldValue, bool newValue)
        {
            RoutedPropertyChangedEventArgs<bool> e = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue);
            e.RoutedEvent = AllowNegativeCoefficientsChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseAllowNegativeCoefficientsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialControl element = (PolynomialControl)d;
            element.RaiseAllowNegativeCoefficientsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void RaiseDegreeChanged(BigInteger oldValue, BigInteger newValue)
        {
            RoutedPropertyChangedEventArgs<BigInteger> e = new RoutedPropertyChangedEventArgs<BigInteger>(oldValue, newValue);
            e.RoutedEvent = DegreeChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseDegreeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialControl element = (PolynomialControl)d;
            element.RaiseDegreeChanged((BigInteger)e.OldValue, (BigInteger)e.NewValue);
        }

        protected virtual void RaiseIndeterminateValueChanged(BigInteger? oldValue, BigInteger? newValue)
        {
            RoutedPropertyChangedEventArgs<BigInteger?> e = new RoutedPropertyChangedEventArgs<BigInteger?>(oldValue, newValue);
            e.RoutedEvent = IndeterminateValueChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseIndeterminateValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialControl element = (PolynomialControl)d;
            element.RaiseIndeterminateValueChanged((BigInteger?)e.OldValue, (BigInteger?)e.NewValue);
        }

        protected virtual void RaiseValueChanged(BigInteger? oldValue, BigInteger? newValue)
        {
            RoutedPropertyChangedEventArgs<BigInteger?> e = new RoutedPropertyChangedEventArgs<BigInteger?>(oldValue, newValue);
            e.RoutedEvent = ValueChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialControl element = (PolynomialControl)d;
            element.RaiseValueChanged((BigInteger?)e.OldValue, (BigInteger?)e.NewValue);
        }

        protected virtual void RaiseTargetValueChanged(BigInteger? oldValue, BigInteger? newValue)
        {
            RoutedPropertyChangedEventArgs<BigInteger?> e = new RoutedPropertyChangedEventArgs<BigInteger?>(oldValue, newValue);
            e.RoutedEvent = TargetValueChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseTargetValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolynomialControl element = (PolynomialControl)d;
            element.RaiseTargetValueChanged((BigInteger?)e.OldValue, (BigInteger?)e.NewValue);
        }

        #endregion

        #endregion

        #region Template Constants & Private Controls

        private const string ElementBorder = "PART_Border";
        private const string ElementContentsPanel = "PART_ContentsPanel";

        private Border controlBorder;
        private StackPanel controlContentsPanel;

        private ThreadsafeInterlock SuppressUpdateEventsController;
        private Dictionary<int, PolynomialTermControl> _exponentKey_TermControl_Dictionary;

        #endregion

        #region Constructors

        static PolynomialControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PolynomialControl), new FrameworkPropertyMetadata(typeof(PolynomialControl)));
        }

        public PolynomialControl()
        {
            IsHitTestVisible = true;
            _exponentKey_TermControl_Dictionary = new Dictionary<int, PolynomialTermControl>();

            SuppressUpdateEventsController = new ThreadsafeInterlock();

            Loaded += PolynomialControl_Loaded;
            Unloaded += PolynomialControl_Unloaded;

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

        private void PolynomialControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (controlContentsPanel != null)
            {
                RegisterEvents();
            }
        }

        private void PolynomialControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (EventsRegistered)
            {
                AllowNegativeCoefficientsChanged -= PolynomialControl_AllowNegativeCoefficientsChanged;
                TargetValueChanged -= PolynomialControl_TargetValueChanged;
                IndeterminateValueChanged -= PolynomialControl_IndeterminateValueChanged;
                DegreeChanged -= PolnomialControl_DegreeChanged;
                IsLockingEnabledChanged -= PolynomialControl_IsLockingEnabledChanged;
                TextChanged -= PolynomialControl_TextChanged;
                PolynomialChanged -= PolynomialControl_PolynomialChanged;

                EventsRegistered = false;
            }
        }

        private bool EventsRegistered = false;
        private void RegisterEvents()
        {
            if (!EventsRegistered)
            {
                EventsRegistered = true;
                if (DockToParent)
                {
                    FrameworkElement parent = (FrameworkElement)WPFHelper.GetParent(this);

                    double parentActualHeight = parent.ActualHeight;
                    Height = parentActualHeight;
                }

                PolynomialChanged += PolynomialControl_PolynomialChanged;
                TextChanged += PolynomialControl_TextChanged;
                IsLockingEnabledChanged += PolynomialControl_IsLockingEnabledChanged;
                DegreeChanged += PolnomialControl_DegreeChanged;
                IndeterminateValueChanged += PolynomialControl_IndeterminateValueChanged;
                TargetValueChanged += PolynomialControl_TargetValueChanged;
                AllowNegativeCoefficientsChanged += PolynomialControl_AllowNegativeCoefficientsChanged;
            }
        }

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
                    SetFromText(Text);
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

        #endregion

        #region Event Handlers

        private void PolynomialControl_PolynomialChanged(object sender, RoutedPropertyChangedEventArgs<ExtendedArithmetic.Polynomial> e)
        {
            if (SuppressUpdateEventsController.IsLocked) { return; }
            if (e.OldValue.ToString().Equals(e.NewValue.ToString(), StringComparison.OrdinalIgnoreCase)) { return; }

            using (SuppressUpdateEventsController.GetLockToken())
            {
                if (Polynomial == null)
                {
                    Polynomial = ExtendedArithmetic.Polynomial.Zero;
                }

                string temp = Polynomial.ToString();
                if (!temp.Equals(Text, StringComparison.OrdinalIgnoreCase))
                {
                    Text = temp;
                }
            }

            SetFromPolynomial(Polynomial);
        }

        private void PolynomialControl_IsLockingEnabledChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (e.OldValue == e.NewValue) { return; }

            bool isLockingEnabled = e.NewValue;
            foreach (PolynomialTermControl termCtrl in controlContentsPanel.Children)
            {
                termCtrl.IsLockingEnabled = isLockingEnabled;
            }
        }

        private void PolynomialControl_AllowNegativeCoefficientsChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            BigInteger degree = (BigInteger) GetValue(DegreeProperty);
            BigInteger? indeterminateValue =(BigInteger?) GetValue(IndeterminateValueProperty);
            BigInteger? targetValue = (BigInteger?) GetValue(TargetValueProperty);

            Calculate(degree, indeterminateValue, targetValue);
        }

        private void PolynomialControl_TextChanged(object sender, RoutedPropertyChangedEventArgs<string> e)
        {
            if (SuppressUpdateEventsController.IsLocked) { return; }
            if (e.OldValue == e.NewValue) { return; }

            SetFromText(e.NewValue);
        }

        private void PolnomialControl_DegreeChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger> e)
        {
            if (SuppressUpdateEventsController.IsLocked) { return; }
            if (e.OldValue == e.NewValue) { return; }

            BigInteger? targetValue = (BigInteger?) GetValue(TargetValueProperty);
            if (!targetValue.HasValue)
            {
                return;
            }

            BigInteger degree = e.NewValue;
            BigInteger? indeterminateValue = (BigInteger?) GetValue(IndeterminateValueProperty);

            Calculate(degree, indeterminateValue, targetValue);
        }

        private void PolynomialControl_IndeterminateValueChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger?> e)
        {
            if (e.OldValue == e.NewValue) { return; }
            if (!e.NewValue.HasValue) { return; }

            BigInteger degree = (BigInteger) GetValue(DegreeProperty);
            BigInteger? indeterminateValue = e.NewValue;
            BigInteger? targetValue = (BigInteger?) GetValue(TargetValueProperty);

            Calculate(degree, indeterminateValue, targetValue);
        }

        private void PolynomialControl_TargetValueChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger?> e)
        {
            if (e.OldValue == e.NewValue) { return; }

            BigInteger degree = (BigInteger) GetValue(DegreeProperty);
            BigInteger? indeterminateValue = (BigInteger?) GetValue(IndeterminateValueProperty);
            BigInteger? targetValue = (BigInteger?) GetValue(TargetValueProperty);

            Calculate(degree, indeterminateValue, targetValue);
        }

        #endregion

        #region High-level Logic Methods

        private void Calculate(BigInteger degree, BigInteger? indeterminateValue, BigInteger? targetValue)
        {
            if (!indeterminateValue.HasValue)
            {
                return;
            }
            if (indeterminateValue.Value == 0)
            {
                return;
            }

            if (!targetValue.HasValue)
            {
                ForwardCalculateValue(degree, indeterminateValue.Value);
            }
            else
            {
                BackCalculateFromTargetValue(degree, indeterminateValue.Value, targetValue.Value);
            }
        }

        private void ForwardCalculateValue(BigInteger degree, BigInteger indeterminateValue)
        {
            using (SuppressUpdateEventsController.GetLockToken())
            {
                ExtendedArithmetic.Polynomial newPoly = new ExtendedArithmetic.Polynomial(Children.Select(ctrl => ctrl.Term).OrderBy(trm => trm.Exponent).ToArray());
                Polynomial = newPoly;
                Text = Polynomial.ToString();
                Value = ExtendedArithmetic.Polynomial.Evaluate(Polynomial, indeterminateValue);
            }
        }

        private void BackCalculateFromTargetValue(BigInteger degree, BigInteger indeterminateValue, BigInteger targetValue)
        {
            using (SuppressUpdateEventsController.GetLockToken())
            {
                bool firstPass = true;
                BigInteger valueRemaining = targetValue;


                Dictionary<int, PolynomialTermControl> oldTerms = new Dictionary<int, PolynomialTermControl>();
                foreach (PolynomialTermControl termCtrl in Children)
                {
                    oldTerms[termCtrl.Exponent] = termCtrl;
                }

                int maxDegree = oldTerms.Keys.Max();

                controlContentsPanel.Children.Clear();

                List<PolynomialTermControl> selectedTerms = new List<PolynomialTermControl>();

                int deg = (int)degree;
                while (deg >= 0)
                {
                    BigInteger placeValue = BigInteger.Pow(indeterminateValue, deg);
                    BigInteger quotient = BigInteger.Divide(valueRemaining, placeValue);

                    if (quotient != 0)
                    {
                        PolynomialTermControl termCtrl = null;

                        if (!oldTerms.ContainsKey(deg))
                        {
                            quotient = DetermineCoefficient(placeValue, valueRemaining);
                            termCtrl = ConstructTermControl(new Term(quotient, deg));
                        }
                        else
                        {
                            termCtrl = oldTerms[deg];

                            if (termCtrl.IsLocked)
                            {
                                quotient = termCtrl.Coefficient;
                            }
                            else
                            {
                                quotient = DetermineCoefficient(placeValue, valueRemaining);
                                termCtrl.Coefficient = quotient;
                            }
                        }

                        termCtrl.IsLeadingTerm = firstPass;
                        if (firstPass) { firstPass = false; }

                        BigInteger toSubtract = BigInteger.Multiply(quotient, placeValue);
                        valueRemaining -= toSubtract;

                        selectedTerms.Add(termCtrl);
                    }

                    deg--;
                }

                selectedTerms = selectedTerms.Where(ctrl => ctrl.Coefficient != 0).OrderByDescending(t => t.Exponent).ToList();

                foreach (var termCtrl in selectedTerms)
                {
                    controlContentsPanel.Children.Add(termCtrl);
                }

                if (AllowNegativeCoefficients)
                {
                    BigInteger maxCoeff = indeterminateValue / 2;

                    int i = 0;
                    for (int max = selectedTerms.Max(trm => trm.Exponent); i < max; i++)
                    {
                        var newTerm = this[i];

                        if (this[i].Coefficient > maxCoeff)
                        {
                            if (this[i].IsLocked || this[i + 1].IsLocked || (i + 1) > max)
                            {
                                continue;
                            }

                            BigInteger newCoeff = -(indeterminateValue - this[i].Coefficient);
                            BigInteger newCoeff2 = this[i + 1].Coefficient + 1;

                            this[i].Coefficient = newCoeff;
                            this[i + 1].Coefficient = newCoeff2;
                        }
                    }
                }

                Value = TargetValue;

                var orderedTerms = selectedTerms.Select(ctrl => ctrl.Term).OrderBy(t => t.Exponent).ToList();
                Polynomial = new ExtendedArithmetic.Polynomial(orderedTerms.ToArray());
                Text = Polynomial.ToString();
            }
        }

        private BigInteger DetermineCoefficient(BigInteger placeValue, BigInteger valueRemaining)
        {
            BigInteger coefficient = 0;

            if (placeValue == 1)
            {
                coefficient = valueRemaining;
            }
            else if (placeValue == BigInteger.Abs(valueRemaining))
            {
                coefficient = valueRemaining.Sign;
            }
            else if (placeValue < BigInteger.Abs(valueRemaining))
            {
                BigInteger quotient = BigInteger.Divide(valueRemaining, placeValue);
                coefficient = quotient;
            }
            else if (placeValue > BigInteger.Abs(valueRemaining))
            {
                coefficient = 0;
            }

            return coefficient;
        }

        private void SetFromText(string text)
        {
            if (Polynomial != null)
            {
                string polyString = Polynomial.ToString();
                if (text.Equals(polyString, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            ExtendedArithmetic.Polynomial result = null;
            try
            {
                result = ExtendedArithmetic.Polynomial.Parse(text);
            }
            catch
            {
                return;
            }

            if (result != null)
            {
                Polynomial = result;
            }
        }

        private void SetFromPolynomial(ExtendedArithmetic.Polynomial poly)
        {
            using (SuppressUpdateEventsController.GetLockToken())
            {
                controlContentsPanel.Children.Clear();

                bool firstPass = true;
                List<Term> termsToIterate = poly.Terms.ToList();
                termsToIterate.Reverse();
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

                    PolynomialTermControl termCtrl = ConstructTermControl(term);
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
            }
        }

        private PolynomialTermControl ConstructTermControl(Term term)
        {
            PolynomialTermControl result = null;
            if (_exponentKey_TermControl_Dictionary.ContainsKey(term.Exponent))
            {
                result = _exponentKey_TermControl_Dictionary[term.Exponent];
                result.IsLockingEnabled = IsLockingEnabled;
                result.IsLocked = false;
                result.Coefficient = term.CoEfficient;
            }
            else
            {
                result = new PolynomialTermControl(term);
                result.Style = (Style)FindResource("PolynomialTermStyle");
                result.Height = Height;
                result.IsLockingEnabled = IsLockingEnabled;
                result.TermUpdated += TermCtrl_TermUpdated;
                _exponentKey_TermControl_Dictionary[term.Exponent] = result;
            }
            return result;
        }

        private void TermCtrl_TermUpdated(object sender, TermUpdatedEventArgs e)
        {
            if (!SuppressUpdateEventsController.IsLocked)
            {
                SetFromTerms();

                if (!TargetValue.HasValue)
                {
                    return;
                }

                if (e.TermValue.Exponent == 0)
                {
                    return;
                }

                if (!IndeterminateValue.HasValue)
                {
                    return;
                }

                BigInteger diff = TargetValue.Value - Value.Value;
                if (diff == 0)
                {
                    return;
                }

                PolynomialTermControl nextSmallerTerm = null;
                int nextEditableLowerExponent = e.TermValue.Exponent - 1;
                while (nextEditableLowerExponent >= 0)
                {
                    nextSmallerTerm = this[nextEditableLowerExponent];

                    if (nextSmallerTerm.IsLocked)
                    {
                        nextEditableLowerExponent--;
                        continue;
                    }
                    else
                    {
                        BigInteger placeValue = BigInteger.Pow(IndeterminateValue.Value, nextEditableLowerExponent);

                        BigInteger quotient = diff/placeValue;

                        BigInteger newCoefficient = nextSmallerTerm.Coefficient + quotient;

                        nextSmallerTerm.Coefficient = newCoefficient;
                        break;
                    }
                }
            }
        }

        private void SetFromTerms()
        {
            using (SuppressUpdateEventsController.GetLockToken())
            {
                var terms = Children.Select(ctrl => ctrl.Term).ToArray();
                Polynomial = new ExtendedArithmetic.Polynomial(terms);
                Text = Polynomial.ToString();
                Value = ExtendedArithmetic.Polynomial.Evaluate(Polynomial, IndeterminateValue.Value);
            }
        }

        #endregion

        public override string ToString()
        {
            return Text;
        }

    }
}
