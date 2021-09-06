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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			this.Loaded += MainWindow_Loaded;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private void Add_Click(object sender, RoutedEventArgs e)
		{
			polynomialCtrl.Polynomial = "144*x^2 + 12*x^1 + 1*x^0";
		}

		private void polynomialCtrl_PolynomialUpdated(object sender, EventArgs e)
		{
			outputTextBox.AppendText("PolynomialUpdated!" + Environment.NewLine);
		}
	}
}
