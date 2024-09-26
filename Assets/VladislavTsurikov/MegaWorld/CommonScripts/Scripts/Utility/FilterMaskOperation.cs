using UnityEngine;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.Utility;
#if UNITY_2021_2_OR_NEWER
using UnityEngine.TerrainTools;
#else
using UnityEngine.Experimental.TerrainAPI;
#endif

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility
{
    public static class FilterMaskOperation 
    {
        public static Texture2D UpdateMaskTexture(MaskFilterComponent maskFilterComponent, AreaVariables areaVariables)
        {
            if(areaVariables.TerrainUnderCursor == null)
            {
                return null;
            }
            
            if(maskFilterComponent.Stack.ComponentList.Count != 0)
            {
                UpdateFilterContext(ref maskFilterComponent.FilterContext, maskFilterComponent.Stack, areaVariables);
                RenderTexture filterMaskRT = maskFilterComponent.FilterContext.GetFilterMaskRT();
                maskFilterComponent.FilterMaskTexture2D = TextureUtility.ToTexture2D(filterMaskRT);
                DisposeMaskTexture(maskFilterComponent);

                return maskFilterComponent.FilterMaskTexture2D;
            }

            return null;
        }

        public static void UpdateFilterContext(ref MaskFilterContext filterContext, MaskFilterStack maskFilterStack, AreaVariables areaVariables)
        {
            if(filterContext != null)
            {
                filterContext.DisposeUnmanagedMemory();
            }

            TerrainPainterRenderHelper terrainPainterRenderHelper = new TerrainPainterRenderHelper(areaVariables);

            PaintContext heightContext = terrainPainterRenderHelper.AcquireHeightmap();
            PaintContext normalContext = terrainPainterRenderHelper.AcquireNormalmap();

            RenderTexture output = new RenderTexture(heightContext.sourceRenderTexture.width, heightContext.sourceRenderTexture.height, heightContext.sourceRenderTexture.depth, RenderTextureFormat.ARGB32);
            //RenderTexture output = new RenderTexture(heightContext.sourceRenderTexture.width, heightContext.sourceRenderTexture.height, heightContext.sourceRenderTexture.depth, GraphicsFormat.R16_SFloat);
            output.enableRandomWrite = true;
            output.Create();

            filterContext = new MaskFilterContext(maskFilterStack, heightContext, normalContext, output, terrainPainterRenderHelper.AreaVariables);
        }

        private static void DisposeMaskTexture(MaskFilterComponent maskFilterComponent)
        {
            if(maskFilterComponent.FilterContext != null)
            {
                maskFilterComponent.FilterContext.DisposeUnmanagedMemory();
                maskFilterComponent.FilterContext = null;
            }
        }
    }
}