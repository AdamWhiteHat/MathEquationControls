using ExtendedArithmetic;
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
		private Polynomial dividendPoly = null;
		private Polynomial modPoly = null;
		private Polynomial quotientPoly = null;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_ContentRendered(object sender, EventArgs e)
		{
			dividendPolynomialCtrl.Polynomial = "36*X^3 + 144*X^2 + 12*X + 13";
			modulusPolynomialCtrl.Polynomial = "X^2 - 1";

			dividendPoly = Polynomial.Parse(dividendPolynomialCtrl.Polynomial);
			modPoly = Polynomial.Parse(modulusPolynomialCtrl.Polynomial);
			Calculate();

			dividendPolynomialCtrl.PolynomialChanged += dividendPolynomialCtrl_PolynomialChanged;
			modulusPolynomialCtrl.PolynomialChanged += modulusPolynomialCtrl_PolynomialChanged;
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

		private void modulusInteger_TextChanged(object sender, TextChangedEventArgs e)
		{
			Calculate();
		}

		private void Calculate()
		{
			if (dividendPoly != null && modPoly != null && !string.IsNullOrWhiteSpace(modulusInteger.Text))
			{
				BigInteger mod = BigInteger.Parse(modulusInteger.Text);

				if (mod != 0)
				{
					quotientPoly = Polynomial.Field.ModMod(dividendPoly, modPoly, mod);
					quotient.Text = quotientPoly.ToString();
				}
			}
		}
	}
}
