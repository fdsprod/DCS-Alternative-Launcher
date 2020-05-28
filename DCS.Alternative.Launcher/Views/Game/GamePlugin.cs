using System.Threading.Tasks;
using DCS.Alternative.Launcher.Plugins.Game.Views;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugins.Game
{
    public class GamePlugin : PluginBase
    {
        public override string Name
        {
            get { return "Game Plugin"; }
        }
        public override int LoadOrder
        {
            get { return 0; }
        }
        public override string Author
        {
            get { return "Jabbers"; }
        }
        public override string SupportUrl
        {
            get { return "https://github.com/jeffboulanger/DCS-Alternative-Launcher"; }
        }

        protected override Task RegisterContainerItemsAsync(IContainer container)
        {
            container.Register(new GameController(container));

            return base.RegisterContainerItemsAsync(container);
        }

        protected override async Task RegisterUISiteItemsAsync(IPluginNavigationSite site)
        {
            await site.RegisterPluginNavigationAsync<GameView, GameViewModel>("GAME", this);
            await base.RegisterUISiteItemsAsync(site);
        }
    }
}