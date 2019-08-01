using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace DCS.Alternative.Launcher.Controls
{

    [ContentProperty("Content")]
    [TemplatePart(Name = PART_Content1, Type = typeof(ContentControl))]
    [TemplatePart(Name = PART_Content2, Type = typeof(ContentControl))]
    public class TransitionControl : ContentControl
    {
        public const string PART_Content1 = "PART_Content1";
        public const string PART_Content2 = "PART_Content2";

        public static readonly DependencyProperty ExitTransitionProperty =
            DependencyProperty.Register("ExitTransition", typeof(Storyboard), typeof(TransitionControl), new PropertyMetadata(EmptyStoryboard.Value));

        public static readonly DependencyProperty EnterTransitionProperty =
            DependencyProperty.Register("EnterTransition", typeof(Storyboard), typeof(TransitionControl), new PropertyMetadata(EmptyStoryboard.Value));

        public static readonly DependencyProperty EnterTransitionFactoryProperty =
            DependencyProperty.Register("EnterTransitionFactory", typeof(Func<object, Storyboard>), typeof(TransitionControl), new PropertyMetadata(default(Func<object, Storyboard>)));

        public static readonly DependencyProperty ExitTransitionFactoryProperty =
            DependencyProperty.Register("ExitTransitionFactory", typeof(Func<object, Storyboard>), typeof(TransitionControl), new PropertyMetadata(default(Func<object, Storyboard>)));

        static TransitionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransitionControl), new FrameworkPropertyMetadata(typeof(TransitionControl)));
        }

        private Grid _transitionContainer;

        private ContentControl LastChild
        {
            get
            {
                if (_transitionContainer == null)
                {
                    return null;
                }

                if (_transitionContainer.Children.Count == 0)
                {
                    return null;
                }

                return _transitionContainer.Children[_transitionContainer.Children.Count - 1] as ContentControl;
            }
        }

        public Func<object, Storyboard> ExitTransitionFactory
        {
            get { return (Func<object, Storyboard>)GetValue(ExitTransitionFactoryProperty); }
            set { SetValue(ExitTransitionFactoryProperty, value); }
        }

        public Func<object, Storyboard> EnterTransitionFactory
        {
            get { return (Func<object, Storyboard>)GetValue(EnterTransitionFactoryProperty); }
            set { SetValue(EnterTransitionFactoryProperty, value); }
        }

        public Storyboard EnterTransition
        {
            get { return (Storyboard)GetValue(EnterTransitionProperty); }
            set { SetValue(EnterTransitionProperty, value); }
        }

        public Storyboard ExitTransition
        {
            get { return (Storyboard)GetValue(ExitTransitionProperty); }
            set { SetValue(ExitTransitionProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _transitionContainer = (Grid)GetTemplateChild("PART_TransitionContainer");

            var currentContentPresenter = LastChild;
            var nextContentPresenter = createNext(Content);

            nextContentPresenter.Content = Content;

            Debug.WriteLine($"Loaded - Current: {currentContentPresenter?.Name} Next: {nextContentPresenter.Name}");

            Animate(currentContentPresenter, nextContentPresenter);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Unloaded += TransitionControl_Unloaded;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (_transitionContainer == null)
            {
                return;
            }

            if (ReferenceEquals(oldContent, newContent))
            {
                return;
            }

            var currentContentPresenter = LastChild;
            var nextContentPresenter = createNext(newContent);

            Debug.WriteLine($"ContentChanged - Current: {currentContentPresenter?.GetType().Name} Next: {nextContentPresenter.GetType().Name}");

            nextContentPresenter.Content = newContent;

            if (IsLoaded)
            {
                Animate(currentContentPresenter, nextContentPresenter);
            }
        }

        private void TransitionControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= TransitionControl_Unloaded;

            _transitionContainer?.Children.Clear();

            BindingOperations.ClearAllBindings(this);
        }

        private ContentControl createNext(object content)
        {
            var container = new ContentControl
            {
                Content = content,
                ContentTemplate = ContentTemplate,
                ContentTemplateSelector = ContentTemplateSelector,
                IsTabStop = false
            };

            _transitionContainer.Children.Add(container);

            return container;
        }

        private void Animate(ContentControl currentContentPresenter, ContentControl nextContentPresenter)
        {
            var currentExitTransition = ExitTransitionFactory?.Invoke(currentContentPresenter) ?? ExitTransition.Clone();
            var currentEnterTransition = EnterTransitionFactory?.Invoke(nextContentPresenter) ?? EnterTransition.Clone();

            var exitingContentPresenter = currentContentPresenter;
            var enteringContentPresenter = nextContentPresenter;

            if (exitingContentPresenter != null)
            {
                void onExitTransitionCompleted(object sender, System.EventArgs e)
                {
                    var storyboard = sender as Storyboard;

                    if (storyboard != null)
                    {
                        storyboard.Completed -= onExitTransitionCompleted;
                    }

                    if (currentExitTransition != null)
                    {
                        currentExitTransition.Completed -= onExitTransitionCompleted;
                    }

                    BindingOperations.ClearAllBindings(exitingContentPresenter);
                    _transitionContainer.Children.Remove(exitingContentPresenter);
                }

                currentExitTransition.Completed += onExitTransitionCompleted;
                currentExitTransition.Begin(exitingContentPresenter);
            }

            currentEnterTransition.Begin(enteringContentPresenter);
        }
    }
}
