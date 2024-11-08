#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.SceneDataSystem.Scripts;

namespace VladislavTsurikov.ColliderSystem.Scripts.UnityIntegration.Utility
{
    public class GameObjectColliderUtility
    {
        public static List<ColliderObject> OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, ObjectFilter objectFilter, bool checkOBBIntersection = false)
        {
            var sceneDataManagerList = FindScene.SceneObjectsCells.OverlapBox(boxCenter, boxSize, boxRotation);

            List<ColliderObject> overlappedObjects = new List<ColliderObject>();

            foreach(var node in sceneDataManagerList)
            {
                GameObjectCollider gameObjectCollider = (GameObjectCollider)SceneDataManager.InstanceSceneData(typeof(GameObjectCollider), node.GetScene());

                if(gameObjectCollider != null)
                    overlappedObjects.AddRange(gameObjectCollider.SceneObjectTree.OverlapBox(boxCenter, boxSize, boxRotation, objectFilter, checkOBBIntersection));
            }

            return overlappedObjects;
        }

        public static List<ColliderObject> OverlapSphere(Vector3 sphereCenter, float sphereRadius, ObjectFilter objectFilter, bool checkOBBIntersection = false)
        {
            var sceneDataManagerList = FindScene.SceneObjectsCells.OverlapSphere(sphereCenter, sphereRadius);

            List<ColliderObject> overlappedObjects = new List<ColliderObject>();
            foreach(var node in sceneDataManagerList)
            {
                GameObjectCollider gameObjectCollider = (GameObjectCollider)SceneDataManager.InstanceSceneData(typeof(GameObjectCollider), node.GetScene());

                if(gameObjectCollider != null)
                    overlappedObjects.AddRange(gameObjectCollider.SceneObjectTree.OverlapSphere(sphereCenter, sphereRadius, objectFilter, checkOBBIntersection));
            }

            return overlappedObjects;
        }
    }
}
#endif