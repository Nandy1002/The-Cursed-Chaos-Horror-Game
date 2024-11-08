using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.ScatterSettingsSystem;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility.Spawn;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem.GlobalToolSettings;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using VladislavTsurikov.Utility;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using GUIUtility = VladislavTsurikov.Utility.GUIUtility;
#endif

namespace VladislavTsurikov.MegaWorld.AdvancedBrushTool.ScriptsEditor
{
    [Name("Advanced Brush")]
    [WindowTool(true)]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new []{typeof(PrototypeLargeObject), typeof(PrototypeGameObject), typeof(PrototypeTerrainDetail), typeof(PrototypeTerrainTexture)})]
    [AddGlobalToolSettings(new[] { typeof(AdvancedBrushToolComponent), typeof(BrushComponent)})]
    [AddGeneralPrototypeComponents(new []{typeof(PrototypeLargeObject), typeof(PrototypeGameObject)}, new []{typeof(SuccessComponent), typeof(OverlapCheckComponent), typeof(TransformStackComponent)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainDetail), new []{typeof(SpawnDetailComponent), typeof(MaskFilterComponent)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainTexture), new []{typeof(MaskFilterComponent)})]
    [AddGeneralGroupComponents(new []{typeof(PrototypeLargeObject), typeof(PrototypeGameObject)}, new []{typeof(ScatterComponent), typeof(FilterComponent)})]
    public class AdvancedBrushTool : ToolWindow
    {
        private MouseMoveData _mouseMoveData = new MouseMoveData();
        private bool _startDrag = false;

        [OnDeserializing]
        private void Initialize()
        {
            _mouseMoveData = new MouseMoveData();
            _startDrag = false;
        }

#if UNITY_EDITOR
        public override void DoTool()
        {
            AdvancedBrushToolComponent advancedBrushToolComponent = (AdvancedBrushToolComponent)GlobalToolComponentStack.GetInstance(typeof(AdvancedBrushTool), typeof(AdvancedBrushToolComponent));
            BrushComponent brushComponent = (BrushComponent)GlobalToolComponentStack.GetInstance(typeof(AdvancedBrushTool), typeof(BrushComponent));

            int controlID = UnityEngine.GUIUtility.GetControlID(EditorHash, FocusType.Passive);
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
                                PaintType(group, areaVariables);
                            }
                        }
                    }

                    _mouseMoveData.DragDistance = 0;
                    _mouseMoveData.PrevRaycast = _mouseMoveData.Raycast;
                    _startDrag = true;

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

                    float brushSpacing = brushComponent.GetCurrentSpacing();
                    if(_startDrag)
                    {
                        if(WindowDataPackage.Instance.SelectedVariables.GetSelectedPrototypes(typeof(PrototypeLargeObject)).Count != 0
                        || WindowDataPackage.Instance.SelectedVariables.GetSelectedPrototypes(typeof(PrototypeGameObject)).Count != 0)
                        {
                            if(brushSpacing < brushComponent.BrushSize / 2)
                            {
                                brushSpacing = brushComponent.BrushSize / 2;
                            }
                        }
                    }

                    _mouseMoveData.DragMouse(brushSpacing, (dragPoint) =>
                    {
                        _startDrag = false;

                        foreach (Group group in WindowDataPackage.Instance.SelectedVariables.SelectedGroupList)
                        {
                            LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;
                            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(dragPoint), layerSettings.GetCurrentPaintLayers(group.PrototypeType));

                            if(rayHit != null)
                            {
                                AreaVariables areaVariables = brushComponent.BrushJitterSettings.GetAreaVariables(brushComponent, rayHit.Point, group);

                                if(areaVariables.RayHit != null)
                                {
                                    PaintType(group, areaVariables, advancedBrushToolComponent.EnableFailureRateOnMouseDrag);
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
                    AdvancedBrushToolVisualisation.Draw(areaVariables);

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
                            if (GUIUtility.IsModifierDown(EventModifiers.None) && _mouseMoveData.Raycast != null)
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

        public override void HandleKeyboardEvents()
        {
            BrushComponent brushComponent = (BrushComponent)GlobalToolComponentStack.GetInstance(typeof(AdvancedBrushTool), typeof(BrushComponent));

            brushComponent.ScrollBrushRadiusEvent();
		}
#endif

        public void PaintType(Group group, AreaVariables areaVariables, bool dragMouse = false)
        {
            AdvancedBrushToolComponent advancedBrushToolComponent = (AdvancedBrushToolComponent)GlobalToolComponentStack.GetInstance(typeof(AdvancedBrushTool), typeof(AdvancedBrushToolComponent));

            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                SpawnTypeGameObject(group, areaVariables, dragMouse);
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainDetail))
            {
                SpawnGroup.SpawnTerrainDetails(group, group.PrototypeList, areaVariables);
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainTexture))
            {
                SpawnGroup.SpawnTerrainTexture(group, group.GetAllSelectedPrototypes(), areaVariables, advancedBrushToolComponent.TextureTargetStrength);
            }
            else if (group.PrototypeType == typeof(PrototypeLargeObject))
            {
                SpawnTypeInstantItem(group, areaVariables, dragMouse);
            }
        }

        public void SpawnTypeGameObject(Group group, AreaVariables areaVariables, bool dragMouse)
        {
            AdvancedBrushToolComponent advancedBrushToolComponent = (AdvancedBrushToolComponent)GlobalToolComponentStack.GetInstance(typeof(AdvancedBrushTool), typeof(AdvancedBrushToolComponent));

            FilterComponent filterComponent = (FilterComponent)group.GetSettings(typeof(FilterComponent));

            if(filterComponent.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateMaskTexture(filterComponent.MaskFilterComponent, areaVariables);
            }

            ScatterComponent scatterComponent = (ScatterComponent)group.GetSettings(typeof(ScatterComponent));
            LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;

            scatterComponent.Stack.Samples(areaVariables, sample =>
            {
                if(dragMouse)
                {
                    if(Random.Range(0f, 100f) < advancedBrushToolComponent.FailureRate)
                    {
                        return;
                    }
                }

                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, areaVariables.RayHit.Point.y, sample.y)), layerSettings.GetCurrentPaintLayers(group.PrototypeType));
                if(rayHit != null)
                {
                    PrototypeGameObject proto = (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

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

                        SpawnPrototype.SpawnGameObject(group, proto, rayHit, fitness);
                    }
                }
            });
        }

        public void SpawnTypeInstantItem(Group group, AreaVariables areaVariables, bool dragMouse = false)
        {
#if INSTANT_RENDERER
            AdvancedBrushToolComponent advancedBrushToolComponent = (AdvancedBrushToolComponent)GlobalToolComponentStack.GetInstance(typeof(AdvancedBrushTool), typeof(AdvancedBrushToolComponent));

            FilterComponent filterComponent = (FilterComponent)group.GetSettings(typeof(FilterComponent));

            if(filterComponent.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateMaskTexture(filterComponent.MaskFilterComponent, areaVariables);
            }

            LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;
            ScatterComponent scatterComponent = (ScatterComponent)group.GetSettings(typeof(ScatterComponent));

            scatterComponent.Stack.Samples(areaVariables, sample =>
            {
                if(dragMouse)
                {
                    if(Random.Range(0f, 100f) < advancedBrushToolComponent.FailureRate)
                    {
                        return;
                    }
                }

                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, areaVariables.RayHit.Point.y, sample.y)), layerSettings.GetCurrentPaintLayers(group.PrototypeType));
                if(rayHit != null)
                {
                    PrototypeLargeObject proto = (PrototypeLargeObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

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
    }
}