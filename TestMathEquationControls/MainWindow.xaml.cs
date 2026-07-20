using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
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
using System.Windows.Shell;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using TestMathEquationControls.CompositeControls;

namespace TestMathEquationControls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            UpdateWindowBackground();
            UpdateMainWindowVisuals();

            WindowChrome.SetWindowChrome(this,
                new WindowChrome
                {
                    CaptionHeight = 50,
                    CornerRadius = new CornerRadius(12),
                    GlassFrameThickness = new Thickness(-1),
                    ResizeBorderThickness = ResizeMode == ResizeMode.NoResize ? default : new Thickness(4),
                    UseAeroCaptionButtons = true,
                    NonClientFrameEdges = NonClientFrameEdges.None
                }
            );

            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
            this.StateChanged += (s, e) => UpdateMainWindowVisuals();
            this.Activated += (s, e) => UpdateMainWindowVisuals();
            this.Deactivated += (s, e) => UpdateMainWindowVisuals();
            this.RootContentFrame.NavigationService.LoadCompleted += NavigationService_LoadCompleted;
        }

        #region Navigation Commands

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var pageUri = new Uri(e.Uri.ToString(), UriKind.RelativeOrAbsolute);
            RootContentFrame.NavigationService?.Navigate(pageUri);
            e.Handled = true;
        }

        private void CommandBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (RootContentFrame.NavigationService.CanGoBack)
            {
                RootContentFrame.NavigationService?.GoBack();
            }
        }

        private void CommandForward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (RootContentFrame.NavigationService.CanGoForward)
            {
                RootContentFrame.NavigationService?.GoForward();
            }
        }

        private void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
        {
            EnableBackButtonCheck();
        }

        private void EnableBackButtonCheck()
        {
            if (RootContentFrame.NavigationService.CanGoBack)
            {
                BackButton.IsEnabled = true;
            }
            else
            {
                BackButton.IsEnabled = false;
            }
        }

        #endregion

        #region Windows Backdrop

        private void UpdateWindowBackground()
        {
            if ((!IsBackdropDisabled() && !IsBackdropSupported()))
            {
                this.SetResourceReference(BackgroundProperty, "WindowBackground");
            }
        }

        public static bool IsBackdropSupported()
        {
            var os = Environment.OSVersion;
            var version = os.Version;
            return version.Major >= 10 && version.Build >= 22621;
        }

        public static bool IsBackdropDisabled()
        {
            var appContextBackdropData = AppContext.GetData("Switch.System.Windows.Appearance.DisableFluentThemeWindowBackdrop");
            bool disableFluentThemeWindowBackdrop = false;
            if (appContextBackdropData != null)
            {
                disableFluentThemeWindowBackdrop = bool.Parse(Convert.ToString(appContextBackdropData));
            }
            return disableFluentThemeWindowBackdrop;
        }

        #endregion

        #region Window Chrome Events

        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateMainWindowVisuals();
            });
        }

        private void UpdateMainWindowVisuals()
        {
            /*
            //   MainGrid.Margin = default;
            if (WindowState == WindowState.Maximized)
            {
                //     MainGrid.Margin = SystemParameters.HighContrast ? new Thickness(0, 8, 0, 0) : new Thickness(8);
            }

            if (SystemParameters.HighContrast == true)
            {
                HighContrastBorder.SetResourceReference(BorderBrushProperty, IsActive ? SystemColors.ActiveCaptionBrushKey :
                                                                                        SystemColors.InactiveCaptionBrushKey);
                HighContrastBorder.BorderThickness = new Thickness(80, 1, 8, 8);

                WindowChrome wc = WindowChrome.GetWindowChrome(this);
                if (wc is not null)
                {
                    wc.NonClientFrameEdges = NonClientFrameEdges.None;
                }
            }
            else
            {
            */
            HighContrastBorder.SetResourceReference(BorderBrushProperty, IsActive ? SystemColors.ActiveCaptionBrushKey :
                                                                                    SystemColors.InactiveCaptionBrushKey);
            HighContrastBorder.BorderThickness = new Thickness(1);
            HighContrastBorder.Margin = new Thickness(1);

            TitlebarBorder.SetResourceReference(BorderBrushProperty, IsActive ? SystemColors.ActiveCaptionBrushKey :
                                                                                    SystemColors.InactiveCaptionBrushKey);

            var wc = WindowChrome.GetWindowChrome(this);
            if (wc is not null)
            {
                wc.NonClientFrameEdges = NonClientFrameEdges.None;
            }
            //}
        }

        private void UpdateTitleBarButtonsVisibility()
        {
            if (IsBackdropDisabled() || !IsBackdropSupported() || SystemParameters.HighContrast == true)
            {
                MinimizeButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Visible;
                CloseButton.Visibility = Visibility.Visible;
            }
            else
            {
                MinimizeButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Collapsed;
                CloseButton.Visibility = Visibility.Collapsed;
            }
        }

        private void CommandMinimizeWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CommandMaximizeWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                MaximizeButton.Content = "1";
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                MaximizeButton.Content = "2";
            }
        }

        private void CommandCloseWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region INotifyPropertyChanged

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
