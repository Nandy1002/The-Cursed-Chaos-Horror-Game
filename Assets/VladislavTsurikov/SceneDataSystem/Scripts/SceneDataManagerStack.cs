using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.SceneDataSystem.Scripts
{
    public static class SceneDataManagerStack 
    {
        private static Dictionary<Scene, SceneDataManager> s_sceneDataManagers = new Dictionary<Scene, SceneDataManager>();

        public static void InstanceAllSceneDataManager()
        {
            foreach (Scene scene in SceneManagement.GetAllScenes()) 
            {
                if(!scene.isLoaded) continue;
                
                InstanceSceneDataManager(scene);   
            }
        }

        public static void RefreshAllSceneDataManager()
        {
            FindScene.ClearSceneCells();
            s_sceneDataManagers.Clear();
            InstanceAllSceneDataManager();
        }

        public static SceneDataManager InstanceSceneDataManager(Scene scene)
        {
            SceneDataManager sceneDataManager = GetSceneDataManager(scene);

            if(sceneDataManager == null)
            {
                GameObject pluginObject = new GameObject("Scene Data Manager");
                pluginObject.hideFlags = HideFlags.HideInHierarchy;
                SceneManager.MoveGameObjectToScene(pluginObject, scene);

                sceneDataManager = pluginObject.AddComponent<SceneDataManager>();
            }

            AddSceneDataManager(sceneDataManager);

            return sceneDataManager;
        }

        public static void AddSceneDataManager(SceneDataManager sceneDataManager)
        {
            if(!sceneDataManager.gameObject.activeInHierarchy)
                return;
            
            if(!s_sceneDataManagers.ContainsKey(sceneDataManager.GetScene()))
            {
                s_sceneDataManagers.Add(sceneDataManager.GetScene(), sceneDataManager);
                sceneDataManager.Setup();
                FindScene.AddSceneCell(sceneDataManager);
            }
        }

        public static void RemoveSceneDataManager(Scene scene)
        {
            if(s_sceneDataManagers.ContainsKey(scene))
            {
                FindScene.RemoveSceneCell(s_sceneDataManagers[scene]);
                s_sceneDataManagers.Remove(scene);
            }
        }

        public static SceneDataManager GetSceneDataManager(Scene scene)
        {
            if(s_sceneDataManagers.ContainsKey(scene))
            {
                return s_sceneDataManagers[scene];
            }

            return FindSceneDataManager(scene);
        }

        public static List<SceneDataManager> GetAllSceneDataManager()
        {
            List<SceneDataManager> sceneDataManagers = new List<SceneDataManager>();

            foreach (var item in s_sceneDataManagers)
            {
                sceneDataManagers.Add(item.Value);
            }

            return sceneDataManagers;
        }

        //The Object.FindObjectOfType method does not allow me to find a component with a hidden GameObject, but this method allows me to
        private static SceneDataManager FindSceneDataManager(Scene scene)
        {
            GameObject[] gameObjects = scene.GetRootGameObjects();

            foreach (GameObject go in gameObjects)
            {
                Object obj = go.GetComponentInChildren(typeof(SceneDataManager));
                if(obj != null)
                {
                    return (SceneDataManager)obj;
                }
                
            }

            return null;
        }
    }
}