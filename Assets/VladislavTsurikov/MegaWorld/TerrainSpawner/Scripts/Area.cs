#if GRIFFIN_2020 || GRIFFIN_2021
using Pinwheel.Griffin;
#endif
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using VladislavTsurikov.Utility;
#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.TerrainSpawner.ScriptsEditor;
#endif

namespace VladislavTsurikov.MegaWorld.TerrainSpawner.Scripts
{
    public enum HandleSettingsMode
    { 
        Custom,
        Standard
    }

    [Serializable]
    public class Area 
    {
        public Vector3 PastThisPosition = Vector3.zero;
        public Vector3 PastScale = Vector3.one;
        public Bounds Bounds;

        public Color ColorCube = Color.HSVToRGB(0.0f, 0.75f, 1.0f);
        public float PixelWidth = 4.0f;
        public bool Dotted = false;
        public HandleSettingsMode HandleSettingsMode = HandleSettingsMode.Standard;
        public bool DrawHandleIfNotSelected = false;

#if UNITY_EDITOR
        public AreaEditor AreaEditor = new AreaEditor();
        public bool AreaSettingsFoldout = true;

        public void OnGUI(TerrainSpawner stamper)
        {
            AreaEditor.OnGUI(stamper, this);
        }
#endif

        public void SetAreaBounds(TerrainSpawner stamperTool)
        {
            Bounds = new Bounds();
            Bounds.size = new Vector3(stamperTool.transform.localScale.x, stamperTool.transform.localScale.y, stamperTool.transform.localScale.z);
            Bounds.center = stamperTool.transform.position;
        }

        public void FitToTerrainSize(TerrainSpawner stamperTool)
        {
            if(Terrain.activeTerrains.Length != 0)
            {
                Bounds newBounds = new Bounds(Vector3.zero, Vector3.zero);
                for (int i = 0; i < Terrain.activeTerrains.Length; i++)
                {
                    Terrain terrain = Terrain.activeTerrains[i];

                    Bounds terrainBounds = new Bounds(terrain.terrainData.bounds.center + terrain.transform.position, terrain.terrainData.bounds.size);

                    if (i == 0)
                    {
                        newBounds = terrainBounds;
                    }
                    else
                    {
                        newBounds.Encapsulate(terrainBounds);
                    }
                }

                if(newBounds.size.z > newBounds.size.x)
                {
                    newBounds = new Bounds(newBounds.center, new Vector3(newBounds.size.z, newBounds.size.y, newBounds.size.z)); 
                }
                else if(newBounds.size.x > newBounds.size.z)
                {
                    newBounds = new Bounds(newBounds.center, new Vector3(newBounds.size.x, newBounds.size.y, newBounds.size.x));
                }

                var transform = stamperTool.transform;
                transform.position = newBounds.center + new Vector3(1, 0, 1);
                transform.localScale = newBounds.size + new Vector3(1, 0, 1);
            }
            #if GRIFFIN_2020 || GRIFFIN_2021
            else
            {
                Bounds b = GCommon.GetLevelBounds();
                stamperTool.transform.position = new Vector3(b.center.x, b.size.y / 2, b.center.z);
                stamperTool.transform.localScale = new Vector3(b.size.x, b.size.y, b.size.z);
            }
            #endif
        }

        public void SetAreaBoundsIfNecessary(TerrainSpawner stamperTool, bool setForce = false)
        {
            bool hasChangedPosition = PastThisPosition != stamperTool.transform.position;
            bool hasChangedSize = stamperTool.transform.localScale != PastScale;

            if(hasChangedPosition || hasChangedSize)
            {
#if UNITY_EDITOR
                stamperTool.StamperVisualisation.UpdateMask = true;
#endif
                
                SetAreaBounds(stamperTool);
            }
            else if(setForce)
            {
#if UNITY_EDITOR
                stamperTool.StamperVisualisation.UpdateMask = true;
#endif

                SetAreaBounds(stamperTool);
            }

            PastScale = stamperTool.transform.localScale;

            PastThisPosition = stamperTool.transform.position;
        }

        public Texture2D GetCurrentRaw()
        {
            return Texture2D.whiteTexture;
        }

        public AreaVariables GetAreaVariables(RayHit hit)
        {
            return new AreaVariables(Bounds, hit);
        }
    }

#if UNITY_EDITOR 
    public static class AreaGizmoDrawer
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.Selected)]
        private static void DrawGizmoForArea(TerrainSpawner stamperTool, GizmoType gizmoType)
        {
            bool isFaded = (int)gizmoType == (int)GizmoType.NonSelected || (int)gizmoType == (int)GizmoType.NotInSelectionHierarchy || (int)gizmoType == (int)GizmoType.NonSelected + (int)GizmoType.NotInSelectionHierarchy;
            
            if(stamperTool.Area.DrawHandleIfNotSelected == false)
            {
                if(isFaded)
                {
                    return;
                }
            }

            if(stamperTool.Data.SelectedVariables.GetSelectedPrototypes(typeof(PrototypeGameObject)).Count != 0)
            {
                Bounds bounds = new Bounds(Camera.current.transform.position, new Vector3(50f, 50f, 50f));
                OverlapCheckComponent.VisualizeOverlapForGameObject(bounds, true);
            }
            
			if(stamperTool.Data.SelectedVariables.GetSelectedPrototypes(typeof(PrototypeLargeObject)).Count != 0)
			{
                Bounds bounds = new Bounds(Camera.current.transform.position, new Vector3(50f, 50f, 50f));
                OverlapCheckComponent.VisualizeOverlapForInstantItem(bounds, true);
			}

            float opacity = isFaded ? 0.5f : 1.0f;

            DrawStamperVisualizationifNecessary(stamperTool, opacity);

            DrawBox(stamperTool, opacity);
        }

        private static void DrawStamperVisualizationifNecessary(TerrainSpawner stamperTool, float multiplyAlpha)
        {
            if(stamperTool.StamperToolControllerSettings.Visualisation == false)
            {
                return;
            }

            if(stamperTool.Data.SelectedVariables.HasOneSelectedGroup() == false)
            {
                return;
            }

            Group group = stamperTool.Data.SelectedVariables.SelectedGroup;

            LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;

            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(stamperTool.Area.Bounds.center), 
                layerSettings.GetCurrentPaintLayers(group.PrototypeType));
            if(rayHit != null)
            {
                AreaVariables areaVariables = stamperTool.Area.GetAreaVariables(rayHit);
                stamperTool.StamperVisualisation.Draw(areaVariables, stamperTool.Data, multiplyAlpha);
            }
        }

        private static void DrawBox(TerrainSpawner stamperTool, float alpha)
        {
            Transform newTransform = stamperTool.transform;
            newTransform.rotation = Quaternion.identity;
            newTransform.transform.localScale = new Vector3 (Mathf.Max(1f, newTransform.transform.localScale.z), Mathf.Max(1f, newTransform.transform.localScale.y), Mathf.Max(1f, newTransform.transform.localScale.z));

            if(stamperTool.Area.HandleSettingsMode == HandleSettingsMode.Custom)
            {
                Color color = stamperTool.Area.ColorCube;
                color.a *= alpha;
                DrawHandles.DrawCube(newTransform.localToWorldMatrix, color, stamperTool.Area.PixelWidth, stamperTool.Area.Dotted);
            }
            else
            {
                float thickness = 4.0f;
                Color color = Color.yellow;
                color.a *= alpha;
                DrawHandles.DrawCube(newTransform.localToWorldMatrix, color, thickness);
            }
        }
    }
#endif
}