#if UNITY_2021_2_OR_NEWER
using UnityEngine.TerrainTools;
#else
using UnityEngine.Experimental.TerrainAPI;
#endif
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.ColliderSystem.Scripts.UnityIntegration;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using VladislavTsurikov.Scripts;
using VladislavTsurikov.Utility;
#if INSTANT_RENDERER
using VladislavTsurikov.InstantRenderer.LargeObjectRenderer.Scripts.API;
#endif
#if UNITY_EDITOR
using VladislavTsurikov.Undo.ScriptsEditor.UndoActions;
#endif

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility.Spawn
{
    public static class SpawnPrototype 
    {
        public static void SpawnInstantProto(PrototypeLargeObject proto, RayHit rayHit, float fitness)
        {
#if INSTANT_RENDERER
            OverlapCheckComponent overlapCheckComponent = (OverlapCheckComponent)proto.GetSettings(typeof(OverlapCheckComponent));

            InstanceData instanceData = new InstanceData(rayHit.Point, proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            TransformStackComponent transformStackComponent = (TransformStackComponent)proto.GetSettings(typeof(TransformStackComponent));
            transformStackComponent.Stack.SetInstanceData(ref instanceData, fitness, rayHit.Normal);

            if(OverlapCheckComponent.RunOverlapCheck(proto.GetType(), overlapCheckComponent, proto.Extents, instanceData))
            {
                LargeObjectRendererAPI.AddInstance(proto.RendererPrototype, instanceData.Position, instanceData.Scale, instanceData.Rotation);
            }
#endif
        }

        public static void SpawnGameObject(Group group, PrototypeGameObject proto, RayHit rayHit, float fitness, bool registerUndo = true)
        {
            OverlapCheckComponent overlapCheckComponent = (OverlapCheckComponent)proto.GetSettings(typeof(OverlapCheckComponent));

            InstanceData instanceData = new InstanceData(rayHit.Point, proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            TransformStackComponent transformStackComponent = (TransformStackComponent)proto.GetSettings(typeof(TransformStackComponent));
            transformStackComponent.Stack.SetInstanceData(ref instanceData, fitness, rayHit.Normal);

            if(OverlapCheckComponent.RunOverlapCheck(proto.GetType(), overlapCheckComponent, proto.Extents, instanceData))
            {
                PlacedObject objectInfo = PlacedObjectUtility.PlaceObject(proto.Prefab, instanceData.Position, instanceData.Scale, instanceData.Rotation);
                PlacedObjectUtility.ParentGameObject(group, objectInfo);

#if UNITY_EDITOR
                GameObjectCollider.RegisterGameObjectToCurrentScene(objectInfo.GameObject);  
                
                if(registerUndo)
                {
                    Undo.ScriptsEditor.Undo.RegisterUndoAfterMouseUp(new CreatedGameObject(objectInfo.GameObject));
                }
#endif
                objectInfo.GameObject.transform.hasChanged = false;
                
            }
        }

        public static void SpawnTerrainDetails(PrototypeTerrainDetail proto, AreaVariables areaVariables, Terrain terrain)
        {
            var terrainData = terrain.terrainData;
            var spawnSize = new Vector2Int(
                CommonUtility.WorldToDetail(areaVariables.Size * areaVariables.SizeMultiplier, terrainData.size.x, terrainData),
                CommonUtility.WorldToDetail(areaVariables.Size * areaVariables.SizeMultiplier, terrainData.size.z, terrainData));
            
            var halfBrushSize = spawnSize / 2;
            
            var center = new Vector2Int(
                CommonUtility.WorldToDetail(areaVariables.RayHit.Point.x - terrain.transform.position.x, terrain.terrainData),
                CommonUtility.WorldToDetail(areaVariables.RayHit.Point.z - terrain.transform.position.z, terrain.terrainData.size.z, terrain.terrainData));
            
            var position = center - halfBrushSize;
            var startPosition = Vector2Int.Max(position, Vector2Int.zero);
                
            Vector2Int offset = startPosition - position;

            float fitness = 1;
            float detailmapResolution = terrain.terrainData.detailResolution;

            int[,] localData = terrain.terrainData.GetDetailLayer(
                startPosition.x, startPosition.y,
                Mathf.Max(0, Mathf.Min(position.x + spawnSize.x, (int)detailmapResolution) - startPosition.x),
                Mathf.Max(0, Mathf.Min(position.y + spawnSize.y, (int)detailmapResolution) - startPosition.y), proto.TerrainProtoId);
        
            float widthY = localData.GetLength(1);
            float heightX = localData.GetLength(0);

            MaskFilterComponent maskFilterComponent = (MaskFilterComponent)proto.GetSettings(typeof(MaskFilterComponent));
            SpawnDetailComponent spawnDetailComponent = (SpawnDetailComponent)proto.GetSettings(typeof(SpawnDetailComponent));
                   
            for (int y = 0; y < widthY; y++)
            {
                for (int x = 0; x < heightX; x++)
                {
                    var current = new Vector2Int(y, x);
        
                    Vector2 normal = current + startPosition; 
                    normal /= detailmapResolution;
        
                    Vector2 worldPostion = CommonUtility.GetTerrainWorldPositionFromRange(normal, terrain);

                    if (maskFilterComponent.Stack.ComponentList.Count > 0)
			        {
                        fitness = GrayscaleFromTexture.GetFromWorldPosition(areaVariables.Bounds, new Vector3(worldPostion.x, 0, worldPostion.y), maskFilterComponent.FilterMaskTexture2D);
                    }
                    
                    float maskFitness = BrushJitterSettings.GetAlpha(areaVariables, current + offset, spawnSize);
        
                    int targetStrength;
        
                    if (spawnDetailComponent.UseRandomOpacity) 
                    {
                        float randomFitness = fitness;
                        randomFitness *= Random.value;
        
                        targetStrength = Mathf.RoundToInt(Mathf.Lerp(0, spawnDetailComponent.Density, randomFitness));
                    }
                    else
                    {
                        targetStrength = Mathf.RoundToInt(Mathf.Lerp(0, spawnDetailComponent.Density, fitness));
                    }
        
                    targetStrength = Mathf.RoundToInt(Mathf.Lerp(localData[x, y], targetStrength, maskFitness));

                    if (Random.Range(0f, 1f) < (1 - fitness) || Random.Range(0f, 1f) < spawnDetailComponent.FailureRate / 100)
                    {
                        targetStrength = 0;
                    }

                    if (Random.Range(0f, 1f) < (1 - maskFitness))
                    {
                        targetStrength = localData[x, y];
                    }
        
                    localData[x, y] = targetStrength;
                }
            }
        
            terrain.terrainData.SetDetailLayer(startPosition.x, startPosition.y, proto.TerrainProtoId, localData);
        }

        public static void SpawnTexture(PrototypeTerrainTexture proto, TerrainPainterRenderHelper terrainPainterRenderHelper, float textureTargetStrength)
        {
            MaskFilterComponent maskFilterComponent = (MaskFilterComponent)proto.GetSettings(typeof(MaskFilterComponent));

            PaintContext paintContext = terrainPainterRenderHelper.AcquireTexture(proto.TerrainLayer);

			if (paintContext != null)
			{
                FilterMaskOperation.UpdateFilterContext(ref maskFilterComponent.FilterContext, maskFilterComponent.Stack, terrainPainterRenderHelper.AreaVariables);

                RenderTexture filterMaskRT = maskFilterComponent.FilterContext.GetFilterMaskRT();

				Material mat = MaskFilterUtility.GetPaintMaterial();

                // apply brush
                float targetAlpha = textureTargetStrength;
                float s = 1;
				Vector4 brushParams = new Vector4(s, targetAlpha, 0.0f, 0.0f);
				mat.SetTexture("_BrushTex", terrainPainterRenderHelper.AreaVariables.Mask);
				mat.SetVector("_BrushParams", brushParams);
				mat.SetTexture("_FilterTex", filterMaskRT);
                mat.SetTexture("_SourceAlphamapRenderTextureTex", paintContext.sourceRenderTexture);

                terrainPainterRenderHelper.SetupTerrainToolMaterialProperties(paintContext, mat);

                terrainPainterRenderHelper.RenderBrush(paintContext, mat, 0);

                TerrainPaintUtility.EndPaintTexture(paintContext, "Terrain Paint - Texture");

                if(maskFilterComponent.FilterContext != null)
                {
                    maskFilterComponent.FilterContext.DisposeUnmanagedMemory();
                }

                TerrainPaintUtility.ReleaseContextResources(paintContext);
			}
        }
    }
}