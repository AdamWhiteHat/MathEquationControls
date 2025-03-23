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

namespace MathEquationControls
{
	[TemplatePart(Name = Fraction.ElementControlStackPanel, Type = typeof(StackPanel))]
	[TemplatePart(Name = Fraction.ElementNumeratorTextBlock, Type = typeof(TextBlock))]
	[TemplatePart(Name = Fraction.ElementDenominatorTextBlock, Type = typeof(TextBlock))]
	public class Fraction : Control
	{
		#region Public Properties

		#region Numerator

		public int Numerator
		{
			get => (int)GetValue(NumeratorProperty);
			set => SetValue(NumeratorProperty, value);
		}


		public static readonly DependencyProperty NumeratorProperty = DependencyProperty.Register(
																				nameof(Numerator),
																				typeof(int),
																				typeof(Fraction),
																				new PropertyMetadata(
																					default(int),
																					new PropertyChangedCallback(Fraction.OnNumeratorChanged)
																				)
																	  );

		public event RoutedPropertyChangedEventHandler<int> NumeratorChanged
		{
			add { base.AddHandler(NumeratorChangedEvent, value); }
			remove { base.RemoveHandler(NumeratorChangedEvent, value); }
		}

		public static readonly RoutedEvent NumeratorChangedEvent = EventManager.RegisterRoutedEvent(
																				nameof(NumeratorChanged),
																				RoutingStrategy.Bubble,
																				typeof(RoutedPropertyChangedEventHandler<int>),
																				typeof(Fraction));

		protected virtual void OnNumeratorChanged(int oldValue, int newValue)
		{
			RoutedPropertyChangedEventArgs<int> e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue);
			e.RoutedEvent = NumeratorChangedEvent;
			base.RaiseEvent(e);
		}

		private static void OnNumeratorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Fraction element = (Fraction)d;
			element.OnNumeratorChanged((int)e.OldValue, (int)e.NewValue);
		}

		#endregion

		#region Denominator

		public int Denominator
		{
			get => (int)GetValue(DenominatorProperty);
			set => SetValue(DenominatorProperty, value);
		}

		public static readonly DependencyProperty DenominatorProperty = DependencyProperty.Register(
																					nameof(Denominator),
																					typeof(int),
																					typeof(Fraction),
																					new PropertyMetadata(
																						default(int),
																						new PropertyChangedCallback(Fraction.OnDenominatorChanged)
																					)
																			);

		public event RoutedPropertyChangedEventHandler<int> DenominatorChanged
		{
			add { base.AddHandler(DenominatorChangedEvent, value); }
			remove { base.RemoveHandler(DenominatorChangedEvent, value); }
		}

		public static readonly RoutedEvent DenominatorChangedEvent = EventManager.RegisterRoutedEvent(
																			nameof(DenominatorChanged),
																			RoutingStrategy.Bubble,
																			typeof(RoutedPropertyChangedEventHandler<int>),
																			typeof(Fraction));
		protected virtual void OnDenominatorChanged(int oldValue, int newValue)
		{
			RoutedPropertyChangedEventArgs<int> e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue);
			e.RoutedEvent = DenominatorChangedEvent;
			base.RaiseEvent(e);
		}

		private static void OnDenominatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Fraction element = (Fraction)d;
			element.OnDenominatorChanged((int)e.OldValue, (int)e.NewValue);
		}

		#endregion

		#endregion

		#region Template Constants & Private Controls

		private const string ElementControlStackPanel = "PART_ControlStackPanel";
		private const string ElementNumeratorTextBlock = "PART_NumeratorTextBlock";
		private const string ElementDenominatorTextBlock = "PART_DenominatorTextBlock";

		private StackPanel controlStackPanel;
		private TextBlock controlNumerator;
		private TextBlock controlDenominator;

		#endregion

		public Fraction()
		{
			this.Loaded += Fraction_Loaded;
		}

		private void Fraction_Loaded(object sender, RoutedEventArgs e)
		{
			SetControls();
		}

		public Fraction(int numerator, int denominator)
			: this()
		{
			this.SetCurrentValue(NumeratorProperty, numerator);
			this.SetCurrentValue(DenominatorProperty, denominator);
		}

		static Fraction()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Fraction), new FrameworkPropertyMetadata(typeof(Fraction)));
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			controlStackPanel = GetTemplateChild(ElementControlStackPanel) as StackPanel;

			controlNumerator = GetTemplateChild(ElementNumeratorTextBlock) as TextBlock;
			if (controlNumerator != null)
			{
				NumeratorChanged += Fraction_NumeratorChanged;
			}

			controlDenominator = GetTemplateChild(ElementDenominatorTextBlock) as TextBlock;
			if (controlDenominator != null)
			{
				DenominatorChanged += Fraction_DenominatorChanged;
			}
		}

		private void Fraction_NumeratorChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			SetControls();
		}

		private void Fraction_DenominatorChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			SetControls();
		}

		private void SetControls()
		{
			if (controlNumerator != null && Numerator != default(int))
			{
				if (controlDenominator != null && Denominator != default(int))
				{
					controlNumerator.Text = Numerator.ToString();
					controlDenominator.Text = Denominator.ToString();
				}
			}
		}
	}
}
