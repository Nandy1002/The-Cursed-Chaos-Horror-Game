using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.ScatterSettingsSystem;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility;
using VladislavTsurikov.MegaWorld.Core.Scripts.ResourcesController;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
#if UNITY_EDITOR
using VladislavTsurikov.EditorCoroutines.ScriptsEditor;
#endif

namespace VladislavTsurikov.MegaWorld.TerrainSpawner.Scripts.Utility
{
    public static class SpawnGroupCoroutine 
    {
#if UNITY_EDITOR
        public static IEnumerator SpawnGameObject(Group group, AreaVariables areaVariables, bool registerUndo = true)
        {
            ScatterComponent scatterComponent = (ScatterComponent)group.GetSettings(typeof(ScatterComponent));
            LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;
            
            EditorCoroutine editorCoroutine = EditorCoroutines.ScriptsEditor.EditorCoroutines.StartCoroutine(scatterComponent.Stack.SamplesCoroutine(areaVariables, sample =>
            {
                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, areaVariables.RayHit.Point.y, sample.y)), layerSettings.GetCurrentPaintLayers(group.PrototypeType));
                if(rayHit != null)
                {
                    PrototypeGameObject proto = (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.PrototypeList);;

                    if(proto == null || proto.Active == false)
                    {
                        return;
                    }

                    float fitness = GetFitness.Get(group, rayHit);

                    if(fitness != 0)
                    {
                        if (Random.Range(0f, 1f) < (1 - fitness))
                        {
                            return;
                        }

                        CommonScripts.Scripts.Utility.Spawn.SpawnPrototype.SpawnGameObject(group, proto, rayHit, fitness, registerUndo);
                    }
                }
            }), scatterComponent.Stack);
            
            yield return editorCoroutine;
        }
#endif
        
#if UNITY_EDITOR
        public static IEnumerator SpawnInstantItem(Group group, AreaVariables areaVariables)
        {            
#if INSTANT_RENDERER
            ScatterComponent scatterComponent = (ScatterComponent)group.GetSettings(typeof(ScatterComponent));
            LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;
            
            EditorCoroutine editorCoroutine = EditorCoroutines.ScriptsEditor.EditorCoroutines.StartCoroutine(scatterComponent.Stack.SamplesCoroutine(areaVariables, sample =>
            {
                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, areaVariables.RayHit.Point.y, sample.y)), layerSettings.GetCurrentPaintLayers(group.PrototypeType));
                if(rayHit != null)
                {
                    PrototypeLargeObject proto = (PrototypeLargeObject)GetRandomPrototype.GetMaxSuccessProto(group.PrototypeList);

                    if(proto == null || proto.Active == false)
                    {
                        return;
                    }
                    
                    float fitness = GetFitness.Get(group, rayHit);

                    if(fitness != 0)
                    {
                        if (Random.Range(0f, 1f) < (1 - fitness))
                        {
                            return;
                        }

                        CommonScripts.Scripts.Utility.Spawn.SpawnPrototype.SpawnInstantProto(proto, rayHit, fitness);
                    }
                }
            }), scatterComponent.Stack);

            yield return editorCoroutine;
#endif

            yield return null;
        }
#endif

#if UNITY_EDITOR
        public static IEnumerator SpawnTerrainDetails(Group group, List<Prototype> protoTerrainDetailList, AreaVariables areaVariables)
        {

            if (TerrainResourcesController.IsSyncError(group, Terrain.activeTerrain)) yield break;
            
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                Bounds terrainBounds = new Bounds(terrain.terrainData.bounds.center + terrain.transform.position, terrain.terrainData.bounds.size);

                if(terrainBounds.Intersects(areaVariables.Bounds))
                {
                    if(terrain.terrainData.detailPrototypes.Length == 0)
                    {
                        Debug.LogWarning("Add Terrain Details");
                        continue;
                    }
        
                    foreach (PrototypeTerrainDetail proto in protoTerrainDetailList)
                    {
                        if(proto.Active == false)
                        {
                            continue;
                        }
                        
                        SpawnPrototype.SpawnTerrainDetails(group, proto, areaVariables, terrain);
                        
                        yield return null;
                    }
                    
                    yield return null;
                }
            }

        }
#endif
    }
}