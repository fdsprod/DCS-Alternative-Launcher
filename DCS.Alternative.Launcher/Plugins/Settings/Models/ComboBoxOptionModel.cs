using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
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
            var value = option.Value;

            if (!Params.TryGetValue("Items", out var items))
            {
                return;
            }

            Items.Clear();

            var array = (JArray)items;

            foreach (var item in array)
            {
                var converter = TypeDescriptor.GetConverter(value.GetType());

                Items.Add(new SelectorItem
                {
                    DisplayName = item["DisplayName"].ToString(),
                    Value = converter.ConvertFromString(item["Value"].ToString())
                });
            }

            UpdateValue(value);
        }
        
        private void UpdateValue(object value)
        {
            Value.Value = Items.FirstOrDefault(i => i.Value.Equals(value));

            if (Value.Value != null)
            {
                return;
            }

            Value.Value = Items.FirstOrDefault();
            Tracer.Warn($"Unable to select default value {value} for option {this.Id}");
        }


        protected override object ConvertOutputValue(SelectorItem value)
        {
            return value.Value;
        }

        public ReactiveCollection<SelectorItem> Items
        {
            get;
        } = new ReactiveCollection<SelectorItem>();

        public override void ResetValue(object value)
        {
            UpdateValue(value);
        }
    }
}