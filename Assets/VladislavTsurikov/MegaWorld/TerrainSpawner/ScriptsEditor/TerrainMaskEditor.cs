#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.TerrainSpawner.Scripts;

namespace VladislavTsurikov.MegaWorld.TerrainSpawner.ScriptsEditor
{
    [CustomEditor(typeof(TerrainMask))]
    public class TerrainMaskEditor : Editor
    {
        private TerrainMask _terrainMask;

        private void OnEnable()
        {
            _terrainMask = (TerrainMask)target;
        }

        public override void OnInspectorGUI()
        {
            _terrainMask.Group = (Group)CustomEditorGUILayout.ObjectField(new GUIContent("Group"), _terrainMask.Group == null,
                _terrainMask.Group, typeof(Group));
            
            _terrainMask.Mask = (Texture2D)CustomEditorGUILayout.ObjectField(new GUIContent("Mask"), _terrainMask.Mask == null,
                _terrainMask.Mask, typeof(Texture2D));
        }
    }
}
#endif