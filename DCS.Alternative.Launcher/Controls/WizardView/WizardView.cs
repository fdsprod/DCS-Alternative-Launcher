using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using DCS.Alternative.Launcher.Data;

namespace DCS.Alternative.Launcher.Controls
{
    [ContentProperty("CurrentStep")]
    [TemplatePart(Name = PART_NextButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_BackButton, Type = typeof(Button))]
    public class WizardView : Control
    {
        public const string PART_CurrentStep = "PART_CurrentStep";
        public const string PART_NextButton = "PART_NextButton";
        public const string PART_BackButton = "PART_BackButton";

        public static readonly DependencyProperty EnterTransitionNextProperty =
            DependencyProperty.Register("EnterTransitionNext", typeof(Storyboard), typeof(WizardView), new PropertyMetadata(EmptyStoryboard.Value));

        public static readonly DependencyProperty ExitTransitionNextProperty =
            DependencyProperty.Register("ExitTransitionNext", typeof(Storyboard), typeof(WizardView), new PropertyMetadata(EmptyStoryboard.Value));

        public static readonly DependencyProperty EnterTransitionBackProperty =
            DependencyProperty.Register("EnterTransitionBack", typeof(Storyboard), typeof(WizardView), new PropertyMetadata(EmptyStoryboard.Value));

        public static readonly DependencyProperty ExitTransitionBackProperty =
            DependencyProperty.Register("ExitTransitionBack", typeof(Storyboard), typeof(WizardView), new PropertyMetadata(EmptyStoryboard.Value));

        public static readonly DependencyProperty NextButtonContentProperty =
            DependencyProperty.Register("NextButtonContent", typeof(object), typeof(WizardView), new PropertyMetadata("NEXT >"));

        public static readonly DependencyProperty BackButtonVisibilityProperty =
            DependencyProperty.Register("BackButtonVisibility", typeof(Visibility), typeof(WizardView), new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty NextButtonVisibilityProperty =
            DependencyProperty.Register("NextButtonVisibility", typeof(Visibility), typeof(WizardView), new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty IsBackEnabledProperty =
            DependencyProperty.Register("IsBackEnabled", typeof(bool), typeof(WizardView), new PropertyMetadata(true));

        public static readonly DependencyProperty IsNextEnabledProperty =
            DependencyProperty.Register("IsNextEnabled", typeof(bool), typeof(WizardView), new PropertyMetadata(true));

        public static readonly DependencyProperty BackButtonContentProperty =
            DependencyProperty.Register("BackButtonContent", typeof(object), typeof(WizardView), new PropertyMetadata("< BACK"));

        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register("Controller", typeof(object), typeof(WizardView), new PropertyMetadata(null, OnControllerPropertyChanged));

        public static readonly DependencyProperty CurrentStepProperty =
            DependencyProperty.Register("CurrentStep", typeof(object), typeof(WizardView), new PropertyMetadata(null));

        private TransitionControl _currentStepControl;

        static WizardView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WizardView), new FrameworkPropertyMetadata(typeof(WizardView)));
        }

        public Storyboard EnterTransitionNext
        {
            get { return (Storyboard) GetValue(EnterTransitionNextProperty); }
            set { SetValue(EnterTransitionNextProperty, value); }
        }

        public Storyboard ExitTransitionNext
        {
            get { return (Storyboard) GetValue(ExitTransitionNextProperty); }
            set { SetValue(ExitTransitionNextProperty, value); }
        }

        public Storyboard EnterTransitionBack
        {
            get { return (Storyboard) GetValue(EnterTransitionBackProperty); }
            set { SetValue(EnterTransitionBackProperty, value); }
        }

        public Storyboard ExitTransitionBack
        {
            get { return (Storyboard) GetValue(ExitTransitionBackProperty); }
            set { SetValue(ExitTransitionBackProperty, value); }
        }

        public bool IsBackEnabled
        {
            get { return (bool) GetValue(IsBackEnabledProperty); }
            set { SetValue(IsBackEnabledProperty, value); }
        }

        public bool IsNextEnabled
        {
            get { return (bool) GetValue(IsNextEnabledProperty); }
            set { SetValue(IsNextEnabledProperty, value); }
        }

        public object CurrentStep
        {
            get { return GetValue(CurrentStepProperty); }
            set { SetValue(CurrentStepProperty, value); }
        }

        public WizardController Controller
        {
            get { return GetValue(ControllerProperty) as WizardController; }
            set { SetValue(ControllerProperty, value); }
        }

        public object NextButtonContent
        {
            get { return GetValue(NextButtonContentProperty); }
            set { SetValue(NextButtonContentProperty, value); }
        }

        public Visibility BackButtonVisibility
        {
            get { return (Visibility) GetValue(BackButtonVisibilityProperty); }
            set { SetValue(BackButtonVisibilityProperty, value); }
        }

        public Visibility NextButtonVisibility
        {
            get { return (Visibility) GetValue(NextButtonVisibilityProperty); }
            set { SetValue(NextButtonVisibilityProperty, value); }
        }

        public object BackButtonContent
        {
            get { return GetValue(BackButtonContentProperty); }
            set { SetValue(BackButtonContentProperty, value); }
        }

        private static void OnControllerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = d as WizardView;

            var oldController = e.OldValue as WizardController;
            var newController = e.NewValue as WizardController;

            if (oldController != null)
            {
                oldController.Wizard = null;
            }

            if (newController != null)
            {
                newController.Wizard = source;
            }

            source?.OnControllerChanged(oldController, newController);
        }

        private void OnControllerChanged(WizardController oldValue, WizardController newValue)
        {
            if (oldValue != null)
            {
                oldValue.BeforeBack -= OnBeforeBack;
                oldValue.BeforeNext -= OnBeforeNext;
            }

            if (newValue == null)
            {
                return;
            }

            newValue.BeforeBack += OnBeforeBack;
            newValue.BeforeNext += OnBeforeNext;

            BindingHelper.BindProperty(newValue, this, CurrentStepProperty, "CurrentStep.Value");
            BindingHelper.BindProperty(newValue, this, BackButtonVisibilityProperty, "IsBackVisible.Value", converter: VisibilityConverter.Instance);
            BindingHelper.BindProperty(newValue, this, NextButtonVisibilityProperty, "IsNextVisible.Value", converter: VisibilityConverter.Instance);
            BindingHelper.BindProperty(newValue, this, IsNextEnabledProperty, "CanGoNext.Value");
        }

        private void OnBeforeNext(object sender, EventArgs e)
        {
            if (_currentStepControl != null)
            {
                _currentStepControl.ExitTransition = ExitTransitionNext;
                _currentStepControl.EnterTransition = EnterTransitionNext;
            }
        }

        private void OnBeforeBack(object sender, EventArgs e)
        {
            if (_currentStepControl != null)
            {
                _currentStepControl.ExitTransition = ExitTransitionBack;
                _currentStepControl.EnterTransition = EnterTransitionBack;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _currentStepControl = (TransitionControl) GetTemplateChild(PART_CurrentStep);

            if (_currentStepControl != null)
            {
                _currentStepControl.ExitTransition = ExitTransitionNext;
                _currentStepControl.EnterTransition = EnterTransitionNext;
            }

            var nextButton = (Button) GetTemplateChild(PART_NextButton);
            var backButton = (Button) GetTemplateChild(PART_BackButton);

            if (nextButton != null)
            {
                nextButton.Click += OnNextButtonTapped;
            }

            if (backButton != null)
            {
                backButton.Click += OnBackButtonTapped;
            }
        }

        private async void OnBackButtonTapped(object sender, RoutedEventArgs e)
        {
            var controller = Controller;

            await controller?.GoBackAsync();
        }

        private async void OnNextButtonTapped(object sender, RoutedEventArgs e)
        {
            var controller = Controller;

            await controller?.GoNextAsync();
        }
    }
}