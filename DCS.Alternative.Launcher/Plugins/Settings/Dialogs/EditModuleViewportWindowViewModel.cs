using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using DCS.Alternative.Launcher.Modules;
using DCS.Alternative.Launcher.Plugins.Settings.Models;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Dialogs
{
    public class MonitorModel
    {
        public MonitorModel(string deviceName, Rectangle screenBounds)
        {
            Name = deviceName;
            DisplayName.Value = $"{deviceName.Replace("\\\\.\\DISPLAY", "Display ")} {screenBounds.Width}x{screenBounds.Height}";
        }

        public ReactiveProperty<string> DisplayName
        {
            get;
        } = new ReactiveProperty<string>();

        public string Name
        {
            get;
        }
    }

    public class BoundsModel
    {
        public ReactiveProperty<int> X
        {
            get;
            set;
        } = new ReactiveProperty<int>();

        public ReactiveProperty<int> Y
        {
            get;
            set;
        } = new ReactiveProperty<int>();

        public ReactiveProperty<int> Width
        {
            get;
            set;
        } = new ReactiveProperty<int>();

        public ReactiveProperty<int> Height
        {
            get;
            set;
        } = new ReactiveProperty<int>();
    }

    public enum LocationIndicator
    {
        None,
        Left,
        Right
    }

    public class EditModuleViewportWindowViewModel
    {
        private readonly InstallLocation _install;

        public EditModuleViewportWindowViewModel(
            InstallLocation install, 
            IEnumerable<Module> modules,
            string moduleId = null,
            string selectedMonitorName = null,
            string initFilePath = null,
            string viewportName = null,
            LocationIndicator locationIndicator = LocationIndicator.None,
            Bounds bounds = null)
        {
            _install = install;

            var screens = Screen.AllScreens;

            foreach (var screen in screens)
            {
                Monitors.Add(new MonitorModel(screen.DeviceName, screen.Bounds));
            }

            IsCreateMode = moduleId == null;

            SelectedMonitor.Value =
                Monitors.FirstOrDefault(m => m.Name == selectedMonitorName) ?? Monitors.FirstOrDefault();

            foreach (var module in modules)
            {
                Modules.Add(module);
            }

            SelectedModule.Value = Modules.FirstOrDefault(m => m.ModuleId == moduleId);
            InitFilePath.Value = initFilePath;
            InitFilePath.Subscribe(OnInitFilePathChanged);
            ViewportName.Value = viewportName;
            IsNoLocationIndicator.Value = locationIndicator == LocationIndicator.None;
            IsLeftLocationIndicator.Value = locationIndicator == LocationIndicator.Left;
            IsRightLocationIndicator.Value = locationIndicator == LocationIndicator.Right;

            Bounds = new ReactiveProperty<BoundsModel>(new BoundsModel());

            if (bounds != null)
            {
                Bounds.Value.X.Value = (int)bounds.X;
                Bounds.Value.Y.Value = (int)bounds.Y;
                Bounds.Value.Width.Value = (int)bounds.Width;
                Bounds.Value.Height.Value = (int)bounds.Height;
            }

            SelectInitFilePathCommand.Subscribe(OnSelectInitFilePath);
            SnipBoundsCommand.Subscribe(OnSnipBounds);

            IsValid =
                SelectedMonitor.Select(_ => Unit.Default).Merge(
                    SelectedModule.Select(_ => Unit.Default).Merge(
                        InitFilePath.Select(_ => Unit.Default).Merge(
                            ViewportName.Select(_ => Unit.Default)))).Select(_ =>
                    SelectedMonitor.Value != null &&
                    SelectedModule.Value != null &&
                    !string.IsNullOrWhiteSpace(InitFilePath.Value) &&
                    !string.IsNullOrWhiteSpace(ViewportName.Value)).ToReactiveProperty();
        }

        public bool IsCreateMode
        {
            get;
        }

        public ReactiveProperty<bool> IsValid
        {
            get;
        } 

        public ReactiveCollection<MonitorModel> Monitors
        {
            get;
        } = new ReactiveCollection<MonitorModel>();

        public ReactiveProperty<MonitorModel> SelectedMonitor
        {
            get;
        } = new ReactiveProperty<MonitorModel>();

        public ReactiveCollection<Module> Modules
        {
            get;
        } = new ReactiveCollection<Module>();

        public ReactiveProperty<Module> SelectedModule
        {
            get;
        } = new ReactiveProperty<Module>();

        public ReactiveProperty<string> InitFilePath
        {
            get;
        } = new ReactiveProperty<string>();

        public ReactiveCommand SelectInitFilePathCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveProperty<string> ViewportName
        {
            get;
        } = new ReactiveProperty<string>();

        public ReactiveProperty<bool> IsNoLocationIndicator
        {
            get;
        } = new ReactiveProperty<bool>(true);

        public ReactiveProperty<bool> IsLeftLocationIndicator
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> IsRightLocationIndicator
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveProperty<BoundsModel> Bounds
        {
            get;
        } = new ReactiveProperty<BoundsModel>();

        public ReactiveCommand SnipBoundsCommand
        {
            get;
        } = new ReactiveCommand();

        private void OnSnipBounds()
        {
            
        }

        private Regex _viewportNameRegex = new Regex("(?<=try_find_assigned_viewport\\(\")(.*?)(?=\"\\))");

        private void OnInitFilePathChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(ViewportName.Value) && File.Exists(value))
            {
                var contents = File.ReadAllText(value);
                var match = _viewportNameRegex.Match(contents);

                if (match.Success)
                {
                    var name = contents.Substring(match.Index, match.Length);
                    ViewportName.Value = name;
                }
            }
        }

        private void OnSelectInitFilePath()
        {
            var dialog = new OpenFileDialog();

            dialog.InitialDirectory = Path.GetFullPath(
                SelectedModule.Value == null
                    ? _install.Directory
                    : Directory.Exists(SelectedModule.Value.CockpitScriptsFolderPath) 
                        ? SelectedModule.Value.CockpitScriptsFolderPath 
                        : SelectedModule.Value.BaseFolderPath);
            dialog.AutoUpgradeEnabled = true;
            dialog.RestoreDirectory = true;
            dialog.Filter = "Init Lua (*_init.lua)|*_init.lua";
            dialog.CheckFileExists = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                InitFilePath.Value = dialog.FileName;
            }
        }
    }
}