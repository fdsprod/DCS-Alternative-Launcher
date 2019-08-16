using System.Collections.Generic;

namespace DCS.Alternative.Launcher.Plugins.Settings.Models
{
    public abstract class MultiValueOptionModel<T> : OptionModel
    {
        protected MultiValueOptionModel(string id, string displayName, string description, Dictionary<string, object> @params)
            : base(id, displayName, description, @params)
        {
        }

        protected abstract object ConvertOutputValue();

        protected void SignalValueChanged()
        {
            ValueChangeObservable.Value = ConvertOutputValue();
        }
    }

    
}