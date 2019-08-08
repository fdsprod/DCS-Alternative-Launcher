using DCS.Alternative.Launcher.DomainObjects;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Models
{
    public class TextAdvancedOptionModel : SingleValueAdvancedOptionModel<string>
    {
        public TextAdvancedOptionModel(AdvancedOption option)
            : base(option.Id, option.DisplayName, option.Description, option.Params)
        {
            Value.Value = (string)option.Value;

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
    }
}