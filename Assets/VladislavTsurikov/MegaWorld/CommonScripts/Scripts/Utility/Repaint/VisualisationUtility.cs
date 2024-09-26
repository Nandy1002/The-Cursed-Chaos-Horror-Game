#if UNITY_EDITOR
#if UNITY_2021_2_OR_NEWER
using UnityEditor.TerrainTools;
using UnityEngine.TerrainTools;
#else
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine.Experimental.TerrainAPI;
#endif
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.PreferencesSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using VladislavTsurikov.Utility;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility.Repaint
{
    public static class VisualisationUtility 
    {
        public static void DrawMaskFilterVisualization(MaskFilterStack maskFilterStack, AreaVariables areaVariables, float multiplyAlpha = 1)
        {
            if(areaVariables.TerrainUnderCursor == null)
            {
                return;
            }

            if(maskFilterStack == null)
            {
                return;
            }

            if (maskFilterStack.ComponentList.Count > 0)
            {
                MaskFilterContext filterContext = new MaskFilterContext(areaVariables);
                FilterMaskOperation.UpdateFilterContext(ref filterContext, maskFilterStack, areaVariables);

                DrawMaskFilter(filterContext, areaVariables, multiplyAlpha);

                filterContext.DisposeUnmanagedMemory();
            }
            else
            {
                DrawAreaPreview(areaVariables);
            }
        }

        public static void DrawMaskFilter(MaskFilterContext filterContext, AreaVariables areaVariables, float multiplyAlpha = 1)
        {
            if(areaVariables.TerrainUnderCursor == null)
            {
                return;
            }

            TerrainPainterRenderHelper terrainPainterRenderHelper = new TerrainPainterRenderHelper(areaVariables);

            DrawSpawnerShaderVisualisation(terrainPainterRenderHelper, filterContext.HeightContext, filterContext, multiplyAlpha);
        }

        public static void DrawAreaPreview(AreaVariables areaVariables)
        {
            if(areaVariables.TerrainUnderCursor == null)
            {
                return;
            }

            TerrainPainterRenderHelper terrainPainterRenderHelper = new TerrainPainterRenderHelper(areaVariables);

            PaintContext heightContext = terrainPainterRenderHelper.AcquireHeightmap();

            if(heightContext == null)
            {
                return;
            }

#if UNITY_2021_2_OR_NEWER
            terrainPainterRenderHelper.RenderAreaPreview(heightContext, TerrainBrushPreviewMode.SourceRenderTexture, TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial(), 0);
#else
            terrainPainterRenderHelper.RenderAreaPreview(heightContext, TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture, TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial(), 0);
#endif

            TerrainPaintUtility.ReleaseContextResources(heightContext);
        }

        public static void DrawSpawnerShaderVisualisation(TerrainPainterRenderHelper terrainPainterRenderHelper, PaintContext heightContext, MaskFilterContext filterContext, float multiplyAlpha = 1)
    	{
            Texture brushTexture = terrainPainterRenderHelper.AreaVariables.Mask;

            Material brushMaterial = MaskFilterUtility.GetBrushPreviewMaterial();
            RenderTexture tmpRT = RenderTexture.active;

            RenderTexture filterMaskRT = filterContext.GetFilterMaskRT();

            if(filterMaskRT != null)
            {                
                //Composite the brush texture onto the filter stack result
                RenderTexture compRT = RenderTexture.GetTemporary(filterMaskRT.descriptor);
    			Material blendMat = MaskFilterUtility.GetBlendMaterial();
    			blendMat.SetTexture("_BlendTex", brushTexture);
    			blendMat.SetVector("_BlendParams", new Vector4(0.0f, 0.0f, -(terrainPainterRenderHelper.AreaVariables.Rotation * Mathf.Deg2Rad), 0.0f));
    			TerrainPaintUtility.SetupTerrainToolMaterialProperties(heightContext, terrainPainterRenderHelper.BrushTransform, blendMat);
    			Graphics.Blit(filterMaskRT, compRT, blendMat, 0);

    			RenderTexture.active = tmpRT;

                BrushTransform identityBrushTransform = TerrainPaintUtility.CalculateBrushTransform(terrainPainterRenderHelper.AreaVariables.TerrainUnderCursor, RaycastUtility.GetTextureCoordFromUnityTerrain(terrainPainterRenderHelper.AreaVariables.RayHit.Point), terrainPainterRenderHelper.AreaVariables.Size, 0.0f);

                brushMaterial.SetColor("_Color", AdvancedSettings.Instance.VisualisationSettings.MaskFiltersSettings.Color);
                brushMaterial.SetInt("_EnableBrushStripe", AdvancedSettings.Instance.VisualisationSettings.MaskFiltersSettings.EnableStripe ? 1 : 0);      
                brushMaterial.SetInt("_ColorSpace", (int)AdvancedSettings.Instance.VisualisationSettings.MaskFiltersSettings.ColorSpace);   
                brushMaterial.SetInt("_AlphaVisualisationType", (int)AdvancedSettings.Instance.VisualisationSettings.MaskFiltersSettings.AlphaVisualisationType);   
                brushMaterial.SetFloat("_Alpha", AdvancedSettings.Instance.VisualisationSettings.MaskFiltersSettings.CustomAlpha * multiplyAlpha);   

                TerrainPaintUtility.SetupTerrainToolMaterialProperties(heightContext, identityBrushTransform, brushMaterial);
#if UNITY_2021_2_OR_NEWER
    			TerrainPaintUtilityEditor.DrawBrushPreview(heightContext, TerrainBrushPreviewMode.SourceRenderTexture, compRT, identityBrushTransform, brushMaterial, 0);

#else
    			TerrainPaintUtilityEditor.DrawBrushPreview(heightContext, TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture, compRT, identityBrushTransform, brushMaterial, 0);
#endif
    			RenderTexture.ReleaseTemporary(compRT);
            }
    	}

        public static void DrawSimpleFilter(Group group, Vector3 originPoint, AreaVariables areaVariables, SimpleFilterComponent filterComponent, bool showPointsAroundRadius = false)
        {
            if(!AdvancedSettings.Instance.VisualisationSettings.SimpleFilterSettings.EnableSpawnVisualization)
            {
                return;
            }

            float stepIncrement = areaVariables.Size / ((float)AdvancedSettings.Instance.VisualisationSettings.SimpleFilterSettings.VisualiserResolution - 1f);

            Vector3 position = Vector3.zero;
            position.y = areaVariables.RayHit.Point.y;

            float halfSpawnRange = areaVariables.Radius;

            Bounds originBoundsSize = new Bounds(originPoint, new Vector3(areaVariables.Size, areaVariables.Size, areaVariables.Size));
            Bounds offsetBoundsSize = new Bounds(originPoint, new Vector3(halfSpawnRange * 2, halfSpawnRange * 2, halfSpawnRange * 2));

            Vector3 maxPosition = originPoint + (Vector3.one * halfSpawnRange);
            Vector3 minExtents = Vector3.zero;

            float step = areaVariables.Size  / ((float)AdvancedSettings.Instance.VisualisationSettings.SimpleFilterSettings.VisualiserResolution - 1f);

            Vector3 localPoint = position;
            Vector3 localNormal = Vector3.up;

            for (position.x = originPoint.x - halfSpawnRange; position.x < maxPosition.x; position.x += step)
            {
                for (position.z = originPoint.z - halfSpawnRange; position.z < maxPosition.z; position.z += step)
                {
                    if(showPointsAroundRadius)
                    {
                        if(Vector3.Distance(originPoint, position) > halfSpawnRange)
                        {
                            continue;
                        }
                    }
                    
                    float fitness = 0;
                    float alpha = 1;

                    float maskFitness = GrayscaleFromTexture.GetFromWorldPosition(areaVariables.Bounds, position, areaVariables.Mask);

                    fitness = GetFitnessForSpawnVisualisation(group, filterComponent, position, out localPoint, out localNormal);

                    fitness *= maskFitness;

                    alpha = AdvancedSettings.Instance.VisualisationSettings.SimpleFilterSettings.Alpha;

                    DrawHandles.DrawSpawnVisualizerPixel(new SpawnVisualizerPixel(localPoint, fitness, alpha), stepIncrement);
                }
            }
        }

        public static void DrawCircleHandles(float size, RayHit hit)
        {   
            float radius = size / 2;
            if(AdvancedSettings.Instance.VisualisationSettings.BrushHandlesSettings.DrawSolidDisc == true)
            {
                Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.1f);
                Handles.DrawSolidDisc(hit.Point, hit.Normal, radius);
            }

            DrawCircle(size, hit);
        }

        public static void DrawCircle(float size, RayHit hit)
        {
            Matrix4x4 localTransform = Matrix4x4.TRS(hit.Point, Quaternion.LookRotation(hit.Normal), new Vector3(size, size, size));

            BrushHandlesSettings brushHandlesSettings = AdvancedSettings.Instance.VisualisationSettings.BrushHandlesSettings;

            Color color = brushHandlesSettings.CircleColor;

            float thickness = brushHandlesSettings.CirclePixelWidth;
            VladislavTsurikov.Utility.DrawHandles.DrawCircleWithoutZTest(localTransform, Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.right), Vector3.one), color, thickness);
        }

        public static bool IsActiveSimpleFilter(Type toolType, SelectedVariables selectedVariables)
        {
            foreach (Group group in selectedVariables.SelectedGroupList)
            {
                FilterComponent filterComponent = (FilterComponent)group.GetSettings(toolType, typeof(FilterComponent));

                if (filterComponent == null) 
                {
                    continue;
                }
                
                if(filterComponent.FilterType == FilterType.SimpleFilter)
                {
                    return true;
                }
            }

            return false;
        }

        public static float GetFitnessForSpawnVisualisation(Group group, SimpleFilterComponent filterComponent, Vector3 checkPoint, out Vector3 point, out Vector3 normal)
        {                
            point = checkPoint;
            normal = Vector3.up;

            RaycastHit hit;

            LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;

            if (Physics.Raycast(new Ray(new Vector3(checkPoint.x, checkPoint.y + AdvancedSettings.Instance.EditorSettings.raycastSettings.Offset, checkPoint.z), Vector3.down), 
                out hit, AdvancedSettings.Instance.EditorSettings.raycastSettings.MaxRayDistance, layerSettings.GetCurrentPaintLayers(group.PrototypeType)))
    	    {
                point = hit.point;
                normal = hit.normal;

                float fitness = filterComponent.GetFitness(hit.point, hit.normal);

                return fitness;
            }

            return 0;
        }
        
        public static MaskFilterStack GetMaskFilterFromSelectedPrototype(BasicData data)
        {
            if(data.SelectedVariables.HasOneSelectedPrototype())
            {
                MaskFilterComponent maskFilterComponent = (MaskFilterComponent)data.SelectedVariables.SelectedPrototype.ComponentStack.GetComponent(typeof(MaskFilterComponent));
                return maskFilterComponent.Stack;
            }

            return null;
        }
    }
}
#endif