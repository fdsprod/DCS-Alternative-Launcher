using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.ServiceModel;

namespace DCS.Alternative.Launcher.Plugins.Support.Views
{
    public class SupportViewModel : NavigationAwareBase
    {
        private readonly IContainer _container;
        private readonly SupportController _controller;

        public SupportViewModel(IContainer container)
        {
            _container = container;
            _controller = container.Resolve<SupportController>();
        }
    }
}