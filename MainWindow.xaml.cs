using ExtendedArithmetic;
using MathEquationControl.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MathEquationControl
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public BigInteger Modulus
		{
			get => (BigInteger)GetValue(ModulusProperty);
			set => SetValue(ModulusProperty, value);
		}

		public static readonly DependencyProperty ModulusProperty = DependencyProperty.Register(
																		nameof(Modulus),
																		typeof(BigInteger),
																		typeof(MainWindow),
																		new PropertyMetadata(
																			default(BigInteger),
																			new PropertyChangedCallback(MainWindow.OnModulusChanged)
																		)
															   );


		public event RoutedPropertyChangedEventHandler<BigInteger> ModulusChanged
		{
			add { base.AddHandler(ModulusChangedEvent, value); }
			remove { base.RemoveHandler(ModulusChangedEvent, value); }
		}

		public static readonly RoutedEvent ModulusChangedEvent = EventManager.RegisterRoutedEvent(
																		nameof(ModulusChanged),
																		RoutingStrategy.Bubble,
																		typeof(RoutedPropertyChangedEventHandler<BigInteger>),
																		typeof(MainWindow));

		protected virtual void OnModulusChanged(BigInteger oldValue, BigInteger newValue)
		{
			RoutedPropertyChangedEventArgs<BigInteger> e = new RoutedPropertyChangedEventArgs<BigInteger>(oldValue, newValue);
			e.RoutedEvent = ModulusChangedEvent;
			base.RaiseEvent(e);
		}

		private static void OnModulusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MainWindow element = (MainWindow)d;
			element.OnModulusChanged((BigInteger)e.OldValue, (BigInteger)e.NewValue);
		}

		private Polynomial dividendPoly = null;
		private Polynomial modPoly = null;
		private Polynomial quotientPoly = null;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_ContentRendered(object sender, EventArgs e)
		{
			dividendPolynomialCtrl.Polynomial = "429*X^5 + 221*X^4 + 136*X^3 + 144*X^2 + 112*X + 133";
			modulusPolynomialCtrl.Polynomial = "X^2 - 1";
			modulusInteger.Value = 2;

			dividendPoly = Polynomial.Parse(dividendPolynomialCtrl.Polynomial);
			modPoly = Polynomial.Parse(modulusPolynomialCtrl.Polynomial);

			dividendPolynomialCtrl.PolynomialChanged += dividendPolynomialCtrl_PolynomialChanged;
			modulusPolynomialCtrl.PolynomialChanged += modulusPolynomialCtrl_PolynomialChanged;

			base.DataContext = this;
			modulusInteger.DataContext = this;

			Calculate();
		}

		private void dividendPolynomialCtrl_PolynomialChanged(object sender, EventArgs e)
		{
			dividendPoly = Polynomial.Parse(dividendPolynomialCtrl.Polynomial);
			Calculate();
		}

		private void modulusPolynomialCtrl_PolynomialChanged(object sender, EventArgs e)
		{
			modPoly = Polynomial.Parse(modulusPolynomialCtrl.Polynomial);
			Calculate();
		}

		private void modulusInteger_ValueChanged(object sender, RoutedPropertyChangedEventArgs<BigInteger> e)
		{
			Calculate();
		}

		private void Calculate()
		{
			if (dividendPoly != null && modPoly != null)
			{
				if (modulusInteger.Value == 0 || dividendPoly.Equals(Polynomial.Zero) || modPoly.Equals(Polynomial.Zero))
				{
					quotientPoly = Polynomial.Zero;
					quotient.Text = quotientPoly.ToString();
				}
				else
				{
					quotientPoly = Polynomial.Field.ModMod(dividendPoly, modPoly, modulusInteger.Value);
					quotient.Text = quotientPoly.ToString();
				}
			}
		}

	}
}
