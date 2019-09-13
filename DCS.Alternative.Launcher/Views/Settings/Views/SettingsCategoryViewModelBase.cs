using System.Threading.Tasks;
using DCS.Alternative.Launcher.ComponentModel;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views
{
    public abstract class SettingsCategoryViewModelBase : IActivate
    {
        protected SettingsController Controller;

        protected SettingsCategoryViewModelBase(string name, SettingsController controller, bool isHitTestVisible = true)
        {
            Name = name;
            Controller = controller;
            IsHitTestVisible = isHitTestVisible;
        }

        public bool IsInitialized
        {
            get;
            private set;
        }

        public bool IsHitTestVisible
        {
            get;
        }

        public string Name
        {
            get;
        }

        public virtual async Task ActivateAsync()
        {
            if (!IsInitialized)
            {
                IsInitialized = true;
                await InitializeAsync();
            }
        }

        protected virtual Task InitializeAsync()
        {
            return Task.FromResult(true);
        }
    }
}