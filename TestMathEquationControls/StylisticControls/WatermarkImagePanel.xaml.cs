using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Markup;
using MathEquationControls.CustomControls;

namespace TestMathEquationControls.StylisticControls
{
    /// <summary>
    /// A panel/container for controls that features a blurred, slightly transparent, washed out background image to give your WPF forms some artistic flare.
    /// </summary>
    [TemplatePart(Name = WatermarkImagePanel.ElementGrid, Type = typeof(Grid))]
    [TemplatePart(Name = WatermarkImagePanel.ElementStackPanel, Type = typeof(StackPanel))]
    public class WatermarkImagePanel : StackPanel
    {

        [Bindable(true), Browsable(true), Category("Common")]
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }


        [Bindable(true), Browsable(true), Category("Common")]
        public ImageSource BackgroundImageSource
        {
            get { return (ImageSource)GetValue(BackgroundImageSourceProperty); }
            set { SetValue(BackgroundImageSourceProperty, value); }
        }

        [Bindable(true), Browsable(true), Category("Common")]
        public double ImageOpacity
        {
            get { return (double)GetValue(ImageOpacityProperty); }
            set { SetValue(ImageOpacityProperty, value); }
        }

        [Bindable(true), Browsable(true), Category("Common")]
        public double BlurRadius
        {
            get { return (double)GetValue(BlurRadiusProperty); }
            set { SetValue(BlurRadiusProperty, value); }
        }

        [Bindable(true)]
        public ImageBrush BackgroundImageBrush
        {
            get { return (ImageBrush)GetValue(BackgroundImageBrushProperty); }
            set { SetValue(BackgroundImageBrushProperty, value); }
        }

        #region DependencyProperties

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(WatermarkImagePanel), new FrameworkPropertyMetadata(default(object), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BackgroundImageSourceProperty = DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(WatermarkImagePanel), new FrameworkPropertyMetadata(default(ImageSource)/*new ImageSourceConverter().ConvertFromInvariantString("pack://application:,,,/TestMathEquationControls;/Themes/hexagons-horizontal.bmp")*/, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ImageOpacityProperty = DependencyProperty.Register(nameof(ImageOpacity), typeof(double), typeof(WatermarkImagePanel), new PropertyMetadata(0.5d));

        public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register(nameof(BlurRadius), typeof(double), typeof(WatermarkImagePanel), new PropertyMetadata(15d));

        public static readonly DependencyProperty BackgroundImageBrushProperty = DependencyProperty.Register(nameof(BackgroundImageBrush), typeof(ImageBrush), typeof(WatermarkImagePanel), new FrameworkPropertyMetadata(default(ImageBrush), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(WatermarkImagePanel.RaiseBackgroundImageBrushChanged)));

        #endregion

        #region Events

        public event RoutedPropertyChangedEventHandler<object> ContentChanged
        {
            add { base.AddHandler(ContentChangedEvent, value); }
            remove { base.RemoveHandler(ContentChangedEvent, value); }
        }

        public static readonly RoutedEvent ContentChangedEvent = EventManager.RegisterRoutedEvent(
                                                                                    nameof(ContentChanged),
                                                                                    RoutingStrategy.Bubble,
                                                                                    typeof(RoutedPropertyChangedEventHandler<object>),
                                                                                    typeof(WatermarkImagePanel));

        public event RoutedPropertyChangedEventHandler<ImageSource> BackgroundImageSourceChanged
        {
            add { base.AddHandler(BackgroundImageSourceChangedEvent, value); }
            remove { base.RemoveHandler(BackgroundImageSourceChangedEvent, value); }
        }

        public static readonly RoutedEvent BackgroundImageSourceChangedEvent = EventManager.RegisterRoutedEvent(
                                                                                    nameof(BackgroundImageSourceChanged),
                                                                                    RoutingStrategy.Bubble,
                                                                                    typeof(RoutedPropertyChangedEventHandler<ImageSource>),
                                                                                    typeof(WatermarkImagePanel));

        public event RoutedPropertyChangedEventHandler<double> ImageOpacityChanged
        {
            add { base.AddHandler(ImageOpacityChangedEvent, value); }
            remove { base.RemoveHandler(ImageOpacityChangedEvent, value); }
        }

        public static readonly RoutedEvent ImageOpacityChangedEvent = EventManager.RegisterRoutedEvent(
                                                                                    nameof(ImageOpacityChanged),
                                                                                    RoutingStrategy.Bubble,
                                                                                    typeof(RoutedPropertyChangedEventHandler<double>),
                                                                                    typeof(WatermarkImagePanel));

        public event RoutedPropertyChangedEventHandler<double> BlurRadiusChanged
        {
            add { base.AddHandler(BlurRadiusChangedEvent, value); }
            remove { base.RemoveHandler(BlurRadiusChangedEvent, value); }
        }

        public static readonly RoutedEvent BlurRadiusChangedEvent = EventManager.RegisterRoutedEvent(
                                                                                    nameof(BlurRadiusChanged),
                                                                                    RoutingStrategy.Bubble,
                                                                                    typeof(RoutedPropertyChangedEventHandler<double>),
                                                                                    typeof(WatermarkImagePanel));

        public event RoutedPropertyChangedEventHandler<ImageBrush> BackgroundImageBrushChanged
        {
            add { base.AddHandler(BackgroundImageBrushChangedEvent, value); }
            remove { base.RemoveHandler(BackgroundImageBrushChangedEvent, value); }
        }

        public static readonly RoutedEvent BackgroundImageBrushChangedEvent = EventManager.RegisterRoutedEvent(
                                                                                    nameof(BackgroundImageBrushChanged),
                                                                                    RoutingStrategy.Bubble,
                                                                                    typeof(RoutedPropertyChangedEventHandler<ImageBrush>),
                                                                                    typeof(WatermarkImagePanel));

        #endregion

        #region Raise Events

        protected virtual void RaiseContentChanged(object oldValue, object newValue)
        {
            RoutedPropertyChangedEventArgs<object> e = new RoutedPropertyChangedEventArgs<object>(oldValue, newValue);
            e.RoutedEvent = ContentChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WatermarkImagePanel element = (WatermarkImagePanel)d;
            element.RaiseContentChanged((object)e.OldValue, (object)e.NewValue);
        }

        protected virtual void RaiseBackgroundImageSourceChanged(ImageSource oldValue, ImageSource newValue)
        {
            RoutedPropertyChangedEventArgs<ImageSource> e = new RoutedPropertyChangedEventArgs<ImageSource>(oldValue, newValue);
            e.RoutedEvent = BackgroundImageSourceChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseBackgroundImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WatermarkImagePanel element = (WatermarkImagePanel)d;
            element.RaiseBackgroundImageSourceChanged((ImageSource)e.OldValue, (ImageSource)e.NewValue);
        }

        protected virtual void RaiseImageOpacityChanged(double oldValue, double newValue)
        {
            RoutedPropertyChangedEventArgs<double> e = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue);
            e.RoutedEvent = ImageOpacityChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseImageOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WatermarkImagePanel element = (WatermarkImagePanel)d;
            element.RaiseImageOpacityChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual void RaiseBlurRadiusChanged(double oldValue, double newValue)
        {
            RoutedPropertyChangedEventArgs<double> e = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue);
            e.RoutedEvent = BlurRadiusChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseBlurRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WatermarkImagePanel element = (WatermarkImagePanel)d;
            element.RaiseBlurRadiusChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual void RaiseBackgroundImageBrushChanged(ImageBrush oldValue, ImageBrush newValue)
        {
            RoutedPropertyChangedEventArgs<ImageBrush> e = new RoutedPropertyChangedEventArgs<ImageBrush>(oldValue, newValue);
            e.RoutedEvent = BackgroundImageBrushChangedEvent;
            base.RaiseEvent(e);
        }

        private static void RaiseBackgroundImageBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WatermarkImagePanel element = (WatermarkImagePanel)d;
            element.RaiseBackgroundImageBrushChanged((ImageBrush)e.OldValue, (ImageBrush)e.NewValue);
        }

        #endregion

        #region Private Members

        protected const string ElementGrid = "PART_Grid";
        protected const string ElementStackPanel = "PART_StackPanel";

        private Grid _controlGrid;
        private StackPanel _controlStackPanel;

        #endregion

        #region Constructors / Initialize

        static WatermarkImagePanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WatermarkImagePanel), new FrameworkPropertyMetadata(typeof(WatermarkImagePanel)));
        }

        public WatermarkImagePanel()
        {
            BackgroundImageBrushChanged += WatermarkImagePanel_BackgroundImageBrushChanged;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _controlGrid = GetTemplateChild(ElementGrid) as Grid;
            _controlStackPanel = GetTemplateChild(ElementStackPanel) as StackPanel;
        }

        #endregion

        private void WatermarkImagePanel_BackgroundImageBrushChanged(object sender, RoutedPropertyChangedEventArgs<ImageBrush> e)
        {
            BackgroundImageBrush = e.NewValue;
        }
    }
}
