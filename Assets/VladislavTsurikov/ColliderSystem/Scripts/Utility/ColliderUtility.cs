using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.SceneDataSystem.Scripts;

namespace VladislavTsurikov.ColliderSystem.Scripts.Utility
{    
    public static class ColliderUtility
    {
        public static RayHit Raycast(Ray ray, LayerMask layerMask)
        {
            ObjectFilter raycastFilter = new ObjectFilter();
            raycastFilter.LayerMask = layerMask;

            return Raycast(ray, raycastFilter);
        }
        
        public static RayHit Raycast<T>(Ray ray, LayerMask layerMask)
            where T: RendererSceneData
        {
            ObjectFilter raycastFilter = new ObjectFilter();
            raycastFilter.LayerMask = layerMask;

            return Raycast<T>(ray, raycastFilter);
        }

        public static RayHit Raycast(Ray ray, ObjectFilter raycastFilter)
        {            
            List<RayHit> allObjectHits = RaycastAll(ray, raycastFilter);

            RayHit.SortByHitDistance(allObjectHits);
            RayHit closestObjectHit = allObjectHits.Count != 0 ? allObjectHits[0] : null;

            return closestObjectHit;
        }
        
        public static RayHit Raycast<T>(Ray ray, ObjectFilter raycastFilter)
            where T: RendererSceneData
        {            
            List<RayHit> allObjectHits = RaycastAll<T>(ray, raycastFilter);

            RayHit.SortByHitDistance(allObjectHits);
            RayHit closestObjectHit = allObjectHits.Count != 0 ? allObjectHits[0] : null;

            return closestObjectHit;
        }

        public static List<RayHit> RaycastAll<T>(Ray ray, ObjectFilter raycastFilter)
            where T: RendererSceneData
        {
            List<SceneDataManager> sceneDataManagerList = FindScene.SceneObjectsCells.RaycastAll(ray);

            List<RayHit> allObjectHits = new List<RayHit>();

            foreach (SceneDataManager sceneDataManager in sceneDataManagerList)
            {
                T rendererSceneData = (T)SceneDataManager.InstanceSceneData(typeof(T), sceneDataManager.GetScene());

                if(rendererSceneData != null)
                    allObjectHits.AddRange(rendererSceneData.RaycastAll(ray, raycastFilter));
            }

            return allObjectHits;
        }

        public static List<RayHit> RaycastAll(Ray ray, ObjectFilter raycastFilter)
        {
            List<SceneDataManager> sceneDataManagerList = FindScene.SceneObjectsCells.RaycastAll(ray);

            List<RayHit> allObjectHits = new List<RayHit>();

            foreach (SceneDataManager sceneDataManager in sceneDataManagerList)
            {
                foreach (var sceneData in sceneDataManager.SceneDataList)
                {
                    if (sceneData is RendererSceneData rendererSceneData)
                    {
                        List<RayHit> rayHits = rendererSceneData.RaycastAll(ray, raycastFilter);

                        if (rayHits != null)
                        {
                            allObjectHits.AddRange(rendererSceneData.RaycastAll(ray, raycastFilter));
                        }
                    }
                }
            }

            return allObjectHits;
        }
    }
}