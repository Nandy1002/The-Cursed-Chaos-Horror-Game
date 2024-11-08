using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.ScatterSettingsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.ResourcesController;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using VladislavTsurikov.Utility;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility.Spawn
{
    public static class SpawnGroup 
    {
        public static void SpawnGameObject(Group group, AreaVariables areaVariables, bool registerUndo = true)
        {
            FilterComponent filterComponent = (FilterComponent)group.GetSettings(typeof(FilterComponent));
            
            if(filterComponent.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateMaskTexture(filterComponent.MaskFilterComponent, areaVariables);
            }

            ScatterComponent scatterComponent = (ScatterComponent)group.GetSettings(typeof(ScatterComponent));
            LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;
            
            scatterComponent.Stack.Samples(areaVariables, sample =>
            {
                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, areaVariables.RayHit.Point.y, sample.y)), layerSettings.GetCurrentPaintLayers(group.PrototypeType));
                if(rayHit != null)
                {
                    PrototypeGameObject proto = (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.PrototypeList);

                    if(proto == null || proto.Active == false)
                    {
                        return;
                    }

                    float fitness = GetFitness.Get(group, areaVariables.Bounds, rayHit);

                    float maskFitness = GrayscaleFromTexture.GetFromWorldPosition(areaVariables.Bounds, rayHit.Point, areaVariables.Mask);

                    fitness *= maskFitness;

                    if(fitness != 0)
                    {
                        if (Random.Range(0f, 1f) < (1 - fitness))
                        {
                            return;
                        }

                        SpawnPrototype.SpawnGameObject(group, proto, rayHit, fitness, registerUndo);
                    }
                }
            });
        }

        public static void SpawnInstantItem(Group group, AreaVariables areaVariables)
        {            
#if INSTANT_RENDERER
            FilterComponent filterComponent = (FilterComponent)group.GetSettings(typeof(FilterComponent));

            if(filterComponent.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateMaskTexture(filterComponent.MaskFilterComponent, areaVariables);
            }

            ScatterComponent scatterComponent = (ScatterComponent)group.GetSettings(typeof(ScatterComponent));
            LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;
            
            scatterComponent.Stack.Samples(areaVariables, sample =>
            {
                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, areaVariables.RayHit.Point.y, sample.y)), layerSettings.GetCurrentPaintLayers(group.PrototypeType));
                if(rayHit != null)
                {
                    PrototypeLargeObject proto = (PrototypeLargeObject)GetRandomPrototype.GetMaxSuccessProto(group.PrototypeList);

                    if(proto == null || proto.Active == false)
                    {
                        return;
                    }

                    float fitness = GetFitness.Get(group, areaVariables.Bounds, rayHit);

                    float maskFitness = GrayscaleFromTexture.GetFromWorldPosition(areaVariables.Bounds, rayHit.Point, areaVariables.Mask);

                    fitness *= maskFitness;

                    if(fitness != 0)
                    {
                        if (Random.Range(0f, 1f) < (1 - fitness))
                        {
                            return;
                        }

                        SpawnPrototype.SpawnInstantProto(proto, rayHit, fitness);
                    }
                }
            });
#endif
        }

        public static void SpawnTerrainDetails(Group group, List<Prototype> protoTerrainDetailList, AreaVariables areaVariables)
        {
            if(TerrainResourcesController.IsSyncError(group, Terrain.activeTerrain))
            {
                return;
            }

            UpdateFilterMask.UpdateFilterMaskTextureForAllTerrainDetail(protoTerrainDetailList, areaVariables);            

            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                Bounds terrainBounds = new Bounds(terrain.terrainData.bounds.center + terrain.transform.position, terrain.terrainData.bounds.size);

                if(terrainBounds.Intersects(areaVariables.Bounds))
                {
                    if(terrain.terrainData.detailPrototypes.Length == 0)
                    {
                        Debug.LogWarning("Add Terrain Details");
                        return;
                    }
        
                    foreach (PrototypeTerrainDetail proto in protoTerrainDetailList)
                    {
                        if(proto.Active == false)
                        {
                            continue;
                        }
                        
                        SpawnPrototype.SpawnTerrainDetails(proto, areaVariables, terrain);
                    }
                }
            }
        }

        public static void SpawnTerrainTexture(Group group, List<Prototype> prototypeTerrainTextures, AreaVariables areaVariables, float textureTargetStrength)
        {
            if(TerrainResourcesController.IsSyncError(group, Terrain.activeTerrain))
            {
                return;
            }

            if(areaVariables.TerrainUnderCursor == null)
            {
                return;
            }

            TerrainPainterRenderHelper terrainPainterRenderHelper = new TerrainPainterRenderHelper(areaVariables);

            foreach (PrototypeTerrainTexture proto in prototypeTerrainTextures)
            {
                if(proto.Active)
                {
                    SpawnPrototype.SpawnTexture(proto, terrainPainterRenderHelper, textureTargetStrength);
                }
            }
        }
    }
}