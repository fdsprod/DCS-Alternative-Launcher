using System;
using System.Collections.Generic;
using System.Text;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Models
{
    public class SettingsProfileModel
    {
        public SettingsProfileModel(string name)
        {
            Name.Value = name;
        }

        public ReactiveProperty<string> Name
        {
            get;
        } = new ReactiveProperty<string>();
    }
}
