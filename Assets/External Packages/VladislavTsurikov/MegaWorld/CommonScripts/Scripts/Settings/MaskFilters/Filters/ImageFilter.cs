using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Filters 
{
    public enum FilterMode {GrayScale, Color, RedColorChannel, GreenColorChannel, BlueColorChannel, AlphaChannel }

    [Serializable]
    [Name("Image")]
    public class ImageFilter : MaskFilter 
    {
        public BlendMode BlendMode = BlendMode.Multiply;
        public TextureFormat[] Valid16BitFormats = new TextureFormat[10] {TextureFormat.ARGB4444, TextureFormat.R16, TextureFormat.RFloat, TextureFormat.RGB565, TextureFormat.RGBA4444, TextureFormat.RGBAFloat, TextureFormat.RGBAHalf, TextureFormat.RGFloat, TextureFormat.RGHalf, TextureFormat.RHalf};

        [SerializeField]
        private Texture2D _image;
        public Texture2D Image
        {        
            get
            {
                if (_image == null)
                {
                    if (!string.IsNullOrEmpty(_imageTextureGUID))
                    {
#if UNITY_EDITOR
                    _image = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(_imageTextureGUID), typeof(Texture2D));
#endif
                    }
                }
                return _image;
            }
            set
            {
                if (value != _image)
                {
                    _image = value;
                    if (value != null)
                    {
#if UNITY_EDITOR
                    _imageTextureGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
#endif
                    }
                    else
                    {
                        _imageTextureGUID = "";
                    }
                }
            }
        }

        [SerializeField]
        private string _imageTextureGUID;
        
        public FilterMode FilterMode;
        public Color Color = Color.white;
        public float ColorAccuracy = 0.5f;

        public float OffSetX = 0f;
        public float OffSetZ = 0f;

        private Material _imageMat = null;
        Material GetMaterial() 
        {
            if (_imageMat == null) 
            {
                _imageMat = new Material( Shader.Find( "Hidden/MegaWorld/Image"));
            }
            return _imageMat;
        }

        public override void Eval(MaskFilterContext fc, int index) 
        {
            Material mat = GetMaterial();

            mat.SetTexture("_InputTex", fc.SourceRenderTexture);

            SetMaterial(mat, index);

            Graphics.Blit(fc.SourceRenderTexture, fc.DestinationRenderTexture, mat, 0);
        }

        public void SetMaterial(Material mat, int index)
        {
            if(index == 0)
            {
                mat.SetInt("_BlendMode", (int)BlendMode.Multiply);
            }
            else
            {
                mat.SetInt("_BlendMode", (int)BlendMode);
            }

            mat.SetTexture("_ImageMaskTex", Image);
            mat.SetInt("_FilterMode", (int)FilterMode);
            mat.SetColor("_Color", Color);
            mat.SetFloat("_ColorAccuracy", ColorAccuracy);
            mat.SetFloat("_XOffset", OffSetX);
            mat.SetFloat("_ZOffset", OffSetZ);
        }
    }
}
