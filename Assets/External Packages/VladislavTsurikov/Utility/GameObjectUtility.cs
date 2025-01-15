using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.Utility
{
    public static class GameObjectUtility
    {
        public static bool IsSameGameObject(GameObject go1, GameObject go2, bool checkID = false)
        {
            if (go1 == null || go2 == null)
            {
                return false;
            }

            if (checkID)
            {
                if (go1.GetInstanceID() != go2.GetInstanceID())
                {
                    return false;
                }
                return true;
            }

            if (go1.name != go2.name)
            {
                return false;
            }

            return true;
        }
        
        public static GameObject GetPrefabRoot(GameObject gameObject)
        {
#if UNITY_EDITOR
            if(PrefabUtility.GetPrefabAssetType(gameObject) == PrefabAssetType.NotAPrefab)
            {
                return gameObject;
            }

            return PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
#else
            return gameObject;
#endif
        }

        public static Bounds GetObjectWorldBounds(GameObject gameObject)
        {
            Bounds worldBounds = new Bounds();
            bool found = false;

            ForAllInHierarchy(gameObject, (go) =>
            {
                if (!go.activeInHierarchy)
                    return;

                Renderer renderer = go.GetComponent<Renderer>();
                SkinnedMeshRenderer skinnedMeshRenderer;
                RectTransform rectTransform;

                if (renderer != null)
                {
                    if (!found)
                    {
                        worldBounds = renderer.bounds;
                        found = true;
                    }
                    else
                    {
                        worldBounds.Encapsulate(renderer.bounds);
                    }
                }
                else if ((skinnedMeshRenderer = go.GetComponent<SkinnedMeshRenderer>()) != null)
                {
                    if (!found)
                    {
                        worldBounds = skinnedMeshRenderer.bounds;
                        found = true;
                    }
                    else
                    {
                        worldBounds.Encapsulate(skinnedMeshRenderer.bounds);
                    }
                }
                else if ((rectTransform = go.GetComponent<RectTransform>()) != null)
                {
                    Vector3[] fourCorners = new Vector3[4];
                    rectTransform.GetWorldCorners(fourCorners);
                    Bounds rectBounds = new Bounds();

                    rectBounds.center = fourCorners[0];
                    rectBounds.Encapsulate(fourCorners[1]);
                    rectBounds.Encapsulate(fourCorners[2]);
                    rectBounds.Encapsulate(fourCorners[3]);

                    if (!found)
                    {
                        worldBounds = rectBounds;
                        found = true;
                    }
                    else
                    {
                        worldBounds.Encapsulate(rectBounds);
                    }
                }
             });

            if (!found)
                return new Bounds(gameObject.transform.position, Vector3.one);
            else
                return worldBounds;
        }

        public static Bounds GetBoundsFromGameObject(GameObject gameObject)
        {
            Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
            MeshFilter meshFilter = gameObject.GetComponentInChildren<MeshFilter>();
            Collider collider;

            if(renderer != null && renderer.enabled && renderer is SkinnedMeshRenderer)
            {
                return renderer.bounds;
            }
            else if (renderer != null && renderer.enabled &&
                meshFilter != null && meshFilter.sharedMesh != null)
            {
                return renderer.bounds;
            }
            else if ((collider = gameObject.GetComponent<Collider>()) != null && collider.enabled)
            {
                return collider.bounds;
            }

            return new Bounds();
        }

        public static void ForAllInHierarchy(GameObject gameObject, Action<GameObject> action)
        {
            action(gameObject);

            for (int i = 0; i < gameObject.transform.childCount; i++)
                ForAllInHierarchy(gameObject.transform.GetChild(i).gameObject, action);
        }

        public static void ParentGameObject(GameObject gameObject, GameObject parent)
        {
            if (gameObject != null && parent != null)
            {
                gameObject.transform.SetParent(parent.transform, true);
            }
        }

        public static GameObject FindParentGameObjectObject(string gameObjectName, Scene scene)
        {
            GameObject container = null;
            
            GameObject[] sceneRoots = scene.GetRootGameObjects();
			foreach(GameObject root in sceneRoots)
			{
				if(root.name == gameObjectName) 
                {
					container = root.transform.gameObject;
                    break;
				}
			} 

            if (container == null)
            {
                GameObject childObject = new GameObject(gameObjectName);
                SceneManager.MoveGameObjectToScene(childObject, scene);
                container = childObject.transform.gameObject;
            }

            return container;
        }

        public static UnityEngine.Object FindObjectOfType(Type type, Scene scene, bool fromActiveGameObject = false)
        {
            List<UnityEngine.Object> components = FindObjectsOfType(type, scene, fromActiveGameObject);

            if(components.Count != 0)
                return null;

            return components[0];
        }

        public static List<UnityEngine.Object> FindObjectsOfType(Type type, Scene scene, bool fromActiveGameObject = false)
        {
            if(!scene.isLoaded)
            {
                return new List<UnityEngine.Object>();
            }

            UnityEngine.Object[] components = Object.FindObjectsOfType(type);  

            List<UnityEngine.Object> componentList = new List<UnityEngine.Object>();

            foreach (var component in components)
            {
                Behaviour behaviour = (Behaviour)component;

                if(behaviour.GetType() == type && behaviour.gameObject.scene == scene)
                {
                    if(fromActiveGameObject)
                    {
                        if(!behaviour.gameObject.activeInHierarchy)
                            continue;
                    }

                    componentList.Add(behaviour);
                }
            }

            return componentList;
        }
    }
}