using System;
using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Services.Settings
{
    public class SelectedProfileChangedEventArgs : DeferredEventArgs
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