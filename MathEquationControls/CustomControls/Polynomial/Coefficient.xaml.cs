using ExtendedArithmetic;
using MathEquationControls.Behaviors;
using MathEquationControls.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MathEquationControls.Converters;

namespace MathEquationControls.CustomControls.Polynomial
{
    public class Coefficient : NumberBox
    {
        static Coefficient()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Coefficient), new FrameworkPropertyMetadata(typeof(Coefficient)));
        }

        public Coefficient()
            : base()
        {            
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();       
        }
    }
}
