using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.BVH.Scripts.Math;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;

namespace VladislavTsurikov.SceneDataSystem.Scripts
{
    public class RendererSceneData : SceneData
    {
        public virtual List<RayHit> RaycastAll(Ray ray, ObjectFilter objectFilter)
        {
            return null;
        }
        
        public virtual AABB GetAABB()
        {
            return new AABB();
        }
    }
}