using UnityEngine;

namespace Utilities
{
    public enum TerrainObjectState
    {
        Alive   = 0,
        OnFire  = 1,
        Burnt   = 2,
    }

    public enum MouseModeType
    {
        Add         = 0,
        Remove      = 1,
        FireWater   = 2
    }

    public static class GameSettings
    {
        public const int GRID_SIZE = 20;
        public const float DURATION_ON_FIRE = 0.1f;
        public const int MAX_RANDOM_FIRE_HITS = 5;

        public static float TerrainWidth => Terrain.activeTerrain.terrainData.size.x;
        public static float TerrainHeight => Terrain.activeTerrain.terrainData.size.z;

        public static int AliveTerrainObjects;
        public static int OnFireTerrainObjects;
        public static int BurntTerrainObjects;
        public static int TotalTerrainObjects;

        public static float WindSpeed;
        public static int WindDirection;
        public static bool AllDirectionsWind = true;
        public static int Humidity;

        public static bool IsPaused;
    }
}