using System;
using System.Threading.Tasks;

namespace DCS.Alternative.Launcher.Services.Navigation
{
    public interface INavigationAware
    {
        void NavigatedFrom(Type viewType);

        void NavigatedTo(Type viewType);

        Task OnNavigatingAsync(NavigatingEventArgs args);
    }
}