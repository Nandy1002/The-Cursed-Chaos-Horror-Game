using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.BVH.Scripts.Math;
using VladislavTsurikov.ColliderSystem.Scripts.Mesh;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.ColliderSystem.Scripts.UnityIntegration.Utility;
using VladislavTsurikov.Extensions.Scripts;

namespace VladislavTsurikov.ColliderSystem.Scripts.UnityIntegration
{    
    public class BVHGameObject : ColliderObject 
    {
        public GameObject GameObject
        {
            get
            {
                return (GameObject)Obj;
            }
        }
        
        public GameObject Prefab;

        public BVHGameObject(GameObject gameObject, GameObject prefab) : base(gameObject)
        {
            Prefab = prefab;
        }

        public override bool IsRendererEnabled()
        {
            if (GameObject == null || !GameObject.activeInHierarchy) return false;

            UnityEngine.Mesh mesh = GameObject.GetMesh();
            if (mesh != null && !GameObject.IsRendererEnabled()) return false;

            return true;
        }

        public override OBB GetOBB()
        {
            return GameObjectBounds.CalcWorldOBB(GameObject);
        }

        public override AABB GetAABB()
        {
            return GameObjectBounds.CalcWorldAABB(GameObject);
        }

        public override Matrix4x4 GetMatrix()
        {
            return GameObject.transform.localToWorldMatrix;
        }

        public override Mesh.Mesh GetMesh()
        {
            UnityEngine.Mesh mesh = GameObject.GetMesh();
            if (mesh != null)
            {
                return MeshStack.GetEditorMesh(mesh);
            }

            return null;
        }

        public override int GetLayer()
        {
            return GameObject.layer;
        }

        public override GameObject GetPrefab()
        {
            return Prefab;
        }

        public override void Raycast(Ray ray, List<RayHit> sortedObjectHits)
        {
            UnityEngine.Mesh mesh = GameObject.GetMesh();
            if (mesh != null)
            {
                if (GameObject.IsRendererEnabled())
                {
                    MeshRayHit meshRayHit = GetMesh().Raycast(ray, GameObject.transform.localToWorldMatrix);
                    if (meshRayHit != null) sortedObjectHits.Add(new RayHit(ray, GameObject, meshRayHit));
                }
                return;
            }

            TerrainCollider terrainCollider = GameObject.GetComponent<TerrainCollider>();
            if (terrainCollider != null)
            {
                RaycastHit raycastHit;
                if (terrainCollider.Raycast(ray, out raycastHit, float.MaxValue))
                    sortedObjectHits.Add(new RayHit(ray, GameObject, raycastHit.normal, raycastHit.point, raycastHit.distance));
                return;
            }
        }
    }
}