using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using System.Windows.Input;
using System.Numerics;
using System.Windows.Documents;
using System.Windows.Controls;
using MathEquationControls.Primitives;
using System.Windows.Threading;

namespace MathEquationControls.Behaviors
{
    public class KeyInputSetValueBehavior : Behavior<BigRangeBase>
    {
        public static int TimerResolutionMS = 65;

        private bool _isChanging = false;
        private BigInteger _changeDelta = BigInteger.Zero;
        private ChangeDirection _changingDirection = ChangeDirection.Neither;

        private TextBox _textBoxControl;
        private DependencyProperty _valueProperty;
        private DispatcherTimer _changeTimer = null;

        public KeyInputSetValueBehavior(TextBox textBoxControl, DependencyProperty valueProperty, int updateTimerResolutionMS)
            : this(textBoxControl, valueProperty)
        {
            TimerResolutionMS = updateTimerResolutionMS;
        }

        public KeyInputSetValueBehavior(TextBox textBoxControl, DependencyProperty valueProperty)
        {
            _textBoxControl = textBoxControl;
            _valueProperty = valueProperty;
        }

        private enum ChangeDirection
        {
            Neither,
            Up,
            Down
        }

        protected override void OnAttached()
        {
            _textBoxControl.PreviewKeyDown += TextBox_PreviewKeyDown;
            _textBoxControl.KeyUp += TextBox_KeyUp;

            _changeTimer = new DispatcherTimer(DispatcherPriority.Input);
            _changeTimer.Interval = TimeSpan.FromMilliseconds(TimerResolutionMS);
            _changeTimer.Tick += ChangeTimer_Tick;

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            if (_textBoxControl != null)
            {
                _textBoxControl.PreviewKeyDown -= TextBox_PreviewKeyDown;
                _textBoxControl.KeyUp -= TextBox_KeyUp;
            }

            if (_changeTimer != null)
            {
                _changeTimer.Stop();
                _changeTimer.Tick -= ChangeTimer_Tick;
                _changeTimer = null;
            }

            base.OnDetaching();
        }

        protected void AddValue(BigInteger value)
        {
            BigInteger currentValue = (BigInteger)AssociatedObject.GetValue(_valueProperty);
            BigInteger newValue = currentValue + value;
            AssociatedObject.SetValue(_valueProperty, newValue);
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
            {
                return;
            }

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            else if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.PageUp || e.Key == Key.PageDown)
            {
                if (_isChanging)
                {
                    _changeTimer.Stop();
                    _changeDelta = BigInteger.Zero;
                    _changingDirection = ChangeDirection.Neither;
                    _isChanging = false;
                }
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_isChanging)
            {
                return;
            }

            TextBox textBox = sender as TextBox;
            if (textBox == null)
            {
                return;
            }

            if (e.Key == Key.Up)
            {
                _changingDirection = ChangeDirection.Up;
                _isChanging = true;
            }
            else if (e.Key == Key.Down)
            {
                _changingDirection = ChangeDirection.Down;
                _isChanging = true;
            }
            else if (e.Key == Key.PageUp)
            {
                _changingDirection = ChangeDirection.Up;
                _isChanging = true;
            }
            else if (e.Key == Key.PageDown)
            {
                _changingDirection = ChangeDirection.Down;
                _isChanging = true;
            }

            if (_isChanging)
            {
                if (e.Key == Key.Up || e.Key == Key.Down)
                {
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        _changeDelta = AssociatedObject.SmallChange;
                    }
                    else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        _changeDelta = AssociatedObject.MediumChange;
                    }
                    else
                    {
                        _changeDelta = AssociatedObject.UnitaryChange;
                    }
                }
                else if (e.Key == Key.PageUp || e.Key == Key.PageDown)
                {
                    _changeDelta = AssociatedObject.LargeChange;
                }

                ChangeTick();
                _changeTimer.Start();
            }
        }

        private void ChangeTimer_Tick(object? sender, EventArgs e)
        {
            if (!_isChanging)
            {
                _changeTimer.Stop();
            }

            ChangeTick();
        }

        private void ChangeTick()
        {
            if (!_isChanging)
            {
                return;
            }

            if (_changingDirection == ChangeDirection.Up)
            {
                AddValue(_changeDelta);
            }
            else if (_changingDirection == ChangeDirection.Down)
            {
                AddValue(-_changeDelta);
            }
        }

    }
}
