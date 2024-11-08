using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.BVH.Scripts;
using VladislavTsurikov.BVH.Scripts.Math;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.ColliderSystem.Scripts.Scene
{
    [Serializable]
    public abstract class ColliderObject 
    {
        [OdinSerialize]
        public System.Object Obj;
        
        [NonSerialized]
        public PathToColliderObject PathToColliderObject;

        public ColliderObject(System.Object obj)
        {
            Obj = obj;
        }

        public virtual void Raycast(Ray ray, List<RayHit> sortedObjectHits)
        {
            var mesh = GetMesh();
            if (mesh != null && IsRendererEnabled())
            {
                var meshRayHit = GetMesh().Raycast(ray, GetMatrix());
                if (meshRayHit != null) 
                    sortedObjectHits.Add(new RayHit(ray, this, meshRayHit));
            }
        }
        
        public void SetPathToObjectCollider<T>(List<object> pathDatas, BVHObjectTree<T> tree, BVHNode<T> node) where T: ColliderObject
        {
            if(pathDatas == null)
                return;
            
            pathDatas.Add(tree);
            pathDatas.Add(node);

            SetPathToObjectCollider(pathDatas);
        }

        protected virtual void SetPathToObjectCollider(List<object> pathDatas){}

        public abstract bool IsRendererEnabled();
        public abstract Mesh.Mesh GetMesh();
        public abstract GameObject GetPrefab();
        public abstract Matrix4x4 GetMatrix();
        public abstract OBB GetOBB();
        public abstract AABB GetAABB();
        public abstract int GetLayer();
    }
}