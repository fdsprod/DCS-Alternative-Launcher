using System.Threading.Tasks;
using System.Windows;
using DCS.Alternative.Launcher.ServiceModel;

namespace DCS.Alternative.Launcher.Plugins
{
    public interface IPlugin
    {
        string Name
        {
            get;
        }

        int LoadOrder
        {
            get;
        }

        ResourceDictionary ApplicationResources
        {
            get;
        }

        Task LoadAsync(IContainer container);

        Task UnloadAsync(IContainer container);
    }
}