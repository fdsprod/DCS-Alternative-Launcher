using DCS.Alternative.Launcher.Plugins.Settings.Views;

namespace DCS.Alternative.Launcher.StartupApps.Views.Settings
{
    public class StartupAppsSettingsViewModel : SettingsCategoryViewModelBase
    {
        public StartupAppsSettingsViewModel(StartupAppsController startupAppsController, SettingsController settingsController) 
                : base("    APPS", settingsController)
        {
        }
    }
}