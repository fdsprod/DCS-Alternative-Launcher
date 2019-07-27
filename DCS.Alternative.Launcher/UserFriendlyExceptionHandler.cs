using System;
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
            Clipboard.SetText(e.ToString());
            MessageBoxEx.Show($"An error has occured and has been copied to your clipboard.   Please report the error to the developer.{Environment.NewLine}{e}", "General Exception", MessageBoxButton.OK);

            return base.OnErrorOverrideAsync(e);
        }
    }
}