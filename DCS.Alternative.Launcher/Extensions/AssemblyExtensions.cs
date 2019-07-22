using System;
using System.IO;
using System.Reflection;

namespace DCS.Alternative.Launcher.Extensions
{
    public static class AssemblyExtensions
    {
        public static Type[] SafeGetTypes(this Assembly assembly)
        {
            Type[] assemblies;

            try
            {
                assemblies = assembly.GetTypes();
            }
            catch (FileNotFoundException)
            {
                assemblies = new Type[]
                {
                };
            }
            catch (NotSupportedException)
            {
                assemblies = new Type[]
                {
                };
            }
            catch (ReflectionTypeLoadException)
            {
                assemblies = new Type[]
                {
                };
            }

            return assemblies;
        }
    }
}