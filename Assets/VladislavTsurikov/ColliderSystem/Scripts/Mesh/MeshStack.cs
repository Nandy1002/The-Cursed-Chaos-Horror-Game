using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.ColliderSystem.Scripts.Mesh
{
    public static class MeshStack 
    {
        private static Dictionary<UnityEngine.Mesh, Mesh> _unityMeshToMesh = new Dictionary<UnityEngine.Mesh, Mesh>();

        public static Mesh GetEditorMesh(UnityEngine.Mesh unityMesh)
        {
            if (_unityMeshToMesh.ContainsKey(unityMesh))
                return _unityMeshToMesh[unityMesh];
            else
            {
                Mesh editorMesh = new Mesh(unityMesh);
                _unityMeshToMesh.Add(unityMesh, editorMesh);
                return editorMesh;
            }
        }

#if UNITY_EDITOR
        static MeshStack()
        {
            EditorApplication.projectChanged -= OnProjectChanged;
            EditorApplication.projectChanged += OnProjectChanged;
        }
#endif

        private static void OnProjectChanged()
        {
            var newDictionary = new Dictionary<UnityEngine.Mesh, Mesh>();
            foreach (var pair in _unityMeshToMesh)
            {
                if (pair.Key != null)
                    newDictionary.Add(pair.Key, pair.Value);
            }

            _unityMeshToMesh.Clear();
            _unityMeshToMesh = newDictionary;
        }
    }
}