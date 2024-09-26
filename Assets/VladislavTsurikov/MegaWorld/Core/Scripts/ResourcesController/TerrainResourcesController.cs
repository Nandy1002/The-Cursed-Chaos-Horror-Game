using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using GameObjectUtility = VladislavTsurikov.Utility.GameObjectUtility;
using PrefabType = VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes.PrefabType;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.ResourcesController
{
    public static class TerrainResourcesController
    {
        public enum TerrainResourcesSyncError
        {
            None,
            NotAllProtoAvailable,
            MissingPrototypes,
        }

        public static TerrainResourcesSyncError SyncError = TerrainResourcesSyncError.None;

        public static void AddMissingPrototypesToTerrains(Group group)
        {
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                AddMissingPrototypesToTerrain(terrain, group);
            }
        }

        public static void RemoveAllPrototypesFromTerrains(Group group)
        {
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                RemoveAllPrototypesFromTerrains(terrain, group);
            }
        }

        public static void AddMissingPrototypesToTerrain(Terrain terrain, Group group)
        {
            if (terrain == null)
            {
                Debug.LogWarning("Can not add resources to the terrain as no terrain has been supplied.");
                return;
            }

            if (group.PrototypeType == typeof(PrototypeTerrainDetail))
            {
                AddTerrainDetailToTerrain(terrain, group.PrototypeList);
            }
            else if(group.PrototypeType == typeof(PrototypeTerrainTexture))
            {
                AddTerrainTexturesToTerrain(terrain, group.PrototypeList);
            }

            terrain.Flush();

            SyncTerrainID(terrain, group);
        }

        public static void RemoveAllPrototypesFromTerrains(Terrain terrain, Group group)
        {
            if (terrain == null)
            {
                Debug.LogWarning("Can not add resources to the terrain as no terrain has been supplied.");
                return;
            }

            if (group.PrototypeType == typeof(PrototypeTerrainDetail))
            {
                List<DetailPrototype> terrainDetails = new List<DetailPrototype>();
                    
                terrain.terrainData.detailPrototypes = terrainDetails.ToArray();
            }
            else if(group.PrototypeType == typeof(PrototypeTerrainTexture))
            {
                List<TerrainLayer> terrainLayers = new List<TerrainLayer>();

                terrain.terrainData.terrainLayers = terrainLayers.ToArray();
            }

            terrain.Flush();

            SyncTerrainID(terrain, group);
        }

        public static void AddTerrainDetailToTerrain(Terrain terrain, List<Prototype> protoTerrainDetailList)
        {
            bool found = false;

            DetailPrototype newDetail;
            List<DetailPrototype> terrainDetails = new List<DetailPrototype>(terrain.terrainData.detailPrototypes);
            foreach (PrototypeTerrainDetail proto in protoTerrainDetailList)
            {
                found = false;
                foreach (DetailPrototype dp in terrainDetails)
                {
                    if (proto.PrefabType == PrefabType.Texture)
                    {
                        if (CommonUtility.IsSameTexture(dp.prototypeTexture, proto.DetailTexture, false))
                        {
                            found = true;
                        }
                        if (GameObjectUtility.IsSameGameObject(dp.prototype, proto.Prefab, false))
                        {
                            found = true;
                        }
                    }
                }

                if (!found)
                {
                    newDetail = new DetailPrototype();

                    if (proto.PrefabType == PrefabType.Texture)
                    {
                        newDetail.renderMode = DetailRenderMode.GrassBillboard;
                        newDetail.usePrototypeMesh = false;
                        newDetail.prototypeTexture = proto.DetailTexture;
                    }
                    else
                    {
                        newDetail.renderMode = DetailRenderMode.Grass;
                        newDetail.usePrototypeMesh = true;
                        newDetail.prototype = proto.Prefab;
                    }

                    terrainDetails.Add(newDetail);
                }
            }

            terrain.terrainData.detailPrototypes = terrainDetails.ToArray();
        }

        public static void AddTerrainTexturesToTerrain(Terrain terrain, List<Prototype> protoTerrainTextureList)
        {
            bool found = false;

            TerrainLayer[] currentTerrainLayers = terrain.terrainData.terrainLayers;

            List<TerrainLayer> terrainLayers = new List<TerrainLayer>(terrain.terrainData.terrainLayers);

            foreach (PrototypeTerrainTexture splat in protoTerrainTextureList)
            {
                found = false;

                if(splat.TerrainLayer == null)
                {
                    foreach (TerrainLayer layer in currentTerrainLayers)
                    {
                        if (CommonUtility.IsSameTexture(layer.diffuseTexture, splat.TerrainTextureSettings.DiffuseTexture, false))
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        terrainLayers.Add(splat.TerrainTextureSettings.Convert());

                        RemoveTerrainLayerAssetFiles(splat.TerrainTextureSettings.DiffuseTexture.name);

                        terrainLayers[terrainLayers.Count - 1] = ProfileFactory.SaveTerrainLayerAsAsset(splat.TerrainTextureSettings.DiffuseTexture.name, terrainLayers.Last());

                        splat.TerrainLayer = terrainLayers[terrainLayers.Count - 1];
                    }
                }
                else
                {
                    foreach (TerrainLayer layer in currentTerrainLayers)
                    {
                        if (CommonUtility.IsSameTexture(layer.diffuseTexture, splat.TerrainTextureSettings.DiffuseTexture, false))
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        terrainLayers.Add(splat.TerrainLayer);
                    }
                }
            }

            terrain.terrainData.terrainLayers = terrainLayers.ToArray();
        }

        private static void RemoveTerrainLayerAssetFiles(string terrainName)
        {
            string megaWorldDirectory = "";
            string terrainLayerDirectory = megaWorldDirectory + "Profiles/TerrainLayers";
            DirectoryInfo info = new DirectoryInfo(terrainLayerDirectory);

            if (info.Exists)
            {
                FileInfo[] fileInfo = info.GetFiles(terrainName + "*.asset");

                for (int i = 0; i < fileInfo.Length; i++)
                {
                    File.Delete(fileInfo[i].FullName);
                }
            }
        }

        public static void SyncTerrainID(Terrain terrain, Group group)
        {
            if (group.PrototypeType == typeof(PrototypeTerrainDetail))
            {
                List<DetailPrototype> terrainDetails = new List<DetailPrototype>(terrain.terrainData.detailPrototypes);

                foreach (var prototype in group.PrototypeList)
                {
                    var proto = (PrototypeTerrainDetail)prototype;

                    for (int Id = 0; Id < terrainDetails.Count; Id++)
                    {
                        if (CommonUtility.IsSameTexture(terrainDetails[Id].prototypeTexture, proto.DetailTexture, false))
                        {
                            proto.TerrainProtoId = Id;
                        }
                        if (GameObjectUtility.IsSameGameObject(terrainDetails[Id].prototype, proto.Prefab, false))
                        {
                            proto.TerrainProtoId = Id;
                        }
                    }
                }
            }
        }

        public static void UpdateOnlyTerrainTexture(Terrain terrain, Group group)
        {
            if (group.PrototypeType != typeof(PrototypeTerrainTexture))
                return;
            
            List<PrototypeTerrainTexture> protoTerrainTextureRemoveList = new List<PrototypeTerrainTexture>();

            List<TerrainLayer> terrainLayers = new List<TerrainLayer>(terrain.terrainData.terrainLayers);

            foreach (PrototypeTerrainTexture texture in group.PrototypeList)
            {
                bool find = false;

                for (int Id = 0; Id < terrainLayers.Count; Id++)
                {
                    if (texture.TerrainLayer != null)
                    {
                        if (terrainLayers[Id].GetInstanceID() == texture.TerrainLayer.GetInstanceID())
                        {
                            find = true;
                        }
                    }
                    else
                    {
                        if (CommonUtility.IsSameTexture(terrainLayers[Id].diffuseTexture, texture.TerrainTextureSettings.DiffuseTexture, true))
                        {
                            find = true;
                        }
                    }
                    
                }

                if(find == false)
                {
                    protoTerrainTextureRemoveList.Add(texture);
                }
            }

            foreach (PrototypeTerrainTexture proto in protoTerrainTextureRemoveList)
            {
                group.PrototypeList.Remove(proto);
            }
            
            for (int Id = 0; Id < terrainLayers.Count; Id++)
            {
                bool find = false;

                foreach (PrototypeTerrainTexture texture in group.PrototypeList)
                {
                    if (texture.TerrainLayer != null)
                    {
                        if (terrainLayers[Id].GetInstanceID() == texture.TerrainLayer.GetInstanceID())
                        {
                            find = true;
                        }
                    }
                    else
                    {
                        if (CommonUtility.IsSameTexture(terrainLayers[Id].diffuseTexture, texture.TerrainTextureSettings.DiffuseTexture, true))
                        {
                            find = true;
                        }
                    }
                }

                if(find == false)
                {
                    group.AddMissingPrototype(terrainLayers[Id]);
                }
            }
            
            SyncTerrainID(terrain, group);
        }

        public static void UpdateOnlyTerrainDetail(Terrain terrain, Group group)
        {
            if(group.PrototypeType != typeof(PrototypeTerrainDetail))
                return;
            
            List<PrototypeTerrainDetail> protoTerrainDetailRemoveList = new List<PrototypeTerrainDetail>();

            List<DetailPrototype> terrainDetails = new List<DetailPrototype>(terrain.terrainData.detailPrototypes);

            foreach (PrototypeTerrainDetail proto in group.PrototypeList)
            {
                bool find = false;

                for (int Id = 0; Id < terrainDetails.Count; Id++)
                {
                    if (CommonUtility.IsSameTexture(terrainDetails[Id].prototypeTexture, proto.DetailTexture, false))
                    {
                        find = true;
                    }
                    if (GameObjectUtility.IsSameGameObject(terrainDetails[Id].prototype, proto.Prefab, false))
                    {
                        find = true;
                    }
                }

                if(find == false)
                {
                    protoTerrainDetailRemoveList.Add(proto);
                }
            }

            foreach (PrototypeTerrainDetail proto in protoTerrainDetailRemoveList)
            {
                group.PrototypeList.Remove(proto);
            }

            DetailPrototype unityProto;
            PrototypeTerrainDetail localProto;

            for (int Id = 0; Id < terrainDetails.Count; Id++)
            {
                bool find = false;

                foreach (PrototypeTerrainDetail proto in group.PrototypeList)
                {
                    if (CommonUtility.IsSameTexture(terrainDetails[Id].prototypeTexture, proto.DetailTexture, false))
                    {
                        find = true;
                    }
                    if (GameObjectUtility.IsSameGameObject(terrainDetails[Id].prototype, proto.Prefab, false))
                    {
                        find = true;
                    }
                }

                if(find == false)
                {
                    unityProto = terrain.terrainData.detailPrototypes[Id];
                    
                    if (unityProto.prototype != null)
                    {
                        localProto = (PrototypeTerrainDetail)group.AddMissingPrototype(unityProto.prototype);
                        localProto.PrefabType = PrefabType.Mesh;
                    }
                    else
                    {
                        localProto = (PrototypeTerrainDetail)group.AddMissingPrototype(unityProto.prototypeTexture);
                        localProto.PrefabType = PrefabType.Texture;
                    }
                }
            }

            SyncTerrainID(terrain, group);
        }

        public static void UpdatePrototypesFromTerrain(Terrain terrain, Group group)
        {
            if (terrain == null)
            {
                Debug.LogWarning("Missing active terrain.");
                return;
            }
            
            if (group.PrototypeType == typeof(PrototypeTerrainTexture))
            {
                UpdateOnlyTerrainTexture(terrain, group);
            }
            if (group.PrototypeType == typeof(PrototypeTerrainDetail))
            {
                UpdateOnlyTerrainDetail(terrain, group);
            }

            SyncTerrainID(terrain, group);
        }

        public static void DetectSyncError(Group group, Terrain terrain)
        {
            if(group.PrototypeList.Count == 0)
            {
                SyncError = TerrainResourcesSyncError.MissingPrototypes;
                return;
            }

            if (terrain == null)
            {
                return;
            }

            if (group.PrototypeType == typeof(PrototypeTerrainDetail))
            {
                foreach (PrototypeTerrainDetail proto in group.PrototypeList)
                {
                    bool find = false;

                    foreach (DetailPrototype unityProto in terrain.terrainData.detailPrototypes)
                    {
                        if (CommonUtility.IsSameTexture(unityProto.prototypeTexture, proto.DetailTexture, false))
                        {
                            find = true;
                            break;
                        }
                        if (GameObjectUtility.IsSameGameObject(unityProto.prototype, proto.Prefab, false))
                        {
                            find = true;
                            break;
                        }
                    }

                    if(!find)
                    {
                        SyncError = TerrainResourcesSyncError.NotAllProtoAvailable;
                        return;
                    }
                }
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainTexture))
            {
                foreach (PrototypeTerrainTexture proto in group.PrototypeList)
                {
                    bool find = false;

                    foreach (TerrainLayer terrainLayer in terrain.terrainData.terrainLayers)
                    {
                        if (CommonUtility.IsSameTexture(terrainLayer.diffuseTexture, proto.TerrainTextureSettings.DiffuseTexture, false))
                        {
                            find = true;
                            break;
                        }
                    }

                    if(!find)
                    {
                        SyncError = TerrainResourcesSyncError.NotAllProtoAvailable;
                        return;
                    }
                }
            }

            SyncError = TerrainResourcesSyncError.None;
        }

        public static bool IsSyncError(Group group, Terrain terrain)
        {
            DetectSyncError(group, terrain);

            if(TerrainResourcesSyncError.NotAllProtoAvailable == SyncError)
            {
                if (group.PrototypeType == typeof(PrototypeTerrainDetail))
                {
                    Debug.LogWarning("You need all Terrain Details prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");
                    return true;
                }
                if (group.PrototypeType == typeof(PrototypeTerrainTexture))
                {
                    Debug.LogWarning("You need all Terrain Textures prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");
                    return true;
                }
            }

            return false;
        }

        public static void SyncAllTerrains(Group group, Terrain terrain)
        {
            if(group == null || terrain == null)
            {
                return;
            }

            if(Terrain.activeTerrains.Length == 0)
            {
                return;
            }
            
            if (group.PrototypeType == typeof(PrototypeTerrainDetail))
            {
                List<DetailPrototype> terrainDetails = new List<DetailPrototype>(terrain.terrainData.detailPrototypes);

                foreach (Terrain item in Terrain.activeTerrains)
                {
                    item.terrainData.detailPrototypes = terrainDetails.ToArray();
                }
            }
            if (group.PrototypeType == typeof(PrototypeTerrainTexture))
            {
                List<TerrainLayer> terrainTextures = new List<TerrainLayer>(terrain.terrainData.terrainLayers);

                foreach (Terrain item in Terrain.activeTerrains)
                {
                    item.terrainData.terrainLayers = terrainTextures.ToArray();
                }
            }
        }
    }
}