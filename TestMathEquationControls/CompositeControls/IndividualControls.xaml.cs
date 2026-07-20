using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MathEquationControls.CustomControls.Algebra;

namespace TestMathEquationControls.CompositeControls
{
    /// <summary>
    /// Interaction logic for IndividualControls.xaml
    /// </summary>
    public partial class IndividualControls : Page
    {
        public IndividualControls()
        {
            InitializeComponent();
            this.Loaded += IndividualControls_Loaded;
        }

        private void IndividualControls_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
