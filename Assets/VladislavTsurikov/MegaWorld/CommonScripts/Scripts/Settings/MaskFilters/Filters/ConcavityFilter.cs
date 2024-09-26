using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.Utility;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Filters 
{
    [Serializable]
    [Name("Concavity")]
    public class ConcavityFilter : MaskFilter 
    {
        public enum ConcavityMode 
        {
            Recessed = 0,
            Exposed = 1
        }

        public float ConcavityEpsilon = 1.0f; //kernel size
        public float ConcavityScalar = 1.0f;  //toggles the compute shader between recessed (1.0f) & exposed (-1.0f) surfaces
        public float ConcavityStrength = 1.0f;  //overall strength of the effect
        public AnimationCurve ConcavityRemapCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public Texture2D ConcavityRemapTex = null;
        
        readonly private int _remapTexWidth = 1024;

        Texture2D GetRemapTexture() 
        {
            if (ConcavityRemapTex == null) 
            {
                ConcavityRemapTex = new Texture2D(_remapTexWidth, 1, TextureFormat.RFloat, false, true);
                ConcavityRemapTex.wrapMode = TextureWrapMode.Clamp;

                TextureUtility.AnimationCurveToTexture(ConcavityRemapCurve, ref ConcavityRemapTex);
            }
            
            return ConcavityRemapTex;
        }

        //Compute Shader resource helper
        ComputeShader _concavityCS = null;
        ComputeShader GetComputeShader() 
        {
            if (_concavityCS == null) 
            {
                _concavityCS = (ComputeShader)Resources.Load("Concavity");
            }
            return _concavityCS;
        }

        public override void Eval(MaskFilterContext fc, int index)
        {
            ComputeShader cs = GetComputeShader();
            int kidx = cs.FindKernel("ConcavityMultiply");

            Texture2D remapTex = GetRemapTexture();

            cs.SetTexture(kidx, "In_BaseMaskTex", fc.SourceRenderTexture);
            cs.SetTexture(kidx, "In_HeightTex", fc.HeightContext.sourceRenderTexture);
            cs.SetTexture(kidx, "OutputTex", fc.DestinationRenderTexture);
            cs.SetTexture(kidx, "RemapTex", remapTex);
            cs.SetInt("RemapTexRes", remapTex.width);
            cs.SetFloat("EffectStrength", ConcavityStrength);
            cs.SetVector("TextureResolution", new Vector4(fc.SourceRenderTexture.width, fc.SourceRenderTexture.height, ConcavityEpsilon, ConcavityScalar));

            //using 1s here so we don't need a multiple-of-8 texture in the compute shader (probably not optimal?)
            cs.Dispatch(kidx, fc.SourceRenderTexture.width, fc.SourceRenderTexture.height, 1);
        }
    }
}
