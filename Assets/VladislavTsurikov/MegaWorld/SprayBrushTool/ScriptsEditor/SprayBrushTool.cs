#if UNITY_EDITOR
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.ColliderSystem.Scripts.UnityIntegration;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem.GlobalToolSettings;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using VladislavTsurikov.Scripts;
using VladislavTsurikov.Undo.ScriptsEditor.UndoActions;
using GUIUtility = VladislavTsurikov.Utility.GUIUtility;
#if INSTANT_RENDERER
using VladislavTsurikov.InstantRenderer.LargeObjectRenderer.Scripts.API;
#endif

namespace VladislavTsurikov.MegaWorld.SprayBrushTool.ScriptsEditor
{
    [WindowTool(true)]   
    [Name("Spray Brush")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new []{typeof(PrototypeLargeObject), typeof(PrototypeGameObject)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeLargeObject), new []{typeof(SuccessComponent), typeof(OverlapCheckComponent)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeGameObject), new []{typeof(SuccessComponent), typeof(OverlapCheckComponent)})]
    [AddPrototypeComponents(typeof(PrototypeLargeObject), new []{typeof(SimpleTransformComponentComponent)})]
    [AddPrototypeComponents(typeof(PrototypeGameObject), new []{typeof(SimpleTransformComponentComponent)})]
    [AddGroupComponents(new []{typeof(PrototypeLargeObject), typeof(PrototypeGameObject)}, new []{typeof(SimpleFilterComponent)})]
    [AddGlobalToolSettings(new []{typeof(BrushComponent)})]
    public class SprayBrushTool : ToolWindow
    {
        private MouseMoveData _mouseMoveData = new MouseMoveData();
        
        [OnDeserializing]
        private void Initialize()
        {
            _mouseMoveData = new MouseMoveData();
        }
        
        public override void DoTool()
        {            
            BrushComponent brushComponent = (BrushComponent)GlobalToolComponentStack.GetInstance(typeof(SprayBrushTool), typeof(BrushComponent));

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
                            AreaVariables areaVariables = brushComponent.GetAreaVariables(_mouseMoveData.Raycast);

                            if(areaVariables.RayHit != null)
                            {
                                PaintType(group, areaVariables);
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

                    float brushSpacing = brushComponent.Spacing;

                    LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;

                    _mouseMoveData.DragMouse(brushSpacing, (dragPoint) =>
                    {
                        foreach (Group group in WindowDataPackage.Instance.SelectedVariables.SelectedGroupList)
                        {
                            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayFromCameraPosition(dragPoint), layerSettings.GetCurrentPaintLayers(group.PrototypeType));

                            if(rayHit != null)
                            {
                                AreaVariables areaVariables = brushComponent.GetAreaVariables(rayHit);

                                if(areaVariables.RayHit != null)
                                {
                                    PaintType(group, areaVariables);
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
                    SprayBrushToolVisualisation.Draw(areaVariables);

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
            BrushComponent brushComponent = (BrushComponent)GlobalToolComponentStack.GetInstance(typeof(SprayBrushTool), typeof(BrushComponent)); 
            
            brushComponent.ScrollBrushRadiusEvent();
		}

        public void PaintType(Group group, AreaVariables areaVariables)
        {
            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                SpawnTypeGameObject(group, areaVariables);
            }
            else if (group.PrototypeType == typeof(PrototypeLargeObject))
            {
                SpawnTypeInstantItem(group, areaVariables);
            }
        }

        public void SpawnTypeInstantItem(Group group, AreaVariables areaVariables)
        {
            LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;;

            Vector3 positionForSpawn = areaVariables.RayHit.Point + Vector3.ProjectOnPlane(Random.onUnitSphere, areaVariables.RayHit.Normal) * areaVariables.Radius;

            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayFromCameraPosition(positionForSpawn), layerSettings.GetCurrentPaintLayers(group.PrototypeType));
            if(rayHit != null)
            {
                PrototypeLargeObject proto = (PrototypeLargeObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

                if(proto == null || proto.Active == false)
                {
                    return;
                }

                float fitness = GetFitness.GetFromSimpleFilter((SimpleFilterComponent)group.GetSettings(GetType(), typeof(SimpleFilterComponent)), rayHit);
                
                if(fitness != 0)
                {
                    SpawnInstantItem(proto, rayHit, fitness);
                }
            }
        }

        public void SpawnTypeGameObject(Group group, AreaVariables areaVariables)
        {
            LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;;

            Vector3 positionForSpawn = areaVariables.RayHit.Point + Vector3.ProjectOnPlane(Random.onUnitSphere, areaVariables.RayHit.Normal) * areaVariables.Radius;

            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayFromCameraPosition(positionForSpawn), layerSettings.GetCurrentPaintLayers(group.PrototypeType));
            if(rayHit != null)
            {
                PrototypeGameObject proto = (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

                if(proto == null || proto.Active == false)
                {
                    return;
                }

                float fitness = GetFitness.GetFromSimpleFilter((SimpleFilterComponent)group.GetSettings(GetType(), typeof(SimpleFilterComponent)), rayHit);
                
                if(fitness != 0)
                {
                    SpawnGameObject(group, proto, rayHit, fitness);
                }
            }
        }

        public static void SpawnInstantItem(PrototypeLargeObject proto, RayHit rayHit, float fitness)
        {
#if INSTANT_RENDERER
            OverlapCheckComponent overlapCheckComponent = (OverlapCheckComponent)proto.GetSettings(typeof(OverlapCheckComponent));

            InstanceData instanceData = new InstanceData(rayHit.Point, proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            SimpleTransformComponentComponent transformComponentComponent = (SimpleTransformComponentComponent)proto.GetSettings(typeof(SprayBrushTool), typeof(SimpleTransformComponentComponent));
            transformComponentComponent.Stack.SetInstanceData(ref instanceData, fitness, rayHit.Normal);

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

            SimpleTransformComponentComponent transformComponentComponent = (SimpleTransformComponentComponent)proto.GetSettings(typeof(SprayBrushTool), typeof(SimpleTransformComponentComponent));
            transformComponentComponent.Stack.SetInstanceData(ref instanceData, fitness, rayHit.Normal);

            if(OverlapCheckComponent.RunOverlapCheck(proto.GetType(), overlapCheckComponent, proto.Extents, instanceData))
            {
                PlacedObject objectInfo = PlacedObjectUtility.PlaceObject(proto.Prefab, instanceData.Position, instanceData.Scale, instanceData.Rotation);
                PlacedObjectUtility.ParentGameObject(group, objectInfo);

                GameObjectCollider.RegisterGameObjectToCurrentScene(objectInfo.GameObject);  
                
                if(registerUndo)
                {
                    Undo.ScriptsEditor.Undo.RegisterUndoAfterMouseUp(new CreatedGameObject(objectInfo.GameObject));
                }
                objectInfo.GameObject.transform.hasChanged = false;
                
            }
        }
    }
}
#endif