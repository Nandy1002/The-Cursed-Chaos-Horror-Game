#if UNITY_EDITOR
#endif
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.MegaWorld.Core.Scripts;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
#if UNITY_2021_2_OR_NEWER
using UnityEngine.TerrainTools;
#else
using UnityEngine.Experimental.TerrainAPI;
#endif

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Filters 
{
    [Serializable]
    [Name("Textures")]
    public class TexturesFilter : MaskFilter 
    {
        public BlendMode BlendMode = BlendMode.Multiply;
        public List<TerrainTexture> TerrainTextureList = new List<TerrainTexture>();
        
        [OnDeserializing]
        public void Init()
        {
            if (TerrainTextureList == null)
            {
                TerrainTextureList = new List<TerrainTexture>();
            }
        }

        public override void Eval(MaskFilterContext fc, int index) 
        {
            if(Terrain.activeTerrain == null)
                return;

            Vector2 currUV = CommonUtility.WorldPointToUV(fc.AreaVariables.RayHit.Point, fc.AreaVariables.TerrainUnderCursor);

            BrushTransform brushTransform = TerrainPaintUtility.CalculateBrushTransform(fc.AreaVariables.TerrainUnderCursor, currUV, fc.AreaVariables.Size, 0.0f);
            Rect brushRect = brushTransform.GetBrushXYBounds();

            List<TerrainTexture> addTexturesToRenderTextureList = new List<TerrainTexture>();

            if(IsSyncTerrain(Terrain.activeTerrain) == false)
			{
				UpdateCheckTerrainTextures(Terrain.activeTerrain);
			}

            for (int i = 0; i < TerrainTextureList.Count; i++)
            {
                if(TerrainTextureList[i].selected)
                {
                    addTexturesToRenderTextureList.Add(TerrainTextureList[i]);
                }
            }

            Material blendMat = MaskFilterUtility.blendModesMaterial;

            RenderTexture output = RenderTexture.GetTemporary(fc.SourceRenderTexture.width, fc.SourceRenderTexture.height, fc.SourceRenderTexture.depth, GraphicsFormat.R16_SFloat);
            output.enableRandomWrite = true;

            for (int i = 0; i < addTexturesToRenderTextureList.Count; i++)
            {
                RenderTexture localSourceRender = RenderTexture.GetTemporary(fc.SourceRenderTexture.width, fc.SourceRenderTexture.height, 1, RenderTextureFormat.ARGB32);
                localSourceRender.enableRandomWrite = true;

                PaintContext localTextureContext = TerrainPaintUtility.BeginPaintTexture(fc.AreaVariables.TerrainUnderCursor, brushRect, Terrain.activeTerrain.terrainData.terrainLayers[addTexturesToRenderTextureList[i].terrainProtoId]);

                blendMat.SetInt("_BlendMode", (int)BlendMode.Add);
                blendMat.SetTexture("_MainTex", output);
                blendMat.SetTexture("_BlendTex", localTextureContext.sourceRenderTexture);

                Graphics.Blit(output, localSourceRender, blendMat, 0);
                Graphics.Blit(localSourceRender, output);

                TerrainPaintUtility.ReleaseContextResources(localTextureContext); 

                RenderTexture.ReleaseTemporary(localSourceRender);
            }

            RenderTexture sourceRender = RenderTexture.GetTemporary(fc.SourceRenderTexture.width, fc.SourceRenderTexture.height, 1, RenderTextureFormat.ARGB32);
            sourceRender.enableRandomWrite = true;

            if(index == 0)
            {
                blendMat.SetInt("_BlendMode", (int)BlendMode.Multiply);
            }
            else
            {
                blendMat.SetInt("_BlendMode", (int)BlendMode);
            }

            blendMat.SetTexture("_MainTex", output);
            blendMat.SetTexture("_BlendTex", fc.SourceRenderTexture);

            Graphics.Blit(output, fc.DestinationRenderTexture, blendMat, 0);

            RenderTexture.ReleaseTemporary(output);
            RenderTexture.ReleaseTemporary(sourceRender);
        }

        public bool IsSyncTerrain(Terrain terrain)
        {
            for (int Id = 0; Id < terrain.terrainData.terrainLayers.Length; Id++)
            {
                if(terrain.terrainData.terrainLayers[Id] == null)
                    continue;
                
                bool find = false;

                for (int i = 0; i < TerrainTextureList.Count; i++)
                {
                    if (CommonUtility.IsSameTexture(terrain.terrainData.terrainLayers[Id].diffuseTexture, TerrainTextureList[i].texture, false))
                    {
                        find = true;
                        break;
                    }
                }

                if(find == false)
                {
                    return false;
                }
            }

            return true;
        }

        public void UpdateCheckTerrainTextures(Terrain activeTerrain)
        {
            if (activeTerrain == null)
            {
                Debug.LogWarning("Can not update prototypes from the terrain as there is no terrain currently active in this scene.");
                return;
            }

            int idx;

            TerrainTexture checkTerrainTextures;

            TerrainTextureList.Clear();
            
            TerrainLayer[] terrainLayers = activeTerrain.terrainData.terrainLayers;

            for (idx = 0; idx < terrainLayers.Length; idx++)
            {
                if(terrainLayers[idx] == null)
                    continue;
                
                checkTerrainTextures = new TerrainTexture();
                checkTerrainTextures.texture = terrainLayers[idx].diffuseTexture;
                checkTerrainTextures.terrainProtoId = idx;

                TerrainTextureList.Add(checkTerrainTextures);
            }
        }
    }
}
