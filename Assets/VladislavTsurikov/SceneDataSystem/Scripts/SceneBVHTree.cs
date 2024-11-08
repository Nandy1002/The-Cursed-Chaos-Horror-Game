using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.BVH.Scripts;
using VladislavTsurikov.BVH.Scripts.Math;

namespace VladislavTsurikov.SceneDataSystem.Scripts
{
    public class SceneBVHTree
    {
        private Dictionary<SceneDataManager, BVHNodeAABB<SceneDataManager>> _leafNodes = new Dictionary<SceneDataManager, BVHNodeAABB<SceneDataManager>>();
        private BVHTree<BVHNodeAABB<SceneDataManager>, SceneDataManager> _tree = new BVHTree<BVHNodeAABB<SceneDataManager>, SceneDataManager>();

        public void Clear()
        {
            _tree.Clear();
            _leafNodes.Clear();
        }

        public void RegisterSceneDataManager(SceneDataManager sceneDataManager, AABB objectAABB)
        {
            if(_leafNodes.ContainsKey(sceneDataManager))
            {
                return;
            }
            
            if(objectAABB.Size == Vector3.zero)
            {
                return;
            }
            
            var treeNode = new BVHNodeAABB<SceneDataManager>(sceneDataManager);
            treeNode.Position = objectAABB.Center;
            treeNode.Size = objectAABB.Size;
            _tree.InsertLeafNode(treeNode);
            _leafNodes.Add(sceneDataManager, treeNode); 
        }

        public void RemoveNodes(SceneDataManager sceneDataManager)
        {
            if(!_leafNodes.ContainsKey(sceneDataManager))
            {
                return;
            }

            _tree.RemoveLeafNode(_leafNodes[sceneDataManager]);
            _leafNodes.Remove(sceneDataManager);
        }

        public List<SceneDataManager> OverlapPosition(Vector3 position)
        {
            if(SceneManager.sceneCount == 1)
                return GetSceneDataManagerFromFirstScene();
                
            return GetCurrentOverlapScenes(_tree.OverlapBox(position, Vector3.one, Quaternion.identity));
        }

        public List<SceneDataManager> OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation)
        {
            if(SceneManager.sceneCount == 1)
                return GetSceneDataManagerFromFirstScene();

            return GetCurrentOverlapScenes(_tree.OverlapBox(boxCenter, boxSize, boxRotation));
        }

        public List<SceneDataManager> OverlapSphere(Vector3 sphereCenter, float sphereRadius)
        {
            if(SceneManager.sceneCount == 1)
                return GetSceneDataManagerFromFirstScene();

            return GetCurrentOverlapScenes(_tree.OverlapSphere(sphereCenter, sphereRadius));
        }

        public List<SceneDataManager> RaycastAll(Ray ray)
        {
            if(SceneManager.sceneCount == 1)
                return GetSceneDataManagerFromFirstScene();

            var nodeHits = _tree.RaycastAll(ray, false);
            
            if (nodeHits.Count == 0) 
                return GetSceneDataManagerFromFirstScene();

            var overlappedObjects = new List<SceneDataManager>();
            foreach (var hit in nodeHits)
            {
                overlappedObjects.Add(hit.HitNode.Data);
            }

            return overlappedObjects;
        }

        private List<SceneDataManager> GetCurrentOverlapScenes(List<BVHNode<SceneDataManager>> overlappedNodes)
        {
            if (overlappedNodes.Count == 0)
            {
                return GetSceneDataManagerFromFirstScene();
            }

            var overlappedObjects = new List<SceneDataManager>();
            foreach(var node in overlappedNodes)
            {
                overlappedObjects.Add(node.Data);
            }

            return overlappedObjects;
        }

        private List<SceneDataManager> GetSceneDataManagerFromFirstScene()
        {
            Scene firstScene = SceneManager.GetSceneAt(0);
            SceneDataManager sceneDataManager = SceneDataManagerStack.GetSceneDataManager(firstScene);
            if(sceneDataManager == null)
            {
                return new List<SceneDataManager>();
            }
            
            return new List<SceneDataManager>(){sceneDataManager};
        }

        public void ChangeNodeSize(SceneDataManager sceneDataManager, AABB AABB)
        {
            if(!_leafNodes.ContainsKey(sceneDataManager))
            {
                return;
            }

            _tree.RemoveLeafNode(_leafNodes[sceneDataManager]);
            _leafNodes.Remove(sceneDataManager);
            RegisterSceneDataManager(sceneDataManager, AABB);
        }

        public AABB GetAABB(SceneDataManager sceneDataManager)
        {
            if(!_leafNodes.ContainsKey(sceneDataManager))
            {
                return AABB.GetInvalid();
            }

            BVHNodeAABB<SceneDataManager> node = _leafNodes[sceneDataManager];
            AABB worldAABB = new AABB(node.Position, node.Size);

            return worldAABB;
        }
        
#if UNITY_EDITOR
        #region Gizmos
        public void DrawAllCells(Color nodeColor)
        {
            _tree.DrawAllCells(Matrix4x4.identity, nodeColor);
        }

        public List<BVHNodeRayHit<SceneDataManager>> DrawRaycast(Ray ray, Color nodeColor)
        {
            return _tree.DrawRaycast(ray, Matrix4x4.identity, nodeColor);
        }
        #endregion
#endif
    }
}
