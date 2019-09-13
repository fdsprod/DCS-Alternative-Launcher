using System;
using System.IO;
using System.Reflection;
using System.Text;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using NLua;

namespace DCS.Alternative.Launcher.Lua
{
    public abstract class LuaContextBase : IDisposable
    {
        private NLua.Lua _lua = new NLua.Lua();

        protected LuaContextBase()
        {
            _lua.State.Encoding = Encoding.UTF8;

            RegisterFunction("print", this, typeof(LuaContextBase).GetMethod("Print", BindingFlags.Instance | BindingFlags.NonPublic));

            DoFile(Path.Combine("Resources", "Lua", "Serializer.lua"));
            DoFile(Path.Combine("Resources", "Lua", "IO.lua"));
        }

        public void Dispose()
        {
            _lua?.Dispose();
            _lua = null;
        }

        protected LuaFunction RegisterFunction(string path, MethodBase function)
        {
            return RegisterFunction(path, null, function);
        }

        protected LuaFunction RegisterFunction(string path, object target, MethodBase function)
        {
            return _lua.RegisterFunction(path, target, function);
        }

        protected object DoFile(string file)
        {
            return _lua.DoFile(file.Replace("\\", "\\\\"));
        }

        protected object DoString(string lua)
        {
            return _lua.DoString(lua);
        }

        private void Print(object text)
        {
            if (text == null)
            {
                return;
            }

            Tracer.Info(text?.ToString());
        }
    }
}