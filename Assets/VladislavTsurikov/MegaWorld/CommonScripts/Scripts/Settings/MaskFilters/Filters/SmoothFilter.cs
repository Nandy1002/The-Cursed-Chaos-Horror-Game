using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Filters 
{
    [Serializable]
    [Name("Smooth")]
    public class SmoothFilter : MaskFilter 
    {
        public float SmoothVerticality = 0f;
        public float SmoothBlurRadius = 1f;

        private static Material _material;
        public static Material Material
        {
            get
            {
                if( _material == null )
                {
                    _material = new Material( Shader.Find("Hidden/MegaWorld/SmoothHeight") );
                }

                return _material;
            }
        }
        
        public override void Eval(MaskFilterContext fc, int index)
        {
            Material filterMat = Material;

            Vector4 brushParams = new Vector4(Mathf.Clamp(1, 0.0f, 1.0f), 0.0f, 0.0f, 0.0f);
            filterMat.SetTexture("_MainTex", fc.SourceRenderTexture);
            filterMat.SetTexture("_BrushTex", Texture2D.whiteTexture);
            filterMat.SetVector("_BrushParams", brushParams);
            Vector4 smoothWeights = new Vector4(
                Mathf.Clamp01(1.0f - Mathf.Abs(SmoothVerticality)),   // centered
                Mathf.Clamp01(-SmoothVerticality),                    // min
                Mathf.Clamp01(SmoothVerticality),                     // max
                SmoothBlurRadius);                                  // kernel size
            filterMat.SetVector("_SmoothWeights", smoothWeights);

            RenderTexture tmpRT = RenderTexture.GetTemporary(fc.DestinationRenderTexture.descriptor);
            Graphics.Blit(fc.SourceRenderTexture, tmpRT, filterMat, 0);
            Graphics.Blit(tmpRT, fc.DestinationRenderTexture, filterMat, 1);

            RenderTexture.ReleaseTemporary(tmpRT);
        }
    }
}

