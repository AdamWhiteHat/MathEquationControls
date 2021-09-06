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

namespace MathEquationControl
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

		public string Polynomial
		{
			get => (string)GetValue(PolynomialProperty);
			set => SetValue(PolynomialProperty, value);
		}

		public bool DockToParent
		{
			get => (bool)GetValue(DockToParentProperty);
			set => SetValue(DockToParentProperty, value);
		}

		#endregion

		#region Dependency Properties

		public static readonly DependencyProperty PolynomialProperty = DependencyProperty.Register(
																				nameof(Polynomial),
																				typeof(string),
																				typeof(PolynomialControl),
																				new PropertyMetadata(
																					default(string),
																					new PropertyChangedCallback(PolynomialControl.OnPolynomialChanged)
																				)
																	   );

		public static readonly DependencyProperty DockToParentProperty = DependencyProperty.Register(nameof(DockToParent), typeof(bool), typeof(PolynomialControl));

		#endregion

		#region Events

		public event EventHandler PolynomialUpdated;

		public event RoutedPropertyChangedEventHandler<string> PolynomialChanged
		{
			add { base.AddHandler(PolynomialChangedEvent, value); }
			remove { base.RemoveHandler(PolynomialChangedEvent, value); }
		}

		#region RoutedEvents

		public static readonly RoutedEvent PolynomialChangedEvent = EventManager.RegisterRoutedEvent(
																		nameof(PolynomialChanged),
																		RoutingStrategy.Bubble,
																		typeof(RoutedPropertyChangedEventHandler<string>),
																		typeof(PolynomialControl));

		#endregion

		#region Raise Event Methods

		protected virtual void OnPolynomialUpdated(EventArgs e)
		{
			EventHandler handler = PolynomialUpdated;
			handler?.Invoke(this, e);
		}


		protected virtual void OnPolynomialChanged(string oldValue, string newValue)
		{
			RoutedPropertyChangedEventArgs<string> e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue);
			e.RoutedEvent = PolynomialChangedEvent;
			base.RaiseEvent(e);
		}

		private static void OnPolynomialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PolynomialControl element = (PolynomialControl)d;
			element.OnPolynomialChanged((string)e.OldValue, (string)e.NewValue);
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
		}

		#endregion

		#region Set Controls

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			controlBorder = GetTemplateChild(ElementBorder) as Border;
			controlContentsPanel = GetTemplateChild(ElementContentsPanel) as StackPanel;

			controlBorder.PreviewMouseLeftButtonDown += PolynomialControl_PreviewMouseLeftButtonDown;
			controlBorder.PreviewMouseLeftButtonUp += PolynomialControl_PreviewMouseLeftButtonUp;
			controlBorder.PreviewMouseMove += PolynomialControl_PreviewMouseMove;
			controlBorder.MouseLeave += PolynomialControl_MouseLeave;
		}

		private void PolynomialControl_Loaded(object sender, RoutedEventArgs e)
		{
			this.PolynomialChanged += PolynomialControl_PolynomialChanged;

			if (DockToParent)
			{
				FrameworkElement parent = (FrameworkElement)WPFHelper.GetParent(this);

				double parentHeight = parent.Height;
				double parentActualHeight = parent.ActualHeight;

				this.Height = parentActualHeight;
			}
		}

		private void PolynomialControl_PolynomialChanged(object sender, RoutedPropertyChangedEventArgs<string> e)
		{
			BuidPolynomialTermControls(e.NewValue);
		}

		private void BuidPolynomialTermControls(string polynomial)
		{
			controlContentsPanel.Children.Clear();

			if (string.IsNullOrWhiteSpace(polynomial))
			{
				return;
			}

			ExtendedArithmetic.Polynomial poly = ExtendedArithmetic.Polynomial.Parse(polynomial);

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
			if (_isDragging == false)
			{
				OnPolynomialUpdated(EventArgs.Empty);
			}
		}

		#endregion

		#region Click and Drag

		private bool _isDragging = false;
		private Point _dragStartPosition = default(Point);
		private int _numericStartValue = 0;
		private PolynomialTermControl _polyTermControl = null;

		private void PolynomialControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (_isDragging == false)
			{
				Point pointerLocation = this.PointToScreen(Mouse.GetPosition(this));

				PolynomialTermControl polyTermControl = PolynomialTermHitTest();
				if (polyTermControl != null)
				{
					_polyTermControl = polyTermControl;
					_dragStartPosition = pointerLocation;
					_numericStartValue = _polyTermControl.Coefficient;
					_isDragging = true;
					e.Handled = true;
				}
			}
		}

		private void PolynomialControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			StopDragging(e);
		}

		private void PolynomialControl_MouseLeave(object sender, MouseEventArgs e)
		{
			StopDragging(e);
		}

		private void StopDragging(MouseEventArgs e)
		{
			if (_isDragging == true)
			{
				bool isUpdateRequired = false;

				int deltaY = CalculateDragYDelta();
				if (deltaY != 0)
				{
					isUpdateRequired = true;
				}

				_isDragging = false;
				_dragStartPosition = default(Point);
				_numericStartValue = 0;
				_polyTermControl = null;
				e.Handled = true;

				if (isUpdateRequired)
				{
					OnPolynomialUpdated(EventArgs.Empty);
				}
			}
		}

		private void PolynomialControl_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (_isDragging == true)
			{
				Point currentPosition = this.PointToScreen(Mouse.GetPosition(this));

				PolynomialTermControl polyTermControl = PolynomialTermHitTest();
				if (polyTermControl != null)
				{
					int deltaY = CalculateDragYDelta();

					int newCoeffValue = _numericStartValue + deltaY;

					_polyTermControl.Coefficient = newCoeffValue;

					e.Handled = true;
				}
			}
		}

		private int CalculateDragYDelta()
		{
			Point currentPosition = this.PointToScreen(Mouse.GetPosition(this));
			return -(int)Math.Round(currentPosition.Y - _dragStartPosition.Y);
		}

		private PolynomialTermControl PolynomialTermHitTest()
		{
			object element = InputHitTest(Mouse.GetPosition(this));
			PolynomialTermControl result = null;

			result = WPFHelper.GetParentOfType<PolynomialTermControl>((DependencyObject)element);

			return result;
		}

		private bool IsPointInRect(Point point, Rect rect)
		{
			if (point.X >= rect.Left && point.X <= rect.Right)
			{
				if (point.Y >= rect.Top && point.Y <= rect.Bottom)
				{
					return true;
				}
			}
			return false;
		}

		#endregion

	}
}
