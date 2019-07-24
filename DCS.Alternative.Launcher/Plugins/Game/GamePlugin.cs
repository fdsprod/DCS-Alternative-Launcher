using DCS.Alternative.Launcher.Plugins.Game.Views;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugins.Game
{
    public class GamePlugin : PluginBase
    {
        public override string Name => "Game Plugin";

        public override string Author => "Jabbers";

        public override string SupportUrl => "https://github.com/jeffboulanger/DCS-Alternative-Launcher";

        protected override void RegisterUISiteItems(IPluginNavigationSite site)
        {
            site.RegisterPluginNavigation<GameView, GameViewModel>("GAME", this);

            base.RegisterUISiteItems(site);
        }
    }
}