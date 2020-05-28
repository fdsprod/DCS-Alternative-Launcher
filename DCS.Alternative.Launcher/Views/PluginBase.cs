using System.Threading.Tasks;
using System.Windows;
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

        public virtual ResourceDictionary ApplicationResources
        {
            get;
        } 

        public virtual async Task LoadAsync(IContainer container)
        {
            var site = container.Resolve<IPluginNavigationSite>();

            await RegisterContainerItemsAsync(container);
            await RegisterUISiteItemsAsync(site);
        }

        public virtual Task UnloadAsync(IContainer container)
        {
            return Task.CompletedTask;
        }

        protected virtual Task RegisterContainerItemsAsync(IContainer container)
        {
            return Task.CompletedTask;
        }

        protected virtual Task RegisterUISiteItemsAsync(IPluginNavigationSite site)
        {
            return Task.CompletedTask;
        }
    }
}