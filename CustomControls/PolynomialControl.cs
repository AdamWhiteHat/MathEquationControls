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
	[TemplatePart(Name = PolynomialControl.ElementStackPanel, Type = typeof(StackPanel))]
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

		private const string ElementStackPanel = "PART_StackPanel";

		private StackPanel controlStackPanel;

		#endregion

		static PolynomialControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PolynomialControl), new FrameworkPropertyMetadata(typeof(PolynomialControl)));
		}

		public PolynomialControl()
		{
			this.Loaded += PolynomialControl_Loaded;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			controlStackPanel = GetTemplateChild(ElementStackPanel) as StackPanel;
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

			foreach (ExtendedArithmetic.Term term in poly.Terms.Reverse())
			{
				PolynomialTermControl termCtrl = new PolynomialTermControl(term);
				//termCtrl.BorderBrush = Brushes.Transparent;
				//termCtrl.BorderThickness = new Thickness(0);
				termCtrl.Style = (Style)FindResource("PolynomialTermStyle");
				controlStackPanel.Children.Add(termCtrl);
			}
		}

	}
}
