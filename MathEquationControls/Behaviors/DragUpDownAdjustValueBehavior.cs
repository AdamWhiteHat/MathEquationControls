using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Numerics;
using System.Windows.Controls.Primitives;
using MathEquationControls.Primitives;
using Microsoft.Xaml.Behaviors;
using System.Windows.Threading;
using System.Windows.Controls;

namespace MathEquationControls.Behaviors
{
    public class DragUpDownAdjustValueBehavior : Behavior<BigRangeBase>
    {
        public static int TimerResolutionMS = 65;
        public static int DragDistanceStepSize = 10;
        public static int MaxMultiplier = 4;

        public DragUpDownAdjustValueBehavior(DependencyProperty valueProperty, int updateTimerResolutionMS)
            : this(valueProperty)
        {
            TimerResolutionMS = updateTimerResolutionMS;
        }

        public DragUpDownAdjustValueBehavior(DependencyProperty valueProperty)
            : base()
        {
            _valueProperty = valueProperty;
        }

        private enum DragDirection
        {
            Neither,
            Up,
            Down
        }

        private class DragInfo
        {
            public DragDirection Direction { get; set; }
            public int Multiplier { get; set; }
        }

        private bool _isDragging = false;
        private DragDirection _previousDragDirection = DragDirection.Up;
        private Point _dragStartPosition = default(Point);
        private BigInteger _numericStartValue = 0;

        private DispatcherTimer _draggingTimer = null;
        private DependencyProperty _valueProperty;

        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseDown += AssociatedObject_PreviewMouseDown;
            AssociatedObject.PreviewMouseUp += AssociatedObject_PreviewMouseUp;

            AssociatedObject.MouseEnter += AssociatedObject_MouseEnter;
            AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;

            AssociatedObject.GotKeyboardFocus += AssociatedObject_GotKeyboardFocus;
            AssociatedObject.LostKeyboardFocus += AssociatedObject_LostKeyboardFocus;

            AssociatedObject.PreviewMouseMove += AssociatedObject_PreviewMouseMove;

            _draggingTimer = new DispatcherTimer(DispatcherPriority.Input);
            _draggingTimer.Interval = TimeSpan.FromMilliseconds(TimerResolutionMS);
            _draggingTimer.Tick += DraggingTimer_Tick;

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            if (_draggingTimer != null)
            {
                _draggingTimer.Stop();
                _draggingTimer.Tick -= DraggingTimer_Tick;
                _draggingTimer = null;
            }

            if (AssociatedObject != null)
            {
                AssociatedObject.PreviewMouseDown -= AssociatedObject_PreviewMouseDown;
                AssociatedObject.PreviewMouseUp -= AssociatedObject_PreviewMouseUp;

                AssociatedObject.MouseEnter -= AssociatedObject_MouseEnter;
                AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;

                AssociatedObject.GotKeyboardFocus -= AssociatedObject_GotKeyboardFocus;
                AssociatedObject.LostKeyboardFocus -= AssociatedObject_LostKeyboardFocus;

                AssociatedObject.PreviewMouseMove -= AssociatedObject_PreviewMouseMove;
            }

            base.OnDetaching();
        }

        #region Click and Drag

        private void AssociatedObject_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                InputManager.Current.PrimaryMouseDevice.OverrideCursor = Cursors.ScrollNS;
                StartDragging();
                e.Handled = true;
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                InputManager.Current.PrimaryMouseDevice.OverrideCursor = Cursors.IBeam;
            }
        }

        private void AssociatedObject_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                StopDragging(e);
                e.Handled = true;
            }
        }

        private void AssociatedObject_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!_isDragging)
            {
                if (AssociatedObject.IsKeyboardFocusWithin)
                {
                    InputManager.Current.PrimaryMouseDevice.OverrideCursor = Cursors.IBeam;
                }
                else
                {
                    InputManager.Current.PrimaryMouseDevice.OverrideCursor = Cursors.ScrollNS;
                }
            }
        }

        private void AssociatedObject_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_isDragging)
            {
                if (AssociatedObject.IsKeyboardFocusWithin)
                {
                    InputManager.Current.PrimaryMouseDevice.OverrideCursor = Cursors.IBeam;
                }
                else
                {
                    InputManager.Current.PrimaryMouseDevice.OverrideCursor = Cursors.Arrow;
                }
            }
        }

        private void AssociatedObject_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!_isDragging)
            {
                if (AssociatedObject.IsMouseOver)
                {
                    InputManager.Current.PrimaryMouseDevice.OverrideCursor = Cursors.IBeam;
                }
            }
        }

        private void AssociatedObject_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!_isDragging)
            {
                if (AssociatedObject.IsMouseOver)
                {
                    InputManager.Current.PrimaryMouseDevice.OverrideCursor = Cursors.ScrollNS;
                }
                else
                {
                    InputManager.Current.PrimaryMouseDevice.OverrideCursor = Cursors.Arrow;
                }
            }
        }

        private void StartDragging()
        {
            if (!_isDragging)
            {
                if (AssociatedObject.IsMouseOver)
                {
                    AssociatedObject.CaptureMouse();
                    _dragStartPosition = GetCurrentPointerPosition();
                    _numericStartValue = AssociatedObject.Value;
                    _isDragging = true;
                    if (!_draggingTimer.IsEnabled)
                    {
                        _draggingTimer.Start();
                    }
                    InputManager.Current.PrimaryMouseDevice.OverrideCursor = Cursors.ScrollNS;
                }
            }
        }

        private void StopDragging(MouseEventArgs e)
        {
            if (_isDragging == true)
            {
                _draggingTimer.Stop();
                _isDragging = false;
                AssociatedObject.ReleaseMouseCapture();
                InputManager.Current.PrimaryMouseDevice.OverrideCursor = Cursors.Arrow;
                e.Handled = true;
                /*
                bool isUpdateRequired = false;

                int deltaY = CalculateDragYDelta();
                if (deltaY != 0)
                {
                    isUpdateRequired = true;
                }

                BigInteger newValue = _numericStartValue + deltaY;

                if (isUpdateRequired)
                {
                    AssociatedObject.RaiseEvent(new RoutedPropertyChangedEventArgs<BigInteger>(_numericStartValue, newValue) { RoutedEvent = BigRangeBase.ValueChangedEvent });
                }
                */
                _dragStartPosition = default(Point);
                _numericStartValue = BigInteger.Zero;
            }
        }

        private void AssociatedObject_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging == true)
            {
                if (AssociatedObject.IsMouseOver)
                {
                    int deltaY = CalculateDragYDelta();

                    DragInfo dragInfo = GetDragInformation();

                    if (dragInfo.Direction == DragDirection.Neither)
                    {
                        if (_draggingTimer.IsEnabled)
                        {
                            _draggingTimer.Stop();
                        }

                        BigInteger newCoeffValue = _numericStartValue + (BigInteger)deltaY;
                        AssociatedObject.Value = newCoeffValue;
                    }
                    else
                    {
                        if (_draggingTimer.IsEnabled == false)
                        {
                            _draggingTimer.Start();
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

            DragInfo dragInfo = GetDragInformation();
            if (dragInfo.Direction == DragDirection.Up)
            {
                AssociatedObject.Value += 1 * dragInfo.Multiplier;

                if (dragInfo.Direction != _previousDragDirection)
                {
                    _previousDragDirection = dragInfo.Direction;
                    InputManager.Current.PrimaryMouseDevice.OverrideCursor = Cursors.ScrollN;
                }
            }
            else if (dragInfo.Direction == DragDirection.Down)
            {
                AssociatedObject.Value -= 1 * dragInfo.Multiplier;

                if (dragInfo.Direction != _previousDragDirection)
                {
                    _previousDragDirection = dragInfo.Direction;
                    InputManager.Current.PrimaryMouseDevice.OverrideCursor = Cursors.ScrollS;
                }
            }
            else
            {
                if (dragInfo.Direction != _previousDragDirection)
                {
                    _previousDragDirection = dragInfo.Direction;
                    InputManager.Current.PrimaryMouseDevice.OverrideCursor = Cursors.ScrollNS;
                }
            }
        }

        private int CalculateDragYDelta()
        {
            Point currentMousePosition = GetCurrentPointerPosition();

            int result = 0;

            if (_dragStartPosition.Y > currentMousePosition.Y)
            {
                result = (int)Math.Round(_dragStartPosition.Y - currentMousePosition.Y);
            }
            else if (_dragStartPosition.Y < currentMousePosition.Y)
            {
                result = (int)Math.Round(_dragStartPosition.Y - currentMousePosition.Y);
            }

            return result;
        }

        private Point GetCurrentPointerPosition()
        {
            return AssociatedObject.PointToScreen(Mouse.GetPosition(AssociatedObject));
        }

        private DragInfo GetDragInformation()
        {
            Point currentPointerPosition = GetCurrentPointerPosition();

            Rect ctrlRect = WPFHelper.GetClientRectangle(AssociatedObject);

            double middle = _dragStartPosition.Y;

            double topStart = middle - DragDistanceStepSize;
            double topStop = Math.Max(0, middle - (5 * DragDistanceStepSize));

            if (currentPointerPosition.Y < middle)
            {
                int multiplier = (int)((middle - currentPointerPosition.Y) / DragDistanceStepSize);
                return new DragInfo() { Direction = DragDirection.Up, Multiplier = Math.Min(MaxMultiplier, multiplier) };
            }

            double bottomStart = middle + DragDistanceStepSize;
            double bottomStop = middle + (5 * DragDistanceStepSize);

            if (currentPointerPosition.Y > middle)
            {
                int multiplier = (int)((currentPointerPosition.Y - middle) / DragDistanceStepSize);
                return new DragInfo() { Direction = DragDirection.Down, Multiplier = Math.Min(MaxMultiplier, multiplier) };
            }

            return new DragInfo() { Direction = DragDirection.Neither, Multiplier = 0 };
        }

        private BigRangeBase UIElementHitTest()
        {
            object element = AssociatedObject.InputHitTest(Mouse.GetPosition(AssociatedObject));

            if (element == null)
            {
                return null;
            }
            BigRangeBase result = element as BigRangeBase;

            if (result == null)
            {
                result = WPFHelper.GetParentOfType<BigRangeBase>((DependencyObject)element);
            }

            return result;
        }

        #endregion

    }
}
