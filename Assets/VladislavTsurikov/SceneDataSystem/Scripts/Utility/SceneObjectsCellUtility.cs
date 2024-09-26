using UnityEngine;
using VladislavTsurikov.BVH.Scripts.Math;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace VladislavTsurikov.SceneDataSystem.Scripts.Utility
{
    public static class SceneObjectsCellUtility
    {
        public static void ChangeSceneObjectsCell(SceneDataManager sceneDataManager)
        {
            FindScene.SceneObjectsCells.ChangeNodeSize(sceneDataManager, GetSceneObjectsCell(sceneDataManager));

#if UNITY_EDITOR
            if(!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(sceneDataManager.GetScene());
            }
#endif
        }

        public static AABB GetSceneObjectsCell(SceneDataManager sceneDataManager)
        {
            AABB currentAABB = sceneDataManager.StreamingCell;

            foreach (SceneData sceneData in sceneDataManager.SceneDataList)
            {
                if (sceneData is RendererSceneData rendererSceneData)
                {
                    currentAABB.Encapsulate(rendererSceneData.GetAABB()); 
                }
            }

            return currentAABB;
        }
    }
}