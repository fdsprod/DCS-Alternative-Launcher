using System;
using System.ComponentModel;
using System.Linq;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using Newtonsoft.Json.Linq;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Models
{
    public class ComboBoxOptionModel : SingleValueOptionModel<SelectorItem>
    {
        public ComboBoxOptionModel(Option option)
            : base(option.Id, option.DisplayName, option.Description, option.Params)
        {
            if (Params.TryGetValue("Items", out var items))
            {
                var array = (JArray)items;

                for (var i = 0; i < array.Count; i++)
                {
                    var item = array[i];
                    var converter = TypeDescriptor.GetConverter(option.Value.GetType());

                    Items.Add(new SelectorItem
                    {
                        DisplayName = item["DisplayName"].ToString(),
                        Value = converter.ConvertFromString(item["Value"].ToString())
                    });
                }

                using (SuspendValueChangeNotification())
                {
                    Value.Value = Items.FirstOrDefault(i => i.Value.Equals(option.Value));

                    if (Value.Value == null)
                    {
                        Value.Value = Items.FirstOrDefault();
                        Tracer.Warn($"Unable to select default value {option.Value} for option {option.Id}");
                    }
                }
            }
        }
        
        protected override object ConvertOutputValue(SelectorItem value)
        {
            return value.Value;
        }

        public ReactiveCollection<SelectorItem> Items
        {
            get;
        } = new ReactiveCollection<SelectorItem>();
    }
}