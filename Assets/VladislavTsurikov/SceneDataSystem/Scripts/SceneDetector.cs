using UnityEditor;
using UnityEngine.SceneManagement;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.SceneDataSystem.Scripts
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    
    public static class SceneDetector 
    {
        static SceneDetector()
        {
#if UNITY_EDITOR
            SceneManagement.SceneLoadedOrUnloaded -= SceneLoadedOrUnloaded;
            SceneManagement.SceneLoadedOrUnloaded += SceneLoadedOrUnloaded;
#else
            SceneManagement.SceneLoaded -= OnSceneLoaded;
            SceneManagement.SceneLoaded += OnSceneLoaded;

            SceneManagement.SceneUnloaded -= OnSceneUnloaded;
            SceneManagement.SceneUnloaded += OnSceneUnloaded;
#endif
        }

        private static void SceneLoadedOrUnloaded()
        {
            SceneDataManagerStack.RefreshAllSceneDataManager();
            RequiredSceneData.Create();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneDataManagerStack.InstanceSceneDataManager(scene);
            RequiredSceneData.Create(scene);
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            SceneDataManagerStack.RemoveSceneDataManager(scene);
        }
    }
}