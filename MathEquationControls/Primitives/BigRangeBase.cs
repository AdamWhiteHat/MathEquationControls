using System;
using System.Linq;
using System.Numerics;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Automation;
using MathEquationControls.Converters;

namespace MathEquationControls.Primitives
{
    /// <summary>
    /// The BigRangeBase class is the base class from which all "range-like"
    /// controls derive.  It defines the relevant events and properties, as
    /// well as providing handlers for the relevant input events.
    /// </summary>
    [DefaultEvent("ValueChanged"), DefaultProperty("Value")]
    public abstract class BigRangeBase : Control
    {
        #region Constructors

        /// <summary>
        /// This is the static constructor for the BigRangeBase class.  
        /// Use it to hook the changed notifications needed for visual state changes.
        /// </summary>
        static BigRangeBase()
        {
        }

        /// <summary> Default BigRangeBase constructor </summary>
        /// <remarks> 
        /// Automatic determination of current Dispatcher. 
        /// Use alternative constructor that accepts a Dispatcher for best performance. 
        /// </remarks>
        protected BigRangeBase()
        {
        }

        #endregion Constructors

        #region Events

        /// <summary> Event correspond to Value changed event </summary>
        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(
                "ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<BigInteger>), typeof(BigRangeBase));

        /// <summary> Add / Remove ValueChangedEvent handler </summary>
        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<BigInteger> ValueChanged { add { AddHandler(ValueChangedEvent, value); } remove { RemoveHandler(ValueChangedEvent, value); } }

        #endregion Events

        #region Properties

        #region Minimum Property

        /// <summary>  Minimum restricts the minimum value of the Value property </summary>
        [Bindable(BindableSupport.Yes), Browsable(true), Category("Common")]
        [TypeConverter(typeof(BigIntegerConverter))]
        public BigInteger? Minimum
        {
            get { return (BigInteger?)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        ///     The DependencyProperty for the Minimum property.
        ///     Flags:              none
        ///     Default Value:      0
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                nameof(Minimum), typeof(BigInteger?), typeof(BigRangeBase),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnMinimumChanged)));

        /// <summary> Called when MinimumProperty is changed on "d." </summary>
        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BigRangeBase ctrl = (BigRangeBase)d;

            RangeBaseAutomationPeer peer = UIElementAutomationPeer.FromElement(ctrl) as RangeBaseAutomationPeer;
            if (peer != null)
            {
                peer.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.MinimumProperty, (BigInteger?)e.OldValue, (BigInteger?)e.NewValue);
            }

            ctrl.CoerceValue(MaximumProperty);
            ctrl.CoerceValue(ValueProperty);
            ctrl.OnMinimumChanged((BigInteger?)e.OldValue, (BigInteger?)e.NewValue);
        }

        /// <summary> This method is invoked when the Minimum property changes. </summary>
        /// <param name="oldMinimum">The old value of the Minimum property.</param>
        /// <param name="newMinimum">The new value of the Minimum property.</param>
        protected virtual void OnMinimumChanged(BigInteger? oldMinimum, BigInteger? newMinimum)
        {
        }

        #endregion

        #region Maximum Property

        /// <summary> Maximum restricts the maximum value of the Value property </summary>
        [Bindable(true), Browsable(true), Category("Common")]
        [TypeConverter(typeof(BigIntegerConverter))]
        public BigInteger? Maximum
        {
            get { return (BigInteger?)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        ///     The DependencyProperty for the Maximum property.
        ///     Flags:              none
        ///     Default Value:      1
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                nameof(Maximum), typeof(BigInteger?), typeof(BigRangeBase),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnMaximumChanged), new CoerceValueCallback(CoerceMaximum)));

        /// <summary> Called when MaximumProperty is changed on "d." </summary>
        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BigRangeBase ctrl = (BigRangeBase)d;

            RangeBaseAutomationPeer peer = UIElementAutomationPeer.FromElement(ctrl) as RangeBaseAutomationPeer;
            if (peer != null)
            {
                peer.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.MaximumProperty, (BigInteger?)e.OldValue, (BigInteger?)e.NewValue);
            }

            ctrl.CoerceValue(ValueProperty);
            ctrl.OnMaximumChanged((BigInteger?)e.OldValue, (BigInteger?)e.NewValue);
        }

        /// <summary> This method is invoked when the Maximum property changes. </summary>
        /// <param name="oldMaximum">The old value of the Maximum property.</param>
        /// <param name="newMaximum">The new value of the Maximum property.</param>
        protected virtual void OnMaximumChanged(BigInteger? oldMaximum, BigInteger? newMaximum)
        {
        }

        #endregion

        #region Value Property

        /// <summary>  Value property </summary>
        [Bindable(true), Category("Common")]
        [TypeConverter(typeof(BigIntegerConverter))]
        public BigInteger Value
        {
            get { return (BigInteger)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>   The DependencyProperty for the Value property.
        ///             Flags:              None
        ///             Default Value:      0                        
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register(nameof(Value), typeof(BigInteger), typeof(BigRangeBase),
                        new FrameworkPropertyMetadata(BigInteger.Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.AffectsRender,
                        new PropertyChangedCallback(OnValueChanged), new CoerceValueCallback(ConstrainToRange)));

        /// <summary> Called when ValueID is changed on "d." </summary>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BigRangeBase ctrl = (BigRangeBase)d;
            RangeBaseAutomationPeer peer = UIElementAutomationPeer.FromElement(ctrl) as RangeBaseAutomationPeer;
            if (peer != null)
            {
                peer.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty, (BigInteger)e.OldValue, (BigInteger)e.NewValue);
            }
            ctrl.OnValueChanged((BigInteger)e.OldValue, (BigInteger)e.NewValue);
        }

        /// <summary> This method is invoked when the Value property changes. </summary>
        /// <param name="oldValue">The old value of the Value property.</param>
        /// <param name="newValue">The new value of the Value property.</param>
        protected virtual void OnValueChanged(BigInteger oldValue, BigInteger newValue)
        {
            RoutedPropertyChangedEventArgs<BigInteger> args = new RoutedPropertyChangedEventArgs<BigInteger>(oldValue, newValue);
            args.RoutedEvent = BigRangeBase.ValueChangedEvent;
            RaiseEvent(args);
        }

        #endregion

        #region UnitaryChange Property

        /// <summary> The DependencyProperty for the UnitaryChange property. </summary>
        public static readonly DependencyProperty UnitaryChangeProperty =
            DependencyProperty.Register(nameof(UnitaryChange), typeof(BigInteger), typeof(BigRangeBase),
                new FrameworkPropertyMetadata(BigInteger.One), new ValidateValueCallback(IsValidChange));

        /// <summary> UnitaryChange property </summary>
        [Bindable(true), Browsable(true), Category("Common")]
        [TypeConverter(typeof(BigIntegerConverter))]
        public BigInteger UnitaryChange
        {
            get { return (BigInteger)GetValue(UnitaryChangeProperty); }
            set { SetValue(UnitaryChangeProperty, value); }
        }

        #endregion

        #region SmallChange Property

        /// <summary> The DependencyProperty for the SmallChange property.</summary>
        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register(nameof(SmallChange), typeof(BigInteger), typeof(BigRangeBase),
                new FrameworkPropertyMetadata(new BigInteger(10)), new ValidateValueCallback(IsValidChange));

        /// <summary> SmallChange property </summary>
        [Bindable(true), Browsable(true), Category("Common")]
        [TypeConverter(typeof(BigIntegerConverter))]
        public BigInteger SmallChange
        {
            get { return (BigInteger)GetValue(SmallChangeProperty); }
            set { SetValue(SmallChangeProperty, value); }
        }

        #endregion

        #region MediumChange Property

        /// <summary> The DependencyProperty for the MediumChange property. </summary>
        public static readonly DependencyProperty MediumChangeProperty =
            DependencyProperty.Register(
                nameof(MediumChange), typeof(BigInteger), typeof(BigRangeBase),
                new FrameworkPropertyMetadata(new BigInteger(100)), new ValidateValueCallback(IsValidChange));

        /// <summary> MediumChange property </summary>
        [Bindable(true), Browsable(true), Category("Common")]
        [TypeConverter(typeof(BigIntegerConverter))]
        public BigInteger MediumChange
        {
            get { return (BigInteger)GetValue(MediumChangeProperty); }
            set { SetValue(MediumChangeProperty, value); }
        }

        #endregion

        #region LargeChange Property

        /// <summary> The DependencyProperty for the LargeChange property. </summary>
        public static readonly DependencyProperty LargeChangeProperty =
            DependencyProperty.Register(nameof(LargeChange), typeof(BigInteger), typeof(BigRangeBase),
                new FrameworkPropertyMetadata(new BigInteger(1000)), new ValidateValueCallback(IsValidChange));

        /// <summary> LargeChange property </summary>
        [Bindable(true), Browsable(true), Category("Common")]
        [TypeConverter(typeof(BigIntegerConverter))]
        public BigInteger LargeChange
        {
            get { return (BigInteger)GetValue(LargeChangeProperty); }
            set { SetValue(LargeChangeProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary> Constrains the value to within the Minimum and Maximum properties, if set. </summary>
        internal static object ConstrainToRange(DependencyObject d, object value)
        {
            BigRangeBase ctrl = (BigRangeBase)d;
            BigInteger v = (BigInteger)value;

            if (ctrl.Minimum.HasValue)
            {
                BigInteger min = ctrl.Minimum.Value;
                if (v < min)
                {
                    return min;
                }
            }

            if (ctrl.Maximum.HasValue)
            {
                BigInteger max = ctrl.Maximum.Value;
                if (v > max)
                {
                    return max;
                }
            }

            return value;
        }

        /// <summary> Ensures the Maximum property is not set to be less than the Minimum property, if set. </summary>
        private static object CoerceMaximum(DependencyObject d, object value)
        {
            BigRangeBase ctrl = (BigRangeBase)d;
            BigInteger? v = (BigInteger?)value;
            if (v.HasValue && ctrl.Minimum.HasValue)
            {
                if (v.Value < ctrl.Minimum.Value)
                {
                    return ctrl.Minimum;
                }
            }
            return value;
        }

        /// <summary> Validate input value in BigRangeBase (UnitaryChange, SmallChange, MediumChange and LargeChange). </summary>
        /// <param name="value"></param>
        /// <returns>Returns False if value is a negative value. Otherwise, returns True.</returns>
        private static bool IsValidChange(object value)
        {
            BigInteger d = (BigInteger)value;
            return d >= 0;
        }

        #endregion

        #region Method Overrides

        /// <summary> Gives a string representation of this object. </summary>
        public override string ToString()
        {
            string typeText = this.GetType().ToString();
            string min = "(none)";
            string max = "(none)"; ;
            BigInteger val = 0;

            // Accessing BigRangeBase properties may be thread sensitive
            if (CheckAccess())
            {
                if (Minimum.HasValue)
                {
                    min = Minimum.Value.ToString();
                }
                if (Maximum.HasValue)
                {
                    max = Maximum.Value.ToString();
                }
                val = Value;
            }
            else
            {
                //Not on dispatcher, try posting to the dispatcher with 20ms timeout
                Dispatcher.Invoke(DispatcherPriority.Send, new TimeSpan(0, 0, 0, 0, 20), new DispatcherOperationCallback(delegate (object o)
                {
                    if (Minimum.HasValue)
                    {
                        min = Minimum.Value.ToString();
                    }
                    if (Maximum.HasValue)
                    {
                        max = Maximum.Value.ToString();
                    }
                    val = Value;
                    return null;
                }), null);
            }

            return val.ToString();
        }

        #endregion
    }
}
