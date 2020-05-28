using System;
using System.IO;

namespace DCS.Alternative.Launcher
{
    public static class ApplicationPaths
    {
        private static readonly Guid SavedGamesGuid = Guid.Parse("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4");

        public static readonly string StoragePath;
        public static readonly string ProfilesPath;
        public static readonly string WallpaperPath;
        public static readonly string ViewportPath;
        public static readonly string PluginsPath;
        //public static readonly string LuaPath;
        public static readonly string ResourcesPath;
        public static readonly string OptionsPath;
        public static readonly string ApplicationPath;

        static ApplicationPaths()
        {
            var savedGamesPath = NativeMethods.GetKnownFolderPath(SavedGamesGuid);

            ApplicationPath = Directory.GetCurrentDirectory();
            StoragePath = Path.Combine(savedGamesPath, "DCS Alternative Launcher");
            ProfilesPath = Path.Combine(StoragePath, "Profiles");
            WallpaperPath = Path.Combine(StoragePath, "Images\\Wallpaper");
            ViewportPath = Path.Combine(StoragePath, "Viewports");
            PluginsPath = Path.Combine(ApplicationPath, "Plugins");
            //LuaPath = Path.Combine(StoragePath, "Lua");
            ResourcesPath = Path.Combine(StoragePath, "Resources");
            OptionsPath = Path.Combine(StoragePath, "Options");

            EnsureDirectory(StoragePath);
            EnsureDirectory(ProfilesPath);
            EnsureDirectory(WallpaperPath);
            EnsureDirectory(ViewportPath);
            EnsureDirectory(PluginsPath);
            //EnsureDirectory(LuaPath);
            EnsureDirectory(ResourcesPath);
            EnsureDirectory(OptionsPath);
        }

        public static void EnsureDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}