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
using TestMathEquationControls.CompositeControls;

namespace TestMathEquationControls
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		public MainWindow()
		{
			InitializeComponent();
		}

		private void link_Polynomial_QuotientField_Click(object sender, RoutedEventArgs e)
		{
			Window quotientField = new PolynomialQuotientField();
			quotientField.Owner = this;
			quotientField.Show();
		}

		private void link_Polynomial_BaseMExpansion_Click(object sender, RoutedEventArgs e)
		{
			Window basemExpansion = new BaseMExpansion();
			basemExpansion.Owner = this;
			basemExpansion.Show();
		}
	}
}
