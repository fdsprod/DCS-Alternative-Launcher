using System;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Services.Navigation;

namespace DCS.Alternative.Launcher.ComponentModel
{
    public abstract class NavigationAwareBase : INavigationAware, IActivate, IDeactivate
    {
        public virtual Task ActivateAsync()
        {
            return Task.FromResult(true);
        }

        public virtual Task DeactivateAsync()
        {
            return Task.FromResult(true);
        }

        public virtual void NavigatedFrom(Type viewType)
        {
        }

        public void NavigatedTo(Type viewType)
        {
        }

        public Task OnNavigatingAsync(NavigatingEventArgs args)
        {
            return Task.FromResult(true);
        }
    }
}