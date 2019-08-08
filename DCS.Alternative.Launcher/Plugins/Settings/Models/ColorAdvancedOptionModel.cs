using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using DCS.Alternative.Launcher.DomainObjects;
using Newtonsoft.Json.Linq;

namespace DCS.Alternative.Launcher.Plugins.Settings.Models
{
    public class ColorAdvancedOptionModel : SingleValueAdvancedOptionModel<Color>
    {
        public ColorAdvancedOptionModel(AdvancedOption option)
            : base(option.Id, option.DisplayName, option.Description, option.Params)
        {
            var enumerable = (IEnumerable)option.Value;
            var values = 
                (option.Value is JArray 
                    ? enumerable.OfType<JValue>().Select(j=>j.Value) 
                    : enumerable)
                .Cast<long>()
                .ToArray();

            Value.Value = Color.FromRgb((byte)values[0], (byte)values[1], (byte)values[2]);
        }

        protected override object ConvertOutputValue(Color value)
        {
            return new int[] { (int)value.R, (int)value.G, (int)value.B };
        }
    }
}