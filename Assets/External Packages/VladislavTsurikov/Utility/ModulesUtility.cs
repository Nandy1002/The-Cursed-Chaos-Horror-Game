using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.Utility
{
    public static class ModulesUtility
    {
        static IEnumerable<Type> m_AssemblyTypes;
        public static IEnumerable<Type> GetAllAssemblyTypes()
        {
            if (m_AssemblyTypes == null)
            {
                m_AssemblyTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(t =>
                    {
                        // Ugly hack to handle mis-versioned dlls
                        var innerTypes = new Type[0];
                        try
                        {
                            innerTypes = t.GetTypes();
                        }
                        catch { }
                        return innerTypes;
                    });
            }

            return m_AssemblyTypes;
        }
        /// <summary>
        /// Gets all currently available assembly types derived from type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to look for</typeparam>
        /// <returns>A list of all currently available assembly types derived from type <typeparamref name="T"/></returns>
        public static IEnumerable<Type> GetAllTypesDerivedFrom<T>()
        {
#if UNITY_EDITOR && UNITY_2019_2_OR_NEWER
            return TypeCache.GetTypesDerivedFrom<T>();
#else
            return GetAllAssemblyTypes().Where(t => t.IsSubclassOf(typeof(T)));
#endif
        }

        public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr)
        {
            MemberExpression me;
            switch (expr.Body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    me = expr.Body as MemberExpression;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var members = new List<string>();
            while (me != null)
            {
                members.Add(me.Member.Name);
                me = me.Expression as MemberExpression;
            }

            var sb = new StringBuilder();
            for (int i = members.Count - 1; i >= 0; i--)
            {
                sb.Append(members[i]);
                if (i > 0) sb.Append('.');
            }

            return sb.ToString();
        }

#if UNITY_EDITOR
        public static ScriptableObject CreateAsset(Type type, Object parentAsset, bool saveAssets = true)
        {
            var newAsset = ScriptableObject.CreateInstance(type);
            newAsset.name = type.Name;

            AssetDatabase.AddObjectToAsset(newAsset, parentAsset);  

            if(saveAssets)
            {
                AssetDatabase.SaveAssets();
            } 
            
            return newAsset;
        }

        public static void RemoveAsset(ScriptableObject asset)
        {
            AssetDatabase.RemoveObjectFromAsset(asset);
            AssetDatabase.SaveAssets();
        }
        

        public static void DeleteAsset(Object asset)
        {
            string pathToDelete = AssetDatabase.GetAssetPath(asset);      
            AssetDatabase.DeleteAsset(pathToDelete);
        }
#endif

        public static T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            Type type = original.GetType();
            var dst = destination.GetComponent(type) as T;
            if (!dst) dst = destination.AddComponent(type) as T;
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(dst, field.GetValue(original));
            }
			
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanWrite || prop.Name == "name") continue;
                prop.SetValue(dst, prop.GetValue(original, null), null);
            }
            return dst as T;
        }
        
        public static void CopyObject<T>(T original, T destination) 
        {
            Type type = original.GetType();
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(destination, field.GetValue(original));
            }
            
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanWrite || prop.Name == "name") continue;
                prop.SetValue(destination, prop.GetValue(original, null), null);
            }
        }
    }
}