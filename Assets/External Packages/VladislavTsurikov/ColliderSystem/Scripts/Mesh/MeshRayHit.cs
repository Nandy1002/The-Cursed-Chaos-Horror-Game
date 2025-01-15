using UnityEngine;

namespace VladislavTsurikov.ColliderSystem.Scripts.Mesh
{
    public class MeshRayHit
    {
        private float _hitEnter;
        private Vector3 _hitPoint;
        private Vector3 _hitNormal;
        private int _triangleIndex;

        public float HitEnter { get { return _hitEnter; } }
        public Vector3 HitPoint { get { return _hitPoint; } }
        public Vector3 HitNormal { get { return _hitNormal; } }
        public int TriangleIndex { get { return _triangleIndex; } }

        public MeshRayHit(Ray ray, float hitEnter, int triangleIndex, Vector3 hitNormal)
        {
            _hitEnter = hitEnter;
            _hitPoint = ray.GetPoint(hitEnter);
            _triangleIndex = triangleIndex;
            _hitNormal = hitNormal;
        }
    }
}