using DCS.Alternative.Launcher.ServiceModel;

namespace DCS.Alternative.Launcher.Plugins
{
    public interface IPlugin
    {
        string Name { get; }

        int LoadOrder
        {
            get;
        }

        void OnLoad(IContainer container);

        void OnUnload(IContainer container);
    }
}