using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.ComponentModel;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Categories
{
    public abstract class SettingsCategoryViewModelBase : IActivate
    {
        protected SettingsCategoryViewModelBase(string name, SettingsController controller, bool isHitTestVisible = true)
        {
            Name = name;
            Controller = controller;
            IsHitTestVisible = isHitTestVisible;
        }

        protected SettingsController Controller;

        public bool IsInitialized
        {
            get;
            private set;
        }

        public bool IsHitTestVisible
        {
            get;
            private set;
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
