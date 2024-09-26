using UnityEngine;

namespace VladislavTsurikov.Utility
{
    public static class MeshUtility
    {
        public static GameObject SelectMeshObject(GameObject go, int lodIndex)
        {
            return SelectNormalMesh(go, lodIndex);
        }

        public static int GetLODCount(GameObject go)
        {
            LODGroup lodGroup = go.GetComponentInChildren<LODGroup>();
            if (lodGroup)
            {
                LOD[] lods = lodGroup.GetLODs();
                int lodCount = lods.Length;

                LOD lastLOD = lods[lods.Length - 1];

                return lodCount;
            }
            return 1;
        }

        private static GameObject SelectNormalMesh(GameObject go, int lodIndex)
        {
            LODGroup lodGroup = go.GetComponentInChildren<LODGroup>();
            if (lodGroup)
            {
                LOD[] lods = lodGroup.GetLODs();

                lodIndex = Mathf.Clamp(lodIndex, 0, lods.Length - 1);

                LOD lod = lods[lodIndex];
                if (lod.renderers.Length > 0)
                {
                    if (lod.renderers[0].gameObject.GetComponent<BillboardRenderer>())
                    {
                        if (lodIndex > 0)
                        {
                            lod = lods[lodIndex - 1];
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                return lod.renderers.Length > 0 ? lod.renderers[0].gameObject : null;
            }
            else
            {
                var meshRenderer = go.GetComponent<MeshRenderer>();
                if (meshRenderer) return meshRenderer.gameObject;

                meshRenderer = go.GetComponentInChildren<MeshRenderer>();
                if (meshRenderer) return meshRenderer.gameObject;
                return null;
            }
        }
        
        public static Bounds CalculateBoundsInstantiate(GameObject go)
        {
            if (!go)
            {
                return new Bounds(Vector3.zero, Vector3.one);
            }

            GameObject originalObject = Object.Instantiate(go);
            originalObject.transform.localScale = Vector3.one;
            originalObject.hideFlags = HideFlags.DontSave;
            Bounds objectBounds = CalculateBounds(originalObject);

            if (Application.isPlaying)
            {
                Object.Destroy(originalObject);
            }
            else
            {
                Object.DestroyImmediate(originalObject);
            }

            return objectBounds;
        }

        public static Bounds CalculateBounds(GameObject go)
        {
            Bounds combinedbounds = new Bounds(go.transform.position, Vector3.zero);
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (renderer is SkinnedMeshRenderer)
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
                    Mesh mesh = new Mesh();
                    skinnedMeshRenderer.BakeMesh(mesh);
                    Vector3[] vertices = mesh.vertices;

                    for (int i = 0; i <= vertices.Length - 1; i++)
                    {
                        vertices[i] = skinnedMeshRenderer.transform.TransformPoint(vertices[i]);
                    }
                    mesh.vertices = vertices;
                    mesh.RecalculateBounds();
                    Bounds meshBounds = mesh.bounds;
                    combinedbounds.Encapsulate(meshBounds);
                }
                else
                {
                    combinedbounds.Encapsulate(renderer.bounds);
                }
            }
            return combinedbounds;
        }
    }
}