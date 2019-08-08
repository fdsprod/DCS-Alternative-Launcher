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
            var message = $"Version: {Assembly.GetEntryAssembly().GetName().Version}{Environment.NewLine}{e}";

            Clipboard.SetText(message);
            MessageBoxEx.Show($"An error has occured and has been copied to your clipboard.   Please report the error to the developer.{Environment.NewLine}{message}", "General Exception", MessageBoxButton.OK);

            return base.OnErrorOverrideAsync(e);
        }
    }
}