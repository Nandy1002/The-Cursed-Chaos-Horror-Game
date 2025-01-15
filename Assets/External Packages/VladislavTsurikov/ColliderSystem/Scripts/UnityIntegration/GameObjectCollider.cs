#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.BVH.Scripts;
using VladislavTsurikov.BVH.Scripts.Math;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.Extensions.Scripts;
using VladislavTsurikov.SceneDataSystem.Scripts;
using VladislavTsurikov.SceneDataSystem.Scripts.Attributes;
using VladislavTsurikov.SceneDataSystem.Scripts.Utility;
using GameObjectUtility = VladislavTsurikov.Utility.GameObjectUtility;

namespace VladislavTsurikov.ColliderSystem.Scripts.UnityIntegration
{
    [RequiredSceneData]
    public class GameObjectCollider : RendererSceneData
    {
        private Dictionary<BVHGameObject, BVHNodeAABB<BVHGameObject>> _leafNodes = new Dictionary<BVHGameObject, BVHNodeAABB<BVHGameObject>>();

        public BVHObjectTree<BVHGameObject> SceneObjectTree = new BVHObjectTree<BVHGameObject>();

        protected override void Setup()
        {
            RefreshObjectTree();
        }

        public override AABB GetAABB()
        {
            return SceneObjectTree.Tree.GetAABB();
        }

        public override List<RayHit> RaycastAll(Ray ray, ObjectFilter objectFilter)
        {
            return SceneObjectTree.RaycastAll(ray, objectFilter);
        }

        public GameObject[] GetSceneObjects()
        {
            UnityEngine.SceneManagement.Scene scene = SceneDataManager.GetScene();
            
            var roots = new List<GameObject>(Mathf.Max(1, scene.rootCount));
            scene.GetRootGameObjects(roots);
            List<GameObject> sceneObjects = new List<GameObject>(Mathf.Max(1, scene.rootCount * 5));

            foreach (var root in roots)
            {
                var allChildrenAndSelf = root.GetAllChildrenAndSelf();
                sceneObjects.AddRange(allChildrenAndSelf);
            }

            return sceneObjects.ToArray();
        }    

        public void RefreshObjectTree()
        {
            SceneObjectTree = new BVHObjectTree<BVHGameObject>();
            _leafNodes = new Dictionary<BVHGameObject, BVHNodeAABB<BVHGameObject>>();
            RegisterGameObjects(GetSceneObjects());
        }

        public void RegisterGameObjects(IEnumerable<GameObject> gameObjects)
        {
            foreach (GameObject go in gameObjects)
            {
                RegisterGameObject(go, this);
            }
        }

        public static void RegisterGameObjectToCurrentScene(GameObject instanceGameObject)
        {
            if(instanceGameObject == null) return;

            ChangeGameObjectSceneIfNecessary(instanceGameObject);

            SceneDataManager sceneDataManager = FindScene.StreamingCells.OverlapPosition(instanceGameObject.transform.position)[0];

            GameObjectCollider gameObjectCollider = (GameObjectCollider)sceneDataManager.GetSceneData(typeof(GameObjectCollider));

            List<GameObject> allChildrenIncludingSelf = instanceGameObject.GetAllChildrenAndSelf();

            if(gameObjectCollider == null)
            {
                SceneDataManager.InstanceSceneData(typeof(GameObjectCollider), sceneDataManager.GetScene());
            }
            else
            {
                foreach (GameObject go in allChildrenIncludingSelf)
                {
                    RegisterGameObject(go, gameObjectCollider);
                }
            }
        }

        private static void RegisterGameObject(GameObject gameObject, GameObjectCollider gameObjectCollider)
        {
            if(!gameObjectCollider.CanRegisterGameObject(gameObject)) return;

            GameObject prefab = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);

            BVHGameObject BVHGameObject = new BVHGameObject(gameObject, prefab);

            BVHGameObject.GameObject.transform.hasChanged = false;

            AABB objectAABB = BVHGameObject.GetAABB();
        
            var treeNode = new BVHNodeAABB<BVHGameObject>(BVHGameObject);
            treeNode.Position = objectAABB.Center;
            treeNode.Size = objectAABB.Size;

            gameObjectCollider.SceneObjectTree.Tree.InsertLeafNode(treeNode);

            if(gameObjectCollider._leafNodes == null)
            {
                gameObjectCollider._leafNodes = new Dictionary<BVHGameObject, BVHNodeAABB<BVHGameObject>>();
            }

            gameObjectCollider._leafNodes.Add(BVHGameObject, treeNode); 

            SceneObjectsCellUtility.ChangeSceneObjectsCell(gameObjectCollider.SceneDataManager);
        }

        private void OnSceneObjectsChanged()
        {
            if(SceneDataManager == null)
            {
                return;
            }

            RemoveNullObjectNodes();
        }

        public static void HandleTransformChangesForAllScenes()
        {
            foreach (var item in SceneDataManagerStack.GetAllSceneDataManager())
            {
                GameObjectCollider gameObjectCollider = (GameObjectCollider)item.GetSceneData(typeof(GameObjectCollider));

                gameObjectCollider?.HandleTransformChanges(true);
            }
        }

        public static void RemoveNullObjectNodesForAllScenes()
        {
            foreach (var item in SceneDataManagerStack.GetAllSceneDataManager())
            {
                GameObjectCollider gameObjectCollider = (GameObjectCollider)item.GetSceneData(typeof(GameObjectCollider));

                gameObjectCollider?.RemoveNullObjectNodes();
            }
        }

        public void HandleTransformChanges(bool forceCheck = false)
        {
            if(Selection.gameObjects.Length != 0 || forceCheck)
            {
                Dictionary<BVHGameObject, BVHNodeAABB<BVHGameObject>> changedTransform = new Dictionary<BVHGameObject, BVHNodeAABB<BVHGameObject>>();

                // Loop through all object-to-nodes pairs
                foreach (var pair in _leafNodes)
                {
                    // Can be null if the object was destroyed in the meantime
                    if (pair.Key.GameObject == null) continue;

                    if (pair.Key.GameObject.transform.hasChanged)
                    {
                        changedTransform.Add(pair.Key, pair.Value);
                        pair.Key.GameObject.transform.hasChanged = false;
                    }
                }

                foreach (var pair in changedTransform)
                {
                    SceneObjectTree.Tree.RemoveLeafNode(pair.Value);
                    _leafNodes.Remove(pair.Key);

                    RegisterGameObjectToCurrentScene(pair.Key.GameObject);
                }

                SceneObjectsCellUtility.ChangeSceneObjectsCell(SceneDataManager);
            }
        }

        public static void ChangeGameObjectSceneIfNecessary(GameObject gameObject)
        {
            SceneDataManager findCurrentSceneDataManager = FindScene.StreamingCells.OverlapPosition(gameObject.transform.position)[0];

            if(gameObject.scene != findCurrentSceneDataManager.GetScene())
            {
                GameObject prefabRoot = GameObjectUtility.GetPrefabRoot(gameObject);

                if(prefabRoot != null)
                {
                    if(prefabRoot.transform.parent != null)
                    {
                        string prefabParentName = prefabRoot.transform.parent.gameObject.name;

                        prefabRoot.transform.parent = null;

                        SceneManager.MoveGameObjectToScene(prefabRoot, findCurrentSceneDataManager.GetScene());

                        GameObject parent = GameObjectUtility.FindParentGameObjectObject(prefabParentName, findCurrentSceneDataManager.GetScene());

                        GameObjectUtility.ParentGameObject(prefabRoot, parent);
                    }
                    else
                    {
                        SceneManager.MoveGameObjectToScene(prefabRoot, findCurrentSceneDataManager.GetScene());
                    }
                }
                else
                {
                    SceneManager.MoveGameObjectToScene(gameObject, findCurrentSceneDataManager.GetScene());
                }
            }
        }

        public static void RemoveNode(GameObject gameObject)
        {
            foreach (SceneDataManager sceneDataManager in SceneDataManagerStack.GetAllSceneDataManager())
            {
                GameObjectCollider gameObjectCollider = (GameObjectCollider)sceneDataManager.GetSceneData(typeof(GameObjectCollider));

                if(gameObjectCollider == null) continue;

                foreach (var item in gameObjectCollider._leafNodes)
                {
                    BVHGameObject BVHGameObject = (BVHGameObject)item.Key;

                    if(gameObject.ContainInChildren(BVHGameObject.GameObject))
                    {
                        gameObjectCollider.SceneObjectTree.Tree.RemoveLeafNode(item.Value);
                        gameObjectCollider._leafNodes.Remove(item.Key);
                        SceneObjectsCellUtility.ChangeSceneObjectsCell(sceneDataManager);
                        return;
                    }
                }
            }
        }

        public void RemoveNullObjectNodes()
        {
            bool foundNull = false;
            var newDictionary = new Dictionary<BVHGameObject, BVHNodeAABB<BVHGameObject>>();
            foreach(var pair in _leafNodes)
            {
                BVHGameObject BVHGameObject = pair.Value.Data;
                if (BVHGameObject == null || BVHGameObject.GameObject == null)
                {
                    foundNull = true;
                    SceneObjectTree.Tree.RemoveLeafNode(pair.Value);
                }
                else
                {
                    newDictionary.Add(pair.Key, pair.Value);
                }
            }
            
            if (foundNull)
            {
                _leafNodes.Clear();
                _leafNodes = newDictionary;

                SceneObjectsCellUtility.ChangeSceneObjectsCell(SceneDataManager);
            }
        }

        private BVHNodeAABB<BVHGameObject> GetLeafNode(GameObject gameObject)
        {
            foreach (var item in _leafNodes)
            {
                if(item.Key.GameObject == gameObject)
                {
                    return item.Value;
                }
            }

            return null;
        }

        private bool CanRegisterGameObject(GameObject gameObject)
        {
            if(gameObject == null) return false;
            if (gameObject.GetComponent<RectTransform>() != null) return false; 
            if (gameObject.GetComponent<TerrainCollider>() == null)
            {
                if(!gameObject.IsRendererEnabled()) return false;
            }

            if(PrefabUtility.GetPrefabAssetType(gameObject) != PrefabAssetType.NotAPrefab)
            {
                GameObject prefab = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);

                LODGroup LODGroup = prefab.GetComponent<LODGroup>();

                if(LODGroup != null)
                {
                    LOD[] lods = LODGroup.GetLODs();

                    if(lods.Length != 0)
                    {
                        if(lods[0].renderers.Length != 0)
                        {
                            if(lods[0].renderers[0].gameObject == gameObject)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }

            return true;
        }
    }
}
#endif