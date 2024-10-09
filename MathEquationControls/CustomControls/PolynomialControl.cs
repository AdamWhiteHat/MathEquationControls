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
                if (value == null)
                {
                    return;
                }
                if (!value.Terms.Any())
                {
                    return;
                }
                if (!_polynomial.Equals(value))
                {
                    _polynomial = value;
                    RaisePolynomialChanged();
                }
            }
        }
        private ExtendedArithmetic.Polynomial _polynomial = null;

        public string Text
        {
            //get => (string)GetValue(TextProperty);
            //set => SetValue(TextProperty, value);
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
            PolynomialChanged?.Invoke(this, new EventArgs());
        }

        private void RaiseTextChanged()
        {
            TextChanged?.Invoke(this, new EventArgs());
        }

        #endregion

        #endregion

        #region Template Constants & Private Controls

        private const string ElementBorder = "PART_Border";
        private const string ElementContentsPanel = "PART_ContentsPanel";

        private Border controlBorder;
        private StackPanel controlContentsPanel;

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
            this.PolynomialChanged += PolynomialControl_PolynomialChanged;
            this.TextChanged += PolynomialControl_TextChanged;
        }

        private void PolynomialControl_TextChanged(object? sender, EventArgs e)
        {
            if (_polynomial != null)
            {
                string tempS = _polynomial.ToString();
                if (_text.Equals(tempS, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            Polynomial tempP = null;
            try
            {
                tempP = ExtendedArithmetic.Polynomial.Parse(_text);
            }
            catch
            {
                return;
            }

            if (tempP != null)
            {
                _polynomial = tempP;
                ConstructTermControlsFromPolynomial(_polynomial);
                RaisePolynomialChanged();
            }
        }

        private void PolynomialControl_PolynomialChanged(object? sender, EventArgs e)
        {
            if (_polynomial == null)
            {
                return;
            }
            string temp = _polynomial.ToString();
            if (!temp.Equals(_text, StringComparison.OrdinalIgnoreCase))
            {
                _text = temp;
                RaisePolynomialChanged();
                RaiseTextChanged();
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

        private void PolynomialControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DockToParent)
            {
                FrameworkElement parent = (FrameworkElement)WPFHelper.GetParent(this);

                double parentActualHeight = parent.ActualHeight;
                this.Height = parentActualHeight;
            }

            controlBorder.PreviewMouseLeftButtonDown += PolynomialControl_PreviewMouseLeftButtonDown;
            controlBorder.PreviewMouseLeftButtonUp += PolynomialControl_PreviewMouseLeftButtonUp;

            _draggingTimer = new DispatcherTimer();
            _draggingTimer.Interval = TimeSpan.FromMilliseconds(10);
            _draggingTimer.Tick += DraggingTimer_Tick;
        }

        private void PolynomialControl_Unloaded(object sender, RoutedEventArgs e)
        {
            controlBorder.PreviewMouseLeftButtonDown -= PolynomialControl_PreviewMouseLeftButtonDown;
            controlBorder.PreviewMouseLeftButtonUp -= PolynomialControl_PreviewMouseLeftButtonUp;

            _draggingTimer.Stop();
        }

        private void ConstructTermControlsFromPolynomial(ExtendedArithmetic.Polynomial poly)
        {
            controlContentsPanel.Children.Clear();

            bool firstPass = true;
            foreach (ExtendedArithmetic.Term term in poly.Terms.Reverse())
            {
                if (firstPass)
                {
                    firstPass = false;
                }
                else
                {
                    TextBlock plusSymbol = new TextBlock();
                    plusSymbol.Text = " + ";
                    plusSymbol.Style = (Style)FindResource("OperatorStyle");
                    controlContentsPanel.Children.Add(plusSymbol);
                }

                PolynomialTermControl termCtrl = new PolynomialTermControl(term);
                termCtrl.Style = (Style)FindResource("PolynomialTermStyle");
                termCtrl.Height = this.Height;
                termCtrl.TermUpdated += TermCtrl_TermUpdated;
                controlContentsPanel.Children.Add(termCtrl);
            }
        }

        private void TermCtrl_TermUpdated(object sender, TermUpdatedEventArgs e)
        {
            UpdatePolynomialFromTerms();
        }

        private void UpdatePolynomialFromTerms()
        {
            var terms = controlContentsPanel.Children.OfType<PolynomialTermControl>().Select(ctrl => ctrl.GetPolynomialTerm()).ToArray();
            _polynomial = new ExtendedArithmetic.Polynomial(terms);
            RaisePolynomialChanged();
            _text = _polynomial.ToString();
            RaiseTextChanged();
        }

        #endregion

        #region Click and Drag

        private bool _isDragging = false;
        private Point _dragStartPosition = default(Point);
        private BigInteger _numericStartValue = 0;
        private PolynomialTermControl _polyTermControl = null;

        private DispatcherTimer _draggingTimer = null;
        private DragPosition _dragPosition = DragPosition.Neither;

        private enum DragPosition
        {
            Neither,
            Top,
            Bottom
        }

        private void PolynomialControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging == false)
            {
                if (this.IsMouseOver)
                {
                    PolynomialTermControl polyTermControl = PolynomialTermHitTest();
                    if (polyTermControl != null)
                    {
                        _dragStartPosition = GetCurrentPointerPosition();
                        _numericStartValue = polyTermControl.Coefficient;
                        _polyTermControl = polyTermControl;
                        _polyTermControl.PreviewMouseMove += PolynomialTermControl_PreviewMouseMove;
                        _polyTermControl.MouseLeave += PolynomialTermControl_MouseLeave;
                        _isDragging = true;
                        e.Handled = true;
                    }
                }
            }
        }

        private void PolynomialControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StopDragging(e);
        }

        private void PolynomialTermControl_MouseLeave(object sender, MouseEventArgs e)
        {
            StopDragging(e);
        }

        private void StopDragging(MouseEventArgs e)
        {
            if (_isDragging == true)
            {
                _draggingTimer.Stop();
                _isDragging = false;
                e.Handled = true;

                bool isUpdateRequired = false;

                int deltaY = CalculateDragYDelta();
                if (deltaY != 0)
                {
                    isUpdateRequired = true;
                }

                _dragStartPosition = default(Point);
                _numericStartValue = BigInteger.Zero;
                _dragPosition = DragPosition.Neither;
                _polyTermControl.PreviewMouseMove -= PolynomialTermControl_PreviewMouseMove;
                _polyTermControl.MouseLeave -= PolynomialTermControl_MouseLeave;
                _polyTermControl = null;

                if (isUpdateRequired)
                {
                    UpdatePolynomialFromTerms();
                }
            }
        }

        private void PolynomialTermControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging == true)
            {
                if (this.IsMouseOver)
                {
                    int deltaY = CalculateDragYDelta();

                    if (DockToParent == false)
                    {
                        int sign = Math.Sign(deltaY);

                        if (sign == 1)
                        {
                            _dragPosition = DragPosition.Top;
                        }
                        else if (sign == -1)
                        {
                            _dragPosition = DragPosition.Bottom;
                        }
                        else if (sign == 0)
                        {
                            _dragPosition = DragPosition.Neither;
                        }

                        if (!_draggingTimer.IsEnabled && _dragPosition != DragPosition.Neither)
                        {
                            _draggingTimer.Start();
                        }
                        else if (_draggingTimer.IsEnabled && _dragPosition == DragPosition.Neither)
                        {
                            _draggingTimer.Stop();
                        }
                    }
                    else
                    {
                        _dragPosition = GetDragPosition();

                        if (_dragPosition == DragPosition.Neither)
                        {
                            if (_draggingTimer.IsEnabled)
                            {
                                _draggingTimer.Stop();
                            }

                            BigInteger newCoeffValue = _numericStartValue + (BigInteger)deltaY;
                            _polyTermControl.Coefficient = newCoeffValue;
                        }
                        else
                        {
                            if (_draggingTimer.IsEnabled == false)
                            {
                                _draggingTimer.Start();
                            }
                        }
                    }

                    e.Handled = true;
                }
            }
        }
        private void DraggingTimer_Tick(object sender, EventArgs e)
        {
            if (_isDragging == false)
            {
                _draggingTimer.Stop();
                return;
            }
            if (this.IsMouseOver == false)
            {
                _draggingTimer.Stop();
                return;
            }

            DragPosition position = GetDragPosition();
            if (position == DragPosition.Top)
            {
                _polyTermControl.Coefficient += 1;
                _numericStartValue += BigInteger.One;
            }
            else if (position == DragPosition.Bottom)
            {
                _polyTermControl.Coefficient -= 1;
                _numericStartValue -= BigInteger.One;
            }
        }

        private int CalculateDragYDelta()
        {
            Point currentMousePosition = GetCurrentPointerPosition();
            return -(int)Math.Round(currentMousePosition.Y - _dragStartPosition.Y);
        }

        private Point GetCurrentPointerPosition()
        {
            return this.PointToScreen(Mouse.GetPosition(this));
        }

        private DragPosition GetDragPosition()
        {
            Point currentPointerPosition = GetCurrentPointerPosition();

            Rect ctrlRect = WPFHelper.GetClientRectangle(_polyTermControl);

            double oneFifth = _polyTermControl.ActualHeight / 5;
            double marginSize = Math.Min(oneFifth, 20);

            double topStart = ctrlRect.Top;
            double topStop = ctrlRect.Top + marginSize;

            if (currentPointerPosition.Y >= topStart && currentPointerPosition.Y <= topStop)
            {
                return DragPosition.Top;
            }

            double bottomStart = ctrlRect.Bottom - marginSize;
            double bottomStop = ctrlRect.Bottom;

            if (currentPointerPosition.Y >= bottomStart && currentPointerPosition.Y <= bottomStop)
            {
                return DragPosition.Bottom;
            }

            return DragPosition.Neither;
        }

        private PolynomialTermControl PolynomialTermHitTest()
        {
            object element = InputHitTest(Mouse.GetPosition(this));

            PolynomialTermControl result = null;
            result = WPFHelper.GetParentOfType<PolynomialTermControl>((DependencyObject)element);

            return result;
        }

        #endregion

    }

}
