using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Views
{
    public class SettingsViewModel
    {
    }

    public class GameModuleModel
    {
        public GameModuleModel(string moduleName)
        {
            ModuleName.Value = moduleName;
        }

        public ReactiveProperty<string> ModuleName { get; } = new ReactiveProperty<string>();
    }
}