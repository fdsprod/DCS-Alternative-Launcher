using System;
using System.Collections;
using System.Linq;
using System.Windows.Media;
using DCS.Alternative.Launcher.DomainObjects;
using Newtonsoft.Json.Linq;

namespace DCS.Alternative.Launcher.Plugins.Settings.Models
{
    public class ColorOptionModel : SingleValueOptionModel<Color>
    {
        private bool _isNormalized;

        public ColorOptionModel(Option option)
            : base(option.Id, option.DisplayName, option.Description, option.Params)
        {
            UpdateValue(option.Value);
        }

        private void UpdateValue(object value)
        {
            var enumerable = (IEnumerable) value;
            var values =
                (value is JArray
                    ? enumerable.OfType<JValue>().Select(j => j.Value)
                    : enumerable)
                .Cast<object>()
                .ToArray();

            if (Params.TryGetValue("IsNormalized", out var v) && (bool) v)
            {
                _isNormalized = true;
                var rgb = values.Select(Convert.ToDouble).ToArray();
                Value.Value = Color.FromRgb((byte) (_isNormalized ? rgb[0] * 255 : rgb[0]), (byte) (_isNormalized ? rgb[1] * 255 : rgb[1]), (byte) (_isNormalized ? rgb[2] * 255 : rgb[2]));
            }
            else
            {
                var rgb = values.Select(Convert.ToUInt64).ToArray();
                Value.Value = Color.FromRgb((byte) rgb[0], (byte) rgb[1], (byte) rgb[2]);
            }
        }

        protected override object ConvertOutputValue(Color value)
        {
            if (_isNormalized)
            {
                return new[] {value.R * 1.0, value.G * 1.0, value.B * 1.0};
            }

            return new[] {value.R, value.G, (int) value.B};
        }

        public override void ResetValue(object value)
        {
            UpdateValue(value);
        }
    }
}