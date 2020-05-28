using System.Collections.Generic;
using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Services.Settings
{
    public class ProfilesChangedEvenArgs : DeferredEventArgs
    {
        public ProfilesChangedEvenArgs(IEnumerable<Profile> profiles)
        {
            Profiles = profiles;
        }

        public IEnumerable<Profile> Profiles
        {
            get;
        }
    }
}