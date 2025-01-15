using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.Extensions.Scripts;
using VladislavTsurikov.Utility;

namespace VladislavTsurikov.ColliderSystem.Scripts.Scene
{
    public class ObjectFilter
    {
        public int LayerMask = ~0;
        public List<System.Object> IgnoreObjects = new List<System.Object>();
        public List<GameObject> FindPrefabs = new List<GameObject>();
        public bool FindOnlySpecificInstancePrefabs = false;
        public bool FindOnlyInstancePrefabs = false;

        public void ClearIgnoreObjects()
        {
            IgnoreObjects.Clear();
        }

        public void SetIgnoreObjects(List<System.Object> ignoreObjects)
        {
            if (ignoreObjects == null) return;
            IgnoreObjects = new List<System.Object>(ignoreObjects);
        }

        public void SetFindPrefabs(List<GameObject> prefabs)
        {
            FindOnlySpecificInstancePrefabs = true;

            if (prefabs == null) return;
            FindPrefabs = new List<GameObject>(prefabs);
        }

        public bool IsObjectIgnored(System.Object obj)
        {
            return IgnoreObjects.Contains(obj);
        }

        public bool Filter(int layer, GameObject prefab, object obj)
        {
            if(!LayerEx.IsLayerBitSet(LayerMask, layer) || IgnoreObjects.Contains(obj))
            {
                return false;
            }

            if(FindOnlyInstancePrefabs)
            {
                if(prefab == null)
                {
                    return false;
                }
            }
            
            if(FindOnlySpecificInstancePrefabs)
            {
                if(FindPrefabs.Count == 0 || prefab == null)
                {
                    return false;
                }
                
                foreach (GameObject findPrefab in FindPrefabs)
                {
                    if(GameObjectUtility.IsSameGameObject(findPrefab, prefab))
                    {
                        return true;
                    }
                }
                
                return false;
            }
            
            return true;
        }

        public bool Filter(ColliderObject colliderObject)
        {
            if(colliderObject == null || colliderObject.Obj == null)
            {
                return false;
            }
            
            if(!LayerEx.IsLayerBitSet(LayerMask, colliderObject.GetLayer()) || IgnoreObjects.Contains(colliderObject.Obj))
            {
                return false;
            }

            if(FindOnlyInstancePrefabs)
            {
                if(colliderObject.GetPrefab() == null)
                {
                    return false;
                }
            }
            
            if(FindOnlySpecificInstancePrefabs)
            {
                if(FindPrefabs.Count == 0 || colliderObject.GetPrefab() == null)
                {
                    return false;
                }
                
                foreach (GameObject prefab in FindPrefabs)
                {
                    if(GameObjectUtility.IsSameGameObject(prefab, colliderObject.GetPrefab()))
                    {
                        return true;
                    }
                }
                
                return false;
            }
            
            return true;
        }
    }
}