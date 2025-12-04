using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Xaml.Behaviors;

namespace MathEquationControls.CustomControls.Algebra
{
    /// <summary>
    /// Interaction logic for BinaryOperation.xaml
    /// </summary>
    public partial class BinaryOperation : UserControl, IBinaryExpression
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
                                                                            typeof(BinaryOperation),
                                                                            new PropertyMetadata(
                                                                                default(IExpression),
                                                                                new PropertyChangedCallback(
                                                                                    BinaryOperation.RaiseLHSChanged)
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
                                                                            typeof(BinaryOperation));

        protected virtual void RaiseLHSChanged(IExpression oldValue, IExpression newValue)
        {
            RoutedPropertyChangedEventArgs<IExpression> e = new RoutedPropertyChangedEventArgs<IExpression>(oldValue, newValue);
            e.RoutedEvent = LHSChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseLHSChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BinaryOperation element = (BinaryOperation)d;
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
                                                                            typeof(BinaryOperation),
                                                                            new FrameworkPropertyMetadata(
                                                                                default(IExpression),
                                                                                FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                new PropertyChangedCallback(
                                                                                    BinaryOperation.RaiseRHSChanged)
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
                                                                            typeof(BinaryOperation));

        protected virtual void RaiseRHSChanged(IExpression oldValue, IExpression newValue)
        {
            RoutedPropertyChangedEventArgs<IExpression> e = new RoutedPropertyChangedEventArgs<IExpression>(oldValue, newValue);
            e.RoutedEvent = RHSChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseRHSChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BinaryOperation element = (BinaryOperation)d;
            element.RaiseRHSChanged((IExpression)e.OldValue, (IExpression)e.NewValue);
        }

        #endregion

        #region Operation

        public OperationType Operation
        {
            get { return (OperationType)GetValue(OperationProperty); }
            set { SetValue(OperationProperty, value); }
        }

        public static readonly DependencyProperty OperationProperty = DependencyProperty.Register(
                                                                            nameof(Operation),
                                                                            typeof(OperationType),
                                                                            typeof(BinaryOperation),
                                                                            new FrameworkPropertyMetadata(
                                                                                default(OperationType),
                                                                                FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                new PropertyChangedCallback(
                                                                                    BinaryOperation.RaiseOperationChanged)
                                                                                )
                                                                            );


        public event RoutedPropertyChangedEventHandler<OperationType> OperationChanged
        {
            add { base.AddHandler(OperationChangedEvent, value); }
            remove { base.RemoveHandler(OperationChangedEvent, value); }
        }

        public static readonly RoutedEvent OperationChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(OperationChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<OperationType>),
                                                                            typeof(BinaryOperation));

        protected virtual void RaiseOperationChanged(OperationType oldValue, OperationType newValue)
        {
            RoutedPropertyChangedEventArgs<OperationType> e = new RoutedPropertyChangedEventArgs<OperationType>(oldValue, newValue);
            e.RoutedEvent = OperationChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseOperationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BinaryOperation element = (BinaryOperation)d;
            element.RaiseOperationChanged((OperationType)e.OldValue, (OperationType)e.NewValue);
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
                                                                            typeof(BinaryOperation),
                                                                            new FrameworkPropertyMetadata(
                                                                                default(string),
                                                                                FrameworkPropertyMetadataOptions.AffectsRender,
                                                                                new PropertyChangedCallback(
                                                                                    BinaryOperation.RaiseTextChanged)
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
                                                                            typeof(BinaryOperation));

        protected virtual void RaiseTextChanged(string oldValue, string newValue)
        {
            RoutedPropertyChangedEventArgs<string> e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue);
            e.RoutedEvent = TextChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BinaryOperation element = (BinaryOperation)d;
            element.RaiseTextChanged((string)e.OldValue, (string)e.NewValue);
        }

        #endregion

        public BinaryOperation()
        {
            InitializeComponent();
            //this.SetStyle(ControlStyles.Selectable, false);
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
                OperationChanged += Control_OperationChanged;
                TextChanged += Control_TextChanged;
            }
        }

        private void UnRegisterEvents()
        {
            if (EventsRegistered)
            {
                LHSChanged -= Control_LHSChanged;
                RHSChanged -= Control_RHSChanged;
                OperationChanged -= Control_OperationChanged;
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
            if (LHS == null || RHS == null || Operation == OperationType.None)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(LHS.Text) || string.IsNullOrWhiteSpace(RHS.Text))
            {
                return;
            }

            string newText = $"{LHS.Text} {OperationTypeHelper.OperationType2SymbolDictionary[Operation]} {RHS.Text}";
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
                LHS = new Empty();
                Operation = OperationType.None;
                RHS = new Empty();
                return;
            }

            Parse(newExpression);
        }

        private static char[] AdditiveSymbols = new char[] { '+','-' };
        private static char[] MultiplicativeSymbols = new char[] { '*','/' };
        private static char[] OperationSymbols = new char[] { '+', '-', '*', '/' };
        private void Parse(string expresssion)
        {
            string sanitized = new string(expresssion.Where(c => !char.IsWhiteSpace(c)).ToArray());

            int index = sanitized.IndexOfAny(OperationSymbols);
            if (index == -1)
            {
                throw new FormatException();
            }

            index = sanitized.IndexOfAny(AdditiveSymbols);
            if (index == -1)
            {
                index = sanitized.IndexOfAny(MultiplicativeSymbols);
            }

            if (index == -1)
            {
                throw new FormatException();
            }

            char op = sanitized.Substring(index, 1)[0];
            SetOperation(op);

            string left = sanitized.Substring(0, index);
            string right = sanitized.Substring(index + 1);

            SetOperands(left, right);
        }

        private void SetOperands(string left, string right)
        {
            Utils.SetOperand(this, LHSProperty, left);
            Utils.SetOperand(this, RHSProperty, right);
        }

        private void SetOperation(char opSymbol)
        {
            OperationType newOperation = OperationTypeHelper.Symbol2OperationTypeDictionary[opSymbol];
            OperationType oldOperation = (OperationType)GetValue(OperationProperty);
            if (oldOperation != newOperation)
            {
                SetCurrentValue(OperationProperty, newOperation);
            }
        }
    }
}
