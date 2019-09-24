using System;
using System.IO;
using DCS.Alternative.Launcher.Analytics;

namespace DCS.Alternative.Launcher.Lua
{
    public class OptionLuaContext : LuaContextBase
    {
        private readonly string _optionsPath;

        public OptionLuaContext(InstallLocation install)
        {
            _optionsPath = Path.Combine(install.SavedGamesPath, "Config", "options.lua");

            DoFile(_optionsPath);
        }

        public void SetValue(string categoryId, string id, object value)
        {
            if (value is string)
            {
                DoString($"options[\"{categoryId}\"][\"{id}\"] = \"{value}\"");
            }
            else
            {
                DoString($"options[\"{categoryId}\"][\"{id}\"] = {value.ToString().ToLower()}");
            }
        }

        public void Save()
        {
            DoString($"serializeToFile(\'{_optionsPath.Replace("\\", "\\\\")}\', \'options\', options)");
        }

        public object GetValue(string categoryId, string id)
        {
            var output = DoString($"return options[\"{categoryId}\"][\"{id}\"]");
            var array = output as object[];

            if ((array?.Length ?? 0) > 0)
            {
                return array[0];
            }

            return output;
        }
    }
}