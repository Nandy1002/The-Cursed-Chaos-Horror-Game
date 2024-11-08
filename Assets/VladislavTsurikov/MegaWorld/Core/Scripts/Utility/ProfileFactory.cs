using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.Utility
{
    public static class ProfileFactory
    {        
        public static Group CreateGroup(Type prototypeType)
        {
#if UNITY_EDITOR
            Directory.CreateDirectory(MegaWorldPath.pathToGroup);

            var path = string.Empty;

            path += "Group.asset";

            path = CommonPath.CombinePath(MegaWorldPath.pathToGroup, path);
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            var asset = ScriptableObject.CreateInstance<Group>();
            asset.Init(prototypeType);
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            CreateMegaWorldSettings.CreateGroupSettings(asset);
            return asset;

#else 
            return null;
#endif
        }

        public static TerrainLayer SaveTerrainLayerAsAsset(string textureName, TerrainLayer terrainLayer)
        {
#if UNITY_EDITOR
            Directory.CreateDirectory(MegaWorldPath.terrainLayerStoragePath);

            string path = MegaWorldPath.terrainLayerStoragePath + "/" + textureName + ".asset";

            path = AssetDatabase.GenerateUniqueAssetPath(path);

            AssetDatabase.CreateAsset(terrainLayer, path);
            AssetDatabase.SaveAssets();

            return AssetDatabase.LoadAssetAtPath<TerrainLayer>(path);

#else
            return null;
#endif
        }
    }
}