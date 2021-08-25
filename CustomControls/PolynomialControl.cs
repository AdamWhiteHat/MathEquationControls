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
	[TemplatePart(Name = PolynomialControl.ElementStackPanel, Type = typeof(WrapPanel))]
	public class PolynomialControl : Control
	{

		#region Public Properties

		public string Polynomial
		{
			get => (string)GetValue(PolynomialProperty);
			set => SetValue(PolynomialProperty, value);
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

		#endregion

		#region Events

		public event RoutedPropertyChangedEventHandler<string> PolynomialChanged
		{
			add { base.AddHandler(PolynomialChangedEvent, value); }
			remove { base.RemoveHandler(PolynomialChangedEvent, value); }
		}

		public static readonly RoutedEvent PolynomialChangedEvent = EventManager.RegisterRoutedEvent(
																		nameof(PolynomialChanged),
																		RoutingStrategy.Bubble,
																		typeof(RoutedPropertyChangedEventHandler<string>),
																		typeof(PolynomialControl));

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

		#region Template Constants & Private Controls

		private const string ElementBorder = "PART_Border";
		private const string ElementStackPanel = "PART_StackPanel";

		private Border controlBorder;
		private WrapPanel controlStackPanel;

		#endregion

		static PolynomialControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PolynomialControl), new FrameworkPropertyMetadata(typeof(PolynomialControl)));
		}

		public PolynomialControl()
		{
			this.IsHitTestVisible = true;
			this.Loaded += PolynomialControl_Loaded;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			controlBorder = GetTemplateChild(ElementBorder) as Border;
			controlStackPanel = GetTemplateChild(ElementStackPanel) as WrapPanel;

			this.PreviewMouseLeftButtonDown += PolynomialControl_PreviewMouseLeftButtonDown;
			this.PreviewMouseLeftButtonUp += PolynomialControl_PreviewMouseLeftButtonUp;
			this.PreviewMouseMove += PolynomialControl_PreviewMouseMove;
			this.MouseLeave += PolynomialControl_MouseLeave;

		}

		private void PolynomialControl_Loaded(object sender, RoutedEventArgs e)
		{
			this.PolynomialChanged += PolynomialControl_PolynomialChanged;		
		}

		private void PolynomialControl_PolynomialChanged(object sender, RoutedPropertyChangedEventArgs<string> e)
		{
			BuidPolynomialTermControls(e.NewValue);
		}

		private void BuidPolynomialTermControls(string polynomial)
		{
			controlStackPanel.Children.Clear();

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
					plusSymbol.Text = "+";
					plusSymbol.VerticalAlignment = VerticalAlignment.Stretch;
					plusSymbol.Width = GridLength.Auto.Value;
					controlStackPanel.Children.Add(plusSymbol);
				}

				PolynomialTermControl termCtrl = new PolynomialTermControl(term);
				termCtrl.Style = (Style)FindResource("PolynomialTermStyle");
				controlStackPanel.Children.Add(termCtrl);
			}
		}



		private bool _isDragging = false;
		private Point _dragStartPosition = default(Point);
		private int _numericStartValue = 0;
		private Rect _clientRect = Rect.Empty;
		private PolynomialTermControl _polyTermControl = null;

		private void PolynomialControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			double width = this.ActualWidth;
			double height = this.ActualHeight;

			Point pointerLocation = this.PointToScreen(Mouse.GetPosition(this));

			PolynomialTermControl polyTermControl = PolynomialTermHitTest();
			Rect polyTermCtrl_ClientRect = polyTermControl.GetClientRectangle();

			if (IsPointInRect(pointerLocation, polyTermCtrl_ClientRect))
			{
				_polyTermControl = polyTermControl;
				_dragStartPosition = pointerLocation;
				_numericStartValue = _polyTermControl.Coefficient;
				_clientRect = polyTermCtrl_ClientRect;
				_isDragging = true;
				e.Handled = true;
			}
		}

		private void PolynomialControl_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (_isDragging)
			{
				Point currentPosition = this.PointToScreen(Mouse.GetPosition(this));

				if (IsPointInRect(currentPosition, _clientRect))
				{
					int deltaY = -(int)Math.Round(currentPosition.Y - _dragStartPosition.Y);

					int newCoeff = _numericStartValue + deltaY;

					_polyTermControl.Coefficient = newCoeff;

					e.Handled = true;
				}
			}
		}

		private void PolynomialControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (_isDragging == true)
			{
				_isDragging = false;
				_dragStartPosition = default(Point);
				_numericStartValue = 0;
				_clientRect = Rect.Empty;
				_polyTermControl = null;
				e.Handled = true;
			}
		}

		private void PolynomialControl_MouseLeave(object sender, MouseEventArgs e)
		{
			if (_isDragging == true)
			{
				_isDragging = false;
				_dragStartPosition = default(Point);
				_numericStartValue = 0;
				_clientRect = Rect.Empty;
				_polyTermControl = null;
				e.Handled = true;
			}
		}

		private PolynomialTermControl PolynomialTermHitTest()
		{
			object element = InputHitTest(Mouse.GetPosition(this));
			PolynomialTermControl result = null;

			while (result == null)
			{
				if (element is PolynomialTermControl)
				{
					result = element as PolynomialTermControl;
				}
				else if (element is FrameworkContentElement)
				{
					element = ((FrameworkContentElement)element).Parent;
				}
				else if (element is FrameworkElement)
				{
					FrameworkElement frameworkElement = element as FrameworkElement;

					if (frameworkElement.Parent != null)
					{
						element = frameworkElement.Parent;
					}
					else
					{
						element = frameworkElement.TemplatedParent;
					}
				}
			}

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



	}
}
