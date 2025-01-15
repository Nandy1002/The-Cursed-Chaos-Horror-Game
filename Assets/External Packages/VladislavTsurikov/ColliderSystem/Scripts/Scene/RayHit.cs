using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Mesh;

namespace VladislavTsurikov.ColliderSystem.Scripts.Scene
{
    public class RayHit
    {
        private System.Object _object;
        private Vector3 _point;
        private float _distance;
        private Vector3 _normal;
        private Plane _plane;
        private MeshRayHit _meshRayHit;

        public System.Object Object { get { return _object; } }
        public Vector3 Point { get { return _point; } }
        public float Distance { get { return _distance; } }
        public Vector3 Normal { get { return _normal.normalized; } }
        public Plane Plane { get { return _plane; } }
        public MeshRayHit MeshRayHit { get { return _meshRayHit; } }

        public static void SortByHitDistance(List<RayHit> hits)
        {
            hits.Sort(delegate(RayHit h0, RayHit h1)
            {
                return h0.Distance.CompareTo(h1.Distance);
            });
        }

        public RayHit(Ray hitRay, System.Object hitObject, Vector3 hitNormal, Vector3 point, float distance)
        {
            _object = hitObject;
            _point = point;
            _distance = distance;
            _normal = hitNormal;
            _plane = new Plane(_normal, _point);
        }

        public RayHit(Ray ray, System.Object hitObject, MeshRayHit meshRayHit)
        {
            _object = hitObject;
            _point = meshRayHit.HitPoint;
            _distance = meshRayHit.HitEnter;
            _normal = meshRayHit.HitNormal;
            _plane = new Plane(_normal, _point);
            _meshRayHit = meshRayHit;
        }
    }
}