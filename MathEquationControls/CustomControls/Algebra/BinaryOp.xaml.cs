
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MathEquationControls.CustomControls.Algebra
{
    [TemplatePart(Name = ElementControlStackPanel, Type = typeof(StackPanel))]
    [TemplatePart(Name = ElementLeftOperandTextBlock, Type = typeof(TextBlock))]
    [TemplatePart(Name = ElementOperationTextBlock, Type = typeof(TextBlock))]
    [TemplatePart(Name = ElementRightOperandTextBlock, Type = typeof(TextBlock))]
    public class BinaryOp : UserControl
    {
        #region Public Properties

        #region LeftOperand

        public int LeftOperand
        {
            get => (int)GetValue(LeftOperandProperty);
            set => SetValue(LeftOperandProperty, value);
        }


        public static readonly DependencyProperty LeftOperandProperty = DependencyProperty.Register(
                                                                                nameof(LeftOperand),
                                                                                typeof(int),
                                                                                typeof(BinaryOp),
                                                                                new PropertyMetadata(
                                                                                    default(int),
                                                                                    new PropertyChangedCallback(OnLeftOperandChanged)
                                                                                )
                                                                      );

        public event RoutedPropertyChangedEventHandler<int> LeftOperandChanged
        {
            add { AddHandler(LeftOperandChangedEvent, value); }
            remove { RemoveHandler(LeftOperandChangedEvent, value); }
        }

        public static readonly RoutedEvent LeftOperandChangedEvent = EventManager.RegisterRoutedEvent(
                                                                                nameof(LeftOperandChanged),
                                                                                RoutingStrategy.Bubble,
                                                                                typeof(RoutedPropertyChangedEventHandler<int>),
                                                                                typeof(BinaryOp));

        protected virtual void OnLeftOperandChanged(int oldValue, int newValue)
        {
            RoutedPropertyChangedEventArgs<int> e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue);
            e.RoutedEvent = LeftOperandChangedEvent;
            RaiseEvent(e);
        }

        private static void OnLeftOperandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BinaryOp element = (BinaryOp)d;
            element.OnLeftOperandChanged((int)e.OldValue, (int)e.NewValue);
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
                                                                            typeof(BinaryOp),
                                                                            new PropertyMetadata(
                                                                                default(OperationType),
                                                                                new PropertyChangedCallback(
                                                                                    BinaryOp.RaiseOperationChanged)
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
                                                                            typeof(BinaryOp));

        protected virtual void RaiseOperationChanged(OperationType oldValue, OperationType newValue)
        {
            RoutedPropertyChangedEventArgs<OperationType> e = new RoutedPropertyChangedEventArgs<OperationType>(oldValue, newValue);
            e.RoutedEvent = OperationChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseOperationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BinaryOp element = (BinaryOp)d;
            element.RaiseOperationChanged((OperationType)e.OldValue, (OperationType)e.NewValue);
        }

        #endregion

        #region RightOperand

        public int RightOperand
        {
            get => (int)GetValue(RightOperandProperty);
            set => SetValue(RightOperandProperty, value);
        }

        public static readonly DependencyProperty RightOperandProperty = DependencyProperty.Register(
                                                                                    nameof(RightOperand),
                                                                                    typeof(int),
                                                                                    typeof(BinaryOp),
                                                                                    new PropertyMetadata(
                                                                                        default(int),
                                                                                        new PropertyChangedCallback(OnRightOperandChanged)
                                                                                    )
                                                                            );

        public event RoutedPropertyChangedEventHandler<int> RightOperandChanged
        {
            add { AddHandler(RightOperandChangedEvent, value); }
            remove { RemoveHandler(RightOperandChangedEvent, value); }
        }

        public static readonly RoutedEvent RightOperandChangedEvent = EventManager.RegisterRoutedEvent(
                                                                            nameof(RightOperandChanged),
                                                                            RoutingStrategy.Bubble,
                                                                            typeof(RoutedPropertyChangedEventHandler<int>),
                                                                            typeof(BinaryOp));
        protected virtual void OnRightOperandChanged(int oldValue, int newValue)
        {
            RoutedPropertyChangedEventArgs<int> e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue);
            e.RoutedEvent = RightOperandChangedEvent;
            RaiseEvent(e);
        }

        private static void OnRightOperandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BinaryOp element = (BinaryOp)d;
            element.OnRightOperandChanged((int)e.OldValue, (int)e.NewValue);
        }

        #endregion

        #endregion

        #region Template Constants & Private Controls

        private const string ElementControlStackPanel = "PART_ControlStackPanel";
        private const string ElementLeftOperandTextBlock = "PART_LeftOperandTextBlock";
        private const string ElementOperationTextBlock = "PART_ElementOperationTextBlock";
        private const string ElementRightOperandTextBlock = "PART_RightOperandTextBlock";

        private StackPanel controlStackPanel;
        private TextBlock controlLeftOperand;
        private TextBlock controlOperation;
        private TextBlock controlRightOperand;

        #endregion

        public BinaryOp()
        {
            Loaded += Control_Loaded;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            SetControls();
        }

        public BinaryOp(OperationType operation)
            : this()
        {
            SetCurrentValue(OperationProperty, operation);
        }

        public BinaryOp(int leftOperand, OperationType operation, int rightOperand)
            : this(operation)
        {
            SetCurrentValue(LeftOperandProperty, leftOperand);
            SetCurrentValue(OperationProperty, operation);
            SetCurrentValue(RightOperandProperty, rightOperand);
        }

        static BinaryOp()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BinaryOp), new FrameworkPropertyMetadata(typeof(BinaryOp)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            controlStackPanel = GetTemplateChild(ElementControlStackPanel) as StackPanel;
            controlOperation = GetTemplateChild(ElementOperationTextBlock) as TextBlock;

            controlLeftOperand = GetTemplateChild(ElementLeftOperandTextBlock) as TextBlock;
            if (controlLeftOperand != null)
            {
                LeftOperandChanged += BinaryOperation_LeftOperandChanged;
            }

            controlRightOperand = GetTemplateChild(ElementRightOperandTextBlock) as TextBlock;
            if (controlRightOperand != null)
            {
                RightOperandChanged += BinaryOperation_RightOperandChanged;
            }
        }

        private void BinaryOperation_LeftOperandChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            SetControls();
        }

        private void BinaryOperation_RightOperandChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            SetControls();
        }

        private void SetControls()
        {
            if (controlLeftOperand != null && LeftOperand != default)
            {
                controlLeftOperand.Text = LeftOperand.ToString();
            }
            if (controlRightOperand != null && RightOperand != default)
            {
                controlRightOperand.Text = RightOperand.ToString();
            }
        }
    }
}
