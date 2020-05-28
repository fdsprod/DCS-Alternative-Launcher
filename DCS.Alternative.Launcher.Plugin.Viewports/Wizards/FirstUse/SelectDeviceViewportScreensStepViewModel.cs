using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Drawing;
using DCS.Alternative.Launcher.Plugin.Viewports.DomainObjects;
using DCS.Alternative.Launcher.Plugin.Viewports.Models;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.Storage.Profiles;
using Reactive.Bindings;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Plugin.Viewports.Wizards.FirstUse
{
    public class SelectDeviceViewportScreensStepViewModel : WizardStepBase
    {
        private readonly IProfileService _profileSettingsService;

        public SelectDeviceViewportScreensStepViewModel(IContainer container)
            : base(container)
        {
            _profileSettingsService = container.Resolve<IProfileService>();
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
            var selectedDisplays = _profileSettingsService.GetValue<string[]>(ViewportProfileCategories.Viewports, ViewportSettingKeys.GameDisplays);
            var selectedUIViewports = _profileSettingsService.GetValue(ViewportProfileCategories.Viewports, ViewportSettingKeys.UIViewportsDisplays, new string[0]);
            var selectedDeviceViewports = _profileSettingsService.GetValue(ViewportProfileCategories.Viewports, ViewportSettingKeys.DeviceViewportsDisplays, new string[0]);

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

        public override bool Commit()
        {
            var screens = Screens.Where(s => s.IsSelected.Value).Select(s => s.Id).ToArray();

            _profileSettingsService.SetValue(ViewportProfileCategories.Viewports, ViewportSettingKeys.DeviceViewportsDisplays, screens);

            var profiles = ProfileStorageAdapter.GetAll();
            var selectedProfile = profiles.FirstOrDefault(p => p.Name == _profileSettingsService.SelectedProfileName);

            Guard.RequireIsNotNull(selectedProfile, nameof(selectedProfile));

            switch (selectedProfile.ProfileType)
            {
                case SettingsProfileType.SingleMonitor:
                    break;
                case SettingsProfileType.SimPit:
                case SettingsProfileType.Helios:
                    if (screens.Length == 1)
                    {
                        Controller.Steps.Add(new SelectInitialViewportsWizardStepViewModel(Container));
                    }
                    break;
                case SettingsProfileType.VirtualReality:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return base.Commit();
        }
    }
}