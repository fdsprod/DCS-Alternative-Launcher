using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DCS.Alternative.Launcher.Controls
{
    [TemplatePart(Name = PART_BackgroundTranslate, Type = typeof(TranslateTransform))]
    [TemplatePart(Name = PART_DraggingThumb, Type = typeof(Thumb))]
    [TemplatePart(Name = PART_SwitchTrack, Type = typeof(Grid))]
    [TemplatePart(Name = PART_ThumbIndicator, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_ThumbTranslate, Type = typeof(TranslateTransform))]
    public class ToggleSwitchButton : ToggleButton
    {
        private const string PART_BackgroundTranslate = "PART_BackgroundTranslate";
        private const string PART_DraggingThumb = "PART_DraggingThumb";
        private const string PART_SwitchTrack = "PART_SwitchTrack";
        private const string PART_ThumbIndicator = "PART_ThumbIndicator";
        private const string PART_ThumbTranslate = "PART_ThumbTranslate";

        public static readonly DependencyProperty OnSwitchBrushProperty =
            DependencyProperty.Register("OnSwitchBrush", typeof(Brush), typeof(ToggleSwitchButton), null);

        public static readonly DependencyProperty OffSwitchBrushProperty =
            DependencyProperty.Register("OffSwitchBrush", typeof(Brush), typeof(ToggleSwitchButton), null);

        public static readonly DependencyProperty ThumbIndicatorBrushProperty =
            DependencyProperty.Register("ThumbIndicatorBrush", typeof(Brush), typeof(ToggleSwitchButton), null);

        public static readonly DependencyProperty ThumbIndicatorDisabledBrushProperty =
            DependencyProperty.Register("ThumbIndicatorDisabledBrush", typeof(Brush), typeof(ToggleSwitchButton), null);

        public static readonly DependencyProperty ThumbIndicatorWidthProperty =
            DependencyProperty.Register("ThumbIndicatorWidth", typeof(double), typeof(ToggleSwitchButton), new PropertyMetadata(13d));

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ToggleSwitchButton), new PropertyMetadata(new CornerRadius(0)));

        private readonly PropertyChangeNotifier _isCheckedPropertyChangeNotifier;

        private TranslateTransform _backgroundTranslate;
        private Thumb _draggingThumb;
        private bool _isDragging;
        private Border _offSwitchBorder;

        private double? _lastDragPosition;
        private Grid _switchTrack;

        private DoubleAnimation _thumbAnimation;
        private FrameworkElement _thumbIndicator;
        private TranslateTransform _thumbTranslate;

        static ToggleSwitchButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleSwitchButton), new FrameworkPropertyMetadata(typeof(ToggleSwitchButton)));
        }

        public ToggleSwitchButton()
        {
            _isCheckedPropertyChangeNotifier = new PropertyChangeNotifier(this, IsCheckedProperty);
            _isCheckedPropertyChangeNotifier.ValueChanged += IsCheckedPropertyChangeNotifierValueChanged;
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius) GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
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

        private void IsCheckedPropertyChangeNotifierValueChanged(object sender, EventArgs e)
        {
            UpdateThumb();
        }

        private void UpdateThumb()
        {
            if (_thumbTranslate != null && _switchTrack != null && _thumbIndicator != null)
            {
                var destination = IsChecked.GetValueOrDefault() ? ActualWidth - (_switchTrack.Margin.Left + _switchTrack.Margin.Right + _thumbIndicator.ActualWidth + _thumbIndicator.Margin.Left + _thumbIndicator.Margin.Right) : 0;

                _thumbAnimation = new DoubleAnimation();
                _thumbAnimation.To = destination;
                _thumbAnimation.Duration = TimeSpan.FromMilliseconds(500);
                _thumbAnimation.EasingFunction = new ExponentialEase {Exponent = 9};
                _thumbAnimation.FillBehavior = FillBehavior.Stop;

                AnimationTimeline currentAnimation = _thumbAnimation;
                _thumbAnimation.Completed += (sender, e) =>
                {
                    if (_thumbAnimation != null && currentAnimation == _thumbAnimation)
                    {
                        _thumbTranslate.X = destination;
                        _thumbAnimation = null;
                    }
                };
                _thumbTranslate.BeginAnimation(TranslateTransform.XProperty, _thumbAnimation);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _offSwitchBorder = GetTemplateChild("PART_OffSwitchBorder") as Border;
            _backgroundTranslate = GetTemplateChild(PART_BackgroundTranslate) as TranslateTransform;
            _draggingThumb = GetTemplateChild(PART_DraggingThumb) as Thumb;
            _switchTrack = GetTemplateChild(PART_SwitchTrack) as Grid;
            _thumbIndicator = GetTemplateChild(PART_ThumbIndicator) as FrameworkElement;
            _thumbTranslate = GetTemplateChild(PART_ThumbTranslate) as TranslateTransform;

            if (_thumbIndicator != null && _thumbTranslate != null && _backgroundTranslate != null)
            {
                Binding translationBinding;
                translationBinding = new Binding("X");
                translationBinding.Source = _thumbTranslate;
                BindingOperations.SetBinding(_backgroundTranslate, TranslateTransform.XProperty, translationBinding);
            }

            if (_draggingThumb != null && _thumbIndicator != null && _thumbTranslate != null)
            {
                _draggingThumb.DragStarted -= onDraggingThumb_DragStarted;
                _draggingThumb.DragDelta -= onDraggingThumb_DragDelta;
                _draggingThumb.DragCompleted -= onDraggingThumb_DragCompleted;
                _draggingThumb.DragStarted += onDraggingThumb_DragStarted;
                _draggingThumb.DragDelta += onDraggingThumb_DragDelta;
                _draggingThumb.DragCompleted += onDraggingThumb_DragCompleted;
                if (_switchTrack != null)
                {
                    _switchTrack.SizeChanged -= onSwitchTrack_SizeChanged;
                    _switchTrack.SizeChanged += onSwitchTrack_SizeChanged;
                }
            }
        }

        private void SetIsPressed(bool pressed)
        {
            // we can't use readonly IsPressedProperty
            typeof(ToggleButton).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(this, new object[] {pressed});
        }

        private void onDraggingThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (!IsPressed)
                {
                    SetIsPressed(true);
                }
            }

            if (_thumbTranslate != null)
            {
                _thumbTranslate.BeginAnimation(TranslateTransform.XProperty, null);

                var destination = IsChecked.GetValueOrDefault() ? ActualWidth - (_switchTrack.Margin.Left + _switchTrack.Margin.Right + _thumbIndicator.ActualWidth + _thumbIndicator.Margin.Left + _thumbIndicator.Margin.Right) : 0;
                _thumbTranslate.X = destination;
                _thumbAnimation = null;
            }

            _lastDragPosition = _thumbTranslate.X;
            _isDragging = false;
        }

        private void onDraggingThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_lastDragPosition.HasValue)
            {
                if (Math.Abs(e.HorizontalChange) > 3)
                {
                    _isDragging = true;
                }

                if (_switchTrack != null && _thumbIndicator != null)
                {
                    var lastDragPosition = _lastDragPosition.Value;
                    _thumbTranslate.X = Math.Min(ActualWidth - (_switchTrack.Margin.Left + _switchTrack.Margin.Right + _thumbIndicator.ActualWidth + _thumbIndicator.Margin.Left + _thumbIndicator.Margin.Right), Math.Max(0, lastDragPosition + e.HorizontalChange));
                }
            }
        }

        private void onDraggingThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            SetIsPressed(false);

            _lastDragPosition = null;

            if (!_isDragging)
            {
                OnClick();
            }
            else if (_thumbTranslate != null && _switchTrack != null)
            {
                if (!IsChecked.GetValueOrDefault() && _thumbTranslate.X + 6.5 >= _switchTrack.ActualWidth / 2)
                {
                    OnClick();
                }
                else if (IsChecked.GetValueOrDefault() && _thumbTranslate.X + 6.5 <= _switchTrack.ActualWidth / 2)
                {
                    OnClick();
                }

                UpdateThumb();
            }
        }

        private void onSwitchTrack_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_thumbTranslate == null || _switchTrack == null || _thumbIndicator == null)
            {
                return;
            }

            var destination = IsChecked.GetValueOrDefault() ? ActualWidth - (_switchTrack.Margin.Left + _switchTrack.Margin.Right + _thumbIndicator.ActualWidth + _thumbIndicator.Margin.Left + _thumbIndicator.Margin.Right) : 0;

            _thumbTranslate.X = destination;
        }
    }
}