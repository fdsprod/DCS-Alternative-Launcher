using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Models;
using DCS.Alternative.Launcher.Plugins.Settings.Dialogs;
using DCS.Alternative.Launcher.Plugins.Settings.Views.Categories;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.Windows.FirstUse;
using DCS.Alternative.Launcher.Wizards;
using DCS.Alternative.Launcher.Wizards.Steps;
using Reactive.Bindings;
using Application = System.Windows.Application;
using Screen = WpfScreenHelper.Screen;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views
{
    public class SettingsViewModel : NavigationAwareBase
    {
        private readonly IContainer _container;
        private readonly SettingsController _controller;

        public SettingsViewModel(IContainer container)
        {
            _container = container;
            _controller = container.Resolve<SettingsController>();

            Categories.Add(new CategoryHeaderSettingsViewModel("SETTINGS"));
            Categories.Add(new InstallationSettingsViewModel(_controller));
            Categories.Add(new ViewportSettingsViewModel(_controller));
            Categories.Add(new CategoryHeaderSettingsViewModel("ADVANCED OPTIONS"));
            Categories.Add(new GraphicsSettingsViewModel(_controller));

            SelectedCategory.Value = Categories.First(c => !(c is CategoryHeaderSettingsViewModel));
            SelectedCategory.Subscribe(OnSelectedCategoryChanged);
        }

        private async void OnSelectedCategoryChanged(SettingsCategoryViewModelBase value)
        {
            if (value != null)
            {
                await value.ActivateAsync();
            }
        }

        public ReactiveCollection<SettingsCategoryViewModelBase> Categories
        {
            get;
        } = new ReactiveCollection<SettingsCategoryViewModelBase>();

        public ReactiveProperty<SettingsCategoryViewModelBase> SelectedCategory
        {
            get;
        } = new ReactiveProperty<SettingsCategoryViewModelBase>();

        public override async Task ActivateAsync()
        {
            try
            {
                if (SelectedCategory.Value != null)
                {
                    await SelectedCategory.Value.ActivateAsync();
                }
            }
            catch (Exception e)
            {
                Tracer.Error(e);
            }

            await base.ActivateAsync();
        }
    }
}