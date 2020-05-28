using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Plugins.Settings.Views;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.Services.Settings;
using DCS.Alternative.Launcher.Wizards;

namespace DCS.Alternative.Launcher
{
    public class ApplicationEventRegistry
    {
        private static readonly Task CompletedTask = Task.FromResult(0);

        public event EventHandler<DeferredEventArgs> ApplicationStartupComplete;
        public event EventHandler<DeferredEventArgs> ApplicationStartup;

        public event EventHandler<ProfilesChangedEvenArgs> ProfilesChanged;
        public event EventHandler<SelectedProfileChangedEventArgs> CurrentProfileChanged;
        
        public event EventHandler<PluginRegisteredEventArgs> PluginRegistered;

        public event EventHandler<PopulateSettingsEventArgs> PopulateSettings;

        public event EventHandler<AppendFirstUseWizardStepsEventArgs> AppendFirstUseWizardSteps;

        public event EventHandler<DeferredEventArgs> BeforeDcsLaunched;
        public event EventHandler<DeferredEventArgs> AfterDcsLaunched;

        internal async Task InvokeApplicationStartupAsync(object sender, DeferredEventArgs e)
        {
            var handler = ApplicationStartup;
            await InvokeAsync(handler, sender, e);
        }

        internal async Task InvokeApplicationStartupCompleteAsync(object sender, DeferredEventArgs e)
        {
            var handler = ApplicationStartupComplete;
            await InvokeAsync(handler, sender, e);
        }

        internal async Task InvokeProfilesChangedAsync(object sender, ProfilesChangedEvenArgs e)
        {
            var handler = ProfilesChanged;
            await InvokeAsync(handler, sender, e);
        }

        internal async Task InvokeCurrentProfileChangedAsync(object sender, SelectedProfileChangedEventArgs e)
        {
            var handler = CurrentProfileChanged;
            await InvokeAsync(handler, sender, e);
        }

        internal async Task InvokeBeforeDcsLaunchedAsync(object sender, DeferredEventArgs e)
        {
            var handler = BeforeDcsLaunched;
            await InvokeAsync(handler, sender, e);
        }

        internal async Task InvokeAfterDcsLaunchedAsync(object sender, DeferredEventArgs e)
        {
            var handler = AfterDcsLaunched;
            await InvokeAsync(handler, sender, e);
        }

        internal async Task InvokePluginRegisteredAsync(object sender, PluginRegisteredEventArgs e)
        {
            var handler = PluginRegistered;
            await InvokeAsync(handler, sender, e);
        }

        internal async Task InvokePopulateSettingsAsync(object sender, PopulateSettingsEventArgs e)
        {
            var handler = PopulateSettings;
            await InvokeAsync(handler, sender, e);
        }

        internal async Task InvokeAppendFirstUseWizardStepsAsync(object sender, AppendFirstUseWizardStepsEventArgs e)
        {
            var handler = AppendFirstUseWizardSteps;
            await InvokeAsync(handler, sender, e);
        }

        internal static Task InvokeAsync<T>(EventHandler<T> eventHandler, object sender, T eventArgs)
            where T : DeferredEventArgs
        {
            return InvokeAsync(eventHandler, sender, eventArgs, CancellationToken.None);
        }

        internal static Task InvokeAsync<T>(EventHandler<T> eventHandler, object sender, T eventArgs, CancellationToken cancellationToken)
            where T : DeferredEventArgs
        {
            if (eventHandler == null)
            {
                return CompletedTask;
            }

            var tasks = eventHandler.GetInvocationList()
                .OfType<EventHandler<T>>()
                .Select(invocationDelegate =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    invocationDelegate(sender, eventArgs);

                    var deferral = eventArgs.GetCurrentDeferralAndReset();

                    return deferral?.WaitForCompletion(cancellationToken) ?? CompletedTask;
                })
                .ToArray();

            return Task.WhenAll(tasks);
        }
    }
}