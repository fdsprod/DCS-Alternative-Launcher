using System;
using DCS.Alternative.Launcher.Services.Settings;

namespace DCS.Alternative.Launcher.Services
{
    public class PluginRegisteredEventArgs : DeferredEventArgs
    {
        public PluginRegisteredEventArgs(string name, Type viewType, Type viewModelType)
        {
            Name = name;
            ViewType = viewType;
            ViewModelType = viewModelType;
        }

        public string Name
        {
            get;
        }

        public Type ViewType
        {
            get;
        }

        public Type ViewModelType
        {
            get;
        }
    }
}