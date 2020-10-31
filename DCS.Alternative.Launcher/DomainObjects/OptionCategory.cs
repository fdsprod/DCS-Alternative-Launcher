namespace DCS.Alternative.Launcher.DomainObjects
{
    public static class OptionCategory
    {
        public static readonly string[] All =
        {
            TerrainReflection,
            TerrainMirror,
            Terrain,
            CameraMirrors,
            Camera,
            Graphics,
            Sound,
            General,
        };

        public const string TerrainReflection = "options.graphics.terrainreflection";
        public const string TerrainMirror = "options.graphics.terrainmirror";
        public const string Terrain = "options.graphics.terrain";
        public const string CameraMirrors = "options.graphics.CameraMirrors";
        public const string Camera = "options.graphics.Camera";
        public const string Graphics = "options.graphics";
        public const string Sound = "options.sound";
        public const string General = "";
    }
}