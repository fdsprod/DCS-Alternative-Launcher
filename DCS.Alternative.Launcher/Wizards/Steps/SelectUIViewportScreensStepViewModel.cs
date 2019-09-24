using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DCS.Alternative.Launcher.Analytics;
using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Drawing;
using DCS.Alternative.Launcher.Models;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using Reactive.Bindings;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Wizards.Steps
{
    public class SelectUIViewportScreensStepViewModel : WizardStepBase
    {
        private readonly IProfileSettingsService _profileSettingsService;

        public SelectUIViewportScreensStepViewModel(IContainer container)
            : base(container)
        {
            _profileSettingsService = container.Resolve<IProfileSettingsService>();
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
            var selectedDisplays = _profileSettingsService.GetValue<string[]>(ProfileSettingsCategories.Viewports, SettingsKeys.GameDisplays);
            var selectedUIViewports = _profileSettingsService.GetValue(ProfileSettingsCategories.Viewports, SettingsKeys.UIViewportsDisplays, new string[0]);

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

            var nonNegativeOffsetX = 0.0;
            var nonNegativeOffsetY = 0.0;

            if (boundingBox.X < 0)
            {
                nonNegativeOffsetX = Math.Abs(boundingBox.X);
            }

            if (boundingBox.Y < 0)
            {
                nonNegativeOffsetY = Math.Abs(boundingBox.Y);
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
                var x = (nonNegativeOffsetX + screenBounds.Left) * ratio;
                var y = (nonNegativeOffsetY + screenBounds.Top) * ratio;
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

                model.IsSelected.Value = selectedUIViewports.Contains(deviceName);

                Screens.Add(model);
            }

            ViewportWidth.Value = boundingBox.Width;
            ViewportHeight.Value = boundingBox.Height;

            return base.InitializeAsync();
        }

        public override bool Validate()
        {
            if (!Screens.Any(s => s.IsSelected.Value))
            {
                var window = WindowAssist.GetWindow(Controller);

                MessageBoxEx.Show("You must select at least 1 monitors for the UI Viewport.", "Select Monitor", parent: window);
                return false;
            }

            return base.Validate();
        }

        public override bool Commit()
        {
            var selectedScreens = Screens.Where(s => s.IsSelected.Value).ToArray();
            Tracker.Instance.SendEvent(AnalyticsCategories.Configuration, AnalyticsEvents.TotalUiDisplayCount, selectedScreens.Length.ToString());
            _profileSettingsService.SetValue(ProfileSettingsCategories.Viewports, SettingsKeys.UIViewportsDisplays, selectedScreens.Select(s => s.Id).ToArray());

            return base.Commit();
        }
    }
}