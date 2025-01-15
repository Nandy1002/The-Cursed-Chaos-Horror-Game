using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.Extensions.Scripts;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using VladislavTsurikov.Utility;
#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.MegaWorld.TextureStamperTool.ScriptsEditor;
#endif

namespace VladislavTsurikov.MegaWorld.TextureStamperTool.Scripts
{
    public enum HandleSettingsMode
    { 
        Custom,
        Standard
    }

    [Serializable]
    public class Area 
    {
        public bool UseSpawnCells = false;
        public float CellSize = 1000;
        public List<Bounds> CellList = new List<Bounds>();

        public bool UseMask = false;
        public MaskType MaskType = MaskType.Procedural;
        public ProceduralMask ProceduralMask = new ProceduralMask();
        public CustomMasks CustomMasks = new CustomMasks();
        
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
        public bool ShowCells = true;

        public void OnGUI(TextureStamperTool textureStamper)
        {
            AreaEditor.OnGUI(textureStamper, this);
        }
#endif

        public void SetAreaBounds(TextureStamperTool textureStamperTool)
        {
            Bounds = new Bounds();
            Bounds.size = new Vector3(textureStamperTool.transform.localScale.x, textureStamperTool.transform.localScale.y, textureStamperTool.transform.localScale.z);
            Bounds.center = textureStamperTool.transform.position;
        }

        public void FitToTerrainSize(TextureStamperTool textureStamperTool)
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
                    
                textureStamperTool.transform.position = newBounds.center + new Vector3(1, 0, 1);
                textureStamperTool.transform.localScale = newBounds.size + new Vector3(1, 0, 1);
            }
        }

        public void CreateCells()
        {
            CellList.Clear();

            Bounds expandedBounds = new Bounds(Bounds.center, Bounds.size);
            expandedBounds.Expand(new Vector3(CellSize * 2f, 0, CellSize * 2f));

            int cellXCount = Mathf.CeilToInt(Bounds.size.x / CellSize);
            int cellZCount = Mathf.CeilToInt(Bounds.size.z / CellSize);

            Vector2 corner = new Vector2(Bounds.center.x - Bounds.size.x / 2f, Bounds.center.z - Bounds.size.z / 2f);

            Bounds bounds = new Bounds();

            for (int x = 0; x <= cellXCount - 1; x++)
            {
                for (int z = 0; z <= cellZCount - 1; z++)
                {
                    Rect rect = new Rect(
                        new Vector2(CellSize * x + corner.x, CellSize * z + corner.y),
                        new Vector2(CellSize, CellSize));

                    bounds = RectExtension.CreateBoundsFromRect(rect, Bounds.center.y, Bounds.size.y);

                    CellList.Add(bounds);
                }
            }
        }

        public void SetAreaBoundsIfNecessary(TextureStamperTool textureStamperTool, bool setForce = false)
        {
            bool hasChangedPosition = PastThisPosition != textureStamperTool.transform.position;
            bool hasChangedSize = textureStamperTool.transform.localScale != PastScale;

            if(hasChangedPosition || hasChangedSize)
            {
#if UNITY_EDITOR
                textureStamperTool.StamperVisualisation.UpdateMask = true;
#endif
                
                SetAreaBounds(textureStamperTool);

                if(UseSpawnCells)
                {
                    CellList.Clear();
                }
            }
            else if(setForce)
            {
#if UNITY_EDITOR
                textureStamperTool.StamperVisualisation.UpdateMask = true;
#endif

                SetAreaBounds(textureStamperTool);
            }

            PastScale = textureStamperTool.transform.localScale;

            PastThisPosition = textureStamperTool.transform.position;
        }

        public Texture2D GetCurrentRaw()
        {
            if(UseMask == false || UseSpawnCells)
            {
                return Texture2D.whiteTexture;
            }

            switch (MaskType)
            {
                case MaskType.Custom:
                {
                    Texture2D texture = CustomMasks.GetSelectedBrush();

                    return texture;
                }
                case MaskType.Procedural:
                {
                    Texture2D texture = ProceduralMask.Mask;

                    return texture;
                }
            }

            return Texture2D.whiteTexture;
        }

        public AreaVariables GetAreaVariables(RayHit hit)
        {
            AreaVariables areaVariables = new AreaVariables(Bounds, hit);

            areaVariables.Mask = GetCurrentRaw();

            return areaVariables;
        }

        public AreaVariables GetAreaVariablesFromSpawnCell(RayHit hit, Bounds bounds)
        {
            AreaVariables areaVariables = new AreaVariables(Bounds, hit);

            return areaVariables;
        }
    }

#if UNITY_EDITOR 
    public static class AreaGizmoDrawer
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.Selected)]
        private static void DrawGizmoForArea(TextureStamperTool textureStamperTool, GizmoType gizmoType)
        {
            bool isFaded = (int)gizmoType == (int)GizmoType.NonSelected || (int)gizmoType == (int)GizmoType.NotInSelectionHierarchy || (int)gizmoType == (int)GizmoType.NonSelected + (int)GizmoType.NotInSelectionHierarchy;
            
            if(textureStamperTool.Area.DrawHandleIfNotSelected == false)
            {
                if(isFaded == true)
                {
                    return;
                }
            }

            if(textureStamperTool.Data.SelectedVariables.GetSelectedPrototypes(typeof(PrototypeGameObject)).Count != 0)
            {
                Bounds bounds = new Bounds(Camera.current.transform.position, new Vector3(50f, 50f, 50f));
                OverlapCheckComponent.VisualizeOverlapForGameObject(bounds, true);
            }
            
			if(textureStamperTool.Data.SelectedVariables.GetSelectedPrototypes(typeof(PrototypeLargeObject)).Count != 0)
			{
                Bounds bounds = new Bounds(Camera.current.transform.position, new Vector3(50f, 50f, 50f));
                OverlapCheckComponent.VisualizeOverlapForInstantItem(bounds, true);
			}

            float opacity = isFaded ? 0.5f : 1.0f;

            DrawStamperVisualisationIfNecessary(textureStamperTool, opacity);

            DrawBox(textureStamperTool, opacity);

            if(textureStamperTool.Area.UseSpawnCells)
			{
                DebugCells(textureStamperTool);
            }
        }

        private static void DrawStamperVisualisationIfNecessary(TextureStamperTool textureStamperTool, float multiplyAlpha)
        {
            if(textureStamperTool.textureStamperToolControllerSettings.Visualisation == false)
            {
                return;
            }

            if(textureStamperTool.Data.SelectedVariables.HasOneSelectedGroup() == false)
            {
                return;
            }

            Group group = textureStamperTool.Data.SelectedVariables.SelectedGroup;

            LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;

            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(textureStamperTool.Area.Bounds.center), 
                layerSettings.GetCurrentPaintLayers(group.PrototypeType));
            if(rayHit != null)
            {
                AreaVariables areaVariables = textureStamperTool.Area.GetAreaVariables(rayHit);
                textureStamperTool.StamperVisualisation.Draw(areaVariables, textureStamperTool.Data, multiplyAlpha);
            }
        }

        private static void DrawBox(TextureStamperTool textureStamperTool, float alpha)
        {
            Transform newTransform = textureStamperTool.transform;
            newTransform.rotation = Quaternion.identity;
            newTransform.transform.localScale = new Vector3 (Mathf.Max(1f, newTransform.transform.localScale.z), Mathf.Max(1f, newTransform.transform.localScale.y), Mathf.Max(1f, newTransform.transform.localScale.z));

            if(textureStamperTool.Area.HandleSettingsMode == HandleSettingsMode.Custom)
            {
                Color color = textureStamperTool.Area.ColorCube;
                color.a *= alpha;
                DrawHandles.DrawCube(newTransform.localToWorldMatrix, color, textureStamperTool.Area.PixelWidth, textureStamperTool.Area.Dotted);
            }
            else
            {
                float thickness = 4.0f;
                Color color = Color.yellow;
                color.a *= alpha;
                DrawHandles.DrawCube(newTransform.localToWorldMatrix, color, thickness);
            }
        }

        private static void DebugCells(TextureStamperTool textureStamperTool)
        {
            if(textureStamperTool.Area.ShowCells)
			{
                List<Bounds> cellList = textureStamperTool.Area.CellList;

                for (int i = 0; i <= cellList.Count - 1; i++)
                {                  
                    Gizmos.color = new Color(0, 1, 1, 1);
                    Gizmos.DrawWireCube(cellList[i].center, cellList[i].size);
                }
            }
        }
    }
#endif
}