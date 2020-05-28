using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using Reactive.Bindings;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Wizards.Steps.FirstUse
{
    public class CreateProfileWizardStepViewModel : WizardStepBase
    {
        private readonly IProfileService _profileSettingsService;

        public CreateProfileWizardStepViewModel(IContainer container)
            : base(container)
        {
            _profileSettingsService = container.Resolve<IProfileService>();
        }

        public ReactiveProperty<string> ProfileName
        {
            get;
        } = new ReactiveProperty<string>("Default");

        public ReactiveProperty<bool> IsSingleDisplaySetup
        {
            get;
        } = new ReactiveProperty<bool>(true);

        public ReactiveProperty<bool> IsHeliosSetup
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> IsSimPitSetup
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> IsVirtualRealitySetup
        {
            get;
        } = new ReactiveProperty<bool>();
        
        public override bool Validate()
        {
            if (string.IsNullOrEmpty(ProfileName.Value))
            {
                var window = WindowAssist.GetWindow(Controller);
                MessageBoxEx.Show("A profile name is required to continue.", "Profile Name Required", parent:window);
                return false;
            }

            var invalidChars = Path.GetInvalidFileNameChars();

            if(ProfileName.Value.Any(ch=> invalidChars.Contains(ch)))
            {
                var window = WindowAssist.GetWindow(Controller);
                MessageBoxEx.Show($"The name of the profile cannot contain any of the following characters: {string.Join(",", invalidChars.Where(c => !char.IsControl(c)))}", "Invalid Name", parent: window);
                return false;
            }

            if (!IsSingleDisplaySetup.Value && !IsSimPitSetup.Value && !IsVirtualRealitySetup.Value && !IsHeliosSetup.Value)
            {
                var window = WindowAssist.GetWindow(Controller);
                MessageBoxEx.Show("Please select a primary setup type by clicking on one of the images for either Single, Multiple Monitor or VR setup, to continue.", "Screen Setup", parent: window);
                return false;
            }

            return base.Validate();
        }

        public override bool Commit()
        {
            var profileType = SettingsProfileType.SingleMonitor;

            if (IsSimPitSetup.Value)
            {
                profileType = SettingsProfileType.SimPit;
            }

            if (IsVirtualRealitySetup.Value)
            {
                profileType = SettingsProfileType.VirtualReality;
            }

            if (IsHeliosSetup.Value)
            {
                profileType = SettingsProfileType.Helios;
            }

            var profile = new Profile { Name = ProfileName.Value, ProfileType = profileType };

            _profileSettingsService.AddProfile(profile);
            _profileSettingsService.SelectedProfileName = profile.Name;
            _profileSettingsService.SetValue(ProfileCategories.GameOptions, "options.VR.enabled", IsVirtualRealitySetup.Value);

            var primaryScreen = Screen.PrimaryScreen;
            
            //switch (profileType)
            //{
            //    case SettingsProfileType.SingleMonitor:
            //        _profileSettingsService.SetValue(Viewport.Viewports, SettingsKeys.DeviceViewportsDisplays, new []{primaryScreen.DeviceName});
            //        break;
            //    case SettingsProfileType.Helios:
            //    case SettingsProfileType.SimPit:
            //        Controller.Steps.Add(new SelectGameViewportScreensStepViewModel(Container));
            //        Controller.Steps.Add(new SelectUIViewportScreensStepViewModel(Container));
            //        Controller.Steps.Add(new SelectDeviceViewportScreensStepViewModel(Container));
            //        break;
            //    case SettingsProfileType.VirtualReality:
            //        _profileSettingsService.SetValue(ProfileCategories.Viewports, SettingsKeys.DeviceViewportsDisplays, new[] { primaryScreen.DeviceName });
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}

            return base.Commit();
        }
    }
}