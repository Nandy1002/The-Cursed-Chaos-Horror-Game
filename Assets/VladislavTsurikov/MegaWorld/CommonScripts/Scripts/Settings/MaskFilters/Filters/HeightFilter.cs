using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Filters 
{
    [Serializable]
    [Name("Height")]
    public class HeightFilter : MaskFilter 
    {
        public BlendMode BlendMode = BlendMode.Multiply;

        public float MinHeight = 0;
        public float MaxHeight = 0;

        public FalloffType HeightFalloffType = FalloffType.Add;
        public bool HeightFalloffMinMax = false;

        public float MinAddHeightFalloff = 30;
        public float MaxAddHeightFalloff = 30;

        [Min(0)]
        public float AddHeightFalloff = 30;

        private ComputeShader _heightCS = null;
        public ComputeShader GetComputeShader() 
        {
            if (_heightCS == null) {
                _heightCS = (ComputeShader)Resources.Load("Height");
            }
            return _heightCS;
        }

        public override void Eval(MaskFilterContext fc, int index) 
        {
            ComputeShader cs = GetComputeShader();
            int kidx = cs.FindKernel("Height");

            cs.SetTexture(kidx, "In_BaseMaskTex", fc.SourceRenderTexture);
            cs.SetTexture(kidx, "In_HeightTex", fc.HeightContext.sourceRenderTexture);
            cs.SetTexture(kidx, "OutputTex", fc.DestinationRenderTexture);

            SetMaterial(cs, fc, index);

            //using workgroup size of 1 here to avoid needing to resize render textures
            cs.Dispatch(kidx, fc.SourceRenderTexture.width, fc.SourceRenderTexture.height, 1);
        }

        public void SetMaterial(ComputeShader cs, MaskFilterContext fс, int index)
        {
            if(index == 0)
            {
                cs.SetInt("_BlendMode", (int)BlendMode.Multiply);
            }
            else
            {
                cs.SetInt("_BlendMode", (int)BlendMode);
            }

            cs.SetFloat("_MinHeight", MinHeight);
            cs.SetFloat("_MaxHeight", MaxHeight);

            cs.SetFloat("_ClampMinHeight", fс.AreaVariables.TerrainUnderCursor.transform.position.y);
            cs.SetFloat("_ClampMaxHeight", fс.AreaVariables.TerrainUnderCursor.terrainData.size.y + fс.AreaVariables.TerrainUnderCursor.transform.position.y);

            switch (HeightFalloffType)
            {
                case FalloffType.Add:
                {
                    float localMinAddHeightFalloff = AddHeightFalloff;
                    float localMaxAddHeightFalloff = AddHeightFalloff;

                    if(HeightFalloffMinMax)
                    {
                        localMinAddHeightFalloff = MinAddHeightFalloff;
                        localMaxAddHeightFalloff = MaxAddHeightFalloff;
                    }

                    cs.SetInt("_HeightFalloffType", 1);
                    cs.SetFloat("_MinAddHeightFalloff", localMinAddHeightFalloff);
                    cs.SetFloat("_MaxAddHeightFalloff", localMaxAddHeightFalloff);
                    break;
                }
                default:
                {
                    cs.SetInt("_HeightFalloffType", 0);
                    break;
                }   
            }
        }
    }
}
