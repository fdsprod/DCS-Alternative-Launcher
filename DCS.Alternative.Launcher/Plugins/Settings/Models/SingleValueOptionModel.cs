using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Models
{
    public abstract class SingleValueOptionModel<T> : OptionModel
    {
        private bool _isValueChangeSuspended;

        protected SingleValueOptionModel(string id, string displayName, string description, Dictionary<string, object> @params)
            : base(id, displayName, description, @params)
        {
            Value.Subscribe(value =>
            {
                if (!_isValueChangeSuspended)
                {
                    ValueChangeObservable.Value = ConvertOutputValue(value);
                }
            });
        }

        protected IDisposable SuspendValueChangeNotification()
        {
            _isValueChangeSuspended = true;
            return Disposable.Create(() => _isValueChangeSuspended = false);
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