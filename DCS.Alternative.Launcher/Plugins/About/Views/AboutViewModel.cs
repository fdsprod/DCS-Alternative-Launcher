using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.ServiceModel;

namespace DCS.Alternative.Launcher.Plugins.About.Views
{
    public class AboutViewModel : NavigationAwareBase
    {
        private readonly IContainer _container;
        private readonly AboutController _controller;

        public AboutViewModel(IContainer container)
        {
            _container = container;
            _controller = container.Resolve<AboutController>();
        }
    }
}