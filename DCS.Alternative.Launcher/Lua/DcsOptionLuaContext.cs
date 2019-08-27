using System;
using System.IO;

namespace DCS.Alternative.Launcher.Lua
{
    public class DcsOptionLuaContext : LuaContextBase
    {
        private readonly string _optionsPath;

        public DcsOptionLuaContext(InstallLocation install)
        {
            _optionsPath = Path.Combine(install.SavedGamesPath, "Config", "options.lua");
            ;

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
            DoString($"print(options) {Environment.NewLine} serializeToFile(\'{_optionsPath.Replace("\\", "\\\\")}\', \'options\', options)");
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