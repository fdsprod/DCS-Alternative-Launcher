using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.Models;
using DCS.Alternative.Launcher.Plugins.Settings.Dialogs;
using DCS.Alternative.Launcher.ServiceModel;
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

        public void EditViewports(ModuleViewportModel value)
        {
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
                var vm = new ViewportEditorWindowViewModel(_container, false, monitorId, value.Module.Value, viewportModels.ToArray());

                window.Screen = screen;
                window.DataContext = vm;
                window.Show();
                window.BringIntoView();
            }
        }
    }
}
