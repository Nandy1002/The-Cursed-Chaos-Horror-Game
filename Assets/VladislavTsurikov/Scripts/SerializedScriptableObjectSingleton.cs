using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.OdinSerializer.Unity_Integration.SerializedUnityObjects;

namespace VladislavTsurikov.Scripts
{
    public class SerializedScriptableObjectSingleton<T> : SerializedScriptableObject where T : SerializedScriptableObject
    {
        static T s_Instance;
        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = GetPackage();
                return s_Instance;
            }
        }

        private static T GetPackage()
        {
            LocationAttribute locationAttribute = GetCustomAttributes();

            if (locationAttribute == null)
            {
                Debug.LogError("Location Attribute missing!");  
                return null;
            }
            
            T scriptableObject = Resources.Load<T>(locationAttribute.RelativePath);

            if (scriptableObject == null)
            {
                scriptableObject = CreateInstance<T>();
#if UNITY_EDITOR 
                if (!Application.isPlaying)
                {
                    var locationFilePath = locationAttribute.FilePath;
                    var directoryName = Path.GetDirectoryName(locationFilePath);
                    if (directoryName == null) return scriptableObject;
                    Directory.CreateDirectory(directoryName);

                    AssetDatabase.CreateAsset(scriptableObject, locationAttribute.FilePath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
#endif
            }

            return scriptableObject;
        }

        [CanBeNull]
        static LocationAttribute GetCustomAttributes()
        {
            return (LocationAttribute)typeof(T).GetCustomAttribute(typeof(LocationAttribute));
        }
    }
}