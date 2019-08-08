using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using Newtonsoft.Json.Linq;

namespace DCS.Alternative.Launcher.Plugins.Settings.Models
{
    public static class AdvancedOptionModelFactory
    {
        public static IEnumerable<AdvancedOptionModel> CreateAll(IEnumerable<AdvancedOption> options)
        {
            var models = new List<AdvancedOptionModel>();

            foreach (var option in options)
            {
                var type = option.Value.GetType();

                AdvancedOptionModel model = null;

                if (type == typeof(string))
                {
                    model = new TextAdvancedOptionModel(option);
                }
                else if (type == typeof(long) || type == typeof(double))
                {
                    model = option.MinMax.Count == 0
                        ? (AdvancedOptionModel)new SwitchAdvancedOptionModel(option, true)
                        : new SliderAdvancedOptionModel(option);
                }
                else if (type == typeof(bool))
                {
                    model = new SwitchAdvancedOptionModel(option, false);
                }
                else if ((type == typeof(JArray) || type == typeof(int[])) && option.Id.EndsWith("color", StringComparison.CurrentCultureIgnoreCase))
                {
                    model = new ColorAdvancedOptionModel(option);
                }
                else if (type.GetInterfaces().Contains(typeof(IEnumerable)))
                {
                    var enumerable = (IEnumerable)option.Value;
                    var count = enumerable.OfType<object>().Count();

                    switch (count)
                    {
                        case 2:
                            model = new DoubleRangeSliderAdvancedOptionModel(option);
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
                else
                {
                    Tracer.Warn($"Could not create advanced option model for {option.Id} {option.DisplayName} {type}");
                }

                if (model != null)
                {
                    models.Add(model);
                }
            }

            return models;
        }
    }
}