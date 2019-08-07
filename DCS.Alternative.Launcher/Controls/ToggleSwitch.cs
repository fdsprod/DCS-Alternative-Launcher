using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace DCS.Alternative.Launcher.Controls
{
    [TemplateVisualState(Name = NormalState, GroupName = CommonStates)]
    [TemplateVisualState(Name = DisabledState, GroupName = CommonStates)]
    [TemplatePart(Name = SwitchPart, Type = typeof(ToggleButton))]
    public class ToggleSwitch : HeaderedContentControl
    {
        private const string CommonStates = "CommonStates";
        private const string NormalState = "Normal";
        private const string DisabledState = "Disabled";
        private const string SwitchPart = "Switch";


        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ToggleSwitch), new PropertyMetadata(new CornerRadius(0)));

        public static readonly DependencyProperty HeaderFontFamilyProperty =
            DependencyProperty.Register("HeaderFontFamily", typeof(FontFamily), typeof(ToggleSwitch), new PropertyMetadata(SystemFonts.MessageFontFamily));

        public static readonly DependencyProperty OnLabelProperty =
            DependencyProperty.Register("OnLabel", typeof(string), typeof(ToggleSwitch), new PropertyMetadata("On"));
        public static readonly DependencyProperty OffLabelProperty =
            DependencyProperty.Register("OffLabel", typeof(string), typeof(ToggleSwitch), new PropertyMetadata("Off"));

        public static readonly DependencyProperty OnSwitchBrushProperty =
            DependencyProperty.Register("OnSwitchBrush", typeof(Brush), typeof(ToggleSwitch), null);
        public static readonly DependencyProperty OffSwitchBrushProperty =
            DependencyProperty.Register("OffSwitchBrush", typeof(Brush), typeof(ToggleSwitch), null);

        public static readonly DependencyProperty ThumbIndicatorBrushProperty = 
            DependencyProperty.Register("ThumbIndicatorBrush", typeof(Brush), typeof(ToggleSwitch), null);
        public static readonly DependencyProperty ThumbIndicatorDisabledBrushProperty =
            DependencyProperty.Register("ThumbIndicatorDisabledBrush", typeof(Brush), typeof(ToggleSwitch), null);
        public static readonly DependencyProperty ThumbIndicatorWidthProperty =
            DependencyProperty.Register("ThumbIndicatorWidth", typeof(double), typeof(ToggleSwitch), new PropertyMetadata(13d));

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool?), typeof(ToggleSwitch), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, OnIsCheckedChanged));

        public static readonly DependencyProperty ContentDirectionProperty = 
            DependencyProperty.Register("ContentDirection", typeof(FlowDirection), typeof(ToggleSwitch), new PropertyMetadata(FlowDirection.LeftToRight));
        public static readonly DependencyProperty ContentPaddingProperty =
            DependencyProperty.Register(nameof(ContentPadding), typeof(Thickness), typeof(ToggleSwitch), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsParentMeasure));
        public static readonly DependencyProperty ToggleSwitchButtonStyleProperty = 
            DependencyProperty.Register("ToggleSwitchButtonStyle", typeof(Style), typeof(ToggleSwitch), new FrameworkPropertyMetadata(default(Style), FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        private ToggleButton _toggleButton;

        static ToggleSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleSwitch), new FrameworkPropertyMetadata(typeof(ToggleSwitch)));
        }

        public ToggleSwitch()
        {
            PreviewKeyUp += ToggleSwitch_PreviewKeyUp;
            MouseUp += (sender, args) => Keyboard.Focus(this);
        }


        public CornerRadius CornerRadius
        {
            get { return (CornerRadius) GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        [Bindable(true)]
        [Localizability(LocalizationCategory.Font)]
        public FontFamily HeaderFontFamily
        {
            get { return (FontFamily) GetValue(HeaderFontFamilyProperty); }
            set { SetValue(HeaderFontFamilyProperty, value); }
        }

        public string OnLabel
        {
            get { return (string) GetValue(OnLabelProperty); }
            set { SetValue(OnLabelProperty, value); }
        }

        public string OffLabel
        {
            get { return (string) GetValue(OffLabelProperty); }
            set { SetValue(OffLabelProperty, value); }
        }

        public Brush OnSwitchBrush
        {
            get { return (Brush) GetValue(OnSwitchBrushProperty); }
            set { SetValue(OnSwitchBrushProperty, value); }
        }

        public Brush OffSwitchBrush
        {
            get { return (Brush) GetValue(OffSwitchBrushProperty); }
            set { SetValue(OffSwitchBrushProperty, value); }
        }

        public Brush ThumbIndicatorBrush
        {
            get { return (Brush) GetValue(ThumbIndicatorBrushProperty); }
            set { SetValue(ThumbIndicatorBrushProperty, value); }
        }

        public Brush ThumbIndicatorDisabledBrush
        {
            get { return (Brush) GetValue(ThumbIndicatorDisabledBrushProperty); }
            set { SetValue(ThumbIndicatorDisabledBrushProperty, value); }
        }

        public double ThumbIndicatorWidth
        {
            get { return (double) GetValue(ThumbIndicatorWidthProperty); }
            set { SetValue(ThumbIndicatorWidthProperty, value); }
        }

        public FlowDirection ContentDirection
        {
            get { return (FlowDirection) GetValue(ContentDirectionProperty); }
            set { SetValue(ContentDirectionProperty, value); }
        }

        [Bindable(true)]
        public Thickness ContentPadding
        {
            get { return (Thickness) GetValue(ContentPaddingProperty); }
            set { SetValue(ContentPaddingProperty, value); }
        }

        public Style ToggleSwitchButtonStyle
        {
            get { return (Style) GetValue(ToggleSwitchButtonStyleProperty); }
            set { SetValue(ToggleSwitchButtonStyleProperty, value); }
        }

        [TypeConverter(typeof(NullableBoolConverter))]
        public bool? IsChecked
        {
            get { return (bool?) GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public event EventHandler<RoutedEventArgs> Checked;

        public event EventHandler<RoutedEventArgs> Unchecked;

        public event EventHandler<RoutedEventArgs> Indeterminate;

        public event EventHandler<RoutedEventArgs> Click;

        public event EventHandler IsCheckedChanged;

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toggleSwitch = (ToggleSwitch) d;
            if (toggleSwitch._toggleButton != null)
            {
                var oldValue = (bool?) e.OldValue;
                var newValue = (bool?) e.NewValue;

                if (oldValue != newValue)
                {
                    var handler = toggleSwitch.IsCheckedChanged;
                    handler?.Invoke(toggleSwitch, EventArgs.Empty);
                }
            }
        }

        private void ToggleSwitch_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && e.OriginalSource == sender)
            {
                SetCurrentValue(IsCheckedProperty, !IsChecked);
            }
        }

        private void ChangeVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, IsEnabled ? NormalState : DisabledState, useTransitions);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_toggleButton != null)
            {
                BindingOperations.ClearBinding(_toggleButton, ToggleButton.IsCheckedProperty);

                _toggleButton.IsEnabledChanged -= IsEnabledHandler;
                _toggleButton.PreviewMouseUp -= ToggleButtonPreviewMouseUp;
            }

            _toggleButton = GetTemplateChild(SwitchPart) as ToggleButton;

            if (_toggleButton != null)
            {
                var binding = new Binding("IsChecked") {Source = this};

                _toggleButton.SetBinding(ToggleButton.IsCheckedProperty, binding);
                _toggleButton.IsEnabledChanged += IsEnabledHandler;
                _toggleButton.PreviewMouseUp += ToggleButtonPreviewMouseUp;
            }

            ChangeVisualState(false);
        }

        private void ToggleButtonPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(this);
        }

        private void IsEnabledHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            ChangeVisualState(false);
        }

        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{{ToggleSwitch IsChecked={0}, Content={1}}}",
                IsChecked,
                Content
            );
        }
    }
}