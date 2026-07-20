using MathEquationControls.CustomControls.Polynomial;
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

namespace TestMathEquationControls.CompositeControls
{
    /// <summary>
    /// Interaction logic for BaseMExpansion.xaml
    /// </summary>
    public partial class BaseMExpansion : Page, INotifyPropertyChanged
    {
        public ExtendedArithmetic.Polynomial Polynomial
        {
            get => polyControl.Polynomial;
            set
            {
                if (value != polyControl.Polynomial)
                {
                    polyControl.Polynomial = value;
                    RaisePropertyChanged();
                }
            }
        }

        #region Dependency Properties

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public BaseMExpansion()
        {
            InitializeComponent();

            this.Loaded += Window_Loaded;
            this.Unloaded += Window_Unloaded;
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            polyControl.SetValue(PolynomialControl.IndeterminateValueProperty, new BigInteger(12));
            polyControl.SetValue(PolynomialControl.DegreeProperty, new BigInteger(5));
            polyControl.SetValue(PolynomialControl.TargetValueProperty, new BigInteger(3218148));

            RegisterEvents();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (EventsRegistered)
            {
                // Make sure to unregister events here
            }
        }

        private bool EventsRegistered = false;
        private void RegisterEvents()
        {
            if (!EventsRegistered)
            {
                // None to register

                EventsRegistered = true;
            }
        }

    }
}
