using System;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Windows
{
    public class PluginNavigationButton
    {
        public PluginNavigationButton(string name, Type viewType, Type viewModelType)
        {
            Name = name;
            ViewType = viewType;
            ViewModelType = viewModelType;
        }

        public string Name
        {
            get;
        }

        public ReactiveProperty<bool> IsSelected
        {
            get;
        } = new ReactiveProperty<bool>();

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