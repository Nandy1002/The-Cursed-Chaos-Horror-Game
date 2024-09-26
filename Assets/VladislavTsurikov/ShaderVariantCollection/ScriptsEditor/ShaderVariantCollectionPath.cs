#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.ShaderVariantCollection.ScriptsEditor
{
    public static class ShaderVariantCollectionPath
    {        
        private static UnityEngine.ShaderVariantCollection _shaderVariantCollection;
        public static UnityEngine.ShaderVariantCollection ShaderVariantCollection
        {
            get
            {
                if (_shaderVariantCollection == null) _shaderVariantCollection = GetPackage();
                return _shaderVariantCollection;
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if (_shaderVariantCollection == null) _shaderVariantCollection = GetPackage();
        }

        public static UnityEngine.ShaderVariantCollection GetPackage()
        {
            UnityEngine.ShaderVariantCollection shaderVariantCollection = Resources.Load<UnityEngine.ShaderVariantCollection>(CommonPath.ShaderVariantCollectionName); 

            if (shaderVariantCollection == null)
            {
                shaderVariantCollection = new UnityEngine.ShaderVariantCollection();

                if (!Application.isPlaying)
                {
                    if (!System.IO.Directory.Exists(CommonPath.PathToResources))
                    {
                        System.IO.Directory.CreateDirectory(CommonPath.PathToResources);
                    }

                    AssetDatabase.CreateAsset(shaderVariantCollection, CommonPath.PathToShaderVariantCollection + ".shadervariants");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }

            return shaderVariantCollection;
        }
    }
}
#endif