using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.SceneDataSystem.Scripts;
using GameObjectUtility = VladislavTsurikov.Utility.GameObjectUtility;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.Utility
{
    public static class PlacedObjectUtility
    {
        public static PlacedObject PlaceObject(GameObject prefab, Vector3 position, Vector3 scaleFactor, Quaternion rotation)
        {
            GameObject go;

#if UNITY_EDITOR
            go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
#else
            go = MonoBehaviour.Instantiate(prefab);
#endif

            SceneDataManager sceneDataManager = FindScene.StreamingCells.OverlapPosition(position)[0];
            SceneManager.MoveGameObjectToScene(go, sceneDataManager.GetScene());

            go.transform.position = position;
            go.transform.localScale = scaleFactor;
            go.transform.rotation = rotation;

            PlacedObject objectInfo = new PlacedObject(go, GameObjectUtility.GetObjectWorldBounds(go), sceneDataManager.GetScene());
            objectInfo.GameObject = go;

            return objectInfo;
        }

        public static void FindTypeParentObject(Group group, Scene scene)
        {
            string groupName = group.name;

            group.ContainerForGameObjects.Add(scene, GameObjectUtility.FindParentGameObjectObject(groupName, scene));
        }

        public static void ParentGameObject(Group group, PlacedObject placedObject)
        {            
            if(!group.ContainerForGameObjects.ContainsKey(placedObject.Scene))
            {
                FindTypeParentObject(group, placedObject.Scene);
            }

            GameObjectUtility.ParentGameObject(placedObject.GameObject, group.ContainerForGameObjects[placedObject.Scene]);
        }
    }
}