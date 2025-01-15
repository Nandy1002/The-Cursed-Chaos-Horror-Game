#if UNITY_EDITOR
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.ColliderSystem.Scripts.UnityIntegration;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.BrushEraseTool.ScriptsEditor.PrototypeSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.PreferencesSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility;
using VladislavTsurikov.MegaWorld.Core.Scripts.ResourcesController;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes.Overlap;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem.GlobalToolSettings;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using VladislavTsurikov.Undo.ScriptsEditor.UndoActions;
using VladislavTsurikov.Utility;
using GameObjectUtility = VladislavTsurikov.Utility.GameObjectUtility;
using GUIUtility = UnityEngine.GUIUtility;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
#if INSTANT_RENDERER
using VladislavTsurikov.InstantRenderer.LargeObjectRenderer.Scripts.RendererData;
#endif

namespace VladislavTsurikov.MegaWorld.BrushEraseTool.ScriptsEditor
{
    [Name("Brush Erase")]
    [WindowTool(true)] 
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new []{typeof(PrototypeLargeObject), typeof(PrototypeGameObject), typeof(PrototypeTerrainDetail)})]
    [AddGlobalToolSettings(new[] { typeof(BrushEraseToolComponent), typeof(BrushComponent) })]
    [AddPrototypeComponents(new []{typeof(PrototypeLargeObject), typeof(PrototypeGameObject), typeof(PrototypeTerrainDetail)} , new []{typeof(AdditionalEraseComponent)})]
    [AddGroupComponents(new []{typeof(PrototypeLargeObject), typeof(PrototypeGameObject)}, new []{typeof(FilterComponent)})]
    [AddGroupComponents(new []{typeof(PrototypeTerrainDetail)}, new []{typeof(MaskFilterComponent)})]
    public class BrushEraseTool : ToolWindow
    {
        private MouseMoveData _mouseMoveData = new MouseMoveData();
        
        [OnDeserializing]
        private void Initialize()
        {
            _mouseMoveData = new MouseMoveData();
        }

        public override void DoTool()
        {
            BrushComponent brushComponent = (BrushComponent)GlobalToolComponentStack.GetInstance(typeof(BrushEraseTool), typeof(BrushComponent));

            int controlID = GUIUtility.GetControlID(EditorHash, FocusType.Passive);
            Event e = Event.current;
            EventType eventType = e.GetTypeForControl(controlID);

            switch (eventType)
            {
                case EventType.MouseDown:
                {
                    if (e.button != 0 || Event.current.alt)
                    {
                        return;
                    }

                    if(_mouseMoveData.UpdatePosition() == false)
                    {
                        return;
                    }

                    if(_mouseMoveData.Raycast != null)
                    {
                        foreach (Group group in WindowDataPackage.Instance.SelectedVariables.SelectedGroupList)
                        {
                            AreaVariables areaVariables = brushComponent.BrushJitterSettings.GetAreaVariables(brushComponent, _mouseMoveData.Raycast.Point, group);

                            if(areaVariables.RayHit != null)
                            {
                                EraseType(group, areaVariables);
                            }
                        }
                    }
                    
                    _mouseMoveData.DragDistance = 0;
                    _mouseMoveData.PrevRaycast = _mouseMoveData.Raycast;

                    break;
                }
                case EventType.MouseDrag:
                {
                    if (e.button != 0 || Event.current.alt)
                    {
                        return;
                    }

                    if(_mouseMoveData.UpdatePosition() == false)
                    {
                        return;
                    }

                    _mouseMoveData.DragMouse(brushComponent.GetCurrentSpacing(), dragPoint =>
                    {
                        foreach (Group group in WindowDataPackage.Instance.SelectedVariables.SelectedGroupList)
                        {
                            LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;

                            RayHit originalRaycastInfo = RaycastUtility.Raycast(RayUtility.GetRayDown(dragPoint), layerSettings.GetCurrentPaintLayers(group.PrototypeType));

                            if(originalRaycastInfo != null)
                            {
                                AreaVariables areaVariables = brushComponent.GetAreaVariables(originalRaycastInfo);

                                if(areaVariables.RayHit != null)
                                {
                                    EraseType(group, areaVariables);
                                }
                            }
                        }
                        
                        return true;
                    });

                    e.Use();

                    break;
                }
                case EventType.MouseMove:
                {
                    if(_mouseMoveData.UpdatePosition() == false)
                    {
                        return;
                    }

                    e.Use();
                    break;
                }
                case EventType.Repaint:
                {           
	                if(_mouseMoveData.Raycast == null)
                    {
                        return;
                    }

                    AreaVariables areaVariables = brushComponent.GetAreaVariables(_mouseMoveData.Raycast);
                    BrushEraseToolVisualisation.Draw(areaVariables);

                    break;
                }
                case EventType.Layout:
                {           
                    HandleUtility.AddDefaultControl(controlID);

                    break;
                }
                case EventType.KeyDown:
                {
                    switch (e.keyCode)
                    {
                        case KeyCode.F:
                        {
                            if (Utility.GUIUtility.IsModifierDown(EventModifiers.None) && _mouseMoveData.Raycast != null)
                            {
                                SceneView.lastActiveSceneView.LookAt(_mouseMoveData.Raycast.Point, SceneView.lastActiveSceneView.rotation, brushComponent.BrushSize);
                                e.Use();
                            }
                        }

                        break;
                    }
                    break;
                }
            }
        }
        
        public void EraseType(Group group, AreaVariables areaVariables)
        {
            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                BrushEraseGameObject(group, areaVariables);
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainDetail))
            {
                EraseTerrainDetails(group, areaVariables);
            }
            else if (group.PrototypeType == typeof(PrototypeLargeObject))
            {
                BrushEraseInstantItem(group, areaVariables);
            }
        }

        public void BrushEraseInstantItem(Group group, AreaVariables areaVariables)
        {
#if INSTANT_RENDERER
            BrushEraseToolComponent brushEraseToolComponent = (BrushEraseToolComponent)GlobalToolComponentStack.GetInstance(typeof(BrushEraseTool), typeof(BrushEraseToolComponent));
            
            FilterComponent filterComponent = (FilterComponent)group.GetSettings(typeof(BrushEraseTool), typeof(FilterComponent));
            
            if(filterComponent.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateMaskTexture(filterComponent.MaskFilterComponent, areaVariables);
            }

            LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;

            List<LargeObjectInstance> persistentItemForDestroy = new List<LargeObjectInstance>();

            PrototypeLargeObjectOverlap.OverlapBox(areaVariables.Bounds, null, false, true,(proto, go) =>
            {
                if(proto.Active == false || proto.Selected == false)
                {
                    return true;
                }

                AdditionalEraseComponent additionalEraseComponent = (AdditionalEraseComponent)proto.GetSettings(typeof(BrushEraseTool), typeof(AdditionalEraseComponent));

                float fitness = 1;

                if(filterComponent.FilterType == FilterType.SimpleFilter)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(RayUtility.GetRayFromCameraPosition(go.Position), out hit, AdvancedSettings.Instance.EditorSettings.raycastSettings.MaxRayDistance, 
                        layerSettings.GetCurrentPaintLayers(group.PrototypeType)))
    		        {
                        fitness = filterComponent.SimpleFilterComponent.GetFitness(hit.point, hit.normal);
                    }
                }
                else
                {
                    if(filterComponent.MaskFilterComponent.Stack.ComponentList.Count != 0)
                    {
                        fitness = GrayscaleFromTexture.GetFromWorldPosition(areaVariables.Bounds, go.Position, filterComponent.MaskFilterComponent.FilterMaskTexture2D);
                    }
                }

                float maskFitness = GrayscaleFromTexture.GetFromWorldPosition(areaVariables.Bounds, go.Position, areaVariables.Mask);

                fitness *= maskFitness;

                fitness *= brushEraseToolComponent.EraseStrength;

                float successOfErase = Random.Range(0.0f, 1.0f);

                if(successOfErase < fitness)
                {
                    float randomSuccessForErase = Random.Range(0.0f, 1.0f);

                    if(randomSuccessForErase < additionalEraseComponent.SuccessForErase / 100)
                    {
                        persistentItemForDestroy.Add(go);
                    }
                }

                return true;
            });

            foreach (LargeObjectInstance item in persistentItemForDestroy)
            {
                item.Destroy();
            }
#endif
        }

        public void BrushEraseGameObject(Group group, AreaVariables areaVariables)
        {
            BrushEraseToolComponent brushEraseToolComponent = (BrushEraseToolComponent)GlobalToolComponentStack.GetInstance(typeof(BrushEraseTool), typeof(BrushEraseToolComponent));
            
            FilterComponent filterComponent = (FilterComponent)group.GetSettings(typeof(BrushEraseTool), typeof(FilterComponent));
            
            if(filterComponent.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateMaskTexture(filterComponent.MaskFilterComponent, areaVariables);
            }

            LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;

            PrototypeGameObjectOverlap.OverlapBox(areaVariables.Bounds, (proto, go) =>
            {
                if(proto == null || proto.Active == false || proto.Selected == false)
                {
                    return true;
                }

                AdditionalEraseComponent additionalEraseComponent = (AdditionalEraseComponent)proto.GetSettings(typeof(BrushEraseTool), typeof(AdditionalEraseComponent));
                
                GameObject prefabRoot = GameObjectUtility.GetPrefabRoot(go);
                
                if (prefabRoot == null)
                {
                    return true;
                }

                if(areaVariables.Bounds.Contains(prefabRoot.transform.position))
                {
                    float fitness = 1;

                    if(filterComponent.FilterType == FilterType.SimpleFilter)
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(RayUtility.GetRayFromCameraPosition(prefabRoot.transform.position), out hit, AdvancedSettings.Instance.EditorSettings.raycastSettings.MaxRayDistance, 
                            layerSettings.GetCurrentPaintLayers(group.PrototypeType)))
    		            {
                            fitness = filterComponent.SimpleFilterComponent.GetFitness(hit.point, hit.normal);
                        }
                    }
                    else
                    {
                        if(filterComponent.MaskFilterComponent.Stack.ComponentList.Count != 0)
                        {
                            fitness = GrayscaleFromTexture.GetFromWorldPosition(areaVariables.Bounds, prefabRoot.transform.position, filterComponent.MaskFilterComponent.FilterMaskTexture2D);
                        }
                    }

                    float maskFitness = GrayscaleFromTexture.GetFromWorldPosition(areaVariables.Bounds, prefabRoot.transform.position, areaVariables.Mask);

                    fitness *= maskFitness;

                    fitness *= brushEraseToolComponent.EraseStrength;

                    float successOfErase = Random.Range(0.0f, 1.0f);

                    if(successOfErase < fitness)
                    {
                        float randomSuccessForErase = Random.Range(0.0f, 1.0f);

                        if(randomSuccessForErase < additionalEraseComponent.SuccessForErase / 100)
                        {
                            Undo.ScriptsEditor.Undo.RegisterUndoAfterMouseUp(new DestroyedGameObject(go));
                            Object.DestroyImmediate(prefabRoot);
                        }
                    }
                }

                return true;
            });

            GameObjectCollider.RemoveNullObjectNodesForAllScenes();
        }

        private void EraseTerrainDetails(Group group, AreaVariables areaVariables)
        {
            BrushEraseToolComponent brushEraseToolComponent = (BrushEraseToolComponent)GlobalToolComponentStack.GetInstance(typeof(BrushEraseTool), typeof(BrushEraseToolComponent));
            
            if(TerrainResourcesController.IsSyncError(group, Terrain.activeTerrain))
            {
                return;
            }
            
            MaskFilterComponent maskFilterComponent = (MaskFilterComponent)group.GetSettings(typeof(BrushEraseTool), typeof(MaskFilterComponent));
            
            FilterMaskOperation.UpdateMaskTexture(maskFilterComponent, areaVariables);
            
            Vector2Int eraseSize;
	        Vector2Int position, startPosition;
        
            eraseSize = new Vector2Int(
					CommonUtility.WorldToDetail(areaVariables.Size * areaVariables.SizeMultiplier, areaVariables.TerrainUnderCursor.terrainData.size.x, areaVariables.TerrainUnderCursor.terrainData),
					CommonUtility.WorldToDetail(areaVariables.Size * areaVariables.SizeMultiplier, areaVariables.TerrainUnderCursor.terrainData.size.z, areaVariables.TerrainUnderCursor.terrainData));
        
            Vector2Int halfBrushSize = eraseSize / 2;

            Vector2Int center = new Vector2Int(
                CommonUtility.WorldToDetail(areaVariables.RayHit.Point.x - areaVariables.TerrainUnderCursor.transform.position.x, areaVariables.TerrainUnderCursor.terrainData),
                CommonUtility.WorldToDetail(areaVariables.RayHit.Point.z - areaVariables.TerrainUnderCursor.transform.position.z, areaVariables.TerrainUnderCursor.terrainData.size.z, 
                areaVariables.TerrainUnderCursor.terrainData));
        
            position = center - halfBrushSize;
            startPosition = Vector2Int.Max(position, Vector2Int.zero);
        
            Vector2Int offset = startPosition - position;
        
            Vector2Int current;
            float fitness = 1;
            float detailmapResolution = areaVariables.TerrainUnderCursor.terrainData.detailResolution;
            int x, y;
            
            foreach (Prototype prototype in group.PrototypeList)
            {
                PrototypeTerrainDetail prototypeTerrainDetail = (PrototypeTerrainDetail)prototype;
                
                if(prototypeTerrainDetail.Active == false || prototypeTerrainDetail.Selected == false)
                {
                    continue;
                }

                AdditionalEraseComponent additionalEraseComponent = (AdditionalEraseComponent)prototypeTerrainDetail.GetSettings(typeof(BrushEraseTool), typeof(AdditionalEraseComponent));

                int[,] localData = areaVariables.TerrainUnderCursor.terrainData.GetDetailLayer(
                    startPosition.x, startPosition.y,
                    Mathf.Max(0, Mathf.Min(position.x + eraseSize.x, (int)detailmapResolution) - startPosition.x),
                    Mathf.Max(0, Mathf.Min(position.y + eraseSize.y, (int)detailmapResolution) - startPosition.y), prototypeTerrainDetail.TerrainProtoId);

                float widthY = localData.GetLength(1);
                float heightX = localData.GetLength(0);
                
                if (maskFilterComponent.Stack.ComponentList.Count > 0)
			    {
                    for (y = 0; y < widthY; y++)
                    {
                        for (x = 0; x < heightX; x++)
                        {
                            current = new Vector2Int(y, x);

                            float randomSuccess = Random.Range(0.0f, 1.0f);

                            if(randomSuccess < additionalEraseComponent.SuccessForErase / 100)
                            {
                                Vector2 normal = Vector2.zero;
                                normal.y = Mathf.InverseLerp(0, eraseSize.y, current.y);
                                normal.x = Mathf.InverseLerp(0, eraseSize.x, current.x);

                                fitness = GrayscaleFromTexture.Get(normal, maskFilterComponent.FilterMaskTexture2D);

                                float maskFitness = BrushJitterSettings.GetAlpha(areaVariables, current + offset, eraseSize);

                                fitness *= maskFitness;

                                fitness *= brushEraseToolComponent.EraseStrength;

                                int targetStrength = Mathf.Max(0, localData[x, y] - Mathf.RoundToInt(Mathf.Lerp(0, 10, fitness)));

                                localData[x, y] = targetStrength;
                            }
                        }
                    }

                    areaVariables.TerrainUnderCursor.terrainData.SetDetailLayer(startPosition.x, startPosition.y, prototypeTerrainDetail.TerrainProtoId, localData);
                }
                else
                {
                    for (y = 0; y < widthY; y++)
                    {
                        for (x = 0; x < heightX; x++)
                        {
                            current = new Vector2Int(y, x);

                            float randomSuccess = Random.Range(0.0f, 1.0f);

                            if(randomSuccess < additionalEraseComponent.SuccessForErase / 100)
                            {
                                float maskFitness = BrushJitterSettings.GetAlpha(areaVariables, current + offset, eraseSize);

                                maskFitness *= brushEraseToolComponent.EraseStrength;

                                int targetStrength = Mathf.Max(0, localData[x, y] - Mathf.RoundToInt(Mathf.Lerp(0, 10, maskFitness)));

                                localData[x, y] = targetStrength;
                            }
                        }
                    }

                    areaVariables.TerrainUnderCursor.terrainData.SetDetailLayer(startPosition.x, startPosition.y, prototypeTerrainDetail.TerrainProtoId, localData);
                }
            }
        }

        public override void HandleKeyboardEvents()
        {
            BrushComponent brushComponent = (BrushComponent)GlobalToolComponentStack.GetInstance(typeof(BrushEraseTool), typeof(BrushComponent));
            
            brushComponent.ScrollBrushRadiusEvent();
		}
    }
}
#endif