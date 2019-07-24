using System;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Services.Navigation;

namespace DCS.Alternative.Launcher.ComponentModel
{
    public abstract class NavigationAwareBase : INavigationAware, IActivate, IDeactivate
    {
        private bool _isInitialized;

        public virtual async Task ActivateAsync()
        {
            if (!_isInitialized)
            {
                await InitializeAsync();
                _isInitialized = true;
            }
        }

        public virtual Task DeactivateAsync()
        {
            return Task.FromResult(true);
        }

        public virtual void NavigatedFrom(Type viewType)
        {
        }

        public virtual void NavigatedTo(Type viewType)
        {
        }

        public virtual Task OnNavigatingAsync(NavigatingEventArgs args)
        {
            return Task.FromResult(true);
        }

        protected virtual Task InitializeAsync()
        {
            return Task.FromResult(true);
        }
    }
}