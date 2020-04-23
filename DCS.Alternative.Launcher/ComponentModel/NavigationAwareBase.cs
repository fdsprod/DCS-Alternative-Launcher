using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Services.Navigation;

namespace DCS.Alternative.Launcher.ComponentModel
{
    public abstract class NavigationAwareBase : INavigationAware, IActivate, IDeactivate
    {
        private readonly SemaphoreSlim _asyncLock = new SemaphoreSlim(1);

        private bool _isInitialized;

        public virtual async Task ActivateAsync()
        {
            try
            {
                await _asyncLock.WaitAsync();

                if (!_isInitialized)
                {
                    await InitializeAsync();
                    _isInitialized = true;
                }
            }
            finally
            {
                _asyncLock.Release();
            }

            IsActivated = true;
        }

        public bool IsActivated
        {
            get;
            private set;
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