using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Models;
using DCS.Alternative.Launcher.Plugins.Settings.Dialogs;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.Threading;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views
{
    public class SettingsController
    {
        private readonly IContainer _container;

        public SettingsController(IContainer container)
        {
            _container = container;
        }

        public async Task<Viewport[]> EditViewportsAsync(ModuleViewportModel value)
        {
            var tasks = new List<Task>();
            var devices = _container.Resolve<ISettingsService>().GetViewportDevices(value.Module.Value.ModuleId);
            var windows = new List<Window>();
            var viewModels = new List<ViewportEditorWindowViewModel>();

            foreach (var monitorId in value.MonitorIds)
            {
                var screen = Screen.AllScreens.FirstOrDefault(s => s.DeviceName == monitorId);

                if (screen == null)
                {
                    Tracer.Warn($"Unable to find display id {monitorId} for viewport setup {value.Name}");
                    continue;
                }

                var viewportModels = new List<ViewportModel>();

                foreach (var viewport in value.Viewports.Where(v => v.MonitorId == screen.DeviceName))
                {
                    var model = new ViewportModel();

                    model.Height.Value = viewport.Height;
                    model.InitFile.Value = viewport.RelativeInitFilePath;
                    model.ImageUrl.Value = Path.Combine(Directory.GetCurrentDirectory(), $"Resources/Images/Viewports/{value.Module.Value.ModuleId}/{viewport.ViewportName}.jpg");
                    model.Name.Value = viewport.ViewportName;
                    model.Width.Value = viewport.Width;
                    model.X.Value = viewport.X;
                    model.Y.Value = viewport.Y;

                    viewportModels.Add(model);
                }

                var window = new ViewportEditorWindow();
                var vm = new ViewportEditorWindowViewModel(_container, false, monitorId, value.Module.Value, devices, viewportModels.ToArray());

                window.Screen = screen;
                window.DataContext = vm;
                window.Show();
                window.BringIntoView();

                windows.Add(window);
                viewModels.Add(vm);

                tasks.Add(EventAsync.FromEvent(
                    handler => window.Closed += handler,
                    handler => window.Closed -= handler));
            }

            await Task.WhenAny(tasks);

            foreach (var window in windows)
            {
                window.Close();
            }

            var save = windows.Any(w => w.DialogResult == true);
            var viewports = new List<Viewport>();

            if (save)
            {
                foreach (var viewModel in viewModels)
                {
                    var screen = Screen.AllScreens.First(s => s.DeviceName == viewModel.MonitorId);

                    foreach (var viewportModel in viewModel.Viewports)
                    {
                        viewports.Add(
                            new Viewport
                            {
                                Height = (int) viewportModel.Height.Value,
                                MonitorId = screen.DeviceName,
                                OriginalDisplayHeight = (int) screen.Bounds.Height,
                                OriginalDisplayWidth = (int) screen.Bounds.Width,
                                RelativeInitFilePath = viewportModel.InitFile.Value,
                                ViewportName = viewportModel.Name.Value,
                                Width = (int) viewportModel.Width.Value,
                                X = (int) viewportModel.X.Value,
                                Y = (int) viewportModel.Y.Value,
                            });
                    }

                }
            }

            return viewports.ToArray();
        }
    }
}
