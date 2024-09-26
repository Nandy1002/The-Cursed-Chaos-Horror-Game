using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Filters
{
    public enum MaskOperations
    {
        Add,
        Multiply,
        Power,
        Invert,
        Clamp,
        Remap,
    }

    [Serializable]
    [Name("Mask Operations")]  
    public class MaskOperationsFilter : MaskFilter
    {
        public MaskOperations MaskOperations = MaskOperations.Remap;

        #region Clamp
        public Vector2 ClampRange = new Vector2(0, 1);
        #endregion

        #region Remap
        public Vector2 RemapRange = new Vector2(0.4f, 0.6f);
        #endregion

        #region Invert
        public float StrengthInvert = 1;
        #endregion

        public float Value;

        private static Material _builtinMaterial;
        public static Material BuiltinMaterial
        {
            get
            {
                if(_builtinMaterial == null)
                {
                    _builtinMaterial = new Material(Shader.Find("Hidden/TerrainTools/MaskOperations"));
                }

                return _builtinMaterial;
            }
        }

        public override void Eval(MaskFilterContext fc, int index)
        {
            switch (MaskOperations)
            {
                case MaskOperations.Add:
                {
                    BuiltinMaterial.SetFloat("_Add", Value);

                    Graphics.Blit( fc.SourceRenderTexture, fc.DestinationRenderTexture, BuiltinMaterial, (int)MaskOperations.Add );
                    break;
                }
                case MaskOperations.Multiply:
                {
                    BuiltinMaterial.SetFloat("_Multiply", Value);

                    Graphics.Blit( fc.SourceRenderTexture, fc.DestinationRenderTexture, BuiltinMaterial, (int)MaskOperations.Multiply );
                    break;
                }
                case MaskOperations.Power:
                {
                    BuiltinMaterial.SetFloat("_Pow", Value);

                    Graphics.Blit( fc.SourceRenderTexture, fc.DestinationRenderTexture, BuiltinMaterial, (int)MaskOperations.Power );
                    break;
                }
                case MaskOperations.Clamp:
                {
                    BuiltinMaterial.SetVector("_ClampRange", ClampRange);

                    Graphics.Blit( fc.SourceRenderTexture, fc.DestinationRenderTexture, BuiltinMaterial, (int)MaskOperations.Clamp );
                    break;
                }
                case MaskOperations.Invert:
                {
                    BuiltinMaterial.SetFloat("_Strength", StrengthInvert);

                    Graphics.Blit( fc.SourceRenderTexture, fc.DestinationRenderTexture, BuiltinMaterial, (int)MaskOperations.Invert);
                    break;
                }
                case MaskOperations.Remap:
                {
                    BuiltinMaterial.SetFloat("RemapMin", RemapRange.x);
                    BuiltinMaterial.SetFloat("RemapMax", RemapRange.y);
        
                    Graphics.Blit( fc.SourceRenderTexture, fc.DestinationRenderTexture, BuiltinMaterial, (int)MaskOperations.Remap);
                    break;
                }
            }
        }
    }
}