namespace DCS.Alternative.Launcher.Controls
{
    //public delegate void RangeSelectionChangedEventHandler(object sender, RangeSelectionChangedEventArgs e);
    //public delegate void RangeParameterChangedEventHandler(object sender, RangeParameterChangedEventArgs e);

    //[DefaultEvent("RangeSelectionChanged")]
    //[TemplatePart(Name = "PART_Container", Type = typeof(FrameworkElement))]
    //[TemplatePart(Name = "PART_RangeSliderContainer", Type = typeof(StackPanel))]
    //[TemplatePart(Name = "PART_LeftEdge", Type = typeof(RepeatButton))]
    //[TemplatePart(Name = "PART_LeftThumb", Type = typeof(Thumb))]
    //[TemplatePart(Name = "PART_MiddleThumb", Type = typeof(Thumb))]
    //[TemplatePart(Name = "PART_RightThumb", Type = typeof(Thumb))]
    //[TemplatePart(Name = "PART_RightEdge", Type = typeof(RepeatButton))]
    //public class RangeSlider : RangeBase
    //{
    //    static RangeSlider()
    //    {
    //        DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(typeof(RangeSlider)));
    //        MinimumProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, MinPropertyChangedCallback, CoerceMinimum));
    //        MaximumProperty.OverrideMetadata(typeof(RangeSlider), new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsMeasure, MaxPropertyChangedCallback, CoerceMaximum));
    //    }

    //    public RangeSlider()
    //    {
    //        CommandBindings.Add(new CommandBinding(MoveBack, MoveBackHandler));
    //        CommandBindings.Add(new CommandBinding(MoveForward, MoveForwardHandler));
    //        CommandBindings.Add(new CommandBinding(MoveAllForward, MoveAllForwardHandler));
    //        CommandBindings.Add(new CommandBinding(MoveAllBack, MoveAllBackHandler));

    //        actualWidthPropertyChangeNotifier = new PropertyChangeNotifier(this, ActualWidthProperty);
    //        actualWidthPropertyChangeNotifier.ValueChanged += (s, e) => ReCalculateSize();
    //        actualHeightPropertyChangeNotifier = new PropertyChangeNotifier(this, ActualHeightProperty);
    //        actualHeightPropertyChangeNotifier.ValueChanged += (s, e) => ReCalculateSize();

    //        _timer = new DispatcherTimer();
    //        _timer.Tick += MoveToNextValue;
    //        _timer.Interval = TimeSpan.FromMilliseconds(Interval);
    //    }

    //    public double MovableRange
    //    {
    //        get { return Maximum - Minimum - MinRange; }
    //    }

    //    protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
    //    {
    //        CoerceValue(SelectionStartProperty);
    //        ReCalculateSize();
    //    }

    //    protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
    //    {
    //        CoerceValue(SelectionStartProperty);
    //        CoerceValue(SelectionEndProperty);
    //        ReCalculateSize();
    //    }

    //    private void MoveAllBackHandler(object sender, ExecutedRoutedEventArgs e)
    //    {
    //        ResetSelection(true);
    //    }

    //    private void MoveAllForwardHandler(object sender, ExecutedRoutedEventArgs e)
    //    {
    //        ResetSelection(false);
    //    }

    //    private void MoveBackHandler(object sender, ExecutedRoutedEventArgs e)
    //    {
    //        MoveSelection(true);
    //    }

    //    private void MoveForwardHandler(object sender, ExecutedRoutedEventArgs e)
    //    {
    //        MoveSelection(false);
    //    }

    //    private static void MoveThumb(FrameworkElement x, FrameworkElement y, double change, Orientation orientation)
    //    {
    //        var direction = Direction.Increase;
    //        MoveThumb(x, y, change, orientation, out direction);
    //    }

    //    private static void MoveThumb(FrameworkElement x, FrameworkElement y, double change, Orientation orientation, out Direction direction)
    //    {
    //        direction = Direction.Increase;
    //        if (orientation == Orientation.Horizontal)
    //        {
    //            direction = change < 0 ? Direction.Decrease : Direction.Increase;
    //            MoveThumbHorizontal(x, y, change);
    //        }
    //        else if (orientation == Orientation.Vertical)
    //        {
    //            direction = change < 0 ? Direction.Increase : Direction.Decrease;
    //            MoveThumbVertical(x, y, change);
    //        }
    //    }

    //    private static void MoveThumbHorizontal(FrameworkElement x, FrameworkElement y, double horizonalChange)
    //    {
    //        if (!double.IsNaN(x.Width) && !double.IsNaN(y.Width))
    //        {
    //            if (horizonalChange < 0)
    //            {
    //                var change = GetChangeKeepPositive(x.Width, horizonalChange);
    //                if (x.Name == "PART_MiddleThumb")
    //                {
    //                    if (x.Width > x.MinWidth)
    //                    {
    //                        if (x.Width + change < x.MinWidth)
    //                        {
    //                            var dif = x.Width - x.MinWidth;
    //                            x.Width = x.MinWidth;
    //                            y.Width += dif;
    //                        }
    //                        else
    //                        {
    //                            x.Width += change;
    //                            y.Width -= change;
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    x.Width += change;
    //                    y.Width -= change;
    //                }
    //            }
    //            else if (horizonalChange > 0)
    //            {
    //                var change = -GetChangeKeepPositive(y.Width, -horizonalChange);
    //                if (y.Name == "PART_MiddleThumb")
    //                {
    //                    if (y.Width > y.MinWidth)
    //                    {
    //                        if (y.Width - change < y.MinWidth)
    //                        {
    //                            var dif = y.Width - y.MinWidth;
    //                            y.Width = y.MinWidth;
    //                            x.Width += dif;
    //                        }
    //                        else
    //                        {
    //                            x.Width += change;
    //                            y.Width -= change;
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    x.Width += change;
    //                    y.Width -= change;
    //                }
    //            }
    //        }
    //    }

    //    private static void MoveThumbVertical(FrameworkElement x, FrameworkElement y, double verticalChange)
    //    {
    //        if (!double.IsNaN(x.Height) && !double.IsNaN(y.Height))
    //        {
    //            if (verticalChange < 0)
    //            {
    //                var change = -GetChangeKeepPositive(y.Height, verticalChange);
    //                if (y.Name == "PART_MiddleThumb")
    //                {
    //                    if (y.Height > y.MinHeight)
    //                    {
    //                        if (y.Height - change < y.MinHeight)
    //                        {
    //                            var dif = y.Height - y.MinHeight;
    //                            y.Height = y.MinHeight;
    //                            x.Height += dif;
    //                        }
    //                        else
    //                        {
    //                            x.Height += change;
    //                            y.Height -= change;
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    x.Height += change;
    //                    y.Height -= change;
    //                }
    //            }
    //            else if (verticalChange > 0)
    //            {
    //                var change = GetChangeKeepPositive(x.Height, -verticalChange);
    //                if (x.Name == "PART_MiddleThumb")
    //                {
    //                    if (x.Height > y.MinHeight)
    //                    {
    //                        if (x.Height + change < x.MinHeight)
    //                        {
    //                            var dif = x.Height - x.MinHeight;
    //                            x.Height = x.MinHeight;
    //                            y.Height += dif;
    //                        }
    //                        else
    //                        {
    //                            x.Height += change;
    //                            y.Height -= change;
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    x.Height += change;
    //                    y.Height -= change;
    //                }
    //            }
    //        }
    //    }

    //    private void ReCalculateSize()
    //    {
    //        if (_leftButton != null && _rightButton != null && _centerThumb != null)
    //        {
    //            if (Orientation == Orientation.Horizontal)
    //            {
    //                _movableWidth = Math.Max(ActualWidth - _rightThumb.ActualWidth - _leftThumb.ActualWidth - MinRangeWidth, 1);
    //                if (MovableRange <= 0)
    //                {
    //                    _leftButton.Width = double.NaN;
    //                    _rightButton.Width = double.NaN;
    //                }
    //                else
    //                {
    //                    _leftButton.Width = Math.Max(_movableWidth * (LowerValue - Minimum) / MovableRange, 0);
    //                    _rightButton.Width = Math.Max(_movableWidth * (Maximum - UpperValue) / MovableRange, 0);
    //                }

    //                if (IsValidDouble(_rightButton.Width) && IsValidDouble(_leftButton.Width))
    //                {
    //                    _centerThumb.Width = Math.Max(ActualWidth - (_leftButton.Width + _rightButton.Width + _rightThumb.ActualWidth + _leftThumb.ActualWidth), 0);
    //                }
    //                else
    //                {
    //                    _centerThumb.Width = Math.Max(ActualWidth - (_rightThumb.ActualWidth + _leftThumb.ActualWidth), 0);
    //                }
    //            }
    //            else if (Orientation == Orientation.Vertical)
    //            {
    //                _movableWidth = Math.Max(ActualHeight - _rightThumb.ActualHeight - _leftThumb.ActualHeight - MinRangeWidth, 1);
    //                if (MovableRange <= 0)
    //                {
    //                    _leftButton.Height = double.NaN;
    //                    _rightButton.Height = double.NaN;
    //                }
    //                else
    //                {
    //                    _leftButton.Height = Math.Max(_movableWidth * (LowerValue - Minimum) / MovableRange, 0);
    //                    _rightButton.Height = Math.Max(_movableWidth * (Maximum - UpperValue) / MovableRange, 0);
    //                }

    //                if (IsValidDouble(_rightButton.Height) && IsValidDouble(_leftButton.Height))
    //                {
    //                    _centerThumb.Height = Math.Max(ActualHeight - (_leftButton.Height + _rightButton.Height + _rightThumb.ActualHeight + _leftThumb.ActualHeight), 0);
    //                }
    //                else
    //                {
    //                    _centerThumb.Height = Math.Max(ActualHeight - (_rightThumb.ActualHeight + _leftThumb.ActualHeight), 0);
    //                }
    //            }

    //            _density = _movableWidth / MovableRange;
    //        }
    //    }

    //    private void ReCalculateRangeSelected(bool reCalculateLowerValue, bool reCalculateUpperValue, Direction direction)
    //    {
    //        _internalUpdate = true;
    //        if (direction == Direction.Increase)
    //        {
    //            if (reCalculateUpperValue)
    //            {
    //                _oldUpper = UpperValue;
    //                var width = Orientation == Orientation.Horizontal ? _rightButton.Width : _rightButton.Height;

    //                if (IsValidDouble(width))
    //                {
    //                    var upper = Equals(width, 0.0) ? Maximum : Math.Min(Maximum, Maximum - MovableRange * width / _movableWidth);
    //                    UpperValue = _isMoved ? upper : _roundToPrecision ? Math.Round(upper, _precision) : upper;
    //                }
    //            }

    //            if (reCalculateLowerValue)
    //            {
    //                _oldLower = LowerValue;
    //                var width = Orientation == Orientation.Horizontal ? _leftButton.Width : _leftButton.Height;

    //                if (IsValidDouble(width))
    //                {
    //                    var lower = Equals(width, 0.0) ? Minimum : Math.Max(Minimum, Minimum + MovableRange * width / _movableWidth);
    //                    LowerValue = _isMoved ? lower : _roundToPrecision ? Math.Round(lower, _precision) : lower;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            if (reCalculateLowerValue)
    //            {
    //                _oldLower = LowerValue;
    //                var width = Orientation == Orientation.Horizontal ? _leftButton.Width : _leftButton.Height;

    //                if (IsValidDouble(width))
    //                {
    //                    var lower = Equals(width, 0.0) ? Minimum : Math.Max(Minimum, Minimum + MovableRange * width / _movableWidth);
    //                    LowerValue = _isMoved ? lower : _roundToPrecision ? Math.Round(lower, _precision) : lower;
    //                }
    //            }

    //            if (reCalculateUpperValue)
    //            {
    //                _oldUpper = UpperValue;
    //                var width = Orientation == Orientation.Horizontal ? _rightButton.Width : _rightButton.Height;

    //                if (IsValidDouble(width))
    //                {
    //                    var upper = Equals(width, 0.0) ? Maximum : Math.Min(Maximum, Maximum - MovableRange * width / _movableWidth);
    //                    UpperValue = _isMoved ? upper : _roundToPrecision ? Math.Round(upper, _precision) : upper;
    //                }
    //            }
    //        }

    //        _roundToPrecision = false;
    //        _internalUpdate = false;

    //        RaiseValueChangedEvents(this, reCalculateLowerValue, reCalculateUpperValue);
    //    }

    //    private void ReCalculateRangeSelected(bool reCalculateLowerValue, bool reCalculateUpperValue, double value, Direction direction)
    //    {
    //        _internalUpdate = true;
    //        var tickFrequency = TickFrequency.ToString(CultureInfo.InvariantCulture);
    //        if (reCalculateLowerValue)
    //        {
    //            _oldLower = LowerValue;
    //            double lower = 0;
    //            if (IsSnapToTickEnabled)
    //            {
    //                lower = direction == Direction.Increase ? Math.Min(UpperValue - MinRange, value) : Math.Max(Minimum, value);
    //            }

    //            if (!tickFrequency.ToLower().Contains("e+") && tickFrequency.Contains("."))
    //            {
    //                var decimalPart = tickFrequency.Split('.');
    //                LowerValue = Math.Round(lower, decimalPart[1].Length, MidpointRounding.AwayFromZero);
    //            }
    //            else
    //            {
    //                LowerValue = lower;
    //            }
    //        }

    //        if (reCalculateUpperValue)
    //        {
    //            _oldUpper = UpperValue;
    //            double upper = 0;
    //            if (IsSnapToTickEnabled)
    //            {
    //                upper = direction == Direction.Increase ? Math.Min(value, Maximum) : Math.Max(LowerValue + MinRange, value);
    //            }

    //            if (!tickFrequency.ToLower().Contains("e+") && tickFrequency.Contains("."))
    //            {
    //                var decimalPart = tickFrequency.Split('.');
    //                UpperValue = Math.Round(upper, decimalPart[1].Length, MidpointRounding.AwayFromZero);
    //            }
    //            else
    //            {
    //                UpperValue = upper;
    //            }
    //        }

    //        _internalUpdate = false;

    //        RaiseValueChangedEvents(this, reCalculateLowerValue, reCalculateUpperValue);
    //    }

    //    private void ReCalculateRangeSelected(double newLower, double newUpper, Direction direction)
    //    {
    //        double lower = 0,
    //            upper = 0;
    //        _internalUpdate = true;
    //        _oldLower = LowerValue;
    //        _oldUpper = UpperValue;

    //        if (IsSnapToTickEnabled)
    //        {
    //            if (direction == Direction.Increase)
    //            {
    //                lower = Math.Min(newLower, Maximum - (UpperValue - LowerValue));
    //                upper = Math.Min(newUpper, Maximum);
    //            }
    //            else
    //            {
    //                lower = Math.Max(newLower, Minimum);
    //                upper = Math.Max(Minimum + (UpperValue - LowerValue), newUpper);
    //            }

    //            var tickFrequency = TickFrequency.ToString(CultureInfo.InvariantCulture);
    //            if (!tickFrequency.ToLower().Contains("e+") && tickFrequency.Contains("."))
    //            {
    //                var decimalPart = tickFrequency.Split('.');

    //                if (direction == Direction.Decrease)
    //                {
    //                    LowerValue = Math.Round(lower, decimalPart[1].Length, MidpointRounding.AwayFromZero);
    //                    UpperValue = Math.Round(upper, decimalPart[1].Length, MidpointRounding.AwayFromZero);
    //                }

    //                else
    //                {
    //                    UpperValue = Math.Round(upper, decimalPart[1].Length, MidpointRounding.AwayFromZero);
    //                    LowerValue = Math.Round(lower, decimalPart[1].Length, MidpointRounding.AwayFromZero);
    //                }
    //            }
    //            else
    //            {
    //                if (direction == Direction.Decrease)
    //                {
    //                    LowerValue = lower;
    //                    UpperValue = upper;
    //                }

    //                else
    //                {
    //                    UpperValue = upper;
    //                    LowerValue = lower;
    //                }
    //            }
    //        }

    //        _internalUpdate = false;

    //        RaiseValueChangedEvents(this);
    //    }

    //    private void OnRangeParameterChanged(RangeParameterChangedEventArgs e, RoutedEvent Event)
    //    {
    //        e.RoutedEvent = Event;
    //        RaiseEvent(e);
    //    }

    //    public void MoveSelection(bool isLeft)
    //    {
    //        var widthChange = SmallChange * (UpperValue - LowerValue) * _movableWidth / MovableRange;

    //        widthChange = isLeft ? -widthChange : widthChange;
    //        MoveThumb(_leftButton, _rightButton, widthChange, Orientation, out _direction);
    //        ReCalculateRangeSelected(true, true, _direction);
    //    }

    //    public void ResetSelection(bool isStart)
    //    {
    //        var widthChange = Maximum - Minimum;
    //        widthChange = isStart ? -widthChange : widthChange;

    //        MoveThumb(_leftButton, _rightButton, widthChange, Orientation, out _direction);
    //        ReCalculateRangeSelected(true, true, _direction);
    //    }

    //    private void OnRangeSelectionChanged(RangeSelectionChangedEventArgs e)
    //    {
    //        e.RoutedEvent = RangeSelectionChangedEvent;
    //        RaiseEvent(e);
    //    }

    //    public override void OnApplyTemplate()
    //    {
    //        base.OnApplyTemplate();

    //        _container = GetTemplateChild("PART_Container") as FrameworkElement;
    //        _visualElementsContainer = GetTemplateChild("PART_RangeSliderContainer") as StackPanel;
    //        _centerThumb = GetTemplateChild("PART_MiddleThumb") as Thumb;
    //        _leftButton = GetTemplateChild("PART_LeftEdge") as RepeatButton;
    //        _rightButton = GetTemplateChild("PART_RightEdge") as RepeatButton;
    //        _leftThumb = GetTemplateChild("PART_LeftThumb") as Thumb;
    //        _rightThumb = GetTemplateChild("PART_RightThumb") as Thumb;

    //        InitializeVisualElementsContainer();
    //        ReCalculateSize();
    //    }

    //    private void InitializeVisualElementsContainer()
    //    {
    //        if (_visualElementsContainer != null
    //            && _leftThumb != null
    //            && _rightThumb != null
    //            && _centerThumb != null)
    //        {
    //            _leftThumb.DragCompleted -= LeftThumbDragComplete;
    //            _rightThumb.DragCompleted -= RightThumbDragComplete;
    //            _leftThumb.DragStarted -= LeftThumbDragStart;
    //            _rightThumb.DragStarted -= RightThumbDragStart;
    //            _centerThumb.DragStarted -= CenterThumbDragStarted;
    //            _centerThumb.DragCompleted -= CenterThumbDragCompleted;

    //            _centerThumb.DragDelta -= CenterThumbDragDelta;
    //            _leftThumb.DragDelta -= LeftThumbDragDelta;
    //            _rightThumb.DragDelta -= RightThumbDragDelta;

    //            _visualElementsContainer.PreviewMouseDown -= VisualElementsContainerPreviewMouseDown;
    //            _visualElementsContainer.PreviewMouseUp -= VisualElementsContainerPreviewMouseUp;
    //            _visualElementsContainer.MouseLeave -= VisualElementsContainerMouseLeave;
    //            _visualElementsContainer.MouseDown -= VisualElementsContainerMouseDown;

    //            _leftThumb.DragCompleted += LeftThumbDragComplete;
    //            _rightThumb.DragCompleted += RightThumbDragComplete;
    //            _leftThumb.DragStarted += LeftThumbDragStart;
    //            _rightThumb.DragStarted += RightThumbDragStart;
    //            _centerThumb.DragStarted += CenterThumbDragStarted;
    //            _centerThumb.DragCompleted += CenterThumbDragCompleted;

    //            _centerThumb.DragDelta += CenterThumbDragDelta;
    //            _leftThumb.DragDelta += LeftThumbDragDelta;
    //            _rightThumb.DragDelta += RightThumbDragDelta;

    //            _visualElementsContainer.PreviewMouseDown += VisualElementsContainerPreviewMouseDown;
    //            _visualElementsContainer.PreviewMouseUp += VisualElementsContainerPreviewMouseUp;
    //            _visualElementsContainer.MouseLeave += VisualElementsContainerMouseLeave;
    //            _visualElementsContainer.MouseDown += VisualElementsContainerMouseDown;
    //        }
    //    }

    //    private void VisualElementsContainerPreviewMouseDown(object sender, MouseButtonEventArgs e)
    //    {
    //        var position = Mouse.GetPosition(_visualElementsContainer);
    //        if (Orientation == Orientation.Horizontal)
    //        {
    //            if (position.X < _leftButton.ActualWidth)
    //            {
    //                LeftButtonMouseDown();
    //            }
    //            else if (position.X > ActualWidth - _rightButton.ActualWidth)
    //            {
    //                RightButtonMouseDown();
    //            }
    //            else if (position.X > _leftButton.ActualWidth + _leftThumb.ActualWidth &&
    //                     position.X < ActualWidth - (_rightButton.ActualWidth + _rightThumb.ActualWidth))
    //            {
    //                CentralThumbMouseDown();
    //            }
    //        }
    //        else
    //        {
    //            if (position.Y > ActualHeight - _leftButton.ActualHeight)
    //            {
    //                LeftButtonMouseDown();
    //            }
    //            else if (position.Y < _rightButton.ActualHeight)
    //            {
    //                RightButtonMouseDown();
    //            }
    //            else if (position.Y > _rightButton.ActualHeight + _rightButton.ActualHeight &&
    //                     position.Y < ActualHeight - (_leftButton.ActualHeight + _leftThumb.ActualHeight))
    //            {
    //                CentralThumbMouseDown();
    //            }
    //        }
    //    }

    //    private void VisualElementsContainerMouseDown(object sender, MouseButtonEventArgs e)
    //    {
    //        if (e.MiddleButton == MouseButtonState.Pressed)
    //        {
    //            MoveWholeRange = MoveWholeRange != true;
    //        }
    //    }

    //    private enum ButtonType
    //    {
    //        BottomLeft,
    //        TopRight,
    //        Both
    //    }

    //    private enum Direction
    //    {
    //        Increase,
    //        Decrease
    //    }

    //    #region Routed UI commands

    //    public static RoutedUICommand MoveBack = new RoutedUICommand("MoveBack", "MoveBack", typeof(RangeSlider), new InputGestureCollection(new InputGesture[] {new KeyGesture(Key.B, ModifierKeys.Control)}));
    //    public static RoutedUICommand MoveForward = new RoutedUICommand("MoveForward", "MoveForward", typeof(RangeSlider), new InputGestureCollection(new InputGesture[] {new KeyGesture(Key.F, ModifierKeys.Control)}));
    //    public static RoutedUICommand MoveAllForward = new RoutedUICommand("MoveAllForward", "MoveAllForward", typeof(RangeSlider), new InputGestureCollection(new InputGesture[] {new KeyGesture(Key.F, ModifierKeys.Alt)}));
    //    public static RoutedUICommand MoveAllBack = new RoutedUICommand("MoveAllBack", "MoveAllBack", typeof(RangeSlider), new InputGestureCollection(new InputGesture[] {new KeyGesture(Key.B, ModifierKeys.Alt)}));

    //    #endregion

    //    #region Routed events

    //    public static readonly RoutedEvent RangeSelectionChangedEvent =
    //        EventManager.RegisterRoutedEvent("RangeSelectionChanged", RoutingStrategy.Bubble,
    //            typeof(RangeSelectionChangedEventHandler), typeof(RangeSlider));

    //    public static readonly RoutedEvent LowerValueChangedEvent =
    //        EventManager.RegisterRoutedEvent("LowerValueChanged", RoutingStrategy.Bubble,
    //            typeof(RangeParameterChangedEventHandler), typeof(RangeSlider));

    //    public static readonly RoutedEvent UpperValueChangedEvent =
    //        EventManager.RegisterRoutedEvent("UpperValueChanged", RoutingStrategy.Bubble,
    //            typeof(RangeParameterChangedEventHandler), typeof(RangeSlider));

    //    public static readonly RoutedEvent LowerThumbDragStartedEvent =
    //        EventManager.RegisterRoutedEvent("LowerThumbDragStarted", RoutingStrategy.Bubble,
    //            typeof(DragStartedEventHandler), typeof(RangeSlider));

    //    public static readonly RoutedEvent LowerThumbDragCompletedEvent =
    //        EventManager.RegisterRoutedEvent("LowerThumbDragCompleted", RoutingStrategy.Bubble,
    //            typeof(DragCompletedEventHandler), typeof(RangeSlider));

    //    public static readonly RoutedEvent UpperThumbDragStartedEvent =
    //        EventManager.RegisterRoutedEvent("UpperThumbDragStarted", RoutingStrategy.Bubble,
    //            typeof(DragStartedEventHandler), typeof(RangeSlider));

    //    public static readonly RoutedEvent UpperThumbDragCompletedEvent =
    //        EventManager.RegisterRoutedEvent("UpperThumbDragCompleted", RoutingStrategy.Bubble,
    //            typeof(DragCompletedEventHandler), typeof(RangeSlider));

    //    public static readonly RoutedEvent CentralThumbDragStartedEvent =
    //        EventManager.RegisterRoutedEvent("CentralThumbDragStarted", RoutingStrategy.Bubble,
    //            typeof(DragStartedEventHandler), typeof(RangeSlider));

    //    public static readonly RoutedEvent CentralThumbDragCompletedEvent =
    //        EventManager.RegisterRoutedEvent("CentralThumbDragCompleted", RoutingStrategy.Bubble,
    //            typeof(DragCompletedEventHandler), typeof(RangeSlider));

    //    public static readonly RoutedEvent LowerThumbDragDeltaEvent =
    //        EventManager.RegisterRoutedEvent("LowerThumbDragDelta", RoutingStrategy.Bubble,
    //            typeof(DragDeltaEventHandler), typeof(RangeSlider));

    //    public static readonly RoutedEvent UpperThumbDragDeltaEvent =
    //        EventManager.RegisterRoutedEvent("UpperThumbDragDelta", RoutingStrategy.Bubble,
    //            typeof(DragDeltaEventHandler), typeof(RangeSlider));

    //    public static readonly RoutedEvent CentralThumbDragDeltaEvent =
    //        EventManager.RegisterRoutedEvent("CentralThumbDragDelta", RoutingStrategy.Bubble,
    //            typeof(DragDeltaEventHandler), typeof(RangeSlider));

    //    #endregion

    //    #region Event handlers

    //    public event RangeSelectionChangedEventHandler RangeSelectionChanged
    //    {
    //        add { AddHandler(RangeSelectionChangedEvent, value); }
    //        remove { RemoveHandler(RangeSelectionChangedEvent, value); }
    //    }

    //    public event RangeParameterChangedEventHandler LowerValueChanged
    //    {
    //        add { AddHandler(LowerValueChangedEvent, value); }
    //        remove { RemoveHandler(LowerValueChangedEvent, value); }
    //    }

    //    public event RangeParameterChangedEventHandler UpperValueChanged
    //    {
    //        add { AddHandler(UpperValueChangedEvent, value); }
    //        remove { RemoveHandler(UpperValueChangedEvent, value); }
    //    }

    //    public event DragStartedEventHandler LowerThumbDragStarted
    //    {
    //        add { AddHandler(LowerThumbDragStartedEvent, value); }
    //        remove { RemoveHandler(LowerThumbDragStartedEvent, value); }
    //    }

    //    public event DragCompletedEventHandler LowerThumbDragCompleted
    //    {
    //        add { AddHandler(LowerThumbDragCompletedEvent, value); }
    //        remove { RemoveHandler(LowerThumbDragCompletedEvent, value); }
    //    }

    //    public event DragStartedEventHandler UpperThumbDragStarted
    //    {
    //        add { AddHandler(UpperThumbDragStartedEvent, value); }
    //        remove { RemoveHandler(UpperThumbDragStartedEvent, value); }
    //    }

    //    public event DragCompletedEventHandler UpperThumbDragCompleted
    //    {
    //        add { AddHandler(UpperThumbDragCompletedEvent, value); }
    //        remove { RemoveHandler(UpperThumbDragCompletedEvent, value); }
    //    }

    //    public event DragStartedEventHandler CentralThumbDragStarted
    //    {
    //        add { AddHandler(CentralThumbDragStartedEvent, value); }
    //        remove { RemoveHandler(CentralThumbDragStartedEvent, value); }
    //    }

    //    public event DragCompletedEventHandler CentralThumbDragCompleted
    //    {
    //        add { AddHandler(CentralThumbDragCompletedEvent, value); }
    //        remove { RemoveHandler(CentralThumbDragCompletedEvent, value); }
    //    }

    //    public event DragDeltaEventHandler LowerThumbDragDelta
    //    {
    //        add { AddHandler(LowerThumbDragDeltaEvent, value); }
    //        remove { RemoveHandler(LowerThumbDragDeltaEvent, value); }
    //    }

    //    public event DragDeltaEventHandler UpperThumbDragDelta
    //    {
    //        add { AddHandler(UpperThumbDragDeltaEvent, value); }
    //        remove { RemoveHandler(UpperThumbDragDeltaEvent, value); }
    //    }

    //    public event DragDeltaEventHandler CentralThumbDragDelta
    //    {
    //        add { AddHandler(CentralThumbDragDeltaEvent, value); }
    //        remove { RemoveHandler(CentralThumbDragDeltaEvent, value); }
    //    }

    //    #endregion

    //    #region Dependency properties

    //    public static readonly DependencyProperty UpperValueProperty =
    //        DependencyProperty.Register("UpperValue", typeof(double), typeof(RangeSlider),
    //            new FrameworkPropertyMetadata((double) 0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, RangesChanged, CoerceUpperValue));

    //    public static readonly DependencyProperty LowerValueProperty =
    //        DependencyProperty.Register("LowerValue", typeof(double), typeof(RangeSlider),
    //            new FrameworkPropertyMetadata((double) 0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender, RangesChanged, CoerceLowerValue));

    //    public static readonly DependencyProperty MinRangeProperty =
    //        DependencyProperty.Register("MinRange", typeof(double), typeof(RangeSlider),
    //            new FrameworkPropertyMetadata((double) 0, MinRangeChanged, CoerceMinRange), IsValidMinRange);

    //    public static readonly DependencyProperty MinRangeWidthProperty =
    //        DependencyProperty.Register("MinRangeWidth", typeof(double), typeof(RangeSlider),
    //            new FrameworkPropertyMetadata(30.0, MinRangeWidthChanged, CoerceMinRangeWidth), IsValidMinRange);

    //    public static readonly DependencyProperty MoveWholeRangeProperty =
    //        DependencyProperty.Register("MoveWholeRange", typeof(bool), typeof(RangeSlider),
    //            new PropertyMetadata(false));

    //    public static readonly DependencyProperty ExtendedModeProperty =
    //        DependencyProperty.Register("ExtendedMode", typeof(bool), typeof(RangeSlider),
    //            new PropertyMetadata(false));

    //    public static readonly DependencyProperty IsSnapToTickEnabledProperty =
    //        DependencyProperty.Register("IsSnapToTickEnabled", typeof(bool), typeof(RangeSlider),
    //            new PropertyMetadata(false));

    //    public static readonly DependencyProperty OrientationProperty =
    //        DependencyProperty.Register("Orientation", typeof(Orientation), typeof(RangeSlider),
    //            new FrameworkPropertyMetadata(Orientation.Horizontal));

    //    public static readonly DependencyProperty TickPlacementProperty =
    //        DependencyProperty.Register("TickPlacement", typeof(TickPlacement), typeof(RangeSlider),
    //            new FrameworkPropertyMetadata(TickPlacement.None));

    //    public static readonly DependencyProperty TickFrequencyProperty =
    //        DependencyProperty.Register("TickFrequency", typeof(double), typeof(RangeSlider),
    //            new FrameworkPropertyMetadata(1.0), IsValidDoubleValue);

    //    public static readonly DependencyProperty TicksProperty
    //        = DependencyProperty.Register("Ticks",
    //            typeof(DoubleCollection),
    //            typeof(RangeSlider),
    //            new FrameworkPropertyMetadata(default(DoubleCollection)));

    //    public static readonly DependencyProperty IsMoveToPointEnabledProperty =
    //        DependencyProperty.Register("IsMoveToPointEnabled", typeof(bool), typeof(RangeSlider),
    //            new PropertyMetadata(false));

    //    public static readonly DependencyProperty AutoToolTipPlacementProperty = DependencyProperty.Register(nameof(AutoToolTipPlacement), typeof(AutoToolTipPlacement), typeof(RangeSlider), new FrameworkPropertyMetadata(AutoToolTipPlacement.None));

    //    [Bindable(true)]
    //    [Category("Behavior")]
    //    public AutoToolTipPlacement AutoToolTipPlacement
    //    {
    //        get { return (AutoToolTipPlacement) GetValue(AutoToolTipPlacementProperty); }
    //        set { SetValue(AutoToolTipPlacementProperty, value); }
    //    }

    //    public static readonly DependencyProperty AutoToolTipPrecisionProperty = DependencyProperty.Register(nameof(AutoToolTipPrecision), typeof(int), typeof(RangeSlider), new FrameworkPropertyMetadata(0), IsValidPrecision);

    //    [Bindable(true)]
    //    [Category("Appearance")]
    //    public int AutoToolTipPrecision
    //    {
    //        get { return (int) GetValue(AutoToolTipPrecisionProperty); }
    //        set { SetValue(AutoToolTipPrecisionProperty, value); }
    //    }

    //    public static readonly DependencyProperty AutoToolTipLowerValueTemplateProperty = DependencyProperty.Register(nameof(AutoToolTipLowerValueTemplate), typeof(DataTemplate), typeof(RangeSlider), new FrameworkPropertyMetadata(null));

    //    [Bindable(true)]
    //    [Category("Behavior")]
    //    public DataTemplate AutoToolTipLowerValueTemplate
    //    {
    //        get { return (DataTemplate) GetValue(AutoToolTipLowerValueTemplateProperty); }
    //        set { SetValue(AutoToolTipLowerValueTemplateProperty, value); }
    //    }

    //    public static readonly DependencyProperty AutoToolTipUpperValueTemplateProperty = DependencyProperty.Register(nameof(AutoToolTipUpperValueTemplate), typeof(DataTemplate), typeof(RangeSlider), new FrameworkPropertyMetadata(null));

    //    [Bindable(true)]
    //    [Category("Behavior")]
    //    public DataTemplate AutoToolTipUpperValueTemplate
    //    {
    //        get { return (DataTemplate) GetValue(AutoToolTipUpperValueTemplateProperty); }
    //        set { SetValue(AutoToolTipUpperValueTemplateProperty, value); }
    //    }

    //    public static readonly DependencyProperty AutoToolTipRangeValuesTemplateProperty = DependencyProperty.Register(nameof(AutoToolTipRangeValuesTemplate), typeof(DataTemplate), typeof(RangeSlider), new FrameworkPropertyMetadata(null));

    //    [Bindable(true)]
    //    [Category("Behavior")]
    //    public DataTemplate AutoToolTipRangeValuesTemplate
    //    {
    //        get { return (DataTemplate) GetValue(AutoToolTipRangeValuesTemplateProperty); }
    //        set { SetValue(AutoToolTipRangeValuesTemplateProperty, value); }
    //    }

    //    public static readonly DependencyProperty IntervalProperty =
    //        DependencyProperty.Register("Interval", typeof(int), typeof(RangeSlider),
    //            new FrameworkPropertyMetadata(100, IntervalChangedCallback), IsValidPrecision);

    //    public static readonly DependencyProperty IsSelectionRangeEnabledProperty
    //        = DependencyProperty.Register("IsSelectionRangeEnabled",
    //            typeof(bool),
    //            typeof(RangeSlider),
    //            new FrameworkPropertyMetadata(false));

    //    public static readonly DependencyProperty SelectionStartProperty
    //        = DependencyProperty.Register("SelectionStart",
    //            typeof(double),
    //            typeof(RangeSlider),
    //            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectionStartChanged, CoerceSelectionStart),
    //            IsValidDoubleValue);

    //    public static readonly DependencyProperty SelectionEndProperty
    //        = DependencyProperty.Register("SelectionEnd",
    //            typeof(double),
    //            typeof(RangeSlider),
    //            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectionEndChanged, CoerceSelectionEnd),
    //            IsValidDoubleValue);

    //    [Bindable(true)]
    //    [Category("Behavior")]
    //    public int Interval
    //    {
    //        get { return (int) GetValue(IntervalProperty); }
    //        set { SetValue(IntervalProperty, value); }
    //    }

    //    [Bindable(true)]
    //    [Category("Appearance")]
    //    public TickPlacement TickPlacement
    //    {
    //        get { return (TickPlacement) GetValue(TickPlacementProperty); }
    //        set { SetValue(TickPlacementProperty, value); }
    //    }

    //    [Bindable(true)]
    //    [Category("Appearance")]
    //    public double TickFrequency
    //    {
    //        get { return (double) GetValue(TickFrequencyProperty); }
    //        set { SetValue(TickFrequencyProperty, value); }
    //    }

    //    [Bindable(true)]
    //    [Category("Appearance")]
    //    public DoubleCollection Ticks
    //    {
    //        get { return (DoubleCollection) GetValue(TicksProperty); }
    //        set { SetValue(TicksProperty, value); }
    //    }

    //    [Bindable(true)]
    //    [Category("Behavior")]
    //    public bool IsMoveToPointEnabled
    //    {
    //        get { return (bool) GetValue(IsMoveToPointEnabledProperty); }
    //        set { SetValue(IsMoveToPointEnabledProperty, value); }
    //    }

    //    [Bindable(true)]
    //    [Category("Common")]
    //    public Orientation Orientation
    //    {
    //        get { return (Orientation) GetValue(OrientationProperty); }
    //        set { SetValue(OrientationProperty, value); }
    //    }

    //    [Bindable(true)]
    //    [Category("Appearance")]
    //    public bool IsSnapToTickEnabled
    //    {
    //        get { return (bool) GetValue(IsSnapToTickEnabledProperty); }
    //        set { SetValue(IsSnapToTickEnabledProperty, value); }
    //    }

    //    [Bindable(true)]
    //    [Category("Behavior")]
    //    public bool ExtendedMode
    //    {
    //        get { return (bool) GetValue(ExtendedModeProperty); }
    //        set { SetValue(ExtendedModeProperty, value); }
    //    }

    //    [Bindable(true)]
    //    [Category("Behavior")]
    //    public bool MoveWholeRange
    //    {
    //        get { return (bool) GetValue(MoveWholeRangeProperty); }
    //        set { SetValue(MoveWholeRangeProperty, value); }
    //    }

    //    [Bindable(true)]
    //    [Category("Common")]
    //    public double MinRangeWidth
    //    {
    //        get { return (double) GetValue(MinRangeWidthProperty); }
    //        set { SetValue(MinRangeWidthProperty, value); }
    //    }

    //    [Bindable(true)]
    //    [Category("Common")]
    //    public double LowerValue
    //    {
    //        get { return (double) GetValue(LowerValueProperty); }
    //        set { SetValue(LowerValueProperty, value); }
    //    }

    //    [Bindable(true)]
    //    [Category("Common")]
    //    public double UpperValue
    //    {
    //        get { return (double) GetValue(UpperValueProperty); }
    //        set { SetValue(UpperValueProperty, value); }
    //    }

    //    [Bindable(true)]
    //    [Category("Common")]
    //    public double MinRange
    //    {
    //        get { return (double) GetValue(MinRangeProperty); }
    //        set { SetValue(MinRangeProperty, value); }
    //    }

    //    [Bindable(true)]
    //    [Category("Appearance")]
    //    public bool IsSelectionRangeEnabled
    //    {
    //        get { return (bool) GetValue(IsSelectionRangeEnabledProperty); }
    //        set { SetValue(IsSelectionRangeEnabledProperty, value); }
    //    }

    //    [Bindable(true)]
    //    [Category("Appearance")]
    //    public double SelectionStart
    //    {
    //        get { return (double) GetValue(SelectionStartProperty); }
    //        set { SetValue(SelectionStartProperty, value); }
    //    }

    //    private static void OnSelectionStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        var rangeSlider = (RangeSlider) d;
    //        rangeSlider.CoerceValue(SelectionEndProperty);
    //    }

    //    private static object CoerceSelectionStart(DependencyObject d, object value)
    //    {
    //        var rangeSlider = (RangeSlider) d;
    //        var num = (double) value;
    //        var minimum = rangeSlider.Minimum;
    //        var maximum = rangeSlider.Maximum;
    //        if (num < minimum)
    //        {
    //            return minimum;
    //        }

    //        if (num > maximum)
    //        {
    //            return maximum;
    //        }

    //        return value;
    //    }

    //    [Bindable(true)]
    //    [Category("Appearance")]
    //    public double SelectionEnd
    //    {
    //        get { return (double) GetValue(SelectionEndProperty); }
    //        set { SetValue(SelectionEndProperty, value); }
    //    }

    //    private static void OnSelectionEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //    }

    //    private static object CoerceSelectionEnd(DependencyObject d, object value)
    //    {
    //        var rangeSlider = (RangeSlider) d;
    //        var num = (double) value;
    //        var selectionStart = rangeSlider.SelectionStart;
    //        var maximum = rangeSlider.Maximum;
    //        if (num < selectionStart)
    //        {
    //            return selectionStart;
    //        }

    //        if (num > maximum)
    //        {
    //            return maximum;
    //        }

    //        return value;
    //    }

    //    #endregion

    //    #region Variables

    //    private const double Epsilon = 0.00000153;

    //    private bool _internalUpdate;
    //    private Thumb _centerThumb;
    //    private Thumb _leftThumb;
    //    private Thumb _rightThumb;
    //    private RepeatButton _leftButton;
    //    private RepeatButton _rightButton;
    //    private StackPanel _visualElementsContainer;
    //    private FrameworkElement _container;
    //    private double _movableWidth;
    //    private readonly DispatcherTimer _timer;

    //    private uint _tickCount;
    //    private double _currentpoint;
    //    private bool _isInsideRange;
    //    private bool _centerThumbBlocked;
    //    private Direction _direction;
    //    private ButtonType _bType;
    //    private Point _position;
    //    private Point _basePoint;
    //    private double _currenValue;
    //    private double _density;
    //    private ToolTip _autoToolTip;
    //    private double _oldLower;
    //    private double _oldUpper;
    //    private bool _isMoved;
    //    private bool _roundToPrecision;
    //    private int _precision;
    //    private readonly PropertyChangeNotifier actualWidthPropertyChangeNotifier;
    //    private readonly PropertyChangeNotifier actualHeightPropertyChangeNotifier;

    //    #endregion

    //    #region Mouse events

    //    private void VisualElementsContainerMouseLeave(object sender, MouseEventArgs e)
    //    {
    //        _tickCount = 0;
    //        _timer.Stop();
    //    }

    //    private void VisualElementsContainerPreviewMouseUp(object sender, MouseButtonEventArgs e)
    //    {
    //        _tickCount = 0;
    //        _timer.Stop();
    //        _centerThumbBlocked = false;
    //    }

    //    private void LeftButtonMouseDown()
    //    {
    //        if (Mouse.LeftButton == MouseButtonState.Pressed)
    //        {
    //            var p = Mouse.GetPosition(_visualElementsContainer);
    //            var change = Orientation == Orientation.Horizontal
    //                ? _leftButton.ActualWidth - p.X + _leftThumb.ActualWidth / 2
    //                : -(_leftButton.ActualHeight - (ActualHeight - (p.Y + _leftThumb.ActualHeight / 2)));
    //            if (!IsSnapToTickEnabled)
    //            {
    //                if (IsMoveToPointEnabled && !MoveWholeRange)
    //                {
    //                    MoveThumb(_leftButton, _centerThumb, -change, Orientation, out _direction);
    //                    ReCalculateRangeSelected(true, false, _direction);
    //                }
    //                else if (IsMoveToPointEnabled && MoveWholeRange)
    //                {
    //                    MoveThumb(_leftButton, _rightButton, -change, Orientation, out _direction);
    //                    ReCalculateRangeSelected(true, true, _direction);
    //                }
    //            }
    //            else
    //            {
    //                if (IsMoveToPointEnabled && !MoveWholeRange)
    //                {
    //                    JumpToNextTick(Direction.Decrease, ButtonType.BottomLeft, -change, LowerValue, true);
    //                }
    //                else if (IsMoveToPointEnabled && MoveWholeRange)
    //                {
    //                    JumpToNextTick(Direction.Decrease, ButtonType.Both, -change, LowerValue, true);
    //                }
    //            }

    //            if (!IsMoveToPointEnabled)
    //            {
    //                _position = Mouse.GetPosition(_visualElementsContainer);
    //                _bType = MoveWholeRange ? ButtonType.Both : ButtonType.BottomLeft;
    //                _currentpoint = Orientation == Orientation.Horizontal ? _position.X : _position.Y;
    //                _currenValue = LowerValue;
    //                _isInsideRange = false;
    //                _direction = Direction.Decrease;
    //                _timer.Start();
    //            }
    //        }
    //    }

    //    private void RightButtonMouseDown()
    //    {
    //        if (Mouse.LeftButton == MouseButtonState.Pressed)
    //        {
    //            var p = Mouse.GetPosition(_visualElementsContainer);
    //            var change = Orientation == Orientation.Horizontal
    //                ? _rightButton.ActualWidth - (ActualWidth - (p.X + _rightThumb.ActualWidth / 2))
    //                : -(_rightButton.ActualHeight - (p.Y - _rightThumb.ActualHeight / 2));
    //            if (!IsSnapToTickEnabled)
    //            {
    //                if (IsMoveToPointEnabled && !MoveWholeRange)
    //                {
    //                    MoveThumb(_centerThumb, _rightButton, change, Orientation, out _direction);
    //                    ReCalculateRangeSelected(false, true, _direction);
    //                }
    //                else if (IsMoveToPointEnabled && MoveWholeRange)
    //                {
    //                    MoveThumb(_leftButton, _rightButton, change, Orientation, out _direction);
    //                    ReCalculateRangeSelected(true, true, _direction);
    //                }
    //            }
    //            else
    //            {
    //                if (IsMoveToPointEnabled && !MoveWholeRange)
    //                {
    //                    JumpToNextTick(Direction.Increase, ButtonType.TopRight, change, UpperValue, true);
    //                }
    //                else if (IsMoveToPointEnabled && MoveWholeRange)
    //                {
    //                    JumpToNextTick(Direction.Increase, ButtonType.Both, change, UpperValue, true);
    //                }
    //            }

    //            if (!IsMoveToPointEnabled)
    //            {
    //                _position = Mouse.GetPosition(_visualElementsContainer);
    //                _bType = MoveWholeRange ? ButtonType.Both : ButtonType.TopRight;
    //                _currentpoint = Orientation == Orientation.Horizontal ? _position.X : _position.Y;
    //                _currenValue = UpperValue;
    //                _direction = Direction.Increase;
    //                _isInsideRange = false;
    //                _timer.Start();
    //            }
    //        }
    //    }

    //    private void CentralThumbMouseDown()
    //    {
    //        if (ExtendedMode)
    //        {
    //            if (Mouse.LeftButton == MouseButtonState.Pressed && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
    //            {
    //                _centerThumbBlocked = true;
    //                var p = Mouse.GetPosition(_visualElementsContainer);
    //                var change = Orientation == Orientation.Horizontal
    //                    ? p.X + _leftThumb.ActualWidth / 2 - (_leftButton.ActualWidth + _leftThumb.ActualWidth)
    //                    : -(ActualHeight - (p.Y + _leftThumb.ActualHeight / 2 + _leftButton.ActualHeight));
    //                if (!IsSnapToTickEnabled)
    //                {
    //                    if (IsMoveToPointEnabled && !MoveWholeRange)
    //                    {
    //                        MoveThumb(_leftButton, _centerThumb, change, Orientation, out _direction);
    //                        ReCalculateRangeSelected(true, false, _direction);
    //                    }
    //                    else if (IsMoveToPointEnabled && MoveWholeRange)
    //                    {
    //                        MoveThumb(_leftButton, _rightButton, change, Orientation, out _direction);
    //                        ReCalculateRangeSelected(true, true, _direction);
    //                    }
    //                }
    //                else
    //                {
    //                    if (IsMoveToPointEnabled && !MoveWholeRange)
    //                    {
    //                        JumpToNextTick(Direction.Increase, ButtonType.BottomLeft, change, LowerValue, true);
    //                    }
    //                    else if (IsMoveToPointEnabled && MoveWholeRange)
    //                    {
    //                        JumpToNextTick(Direction.Increase, ButtonType.Both, change, LowerValue, true);
    //                    }
    //                }

    //                if (!IsMoveToPointEnabled)
    //                {
    //                    _position = Mouse.GetPosition(_visualElementsContainer);
    //                    _bType = MoveWholeRange ? ButtonType.Both : ButtonType.BottomLeft;
    //                    _currentpoint = Orientation == Orientation.Horizontal ? _position.X : _position.Y;
    //                    _currenValue = LowerValue;
    //                    _direction = Direction.Increase;
    //                    _isInsideRange = true;
    //                    _timer.Start();
    //                }
    //            }
    //            else if (Mouse.RightButton == MouseButtonState.Pressed && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
    //            {
    //                _centerThumbBlocked = true;
    //                var p = Mouse.GetPosition(_visualElementsContainer);
    //                var change = Orientation == Orientation.Horizontal
    //                    ? ActualWidth - (p.X + _rightThumb.ActualWidth / 2 + _rightButton.ActualWidth)
    //                    : -(p.Y + _rightThumb.ActualHeight / 2 - (_rightButton.ActualHeight + _rightThumb.ActualHeight));
    //                if (!IsSnapToTickEnabled)
    //                {
    //                    if (IsMoveToPointEnabled && !MoveWholeRange)
    //                    {
    //                        MoveThumb(_centerThumb, _rightButton, -change, Orientation, out _direction);
    //                        ReCalculateRangeSelected(false, true, _direction);
    //                    }
    //                    else if (IsMoveToPointEnabled && MoveWholeRange)
    //                    {
    //                        MoveThumb(_leftButton, _rightButton, -change, Orientation, out _direction);
    //                        ReCalculateRangeSelected(true, true, _direction);
    //                    }
    //                }
    //                else
    //                {
    //                    if (IsMoveToPointEnabled && !MoveWholeRange)
    //                    {
    //                        JumpToNextTick(Direction.Decrease, ButtonType.TopRight, -change, UpperValue, true);
    //                    }
    //                    else if (IsMoveToPointEnabled && MoveWholeRange)
    //                    {
    //                        JumpToNextTick(Direction.Decrease, ButtonType.Both, -change, UpperValue, true);
    //                    }
    //                }

    //                if (!IsMoveToPointEnabled)
    //                {
    //                    _position = Mouse.GetPosition(_visualElementsContainer);
    //                    _bType = MoveWholeRange ? ButtonType.Both : ButtonType.TopRight;
    //                    _currentpoint = Orientation == Orientation.Horizontal ? _position.X : _position.Y;
    //                    _currenValue = UpperValue;
    //                    _direction = Direction.Decrease;
    //                    _isInsideRange = true;
    //                    _timer.Start();
    //                }
    //            }
    //        }
    //    }

    //    #endregion

    //    #region Thumb Drag event handlers

    //    private void LeftThumbDragStart(object sender, DragStartedEventArgs e)
    //    {
    //        _isMoved = true;
    //        if (AutoToolTipPlacement != AutoToolTipPlacement.None)
    //        {
    //            if (_autoToolTip == null)
    //            {
    //                _autoToolTip = new ToolTip();
    //                _autoToolTip.Placement = PlacementMode.Custom;
    //                _autoToolTip.CustomPopupPlacementCallback = PopupPlacementCallback;
    //            }

    //            _autoToolTip.SetValue(ContentControl.ContentTemplateProperty, AutoToolTipLowerValueTemplate);
    //            _autoToolTip.Content = GetToolTipNumber(LowerValue);
    //            _autoToolTip.PlacementTarget = _leftThumb;
    //            _autoToolTip.IsOpen = true;
    //        }

    //        _basePoint = Mouse.GetPosition(_container);
    //        e.RoutedEvent = LowerThumbDragStartedEvent;
    //        RaiseEvent(e);
    //    }

    //    private void LeftThumbDragDelta(object sender, DragDeltaEventArgs e)
    //    {
    //        var change = Orientation == Orientation.Horizontal ? e.HorizontalChange : e.VerticalChange;
    //        if (!IsSnapToTickEnabled)
    //        {
    //            MoveThumb(_leftButton, _centerThumb, change, Orientation, out _direction);
    //            ReCalculateRangeSelected(true, false, _direction);
    //        }
    //        else
    //        {
    //            Direction localDirection;
    //            var currentPoint = Mouse.GetPosition(_container);
    //            if (Orientation == Orientation.Horizontal)
    //            {
    //                if (currentPoint.X >= 0 && currentPoint.X < _container.ActualWidth - (_rightButton.ActualWidth + _rightThumb.ActualWidth + _centerThumb.MinWidth))
    //                {
    //                    localDirection = currentPoint.X > _basePoint.X ? Direction.Increase : Direction.Decrease;
    //                    JumpToNextTick(localDirection, ButtonType.BottomLeft, change, LowerValue, false);
    //                }
    //            }
    //            else
    //            {
    //                if (currentPoint.Y <= _container.ActualHeight && currentPoint.Y > _rightButton.ActualHeight + _rightThumb.ActualHeight + _centerThumb.MinHeight)
    //                {
    //                    localDirection = currentPoint.Y < _basePoint.Y ? Direction.Increase : Direction.Decrease;
    //                    JumpToNextTick(localDirection, ButtonType.BottomLeft, -change, LowerValue, false);
    //                }
    //            }
    //        }

    //        _basePoint = Mouse.GetPosition(_container);
    //        if (AutoToolTipPlacement != AutoToolTipPlacement.None)
    //        {
    //            _autoToolTip.Content = GetToolTipNumber(LowerValue);
    //            RelocateAutoToolTip();
    //        }

    //        e.RoutedEvent = LowerThumbDragDeltaEvent;
    //        RaiseEvent(e);
    //    }

    //    private void LeftThumbDragComplete(object sender, DragCompletedEventArgs e)
    //    {
    //        if (_autoToolTip != null)
    //        {
    //            _autoToolTip.IsOpen = false;
    //            _autoToolTip = null;
    //        }

    //        e.RoutedEvent = LowerThumbDragCompletedEvent;
    //        RaiseEvent(e);
    //    }

    //    private void RightThumbDragStart(object sender, DragStartedEventArgs e)
    //    {
    //        _isMoved = true;
    //        if (AutoToolTipPlacement != AutoToolTipPlacement.None)
    //        {
    //            if (_autoToolTip == null)
    //            {
    //                _autoToolTip = new ToolTip();
    //                _autoToolTip.Placement = PlacementMode.Custom;
    //                _autoToolTip.CustomPopupPlacementCallback = PopupPlacementCallback;
    //            }

    //            _autoToolTip.SetValue(ContentControl.ContentTemplateProperty, AutoToolTipUpperValueTemplate);
    //            _autoToolTip.Content = GetToolTipNumber(UpperValue);
    //            _autoToolTip.PlacementTarget = _rightThumb;
    //            _autoToolTip.IsOpen = true;
    //        }

    //        _basePoint = Mouse.GetPosition(_container);
    //        e.RoutedEvent = UpperThumbDragStartedEvent;
    //        RaiseEvent(e);
    //    }

    //    private void RightThumbDragDelta(object sender, DragDeltaEventArgs e)
    //    {
    //        var change = Orientation == Orientation.Horizontal ? e.HorizontalChange : e.VerticalChange;
    //        if (!IsSnapToTickEnabled)
    //        {
    //            MoveThumb(_centerThumb, _rightButton, change, Orientation, out _direction);
    //            ReCalculateRangeSelected(false, true, _direction);
    //        }
    //        else
    //        {
    //            Direction localDirection;
    //            var currentPoint = Mouse.GetPosition(_container);
    //            if (Orientation == Orientation.Horizontal)
    //            {
    //                if (currentPoint.X < _container.ActualWidth && currentPoint.X > _leftButton.ActualWidth + _leftThumb.ActualWidth + _centerThumb.MinWidth)
    //                {
    //                    localDirection = currentPoint.X > _basePoint.X ? Direction.Increase : Direction.Decrease;
    //                    JumpToNextTick(localDirection, ButtonType.TopRight, change, UpperValue, false);
    //                }
    //            }
    //            else
    //            {
    //                if (currentPoint.Y >= 0 && currentPoint.Y < _container.ActualHeight - (_leftButton.ActualHeight + _leftThumb.ActualHeight + _centerThumb.MinHeight))
    //                {
    //                    localDirection = currentPoint.Y < _basePoint.Y ? Direction.Increase : Direction.Decrease;
    //                    JumpToNextTick(localDirection, ButtonType.TopRight, -change, UpperValue, false);
    //                }
    //            }

    //            _basePoint = Mouse.GetPosition(_container);
    //        }

    //        if (AutoToolTipPlacement != AutoToolTipPlacement.None)
    //        {
    //            _autoToolTip.Content = GetToolTipNumber(UpperValue);
    //            RelocateAutoToolTip();
    //        }

    //        e.RoutedEvent = UpperThumbDragDeltaEvent;
    //        RaiseEvent(e);
    //    }

    //    private void RightThumbDragComplete(object sender, DragCompletedEventArgs e)
    //    {
    //        if (_autoToolTip != null)
    //        {
    //            _autoToolTip.IsOpen = false;
    //            _autoToolTip = null;
    //        }

    //        e.RoutedEvent = UpperThumbDragCompletedEvent;
    //        RaiseEvent(e);
    //    }

    //    private void CenterThumbDragStarted(object sender, DragStartedEventArgs e)
    //    {
    //        _isMoved = true;
    //        if (AutoToolTipPlacement != AutoToolTipPlacement.None)
    //        {
    //            if (_autoToolTip == null)
    //            {
    //                _autoToolTip = new ToolTip();
    //                _autoToolTip.Placement = PlacementMode.Custom;
    //                _autoToolTip.CustomPopupPlacementCallback = PopupPlacementCallback;
    //            }

    //            var autoToolTipRangeValuesTemplate = AutoToolTipRangeValuesTemplate;
    //            _autoToolTip.SetValue(ContentControl.ContentTemplateProperty, autoToolTipRangeValuesTemplate);
    //            if (autoToolTipRangeValuesTemplate != null)
    //            {
    //                _autoToolTip.Content = new RangeSliderAutoTooltipValues(this);
    //            }
    //            else
    //            {
    //                _autoToolTip.Content = GetToolTipNumber(LowerValue) + " - " + GetToolTipNumber(UpperValue);
    //            }

    //            _autoToolTip.PlacementTarget = _centerThumb;
    //            _autoToolTip.IsOpen = true;
    //        }

    //        _basePoint = Mouse.GetPosition(_container);
    //        e.RoutedEvent = CentralThumbDragStartedEvent;
    //        RaiseEvent(e);
    //    }

    //    private void CenterThumbDragDelta(object sender, DragDeltaEventArgs e)
    //    {
    //        if (!_centerThumbBlocked)
    //        {
    //            var change = Orientation == Orientation.Horizontal ? e.HorizontalChange : e.VerticalChange;
    //            if (!IsSnapToTickEnabled)
    //            {
    //                MoveThumb(_leftButton, _rightButton, change, Orientation, out _direction);
    //                ReCalculateRangeSelected(true, true, _direction);
    //            }
    //            else
    //            {
    //                Direction localDirection;
    //                var currentPoint = Mouse.GetPosition(_container);
    //                if (Orientation == Orientation.Horizontal)
    //                {
    //                    if (currentPoint.X >= 0 && currentPoint.X < _container.ActualWidth)
    //                    {
    //                        localDirection = currentPoint.X > _basePoint.X ? Direction.Increase : Direction.Decrease;
    //                        JumpToNextTick(localDirection, ButtonType.Both, change, localDirection == Direction.Increase ? UpperValue : LowerValue, false);
    //                    }
    //                }
    //                else
    //                {
    //                    if (currentPoint.Y >= 0 && currentPoint.Y < _container.ActualHeight)
    //                    {
    //                        localDirection = currentPoint.Y < _basePoint.Y ? Direction.Increase : Direction.Decrease;
    //                        JumpToNextTick(localDirection, ButtonType.Both, -change, localDirection == Direction.Increase ? UpperValue : LowerValue, false);
    //                    }
    //                }
    //            }

    //            _basePoint = Mouse.GetPosition(_container);
    //            if (AutoToolTipPlacement != AutoToolTipPlacement.None)
    //            {
    //                if (_autoToolTip.ContentTemplate != null)
    //                {
    //                    (_autoToolTip.Content as RangeSliderAutoTooltipValues)?.UpdateValues(this);
    //                }
    //                else
    //                {
    //                    _autoToolTip.Content = GetToolTipNumber(LowerValue) + " - " + GetToolTipNumber(UpperValue);
    //                }

    //                RelocateAutoToolTip();
    //            }
    //        }

    //        e.RoutedEvent = CentralThumbDragDeltaEvent;
    //        RaiseEvent(e);
    //    }

    //    private void CenterThumbDragCompleted(object sender, DragCompletedEventArgs e)
    //    {
    //        if (_autoToolTip != null)
    //        {
    //            _autoToolTip.IsOpen = false;
    //            _autoToolTip = null;
    //        }

    //        e.RoutedEvent = CentralThumbDragCompletedEvent;
    //        RaiseEvent(e);
    //    }

    //    #endregion

    //    #region Helper methods

    //    private static double GetChangeKeepPositive(double width, double increment)
    //    {
    //        return Math.Max(width + increment, 0) - width;
    //    }

    //    private double UpdateEndPoint(ButtonType type, Direction dir)
    //    {
    //        double d = 0;

    //        if (dir == Direction.Increase)
    //        {
    //            if (type == ButtonType.BottomLeft || type == ButtonType.Both && _isInsideRange)
    //            {
    //                d = Orientation == Orientation.Horizontal ? _leftButton.ActualWidth + _leftThumb.ActualWidth : ActualHeight - (_leftButton.ActualHeight + _leftThumb.ActualHeight);
    //            }
    //            else if (type == ButtonType.TopRight || type == ButtonType.Both && !_isInsideRange)
    //            {
    //                d = Orientation == Orientation.Horizontal ? ActualWidth - _rightButton.ActualWidth : _rightButton.ActualHeight;
    //            }
    //        }
    //        else if (dir == Direction.Decrease)
    //        {
    //            if (type == ButtonType.BottomLeft || type == ButtonType.Both && !_isInsideRange)
    //            {
    //                d = Orientation == Orientation.Horizontal ? _leftButton.ActualWidth : ActualHeight - _leftButton.ActualHeight;
    //            }
    //            else if (type == ButtonType.TopRight || type == ButtonType.Both && _isInsideRange)
    //            {
    //                d = Orientation == Orientation.Horizontal ? ActualWidth - _rightButton.ActualWidth - _rightThumb.ActualWidth : _rightButton.ActualHeight + _rightThumb.ActualHeight;
    //            }
    //        }

    //        return d;
    //    }

    //    private bool GetResult(double currentPoint, double endPoint, Direction direction)
    //    {
    //        if (direction == Direction.Increase)
    //        {
    //            return Orientation == Orientation.Horizontal && currentPoint > endPoint || Orientation == Orientation.Vertical && currentPoint < endPoint;
    //        }

    //        return Orientation == Orientation.Horizontal && currentPoint < endPoint || Orientation == Orientation.Vertical && currentPoint > endPoint;
    //    }

    //    private void MoveToNextValue(object sender, EventArgs e)
    //    {
    //        _position = Mouse.GetPosition(_visualElementsContainer);
    //        _currentpoint = Orientation == Orientation.Horizontal ? _position.X : _position.Y;
    //        var endpoint = UpdateEndPoint(_bType, _direction);
    //        var result = GetResult(_currentpoint, endpoint, _direction);
    //        double widthChange;
    //        if (!IsSnapToTickEnabled)
    //        {
    //            widthChange = SmallChange;
    //            if (_tickCount > 5)
    //            {
    //                widthChange = LargeChange;
    //            }

    //            _roundToPrecision = true;
    //            if (!widthChange.ToString(CultureInfo.InvariantCulture).ToLower().Contains("e") &&
    //                widthChange.ToString(CultureInfo.InvariantCulture).Contains("."))
    //            {
    //                var array = widthChange.ToString(CultureInfo.InvariantCulture).Split('.');
    //                _precision = array[1].Length;
    //            }
    //            else
    //            {
    //                _precision = 0;
    //            }

    //            widthChange = Orientation == Orientation.Horizontal ? widthChange : -widthChange;

    //            widthChange = _direction == Direction.Increase ? widthChange : -widthChange;
    //            if (result)
    //            {
    //                switch (_bType)
    //                {
    //                    case ButtonType.BottomLeft:
    //                        MoveThumb(_leftButton, _centerThumb, widthChange * _density, Orientation, out _direction);
    //                        ReCalculateRangeSelected(true, false, _direction);
    //                        break;
    //                    case ButtonType.TopRight:
    //                        MoveThumb(_centerThumb, _rightButton, widthChange * _density, Orientation, out _direction);
    //                        ReCalculateRangeSelected(false, true, _direction);
    //                        break;
    //                    case ButtonType.Both:
    //                        MoveThumb(_leftButton, _rightButton, widthChange * _density, Orientation, out _direction);
    //                        ReCalculateRangeSelected(true, true, _direction);
    //                        break;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            widthChange = CalculateNextTick(_direction, _currenValue, 0, true);
    //            var value = widthChange;

    //            widthChange = Orientation == Orientation.Horizontal ? widthChange : -widthChange;
    //            if (_direction == Direction.Increase)
    //            {
    //                if (result)
    //                {
    //                    switch (_bType)
    //                    {
    //                        case ButtonType.BottomLeft:
    //                            MoveThumb(_leftButton, _centerThumb, widthChange * _density, Orientation);
    //                            ReCalculateRangeSelected(true, false, LowerValue + value, _direction);
    //                            break;
    //                        case ButtonType.TopRight:
    //                            MoveThumb(_centerThumb, _rightButton, widthChange * _density, Orientation);
    //                            ReCalculateRangeSelected(false, true, UpperValue + value, _direction);
    //                            break;
    //                        case ButtonType.Both:
    //                            MoveThumb(_leftButton, _rightButton, widthChange * _density, Orientation);
    //                            ReCalculateRangeSelected(LowerValue + value, UpperValue + value, _direction);
    //                            break;
    //                    }
    //                }
    //            }
    //            else if (_direction == Direction.Decrease)
    //            {
    //                if (result)
    //                {
    //                    switch (_bType)
    //                    {
    //                        case ButtonType.BottomLeft:
    //                            MoveThumb(_leftButton, _centerThumb, -widthChange * _density, Orientation);
    //                            ReCalculateRangeSelected(true, false, LowerValue - value, _direction);
    //                            break;
    //                        case ButtonType.TopRight:
    //                            MoveThumb(_centerThumb, _rightButton, -widthChange * _density, Orientation);
    //                            ReCalculateRangeSelected(false, true, UpperValue - value, _direction);
    //                            break;
    //                        case ButtonType.Both:
    //                            MoveThumb(_leftButton, _rightButton, -widthChange * _density, Orientation);
    //                            ReCalculateRangeSelected(LowerValue - value, UpperValue - value, _direction);
    //                            break;
    //                    }
    //                }
    //            }
    //        }

    //        _tickCount++;
    //    }

    //    private void SnapToTickHandle(ButtonType type, Direction direction, double difference)
    //    {
    //        var value = difference;

    //        difference = Orientation == Orientation.Horizontal ? difference : -difference;
    //        if (direction == Direction.Increase)
    //        {
    //            switch (type)
    //            {
    //                case ButtonType.TopRight:
    //                    if (UpperValue < Maximum)
    //                    {
    //                        MoveThumb(_centerThumb, _rightButton, difference * _density, Orientation);
    //                        ReCalculateRangeSelected(false, true, UpperValue + value, direction);
    //                    }

    //                    break;
    //                case ButtonType.BottomLeft:
    //                    if (LowerValue < UpperValue - MinRange)
    //                    {
    //                        MoveThumb(_leftButton, _centerThumb, difference * _density, Orientation);
    //                        ReCalculateRangeSelected(true, false, LowerValue + value, direction);
    //                    }

    //                    break;
    //                case ButtonType.Both:
    //                    if (UpperValue < Maximum)
    //                    {
    //                        MoveThumb(_leftButton, _rightButton, difference * _density, Orientation);
    //                        ReCalculateRangeSelected(LowerValue + value, UpperValue + value, direction);
    //                    }

    //                    break;
    //            }
    //        }
    //        else
    //        {
    //            switch (type)
    //            {
    //                case ButtonType.TopRight:
    //                    if (UpperValue > LowerValue + MinRange)
    //                    {
    //                        MoveThumb(_centerThumb, _rightButton, -difference * _density, Orientation);
    //                        ReCalculateRangeSelected(false, true, UpperValue - value, direction);
    //                    }

    //                    break;
    //                case ButtonType.BottomLeft:
    //                    if (LowerValue > Minimum)
    //                    {
    //                        MoveThumb(_leftButton, _centerThumb, -difference * _density, Orientation);
    //                        ReCalculateRangeSelected(true, false, LowerValue - value, direction);
    //                    }

    //                    break;
    //                case ButtonType.Both:
    //                    if (LowerValue > Minimum)
    //                    {
    //                        MoveThumb(_leftButton, _rightButton, -difference * _density, Orientation);
    //                        ReCalculateRangeSelected(LowerValue - value, UpperValue - value, direction);
    //                    }

    //                    break;
    //            }
    //        }
    //    }

    //    private double CalculateNextTick(Direction direction, double checkingValue, double distance, bool moveDirectlyToNextTick)
    //    {
    //        var checkingValuePos = checkingValue - Minimum;
    //        if (!IsMoveToPointEnabled)
    //        {
    //            var checkingValueChanged = checkingValuePos;
    //            var x = checkingValueChanged / TickFrequency;
    //            if (!IsDoubleCloseToInt(x))
    //            {
    //                distance = TickFrequency * (int) x;
    //                if (direction == Direction.Increase)
    //                {
    //                    distance += TickFrequency;
    //                }

    //                distance = distance - Math.Abs(checkingValuePos);
    //                _currenValue = 0;
    //                return Math.Abs(distance);
    //            }
    //        }

    //        if (moveDirectlyToNextTick)
    //        {
    //            distance = TickFrequency;
    //        }

    //        else
    //        {
    //            var currentValue = checkingValuePos + distance / _density;
    //            var x = currentValue / TickFrequency;
    //            if (direction == Direction.Increase)
    //            {
    //                var nextvalue = x.ToString(CultureInfo.InvariantCulture).ToLower().Contains("e+")
    //                    ? x * TickFrequency + TickFrequency
    //                    : (int) x * TickFrequency + TickFrequency;

    //                distance = nextvalue - Math.Abs(checkingValuePos);
    //            }
    //            else
    //            {
    //                var previousValue = x.ToString(CultureInfo.InvariantCulture).ToLower().Contains("e+")
    //                    ? x * TickFrequency
    //                    : (int) x * TickFrequency;
    //                distance = Math.Abs(checkingValuePos) - previousValue;
    //            }
    //        }

    //        return Math.Abs(distance);
    //    }

    //    private void JumpToNextTick(Direction direction, ButtonType type, double distance, double checkingValue, bool jumpDirectlyToTick)
    //    {
    //        var difference = CalculateNextTick(direction, checkingValue, distance, false);
    //        var p = Mouse.GetPosition(_visualElementsContainer);
    //        var pos = Orientation == Orientation.Horizontal ? p.X : p.Y;
    //        var widthHeight = Orientation == Orientation.Horizontal ? ActualWidth : ActualHeight;
    //        var tickIntervalInPixels = direction == Direction.Increase
    //            ? TickFrequency * _density
    //            : -TickFrequency * _density;

    //        if (jumpDirectlyToTick)
    //        {
    //            SnapToTickHandle(type, direction, difference);
    //        }
    //        else
    //        {
    //            if (direction == Direction.Increase)
    //            {
    //                if (!IsDoubleCloseToInt(checkingValue / TickFrequency))
    //                {
    //                    if (distance > difference * _density / 2 || distance >= widthHeight - pos || distance >= pos)
    //                    {
    //                        SnapToTickHandle(type, direction, difference);
    //                    }
    //                }
    //                else
    //                {
    //                    if (distance > tickIntervalInPixels / 2 || distance >= widthHeight - pos || distance >= pos)
    //                    {
    //                        SnapToTickHandle(type, direction, difference);
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                if (!IsDoubleCloseToInt(checkingValue / TickFrequency))
    //                {
    //                    if (distance <= -(difference * _density) / 2 || UpperValue - LowerValue < difference)
    //                    {
    //                        SnapToTickHandle(type, direction, difference);
    //                    }
    //                }
    //                else
    //                {
    //                    if (distance < tickIntervalInPixels / 2 || UpperValue - LowerValue < difference)
    //                    {
    //                        SnapToTickHandle(type, direction, difference);
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    private void RelocateAutoToolTip()
    //    {
    //        var offset = _autoToolTip.HorizontalOffset;
    //        _autoToolTip.HorizontalOffset = offset + 0.001;
    //        _autoToolTip.HorizontalOffset = offset;
    //    }

    //    private bool ApproximatelyEquals(double value1, double value2)
    //    {
    //        return Math.Abs(value1 - value2) <= Epsilon;
    //    }

    //    private bool IsDoubleCloseToInt(double val)
    //    {
    //        return ApproximatelyEquals(Math.Abs(val - Math.Round(val)), 0);
    //    }

    //    internal string GetToolTipNumber(double value)
    //    {
    //        var numberFormatInfo = (NumberFormatInfo) NumberFormatInfo.CurrentInfo.Clone();
    //        numberFormatInfo.NumberDecimalDigits = AutoToolTipPrecision;
    //        return value.ToString("N", numberFormatInfo);
    //    }

    //    private CustomPopupPlacement[] PopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
    //    {
    //        switch (AutoToolTipPlacement)
    //        {
    //            case AutoToolTipPlacement.TopLeft:
    //                if (Orientation == Orientation.Horizontal)
    //                {
    //                    return new[] {new CustomPopupPlacement(new Point((targetSize.Width - popupSize.Width) * 0.5, -popupSize.Height), PopupPrimaryAxis.Horizontal)};
    //                }

    //                return new[] {new CustomPopupPlacement(new Point(-popupSize.Width, (targetSize.Height - popupSize.Height) * 0.5), PopupPrimaryAxis.Vertical)};

    //            case AutoToolTipPlacement.BottomRight:
    //                if (Orientation == Orientation.Horizontal)
    //                {
    //                    return new[] {new CustomPopupPlacement(new Point((targetSize.Width - popupSize.Width) * 0.5, targetSize.Height), PopupPrimaryAxis.Horizontal)};
    //                }

    //                return new[] {new CustomPopupPlacement(new Point(targetSize.Width, (targetSize.Height - popupSize.Height) * 0.5), PopupPrimaryAxis.Vertical)};

    //            default:
    //                return new CustomPopupPlacement[] { };
    //        }
    //    }

    //    #endregion

    //    #region Validation methods

    //    private static bool IsValidDoubleValue(object value)
    //    {
    //        return value is double && IsValidDouble((double) value);
    //    }

    //    private static bool IsValidDouble(double d)
    //    {
    //        return !double.IsNaN(d) && !double.IsInfinity(d);
    //    }

    //    private static bool IsValidPrecision(object value)
    //    {
    //        return (int) value >= 0;
    //    }

    //    private static bool IsValidMinRange(object value)
    //    {
    //        return value is double && IsValidDouble((double) value) && (double) value >= 0d;
    //    }

    //    #endregion

    //    #region Coerce callbacks

    //    private static object CoerceMinimum(DependencyObject d, object basevalue)
    //    {
    //        var rs = (RangeSlider) d;
    //        var value = (double) basevalue;
    //        if (value > rs.Maximum)
    //        {
    //            return rs.Maximum;
    //        }

    //        return basevalue;
    //    }

    //    private static object CoerceMaximum(DependencyObject d, object basevalue)
    //    {
    //        var rs = (RangeSlider) d;
    //        var value = (double) basevalue;
    //        if (value < rs.Minimum)
    //        {
    //            return rs.Minimum;
    //        }

    //        return basevalue;
    //    }

    //    internal static object CoerceLowerValue(DependencyObject d, object basevalue)
    //    {
    //        var rs = (RangeSlider) d;
    //        var value = (double) basevalue;
    //        if (value < rs.Minimum || rs.UpperValue - rs.MinRange < rs.Minimum)
    //        {
    //            return rs.Minimum;
    //        }

    //        if (value > rs.UpperValue - rs.MinRange)
    //        {
    //            return rs.UpperValue - rs.MinRange;
    //        }

    //        return basevalue;
    //    }

    //    internal static object CoerceUpperValue(DependencyObject d, object basevalue)
    //    {
    //        var rs = (RangeSlider) d;
    //        var value = (double) basevalue;
    //        if (value > rs.Maximum || rs.LowerValue + rs.MinRange > rs.Maximum)
    //        {
    //            return rs.Maximum;
    //        }

    //        if (value < rs.LowerValue + rs.MinRange)
    //        {
    //            return rs.LowerValue + rs.MinRange;
    //        }

    //        return basevalue;
    //    }

    //    private static object CoerceMinRange(DependencyObject d, object basevalue)
    //    {
    //        var rs = (RangeSlider) d;
    //        var value = (double) basevalue;
    //        if (rs.LowerValue + value > rs.Maximum)
    //        {
    //            return rs.Maximum - rs.LowerValue;
    //        }

    //        return basevalue;
    //    }

    //    private static object CoerceMinRangeWidth(DependencyObject d, object basevalue)
    //    {
    //        var rs = (RangeSlider) d;
    //        if (rs._leftThumb != null && rs._rightThumb != null)
    //        {
    //            double width;
    //            if (rs.Orientation == Orientation.Horizontal)
    //            {
    //                width = rs.ActualWidth - rs._leftThumb.ActualWidth - rs._rightThumb.ActualWidth;
    //            }
    //            else
    //            {
    //                width = rs.ActualHeight - rs._leftThumb.ActualHeight - rs._rightThumb.ActualHeight;
    //            }

    //            return (double) basevalue > width / 2 ? width / 2 : (double) basevalue;
    //        }

    //        return basevalue;
    //    }

    //    #endregion

    //    #region PropertyChanged CallBacks

    //    private static void MaxPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    //    {
    //        dependencyObject.CoerceValue(MaximumProperty);
    //        dependencyObject.CoerceValue(MinimumProperty);
    //        dependencyObject.CoerceValue(UpperValueProperty);
    //    }

    //    private static void MinPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    //    {
    //        dependencyObject.CoerceValue(MinimumProperty);
    //        dependencyObject.CoerceValue(MaximumProperty);
    //        dependencyObject.CoerceValue(LowerValueProperty);
    //    }

    //    private static void RangesChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    //    {
    //        var slider = (RangeSlider) dependencyObject;
    //        if (slider._internalUpdate)
    //        {
    //            return;
    //        }

    //        dependencyObject.CoerceValue(UpperValueProperty);
    //        dependencyObject.CoerceValue(LowerValueProperty);

    //        RaiseValueChangedEvents(dependencyObject);

    //        slider._oldLower = slider.LowerValue;
    //        slider._oldUpper = slider.UpperValue;
    //        slider.ReCalculateSize();
    //    }

    //    private static void MinRangeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    //    {
    //        var value = (double) e.NewValue;
    //        if (value < 0)
    //        {
    //            value = 0;
    //        }

    //        var slider = (RangeSlider) dependencyObject;
    //        dependencyObject.CoerceValue(MinRangeProperty);
    //        slider._internalUpdate = true;
    //        slider.UpperValue = Math.Max(slider.UpperValue, slider.LowerValue + value);
    //        slider.UpperValue = Math.Min(slider.UpperValue, slider.Maximum);
    //        slider._internalUpdate = false;

    //        slider.CoerceValue(UpperValueProperty);

    //        RaiseValueChangedEvents(dependencyObject);

    //        slider._oldLower = slider.LowerValue;
    //        slider._oldUpper = slider.UpperValue;

    //        slider.ReCalculateSize();
    //    }

    //    private static void MinRangeWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    //    {
    //        var slider = (RangeSlider) sender;
    //        slider.ReCalculateSize();
    //    }

    //    private static void IntervalChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    //    {
    //        var rs = (RangeSlider) dependencyObject;
    //        rs._timer.Interval = TimeSpan.FromMilliseconds((int) e.NewValue);
    //    }

    //    private static void RaiseValueChangedEvents(DependencyObject dependencyObject, bool lowerValueReCalculated = true, bool upperValueReCalculated = true)
    //    {
    //        var slider = (RangeSlider) dependencyObject;
    //        var lowerValueEquals = Equals(slider._oldLower, slider.LowerValue);
    //        var upperValueEquals = Equals(slider._oldUpper, slider.UpperValue);
    //        if ((lowerValueReCalculated || upperValueReCalculated) && (!lowerValueEquals || !upperValueEquals))
    //        {
    //            slider.OnRangeSelectionChanged(new RangeSelectionChangedEventArgs(slider.LowerValue, slider.UpperValue, slider._oldLower, slider._oldUpper));
    //        }

    //        if (lowerValueReCalculated && !lowerValueEquals)
    //        {
    //            slider.OnRangeParameterChanged(new RangeParameterChangedEventArgs(RangeParameterChangeType.Lower, slider._oldLower, slider.LowerValue), LowerValueChangedEvent);
    //        }

    //        if (upperValueReCalculated && !upperValueEquals)
    //        {
    //            slider.OnRangeParameterChanged(new RangeParameterChangedEventArgs(RangeParameterChangeType.Upper, slider._oldUpper, slider.UpperValue), UpperValueChangedEvent);
    //        }
    //    }

    //    #endregion
    //}

    //public class RangeSliderAutoTooltipValues : INotifyPropertyChanged
    //{
    //    private string lowerValue;

    //    private string upperValue;

    //    internal RangeSliderAutoTooltipValues(RangeSlider rangeSlider)
    //    {
    //        UpdateValues(rangeSlider);
    //    }

    //    public string LowerValue
    //    {
    //        get
    //        {
    //            return lowerValue;
    //        }
    //        set
    //        {
    //            if (value.Equals(lowerValue))
    //            {
    //                return;
    //            }

    //            lowerValue = value;
    //            OnPropertyChanged();
    //        }
    //    }

    //    public string UpperValue
    //    {
    //        get
    //        {
    //            return upperValue;
    //        }
    //        set
    //        {
    //            if (value.Equals(upperValue))
    //            {
    //                return;
    //            }

    //            upperValue = value;
    //            OnPropertyChanged();
    //        }
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;

    //    internal void UpdateValues(RangeSlider rangeSlider)
    //    {
    //        LowerValue = rangeSlider.GetToolTipNumber(rangeSlider.LowerValue);
    //        UpperValue = rangeSlider.GetToolTipNumber(rangeSlider.UpperValue);
    //    }

    //    public override string ToString()
    //    {
    //        return LowerValue + " - " + UpperValue;
    //    }

    //    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //}

    //public class RangeSelectionChangedEventArgs : RoutedEventArgs
    //{
    //    internal RangeSelectionChangedEventArgs(double newLowerValue, double newUpperValue, double oldLowerValue, double oldUpperValue)
    //    {
    //        NewLowerValue = newLowerValue;
    //        NewUpperValue = newUpperValue;
    //        OldLowerValue = oldLowerValue;
    //        OldUpperValue = oldUpperValue;
    //    }

    //    public double NewLowerValue { get; set; }

    //    public double NewUpperValue { get; set; }

    //    public double OldLowerValue { get; set; }

    //    public double OldUpperValue { get; set; }
    //}

    //public class RangeParameterChangedEventArgs : RoutedEventArgs
    //{
    //    internal RangeParameterChangedEventArgs(RangeParameterChangeType type, double _old, double _new)
    //    {
    //        ParameterType = type;

    //        OldValue = _old;
    //        NewValue = _new;
    //    }

    //    public RangeParameterChangeType ParameterType
    //    {
    //        get;
    //    }

    //    public double OldValue
    //    {
    //        get;
    //    }

    //    public double NewValue
    //    {
    //        get;
    //    }
    //}

    //public enum RangeParameterChangeType
    //{
    //    Lower = 1,
    //    Upper = 2
    //}
}