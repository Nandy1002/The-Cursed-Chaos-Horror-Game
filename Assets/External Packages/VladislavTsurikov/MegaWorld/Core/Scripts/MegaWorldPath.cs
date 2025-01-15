using System;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.MegaWorld.Core.Scripts
{
    [Serializable]
    public class MegaWorldPath 
    {
        public static string MegaWorld = "MegaWorld";
        
        public static string Groups = "Groups"; 
        public static string Resources = "Resources";
        public static string PolarisBrushes = "Polaris Brushes";
        public static string MegaWorldTerrainLayers = "Mega World Terrain Layers";

        public static string pathToResources = CommonPath.CombinePath("Assets", Resources);
        public static string pathToResourcesMegaWorld = CommonPath.CombinePath(pathToResources, MegaWorld);
        public static string pathToGroup = CommonPath.CombinePath(pathToResourcesMegaWorld, Groups);
        public static string terrainLayerStoragePath = CommonPath.CombinePath("Assets", MegaWorldTerrainLayers);
    }
}