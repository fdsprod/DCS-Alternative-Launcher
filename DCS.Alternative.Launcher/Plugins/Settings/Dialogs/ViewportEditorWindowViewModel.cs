﻿using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Media;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Modules;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using Reactive.Bindings;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Plugins.Settings.Dialogs
{
    public class ViewportEditorWindowViewModel
    {
        private readonly Module _module;
        private readonly ViewportDevice[] _devices;
        private readonly ISettingsService _settingsService;
        public readonly string MonitorId;

        public ViewportEditorWindowViewModel(IContainer container, bool isPreview, string monitorId, Module module, ViewportDevice[] devices, ViewportModel[] viewports)
        {
            _module = module;
            MonitorId = monitorId;
            _settingsService = container.Resolve<ISettingsService>();
            _devices = devices;

            foreach (var viewport in viewports)
            {
                Viewports.Add(viewport);
            }

            IsNotPreview.Value = !isPreview;

            var canAddViewportPredicate = new Func<bool>(() => IsNotPreview.Value && devices.Any(d => Viewports.All(v => d.ViewportName != v.Name.Value)));
            var canAddViewportObservable = IsNotPreview.Select(v => Unit.Default).Merge(Viewports.Select(v => Unit.Default).ToObservable()).Select(v => canAddViewportPredicate());

            AddViewportCommand = new ReactiveCommand(canAddViewportObservable, canAddViewportPredicate());
            DeleteViewportCommand = new ReactiveCommand<ViewportModel>(IsNotPreview.AsObservable(), isPreview);
            SaveCommand = new ReactiveCommand(IsNotPreview.AsObservable(), isPreview);
            CancelCommand = new ReactiveCommand(IsNotPreview.AsObservable(), isPreview);

            AddViewportCommand.Subscribe(OnAddViewport);
            DeleteViewportCommand.Subscribe(OnDeleteViewport);
            SaveCommand.Subscribe(OnSave);
            CancelCommand.Subscribe(OnCancel);
        }

        private void OnCancel()
        {
            DialogResult.Value = false;
        }

        private void OnSave()
        {
            DialogResult.Value = true;
        }

        public ReactiveProperty<bool?> DialogResult
        {
            get;
        } = new ReactiveProperty<bool?>();
        
        public ReactiveProperty<bool> IsNotPreview
        {
            get;
        } = new ReactiveProperty<bool>();

        private void OnDeleteViewport(ViewportModel value)
        {
            var window = WindowAssist.GetWindow(this);

            if (MessageBoxEx.Show($"Are you sure you want to delete viewport {value.Name.Value}?", "Delete Viewport", System.Windows.MessageBoxButton.YesNo, parent: window) == System.Windows.MessageBoxResult.Yes)
            {
                Viewports.Remove(value);
            }
        }

        private void OnAddViewport()
        {
            var window = WindowAssist.GetWindow(this);
            var dialog = new SelectViewportsDialog();
            var screen = Screen.AllScreens.First(d => d.DeviceName == MonitorId);

            foreach (var device in _devices)
            {
                if (Viewports.Any(v => v.Name.Value == device.ViewportName))
                {
                    continue;
                }

                var model = new ViewportModel();
                
                model.Height.Value = device.Height;
                model.InitFile.Value = device.RelativeInitFilePath;
                model.ImageUrl.Value = Path.Combine(Directory.GetCurrentDirectory(), $"Resources/Images/Viewports/{_module.ModuleId}/{device.ViewportName}.jpg");
                model.Name.Value = device.ViewportName;
                model.Width.Value = device.Width;
                model.X.Value = screen.Bounds.Width / 2 - device.Width / 2;
                model.Y.Value = screen.Bounds.Height / 2 - device.Height / 2;

                dialog.Viewports.Add(model);
            }

            dialog.SelectedViewport = dialog.Viewports.First();
            dialog.Owner = window;

            if (dialog.ShowDialog() ?? false)
            {
                var viewport = dialog.SelectedViewport;

                Viewports.Add(viewport);
            }
        }

        public ReactiveCommand SaveCommand
        {
            get;
        } 

        public ReactiveCommand CancelCommand
        {
            get;
        } 

        public ReactiveCommand AddViewportCommand
        {
            get;
        }

        public ReactiveCommand<ViewportModel> DeleteViewportCommand
        {
            get;
        } 

        public ReactiveCollection<ViewportModel> Viewports
        {
            get;
        } = new ReactiveCollection<ViewportModel>();
    }
}