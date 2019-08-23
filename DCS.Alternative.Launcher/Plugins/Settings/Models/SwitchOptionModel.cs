using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Plugins.Settings.Models
{
    public class SwitchOptionModel : SingleValueOptionModel<bool>
    {
        private readonly bool _isNumericOutput;

        public SwitchOptionModel(Option option, bool isNumericOutput)
            : base(option.Id, option.DisplayName, option.Description, option.Params)
        {
            _isNumericOutput = isNumericOutput;

            UpdateValue(option.Value);
        }

        protected override object ConvertOutputValue(bool value)
        {
            if (_isNumericOutput)
            {
                return value ? 1 : 0;
            }

            return base.ConvertOutputValue(value);
        }

        public override void ResetValue(object value)
        {
            UpdateValue(value);
        }

        private void UpdateValue(object value)
        {
            Value.Value = _isNumericOutput ? (long)value == 1 : (bool)value;
        }
    }
}