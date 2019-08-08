using System;
using System.Collections.Generic;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Models
{
    public abstract class SingleValueAdvancedOptionModel<T> : AdvancedOptionModel
    {
        protected SingleValueAdvancedOptionModel(string id, string displayName, string description, Dictionary<string, object> @params)
            : base(id, displayName, description, @params)
        {
            Value.Subscribe(value => { ValueChangeObservable.Value = ConvertOutputValue(value); });
        }

        protected virtual object ConvertOutputValue(T value)
        {
            return value;
        }

        public ReactiveProperty<T> Value
        {
            get;
            set;
        } = new ReactiveProperty<T>(mode: ReactivePropertyMode.DistinctUntilChanged);
    }
}