using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using DCS.Alternative.Launcher.Services.Navigation;

namespace DCS.Alternative.Launcher.Services
{
    public interface INavigationService
    {
        Task<bool> NavigateAsync(Type view, INavigationAware viewModel);

        Task<bool> NavigateAsync<TView, TViewModel>(TViewModel viewModel)
            where TView : UserControl, new()
            where TViewModel : INavigationAware;
    }
}