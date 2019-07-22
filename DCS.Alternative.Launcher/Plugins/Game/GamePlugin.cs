using DCS.Alternative.Launcher.Plugins.Game.Views;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugins.Game
{
    public class GamePlugin : PluginBase
    {
        public override string Name
        {
            get { return "Game Plugin"; }
        }

        public override string Author
        {
            get { return "Jabbers"; }
        }

        public override string SupportUrl
        {
            get { return "(undefined)"; }
        }

        protected override void RegisterUISiteItems(IPluginNavigationSite site)
        {
            site.RegisterPluginNavigation<GameView, GameViewModel>("GAME", this);

            base.RegisterUISiteItems(site);
        }
    }
}