using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using VladislavTsurikov.SceneDataSystem.Scripts.Attributes;
using VladislavTsurikov.Utility;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.SceneDataSystem.Scripts
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class RequiredSceneData
    {
        public delegate void CreateDelegate(SceneDataManager sceneDataManager);
        public static CreateDelegate CreateEvent;
        
        public static List<Type> RequiredTypeList = new List<Type>();

        static RequiredSceneData()
        {
            var types = ModulesUtility.GetAllTypesDerivedFrom<SceneData>()
                                .Where(t => t.IsDefined(typeof(RequiredSceneDataAttribute), false)); 

            foreach (Type type in types)
            {
                RequiredTypeList.Add(type);
            }
        }

        public static void AddRequiredType(Type type)
        {
            RequiredTypeList.Add(type);
            Create();
        }

        public static void Create()
        {
            foreach (SceneDataManager sceneDataManager in SceneDataManagerStack.GetAllSceneDataManager())
            {
                foreach (Type type in RequiredTypeList)
                {
                    if(sceneDataManager.GetSceneData(type) == null)
                    {
                        sceneDataManager.AddSceneDataIfNecessary(type);
                    }
                }

                if(CreateEvent != null)
                {
                    CreateEvent(sceneDataManager);
                }
            }
        }

        public static void Create(Scene scene)
        {
            SceneDataManager sceneDataManager = SceneDataManagerStack.GetSceneDataManager(scene);
            foreach (Type type in RequiredTypeList)
            {
                if(sceneDataManager.GetSceneData(type) == null)
                {
                    sceneDataManager.AddSceneDataIfNecessary(type);
                }
            }

            if(CreateEvent != null) 
            {
                CreateEvent(sceneDataManager);
            }
        }
    }
}