using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Threading;

namespace DCS.Alternative.Launcher.Services.Navigation
{
    internal class NavigationService : INavigationService
    {
        private readonly IContainer _container;
        private readonly ContentControl _frame;

        public NavigationService(IContainer container, ContentControl frame)
        {
            _container = container;
            _frame = frame;
        }


        public async Task<bool> NavigateAsync(Type viewType, INavigationAware viewModel)
        {
            var success = false;

            var dispatcher = _frame.Dispatcher;
            var pattern = new NavigationHandlerTaskSource(
                handler => Navigated += handler,
                handler => Navigated -= handler);

            var view = _container.Resolve(viewType) as UserControl;

            Guard.RequireIsNotNull(view, nameof(view));

            if (dispatcher.CheckAccess())
                success = await navigateAsync(view, viewModel);
            else
                await dispatcher.InvokeAsync(async () => await navigateAsync(view, viewModel));

            await pattern.Task;

            return success;
        }

        public async Task<bool> NavigateAsync<TView, TViewModel>(TViewModel viewModel)
            where TView : UserControl, new()
            where TViewModel : INavigationAware
        {
            var success = false;

            var dispatcher = _frame.Dispatcher;
            var pattern = new NavigationHandlerTaskSource(
                handler => Navigated += handler,
                handler => Navigated -= handler);

            if (dispatcher.CheckAccess())
                success = await navigateAsync<TView, TViewModel>(viewModel);
            else
                await dispatcher.InvokeAsync(async () => await navigateAsync<TView, TViewModel>(viewModel));

            await pattern.Task;

            return success;
        }

        public event EventHandler<NavigatedEventArgs> Navigated;

        private async Task<bool> CanNavigateAsync(NavigatingEventArgs e)
        {
            await OnNavigatingAsync(this, e);

            if (e.Cancel) return false;

            var previousView = _frame.Content as FrameworkElement;

            var navigationAware = previousView?.DataContext as INavigationAware;

            if (navigationAware != null)
            {
                await navigationAware.OnNavigatingAsync(e);

                if (e.Cancel) return false;
            }

            return true;
        }

        protected Task OnNavigatedAsync(object content, object dataContext, Type to, Type from)
        {
            if (content != null)
            {
                var view = content as ContentControl;

                if (view == null) throw new ArgumentException("View '" + content.GetType().FullName + "' should inherit from ContentControl or one of its descendents.");

                view.DataContext = dataContext;
            }

            var navigationAware = dataContext as INavigationAware;

            navigationAware?.NavigatedTo(to);

            var handler = Navigated;

            handler?.Invoke(this, new NavigatedEventArgs(to, from));

            return Task.FromResult(true);
        }

        protected virtual async Task OnNavigatingAsync(object sender, NavigatingEventArgs e)
        {
            var view = _frame.Content as FrameworkElement;

            if (view == null) return;

            var deactivator = view.DataContext as IDeactivate;

            if (deactivator != null) await deactivator.DeactivateAsync();
        }

        private async Task<bool> navigateAsync(UserControl view, INavigationAware viewModel)
        {
            var previousView = _frame.Content;
            var activator = viewModel as IActivate;

            if (activator != null) await activator.ActivateAsync();

            _frame.Content = view;

            var navigationAware = viewModel;

            if (navigationAware != null && previousView != null) navigationAware.NavigatedFrom(previousView.GetType());

            await OnNavigatedAsync(view, viewModel, view.GetType(), previousView?.GetType());

            return true;
        }

        private async Task<bool> navigateAsync<TView, TViewModel>(TViewModel viewModel)
            where TView : UserControl, new()
            where TViewModel : INavigationAware
        {
            var view = _container.Resolve<TView>() as UserControl;
            return await navigateAsync(view, viewModel);
        }

        private class NavigationHandlerTaskSource : HandlerTaskSourceBase<object>
        {
            private readonly Action<EventHandler<NavigatedEventArgs>> _removeEventHandler;
            private readonly TaskCompletionSource<object> _tcs;

            public NavigationHandlerTaskSource(
                Action<EventHandler<NavigatedEventArgs>> addEventHandler,
                Action<EventHandler<NavigatedEventArgs>> removeEventHandler,
                Action beginAction = null)
            {
                if (addEventHandler == null) throw new ArgumentNullException(nameof(addEventHandler));

                _tcs = new TaskCompletionSource<object>();
                _removeEventHandler = removeEventHandler ?? throw new ArgumentNullException(nameof(removeEventHandler));

                addEventHandler.Invoke(EventCompleted);

                beginAction?.Invoke();
            }

            public override Task<object> Task
            {
                get { return _tcs.Task; }
            }

            private void EventCompleted(object sender, NavigatedEventArgs args)
            {
                _removeEventHandler.Invoke(EventCompleted);
                _tcs.SetResult(args);
            }
        }
    }
}