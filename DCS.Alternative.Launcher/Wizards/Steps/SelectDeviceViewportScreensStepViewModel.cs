using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Drawing;
using DCS.Alternative.Launcher.Models;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using Reactive.Bindings;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Wizards.Steps
{
    public class SelectDeviceViewportScreensStepViewModel : WizardStepBase
    {
        private readonly ISettingsService _settingsService;

        public SelectDeviceViewportScreensStepViewModel(IContainer container)
            : base(container)
        {
            _settingsService = container.Resolve<ISettingsService>();
        }

        public ReactiveCollection<ScreenModel> Screens
        {
            get;
        } = new ReactiveCollection<ScreenModel>();

        public ReactiveProperty<double> ViewportWidth
        {
            get;
        } = new ReactiveProperty<double>(1920);

        public ReactiveProperty<double> ViewportHeight
        {
            get;
        } = new ReactiveProperty<double>(1080);

        public override Task ActivateAsync()
        {
            var selectedDisplays = _settingsService.GetValue<string[]>(SettingsCategories.Viewports, SettingsKeys.GameDisplays);
            var selectedUIViewports = _settingsService.GetValue(SettingsCategories.Viewports, SettingsKeys.UIViewportsDisplays, new string[0]);
            var selectedDeviceViewports = _settingsService.GetValue(SettingsCategories.Viewports, SettingsKeys.DeviceViewportsDisplays, new string[0]);

            var screens = Screen.AllScreens.ToArray();
            var boundingBox = Rect.Empty;

            foreach (var screen in screens)
            {
                if (!selectedDisplays.Contains(screen.DeviceName))
                {
                    continue;
                }

                var screenBounds = screen.Bounds;

                if (boundingBox == Rect.Empty)
                {
                    boundingBox = screenBounds;
                }
                else
                {
                    boundingBox.Union(screenBounds);
                }
            }

            var offsetX = 0.0;
            var offsetY = 0.0;

            if (boundingBox.X < 0)
            {
                offsetX = Math.Abs(boundingBox.X);
            }
            else
            {
                offsetX = -boundingBox.X;
            }

            if (boundingBox.Y < 0)
            {
                offsetY = Math.Abs(boundingBox.Y);
            }
            else
            {
                offsetY = -boundingBox.Y;
            }

            var ratioX = 1920 / boundingBox.Width;
            var ratioY = 1080 / boundingBox.Height;
            var ratio = ratioX < ratioY ? ratioX : ratioY;

            boundingBox = Rect.Empty;

            foreach (var screen in screens)
            {
                if (!selectedDisplays.Contains(screen.DeviceName))
                {
                    continue;
                }

                var deviceName = screen.DeviceName;
                var screenBounds = screen.Bounds;
                var x = (offsetX + screenBounds.Left) * ratio;
                var y = (offsetY + screenBounds.Top) * ratio;
                var width = screenBounds.Width * ratio;
                var height = screenBounds.Height * ratio;

                if (boundingBox == Rect.Empty)
                {
                    boundingBox = new Rect(x, y, width, height);
                }
                else
                {
                    boundingBox.Union(new Rect(x, y, width, height));
                }

                var model = new ScreenModel
                {
                    Id = deviceName,
                    DisplayName = $"{deviceName.Replace("\\\\.\\DISPLAY", "Display ")} {screenBounds.Width}x{screenBounds.Height}",
                    RelativeX = x,
                    RelativeY = y,
                    RelativeWidth = width,
                    RelativeHeight = height,
                    ImageSource = ScreenCapture.Snapshot(screen),
                    ScreenBounds = screenBounds,
                    IsUIViewport = selectedUIViewports.Contains(deviceName),
                    IsGameDisplay = selectedDisplays.Contains(deviceName)
                };

                model.IsSelected.Value = selectedDeviceViewports.Contains(deviceName);

                Screens.Add(model);
            }

            ViewportWidth.Value = boundingBox.Width;
            ViewportHeight.Value = boundingBox.Height;

            return base.InitializeAsync();
        }

        public override bool Validate()
        {
            return base.Validate();
        }

        public override bool Commit()
        {
            var screens = Screens.Where(s => s.IsSelected.Value).Select(s => s.Id).ToArray();

            _settingsService.SetValue(SettingsCategories.Viewports, SettingsKeys.DeviceViewportsDisplays, screens);

            if (screens.Length == 1)
            {
                Controller.Steps.Add(new SelectInitialViewportsWizardStepViewModel(Container));
            }

            return base.Commit();
        }
    }
}