using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugins
{
    public abstract class PluginBase : IPlugin
    {
        public abstract string Author
        {
            get;
        }

        public abstract string SupportUrl
        {
            get;
        }

        public abstract int LoadOrder
        {
            get;
        }

        public abstract string Name
        {
            get;
        }

        public virtual void OnLoad(IContainer container)
        {
            var site = container.Resolve<IPluginNavigationSite>();

            RegisterContainerItems(container);
            RegisterUISiteItems(site);
        }

        public virtual void OnUnload(IContainer container)
        {
        }

        protected virtual void RegisterContainerItems(IContainer container)
        {
        }

        protected virtual void RegisterUISiteItems(IPluginNavigationSite site)
        {
        }
    }
}