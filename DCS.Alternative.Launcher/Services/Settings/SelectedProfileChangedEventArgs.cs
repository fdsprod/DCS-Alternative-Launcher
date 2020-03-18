using System;

namespace DCS.Alternative.Launcher.Services.Settings
{
    public class SelectedProfileChangedEventArgs : EventArgs
    {
        public SelectedProfileChangedEventArgs(string profileName)
        {
            ProfileName = profileName;
        }

        public string ProfileName
        {
            get;
        }
    }
}