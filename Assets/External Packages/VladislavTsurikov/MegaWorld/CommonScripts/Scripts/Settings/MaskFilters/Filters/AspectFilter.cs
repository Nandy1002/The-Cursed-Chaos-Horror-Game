using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.Utility;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Filters 
{
    [Serializable]
    [Name("Aspect")]
    public class AspectFilter : MaskFilter 
    {
        public BlendMode BlendMode = BlendMode.Multiply;
        public float Rotation = 0;
        public float Epsilon = 1.0f; //kernel size
        public float EffectStrength = 1.0f;  //overall strength of the effect
        public AnimationCurve RemapCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public Texture2D RemapTex = null;
        
        private static readonly int s_remapTexWidth = 1024;

        Texture2D GetRemapTexture() 
        {
            if (RemapTex == null) 
            {
                RemapTex = new Texture2D(s_remapTexWidth, 1, TextureFormat.RFloat, false, true);
                RemapTex.wrapMode = TextureWrapMode.Clamp;

                TextureUtility.AnimationCurveToTexture(RemapCurve, ref RemapTex);
            }
            
            return RemapTex;
        }

        //Compute Shader resource helper
        ComputeShader m_AspectCS = null;
        ComputeShader GetComputeShader() {
            if (m_AspectCS == null) {
                m_AspectCS = (ComputeShader)Resources.Load("Aspect");
            }
            return m_AspectCS;
        }

        public override void Eval(MaskFilterContext fc, int index) 
        {
            ComputeShader cs = GetComputeShader();
            int kidx = cs.FindKernel("AspectRemap");

            //using 1s here so we don't need a multiple-of-8 texture in the compute shader (probably not optimal?)
            int[] numWorkGroups = { 1, 1, 1 };

            Texture2D remapTex = GetRemapTexture();

            //float rotRad = (fc.properties["brushRotation"] - 90.0f) * Mathf.Deg2Rad;
            float rotRad = (Rotation - 90.0f) * Mathf.Deg2Rad;

            if(index == 0)
            {
                cs.SetInt("_BlendMode", (int)BlendMode.Multiply);
            }
            else
            {
                cs.SetInt("_BlendMode", (int)BlendMode);
            }

            cs.SetTexture(kidx, "In_BaseMaskTex", fc.SourceRenderTexture);
            cs.SetTexture(kidx, "In_HeightTex", fc.HeightContext.sourceRenderTexture);
            cs.SetTexture(kidx, "OutputTex", fc.DestinationRenderTexture);
            cs.SetTexture(kidx, "RemapTex", remapTex);
            cs.SetInt("RemapTexRes", remapTex.width);
            cs.SetFloat("EffectStrength", EffectStrength);
            cs.SetVector("TextureResolution", new Vector4(fc.SourceRenderTexture.width, fc.SourceRenderTexture.height, 0.0f, 0.0f));
            cs.SetVector("AspectValues", new Vector4(Mathf.Cos(rotRad), Mathf.Sin(rotRad), Epsilon, 0.0f));

            cs.Dispatch(kidx, fc.SourceRenderTexture.width / numWorkGroups[0], fc.SourceRenderTexture.height / numWorkGroups[1], numWorkGroups[2]);
        }
    }
}