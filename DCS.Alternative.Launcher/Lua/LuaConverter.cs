using System.Collections.Generic;
using Newtonsoft.Json;
using NLua;

namespace DCS.Alternative.Launcher.Lua
{
    public static class LuaConverter
    {
        public static T ConvertTo<T>(LuaTable table, bool throwOnNotFound = false)
            where T : new()
        {
            var dictionary = new Dictionary<object, object>();

            PopulateDictionary(dictionary, table);

            var json = JsonConvert.SerializeObject(dictionary);
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = throwOnNotFound ? MissingMemberHandling.Error : MissingMemberHandling.Ignore
            };
            var result = JsonConvert.DeserializeObject<T>(json, settings);

            return result;
        }

        private static void PopulateDictionary(Dictionary<object, object> dictionary, LuaTable table)
        {
            foreach (var key in table.Keys)
            {
                var value = table[key];
                var childTable = value as LuaTable;

                if (childTable != null)
                {
                    var child = new Dictionary<object, object>();
                    PopulateDictionary(child, childTable);
                    dictionary[key] = child;
                }
                else
                {
                    dictionary[key] = value;
                }
            }
        }
    }
}