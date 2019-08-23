using System;
using System.Collections;
using System.Linq;
using DCS.Alternative.Launcher.DomainObjects;
using Newtonsoft.Json.Linq;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Models
{
    public class TripleRangeSliderOptionModel : MultiValueOptionModel<double>
    {
        public TripleRangeSliderOptionModel(Option option)
            : base(option.Id, option.DisplayName, option.Description, option.Params)
        {
            UpdateValue(option.Value);

            MaxValue.Value = option.MinMax[0].Max;
            MinValue.Value = option.MinMax[0].Min;

            Value1.Subscribe(_ => SignalValueChanged());
            Value2.Subscribe(_ => SignalValueChanged());
            Value3.Subscribe(_ => SignalValueChanged());
        }

        public ReactiveProperty<double> Value1
        {
            get;
        } = new ReactiveProperty<double>();

        public ReactiveProperty<double> Value2
        {
            get;
        } = new ReactiveProperty<double>();

        public ReactiveProperty<double> Value3
        {
            get;
        } = new ReactiveProperty<double>();

        public ReactiveProperty<double> MinValue
        {
            get;
        } = new ReactiveProperty<double>();

        public ReactiveProperty<double> MaxValue
        {
            get;
        } = new ReactiveProperty<double>();

        protected override object ConvertOutputValue()
        {
            var decimalCount = 0;

            if (Params.TryGetValue("TickFrequency", out var value))
            {
                var str = value.ToString();
                var index = str.LastIndexOf(".", StringComparison.Ordinal);

                decimalCount = index == -1 ? 0 : str.Length - (index + 1);
            }

            return new[]
            {
                Math.Round(Value1.Value, decimalCount, MidpointRounding.AwayFromZero),
                Math.Round(Value2.Value, decimalCount, MidpointRounding.AwayFromZero),
                Math.Round(Value3.Value, decimalCount, MidpointRounding.AwayFromZero)
            };
        }

        public override void ResetValue(object value)
        {
            UpdateValue(value);
        }

        private void UpdateValue(object value)
        {
            var enumerable = (IEnumerable)value;
            var values =
                (value is JArray
                    ? enumerable.OfType<JValue>().Select(j => j.Value)
                    : enumerable)
                .Cast<object>()
                .Select(Convert.ToDouble) // Fucking .Net doesn't like Cast<double>() in this instance
                .ToArray();

            Value1.Value = values[0];
            Value2.Value = values[1];
            Value3.Value = values[2];
        }
    }
}