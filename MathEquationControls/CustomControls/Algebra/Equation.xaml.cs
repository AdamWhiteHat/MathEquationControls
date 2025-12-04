using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MathEquationControls.CustomControls.Algebra
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Equation : UserControl, IBinaryExpression
    {
        public IExpression LHS
        {
            get { return (IExpression)GetValue(LHSProperty); }
            set { SetValue(LHSProperty, value); }
        }

        #region LHS

        public static readonly DependencyProperty LHSProperty = DependencyProperty.Register(
                                                                            nameof(LHS),
                                                                            typeof(IExpression),
                                                                            typeof(Equation),
                                                                            new PropertyMetadata(
                                                                                default(IExpression),
                                                                                new PropertyChangedCallback(
                                                                                    Equation.RaiseLHSChanged)
                                                                                )
                                                                            );


        public event RoutedPropertyChangedEventHandler<IExpression> LHSChanged
        {
            add { base.AddHandler(LHSChangedEvent, value); }
            remove { base.RemoveHandler(LHSChangedEvent, value); }
        }

        public static readonly RoutedEvent LHSChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(LHSChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<IExpression>),
                                                                            typeof(Equation));

        protected virtual void RaiseLHSChanged(IExpression oldValue, IExpression newValue)
        {
            RoutedPropertyChangedEventArgs<IExpression> e = new RoutedPropertyChangedEventArgs<IExpression>(oldValue, newValue);
            e.RoutedEvent = LHSChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseLHSChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Equation element = (Equation)d;
            element.RaiseLHSChanged((IExpression)e.OldValue, (IExpression)e.NewValue);
        }

        #endregion

        public IExpression RHS
        {
            get { return (IExpression)GetValue(RHSProperty); }
            set { SetValue(RHSProperty, value); }
        }

        #region RHS

        public static readonly DependencyProperty RHSProperty = DependencyProperty.Register(
                                                                            nameof(RHS),
                                                                            typeof(IExpression),
                                                                            typeof(Equation),
                                                                            new FrameworkPropertyMetadata(
                                                                                default(IExpression),
                                                                                FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                new PropertyChangedCallback(
                                                                                    Equation.RaiseRHSChanged)
                                                                                )
                                                                            );


        public event RoutedPropertyChangedEventHandler<IExpression> RHSChanged
        {
            add { base.AddHandler(RHSChangedEvent, value); }
            remove { base.RemoveHandler(RHSChangedEvent, value); }
        }

        public static readonly RoutedEvent RHSChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(RHSChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<IExpression>),
                                                                            typeof(Equation));

        protected virtual void RaiseRHSChanged(IExpression oldValue, IExpression newValue)
        {
            RoutedPropertyChangedEventArgs<IExpression> e = new RoutedPropertyChangedEventArgs<IExpression>(oldValue, newValue);
            e.RoutedEvent = RHSChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseRHSChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Equation element = (Equation)d;
            element.RaiseRHSChanged((IExpression)e.OldValue, (IExpression)e.NewValue);
        }

        #endregion

        #region Text

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
                                                                            nameof(Text),
                                                                            typeof(string),
                                                                            typeof(Equation),
                                                                            new FrameworkPropertyMetadata(
                                                                                default(string),
                                                                                FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                new PropertyChangedCallback(
                                                                                    Equation.RaiseTextChanged)
                                                                                )
                                                                            );


        public event RoutedPropertyChangedEventHandler<string> TextChanged
        {
            add { base.AddHandler(TextChangedEvent, value); }
            remove { base.RemoveHandler(TextChangedEvent, value); }
        }

        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(TextChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<string>),
                                                                            typeof(Equation));

        protected virtual void RaiseTextChanged(string oldValue, string newValue)
        {
            RoutedPropertyChangedEventArgs<string> e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue);
            e.RoutedEvent = TextChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Equation element = (Equation)d;
            element.RaiseTextChanged((string)e.OldValue, (string)e.NewValue);
        }

        #endregion

        public Equation()
        {
            InitializeComponent();
            this.Loaded += Control_Loaded;
            this.Unloaded += Control_Unloaded;
            RegisterEvents();
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Control_Unloaded(object sender, RoutedEventArgs e)
        {
            UnRegisterEvents();
        }

        private bool EventsRegistered = false;
        private void RegisterEvents()
        {
            if (!EventsRegistered)
            {
                EventsRegistered = true;

                LHSChanged += Control_LHSChanged;
                RHSChanged += Control_RHSChanged;
                TextChanged += Control_TextChanged;
            }
        }

        private void UnRegisterEvents()
        {
            if (EventsRegistered)
            {
                LHSChanged -= Control_LHSChanged;
                RHSChanged -= Control_RHSChanged;
                TextChanged -= Control_TextChanged;

                EventsRegistered = false;
            }
        }

        private void Control_RHSChanged(object sender, RoutedPropertyChangedEventArgs<IExpression> e)
        {
            if (e.OldValue == e.NewValue)
            {
                return;
            }
            SetText();
        }

        private void Control_LHSChanged(object sender, RoutedPropertyChangedEventArgs<IExpression> e)
        {
            if (e.OldValue == e.NewValue)
            {
                return;
            }
            SetText();
        }

        private void Control_OperationChanged(object sender, RoutedPropertyChangedEventArgs<OperationType> e)
        {
            if (e.OldValue == e.NewValue)
            {
                return;
            }

            SetText();
        }

        private void SetText()
        {
            if (LHS == null || RHS == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(LHS.Text) || string.IsNullOrWhiteSpace(RHS.Text))
            {
                return;
            }

            string newText = $"{LHS.Text} = {RHS.Text}";
            string oldText = (string)GetValue(TextProperty);
            if (oldText != newText)
            {
                SetValue(TextProperty, newText);
            }
        }

        private void Control_TextChanged(object sender, RoutedPropertyChangedEventArgs<string> e)
        {
            if (e.OldValue == e.NewValue)
            {
                return;
            }

            string newExpression = (string)e.NewValue;
            if (string.IsNullOrWhiteSpace(newExpression))
            {
                SetValue(LHSProperty, Expression.Empty);
                SetValue(RHSProperty, Expression.Empty);
                return;
            }

            Parse(newExpression);
        }

        private static char[] EqualitySymbols = new char[] { '=' };
        private static char[] OperationSymbols = new char[] { '+','-','*','/' };
        private void Parse(string expression)
        {
            string sanitized = new string(expression.Where(c => !char.IsWhiteSpace(c)).ToArray());

            if (string.IsNullOrWhiteSpace(sanitized))
            {
                return;
            }

            int index = sanitized.IndexOfAny(EqualitySymbols);
            if (index == -1)
            {
                throw new FormatException("As the name implies, an EQUATION is an expression that EQUATES two expressions. Therefore an expression with an EQUALS symbol was expected, but it was missing.");
            }

            string left = sanitized.Substring(0, index);
            string right = sanitized.Substring(index + 1);

            SetOperands(left, right);
        }

        private void SetOperands(string left, string right)
        {
            Utils.SetOperand(this, LHSProperty, left);
            Utils.SetOperand(this, RHSProperty, right);
        }

    }
}
