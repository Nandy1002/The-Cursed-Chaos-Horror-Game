using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.UnityIntegration;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
#if INSTANT_RENDERER
using VladislavTsurikov.InstantRenderer.LargeObjectRenderer.Scripts.API;
#endif

namespace VladislavTsurikov.MegaWorld.Core.Scripts.Utility
{
    public static class Unspawn 
    {
        public static void UnspawnAllProto(List<Group> groupList, bool unspawnSelected)
        {
            foreach (Group group in groupList)
            {
                if(group.PrototypeType == typeof(PrototypeTerrainDetail))
                    UnspawnTerrainDetail(group.PrototypeList, unspawnSelected);
                else if(group.PrototypeType == typeof(PrototypeLargeObject))
                    UnspawnInstantItem(group, unspawnSelected);
                else if(group.PrototypeType == typeof(PrototypeGameObject))
                    UnspawnGameObject(unspawnSelected);
            }
        }

        public static void UnspawnGameObject(bool unspawnSelected)
        {
            GameObject[] allGameObjects = Object.FindObjectsOfType<GameObject>();

            for (int index = 0; index < allGameObjects.Length; index++)
            {
                PrototypeGameObject proto = GetPrototypeUtility.GetPrototype<PrototypeGameObject>(allGameObjects[index]);

                if(proto != null)
                {
                    if(unspawnSelected)
                    {
                        if(proto.Selected == false)
                        {
                            continue;
                        }
                    }

                    Object.DestroyImmediate(allGameObjects[index]);
                }
            }

#if UNITY_EDITOR
            GameObjectCollider.RemoveNullObjectNodesForAllScenes();
#endif
        }

        public static void UnspawnTerrainDetail(List<Prototype> protoTerrainDetailList, bool unspawnSelected)
        {
            foreach (PrototypeTerrainDetail prototype in protoTerrainDetailList)
            {
                if(unspawnSelected)
                {
                    if(prototype.Selected == false)
                    {
                        continue;
                    }
                }

                foreach (var terrain in Terrain.activeTerrains)
                {
                    if(terrain.terrainData.detailResolution == 0)
                    {
                        continue;
                    }

                    if(prototype.TerrainProtoId > terrain.terrainData.detailPrototypes.Length - 1)
                    {
                        Debug.LogWarning("You need all Terrain Details prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");
                    }
                    else
                    {
                        terrain.terrainData.SetDetailLayer(0, 0, prototype.TerrainProtoId, new int[terrain.terrainData.detailWidth, terrain.terrainData.detailWidth]);
                    }
                }
            }
        }

        public static void UnspawnInstantItem(Group group, bool unspawnSelected)
        {
#if INSTANT_RENDERER
            foreach (PrototypeLargeObject proto in group.PrototypeList)
            {
                if(unspawnSelected)
                {
                    if(proto.Selected == false)
                    {
                        continue;
                    }
                }

                LargeObjectRendererAPI.RemoveInstances(proto.GetID());
            }
#endif
        }
    }
}