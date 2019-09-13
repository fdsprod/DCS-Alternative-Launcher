using System.Collections.Generic;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Models
{
    public abstract class OptionModelBase
    {
        protected OptionModelBase(string id, string displayName, string description, Dictionary<string, object> @params)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            Params = @params ?? new Dictionary<string, object>();
        }

        public Dictionary<string, object> Params
        {
            get;
        }

        public string Description
        {
            get;
        }

        public string Id
        {
            get;
        }

        public string DisplayName
        {
            get;
        }

        public ReactiveProperty<object> ValueChangeObservable
        {
            get;
        } = new ReactiveProperty<object>(mode: ReactivePropertyMode.DistinctUntilChanged);

        public abstract void ResetValue(object value);
    }
}