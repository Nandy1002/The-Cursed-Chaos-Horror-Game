#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.ComponentStack.ScriptsEditor.ReorderableListSupport;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem.Components;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.TransformComponentsSystem.Components
{
    [SettingsEditor(typeof(CliffsPosition))]
    public class CliffsPositionEditor : ReorderableListComponentEditor
    {
        private CliffsPosition _cliffsPosition;
        public override void OnEnable()
        {
            _cliffsPosition = (CliffsPosition)Target;
        }
        
        public override void OnGUI(Rect rect, int index) 
        {
            _cliffsPosition.OffsetPosition = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                new GUIContent("Additional Rotation"), _cliffsPosition.OffsetPosition);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif