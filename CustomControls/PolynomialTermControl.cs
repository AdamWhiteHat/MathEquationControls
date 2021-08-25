using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
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
using ExtendedArithmetic;

namespace MathEquationControl
{
	/// <summary>
	/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
	///
	/// Step 1a) Using this custom control in a XAML file that exists in the current project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:MathEquationControl"
	///
	///
	/// Step 1b) Using this custom control in a XAML file that exists in a different project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:MathEquationControl;assembly=MathEquationControl"
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
	///     <MyNamespace:PolynomialTerm/>
	///
	/// </summary>
	[TemplatePart(Name = PolynomialTermControl.ElementBorder, Type = typeof(Border))]
	[TemplatePart(Name = PolynomialTermControl.ElementRichTextBox, Type = typeof(RichTextBox))]
	[TemplatePart(Name = PolynomialTermControl.ElementFlowDocument, Type = typeof(FlowDocument))]
	//[TemplatePart(Name = PolynomialTermControl.ElementSection, Type = typeof(Section))]
	[TemplatePart(Name = PolynomialTermControl.ElementParagraph, Type = typeof(Paragraph))]
	[TemplatePart(Name = PolynomialTermControl.ElementCoefficient, Type = typeof(Run))]
	[TemplatePart(Name = PolynomialTermControl.ElementMultiplicationSymbol, Type = typeof(Run))]
	[TemplatePart(Name = PolynomialTermControl.ElementIndeteminant, Type = typeof(Run))]
	[TemplatePart(Name = PolynomialTermControl.ElementExponent, Type = typeof(Run))]
	public class PolynomialTermControl : Control
	{
		#region Public Properties

		public int Coefficient
		{
			get => (int)GetValue(CoefficientProperty);
			set => SetValue(CoefficientProperty, value);
		}

		public int Exponent
		{
			get => (int)GetValue(ExponentProperty);
			set => SetValue(ExponentProperty, value);
		}

		#endregion

		#region Dependency Properties

		public static readonly DependencyProperty CoefficientProperty = DependencyProperty.Register(
																				nameof(Coefficient),
																				typeof(int),
																				typeof(PolynomialTermControl),
																				new PropertyMetadata(
																					default(int),
																					new PropertyChangedCallback(PolynomialTermControl.OnCoefficientChanged)
																				)
																	   );

		public static readonly DependencyProperty ExponentProperty = DependencyProperty.Register(
																				nameof(Exponent),
																				typeof(int),
																				typeof(PolynomialTermControl),
																				new PropertyMetadata(
																					default(int),
																					new PropertyChangedCallback(PolynomialTermControl.OnExponentChanged)
																				)
																		);

		#endregion

		#region Events

		public event RoutedPropertyChangedEventHandler<int> CoefficientChanged
		{
			add { base.AddHandler(CoefficientChangedEvent, value); }
			remove { base.RemoveHandler(CoefficientChangedEvent, value); }
		}

		public event RoutedPropertyChangedEventHandler<int> ExponentChanged
		{
			add { base.AddHandler(ExponentChangedEvent, value); }
			remove { base.RemoveHandler(ExponentChangedEvent, value); }
		}

		#region RoutedEvents

		public static readonly RoutedEvent CoefficientChangedEvent = EventManager.RegisterRoutedEvent(
																			nameof(CoefficientChanged),
																			RoutingStrategy.Bubble,
																			typeof(RoutedPropertyChangedEventHandler<int>),
																			typeof(PolynomialTermControl));

		public static readonly RoutedEvent ExponentChangedEvent = EventManager.RegisterRoutedEvent(
																		nameof(ExponentChanged),
																		RoutingStrategy.Bubble,
																		typeof(RoutedPropertyChangedEventHandler<int>),
																		typeof(PolynomialTermControl));

		#endregion

		#region Raise Event Methods

		protected virtual void OnCoefficientChanged(int oldValue, int newValue)
		{
			RoutedPropertyChangedEventArgs<int> e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue);
			e.RoutedEvent = CoefficientChangedEvent;
			base.RaiseEvent(e);
		}

		private static void OnCoefficientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PolynomialTermControl element = (PolynomialTermControl)d;
			element.OnCoefficientChanged((int)e.OldValue, (int)e.NewValue);
		}

		protected virtual void OnExponentChanged(int oldValue, int newValue)
		{
			RoutedPropertyChangedEventArgs<int> e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue);
			e.RoutedEvent = ExponentChangedEvent;
			base.RaiseEvent(e);
		}

		private static void OnExponentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PolynomialTermControl element = (PolynomialTermControl)d;
			element.OnExponentChanged((int)e.OldValue, (int)e.NewValue);
		}

		#endregion

		#endregion

		#region Template Constants & Private Controls

		private const string ElementBorder = "PART_Border";
		private const string ElementRichTextBox = "PART_RichTextBox";
		private const string ElementFlowDocument = "PART_FlowDocument";
		//private const string ElementSection = "PART_Section";
		private const string ElementParagraph = "PART_Paragraph";
		private const string ElementCoefficient = "PART_Coefficient";
		private const string ElementMultiplicationSymbol = "PART_MultiplicationSymbol";
		private const string ElementIndeteminant = "PART_Indeteminant";
		private const string ElementExponent = "PART_Exponent";

		private Border controlBorder;
		private RichTextBox controlRichTextBox;
		private FlowDocument controlFlowDocument;
		//private Section controlSection;
		private Paragraph controlParagraph;
		private Run controlCoefficient;
		private Run controlMultiplicationSymbol;
		private Run controlIndeteminant;
		private Run controlExponent;

		#endregion

		static PolynomialTermControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PolynomialTermControl), new FrameworkPropertyMetadata(typeof(PolynomialTermControl)));
		}

		public PolynomialTermControl()
		{
			this.Loaded += PolynomialTermControl_Loaded;
			//this.PreviewMouseLeftButtonDown += PolynomialTermControl_PreviewMouseLeftButtonDown;
			//this.PreviewMouseLeftButtonUp += PolynomialTermControl_PreviewMouseLeftButtonUp;
			//this.PreviewMouseMove += PolynomialTermControl_PreviewMouseMove;
		}

		public PolynomialTermControl(Term polynomalTerm)
			: this()
		{
			this.Coefficient = (int)polynomalTerm.CoEfficient;
			this.Exponent = polynomalTerm.Exponent;
		}

		public Term GetPolynomialTerm()
		{
			return new Term(this.Coefficient, this.Exponent);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			controlBorder = GetTemplateChild(ElementBorder) as Border;
			controlRichTextBox = GetTemplateChild(ElementRichTextBox) as RichTextBox;
			controlFlowDocument = GetTemplateChild(ElementFlowDocument) as FlowDocument;
			//controlSection = GetTemplateChild(ElementSection) as Section;
			controlParagraph = GetTemplateChild(ElementParagraph) as Paragraph;
			controlIndeteminant = GetTemplateChild(ElementIndeteminant) as Run;
			controlMultiplicationSymbol = GetTemplateChild(ElementMultiplicationSymbol) as Run;

			//if (controlRichTextBox != null)
			//{
			//	controlRichTextBox.PreviewMouseLeftButtonDown += PolynomialTermControl_PreviewMouseLeftButtonDown;
			//	controlRichTextBox.PreviewMouseLeftButtonUp += PolynomialTermControl_PreviewMouseLeftButtonUp;
			//}

			if (controlBorder != null)
			{
				controlBorder.PreviewMouseLeftButtonDown += PolynomialTermControl_PreviewMouseLeftButtonDown;
				controlBorder.PreviewMouseLeftButtonUp += PolynomialTermControl_PreviewMouseLeftButtonUp;
				controlBorder.PreviewMouseMove += PolynomialTermControl_PreviewMouseMove;
			}

			controlCoefficient = GetTemplateChild(ElementCoefficient) as Run;
			if (controlCoefficient != null)
			{
				CoefficientChanged += PolynomialTermControl_CoefficientChanged;
			}

			controlExponent = GetTemplateChild(ElementExponent) as Run;
			if (controlExponent != null)
			{
				ExponentChanged += PolynomialTermControl_ExponentChanged;
			}
		}
		private void PolynomialTermControl_Loaded(object sender, RoutedEventArgs e)
		{
			SetControls();
		}

		private void PolynomialTermControl_ExponentChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			SetControls();
		}

		private void PolynomialTermControl_CoefficientChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			SetControls();
		}

		private void SetControls()
		{
			if (controlExponent != null && Exponent != default(int))
			{
				if (Exponent == 0)
				{
					controlIndeteminant.Text = " ";
					controlMultiplicationSymbol.Text = " ";
					controlExponent.Text = " ";
				}
				else if (Exponent == 1)
				{
					controlExponent.Text = " ";
				}
				else
				{
					controlExponent.Text = Exponent.ToString();
				}
			}

			if (controlCoefficient != null && Coefficient != default(int))
			{
				if (Coefficient == 1 && Exponent != 0)
				{
					controlCoefficient.Text = " ";
				}
				else
				{
					controlCoefficient.Text = Coefficient.ToString();
				}
			}

			string termString = $"{controlCoefficient.Text}*x{controlExponent.Text}";

			double calculatedWidth = MeasureString(termString).Width;

			double padding_1 = controlFlowDocument.PagePadding.Left + controlFlowDocument.PagePadding.Right;
			double padding_2 = controlRichTextBox.Padding.Left + controlRichTextBox.Padding.Right;
			double margin = controlRichTextBox.Margin.Left + controlRichTextBox.Margin.Right;

			double totalWidth = calculatedWidth + padding_1 + padding_2 + margin;

			controlRichTextBox.Width = totalWidth;
		}

		private Size MeasureString(string candidate)
		{
			var formattedText = new FormattedText(
				candidate,
				CultureInfo.CurrentCulture,
				FlowDirection.LeftToRight,
				new Typeface(controlRichTextBox.FontFamily, controlRichTextBox.FontStyle, controlRichTextBox.FontWeight, controlRichTextBox.FontStretch),
				controlRichTextBox.FontSize,
				Brushes.Black,
				new NumberSubstitution(),
				1);

			return new Size(formattedText.Width, formattedText.Height);
		}

		private bool _isDragging = false;
		private Point _dragStartPosition = default(Point);
		private int _numericStartValue = 0;

		private void PolynomialTermControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Point pointerLocation = this.PointToScreen(Mouse.GetPosition(this));

			Point controlLocation = this.PointToScreen(this.TransformToVisual(this).Transform(new Point(0, 0)));

			double from = controlLocation.X;
			double to = controlLocation.X + this.ActualWidth;

			if (pointerLocation.X >= from && pointerLocation.X <= to)
			{
				_dragStartPosition = pointerLocation;
				_isDragging = true;
				_numericStartValue = Coefficient;
				e.Handled = true;
			}
		}

		private void PolynomialTermControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (_isDragging == true)
			{
				_isDragging = false;
				e.Handled = true;
			}
		}

		private void PolynomialTermControl_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (_isDragging)
			{
				Point currentPosition = this.PointToScreen(Mouse.GetPosition(this));

				int deltaY = -(int)Math.Round(currentPosition.Y - _dragStartPosition.Y);

				int newCoeff = _numericStartValue + deltaY;

				Coefficient = newCoeff;

				e.Handled = true;
			}
		}

	}
}
