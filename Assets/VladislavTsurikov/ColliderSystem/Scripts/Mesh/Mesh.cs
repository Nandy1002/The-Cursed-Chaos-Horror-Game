using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.Extensions.Scripts;

namespace VladislavTsurikov.ColliderSystem.Scripts.Mesh
{
    public class Mesh
    {
        private UnityEngine.Mesh _unityMesh;
        private Vector3[] _vertices;
        private int[] _vertIndices;
        private int _numTriangles;
        private MeshTree _tree;

        public UnityEngine.Mesh UnityMesh { get { return _unityMesh; } }
        public int NumTriangles { get { return _numTriangles; } }
        public Vector3[] Vertices { get { return _vertices.Clone() as Vector3[]; } }

        public Mesh(UnityEngine.Mesh unityMesh)
        {
            _unityMesh = unityMesh;
            _vertices = _unityMesh.vertices;
            _vertIndices = _unityMesh.triangles;
            _numTriangles = (int)(_vertIndices.Length / 3);
            _tree = new MeshTree(this);
        }

        public Vector3[] GetTriangleVerts(int triangleIndex)
        {
            int baseIndex = triangleIndex * 3;
            return new Vector3[] { _vertices[_vertIndices[baseIndex]], _vertices[_vertIndices[baseIndex + 1]], _vertices[_vertIndices[baseIndex + 2]] };
        }

        public List<Vector3> GetTriangleVerts(int triangleIndex, Matrix4x4 transformMatrix)
        {
            int baseIndex = triangleIndex * 3;
            return transformMatrix.TransformPoints(new List<Vector3> { _vertices[_vertIndices[baseIndex]], _vertices[_vertIndices[baseIndex + 1]], _vertices[_vertIndices[baseIndex + 2]] });
        }

        public MeshTriangle GetTriangle(int triangleIndex)
        {
            int baseIndex = triangleIndex * 3;
            return new MeshTriangle(_vertIndices[baseIndex], _vertIndices[baseIndex + 1], _vertIndices[baseIndex + 2]);
        }

        public MeshRayHit Raycast(Ray ray, Matrix4x4 transformMtx)
        {
            return _tree.Raycast(ray, transformMtx);
        }

#if UNITY_EDITOR
        public void DrawRaycast(Ray ray, Matrix4x4 transformMtx, Color lineColor)
        {
            _tree.DrawRaycast(ray, transformMtx, lineColor);
        }

        public void DrawAllCells(Matrix4x4 transformMtx, Color lineColor)
        {
            _tree.DrawAllCells(transformMtx, lineColor);
        }
#endif
    }
}