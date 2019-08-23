using System;
using System.Collections.Generic;
using System.Reactive.Disposables;

namespace DCS.Alternative.Launcher.Plugins.Settings.Models
{
    public abstract class MultiValueOptionModel<T> : OptionModelBase
    {
        private bool _isValueChangeSuspended;

        protected MultiValueOptionModel(string id, string displayName, string description, Dictionary<string, object> @params)
            : base(id, displayName, description, @params)
        {
        }

        protected abstract object ConvertOutputValue();

        protected IDisposable SuspendValueChangeNotification()
        {
            _isValueChangeSuspended = true;
            return Disposable.Create(() => _isValueChangeSuspended = false);
        }

        protected void SignalValueChanged()
        {
            if (!_isValueChangeSuspended)
            {
                ValueChangeObservable.Value = ConvertOutputValue();
            }
        }
    }

    
}