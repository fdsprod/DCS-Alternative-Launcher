using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.Diagnostics;

namespace DCS.Alternative.Launcher
{
    internal class UserFriendlyExceptionHandler : GeneralExceptionHandler
    {
        protected override Task OnErrorOverrideAsync(Exception e)
        {
            var assembly = Assembly.GetEntryAssembly();
            var name = assembly.GetName();
            var version = name.Version;
            var buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
            var displayableVersion = $"{version.Major} ({buildDate.ToShortDateString()})";
            
            var userMessage = $"Release {displayableVersion}{Environment.NewLine}{Environment.NewLine}{e.Message}";
            var message = $"Release {displayableVersion}{Environment.NewLine}{Environment.NewLine}{e}";

            Clipboard.SetText(message);
            MessageBoxEx.Show($"An error has occured and has been copied to your clipboard.   Please report the error to the developer.{Environment.NewLine}{userMessage}", "General Exception");

            return base.OnErrorOverrideAsync(e);
        }
    }
}