using System;
using DCS.Alternative.Launcher.DomainObjects;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Models
{
    public class SliderOptionModel : SingleValueOptionModel<double>
    {
        public SliderOptionModel(Option option)
            : base(option.Id, option.DisplayName, option.Description, option.Params)
        {
            Value.Value = Convert.ToDouble(option.Value);

            MaxValue.Value = option.MinMax[0].Max;
            MinValue.Value = option.MinMax[0].Min;
        }

        public ReactiveProperty<double> MinValue
        {
            get;
        } = new ReactiveProperty<double>();

        public ReactiveProperty<double> MaxValue
        {
            get;
        } = new ReactiveProperty<double>();

        protected override object ConvertOutputValue(double value)
        {
            var decimalCount = 0;

            if (Params.TryGetValue("TickFrequency", out var tickFreq))
            {
                var tickFreqStr = tickFreq.ToString();
                var index = tickFreqStr.LastIndexOf(".", StringComparison.Ordinal);

                decimalCount = index == -1 ? 0 : tickFreqStr.Length - (index + 1);
            }

            return Math.Round(value, decimalCount, MidpointRounding.AwayFromZero);
        }
    }
}