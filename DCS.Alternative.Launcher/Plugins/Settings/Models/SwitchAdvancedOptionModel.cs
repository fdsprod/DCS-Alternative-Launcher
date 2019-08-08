using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Plugins.Settings.Models
{
    public class SwitchAdvancedOptionModel : SingleValueAdvancedOptionModel<bool>
    {
        private readonly bool _isNumericOutput;

        public SwitchAdvancedOptionModel(AdvancedOption option, bool isNumericOutput)
            : base(option.Id, option.DisplayName, option.Description, option.Params)
        {
            Value.Value = isNumericOutput ? (long)option.Value == 1 : (bool)option.Value;

            _isNumericOutput = isNumericOutput;
        }

        protected override object ConvertOutputValue(bool value)
        {
            if (_isNumericOutput)
            {
                return value ? 1 : 0;
            }

            return base.ConvertOutputValue(value);
        }
    }
}