using DCS.Alternative.Launcher.DomainObjects;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Models
{
    public class TextOptionModel : SingleValueOptionModel<string>
    {
        public TextOptionModel(Option option)
            : base(option.Id, option.DisplayName, option.Description, option.Params)
        {
            UpdateValue(option.Value);

            MaxValue.Value = option.MinMax[0].Max;
            MinValue.Value = option.MinMax[0].Min;
        }

        public ReactiveProperty<double> MaxValue
        {
            get;
        } = new ReactiveProperty<double>();

        public ReactiveProperty<double> MinValue
        {
            get;
        } = new ReactiveProperty<double>();

        public override void ResetValue(object value)
        {
            UpdateValue(value);
        }

        private void UpdateValue(object value)
        {
            Value.Value = (string)value;
        }
    }
}