#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.ComponentStack.ScriptsEditor.ReorderableListSupport;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem.Components;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.TransformComponentsSystem.Components
{
    [SettingsEditor(typeof(PositionOffset))]
    public class PositionOffsetEditor : ReorderableListComponentEditor
    {
        private PositionOffset _positionOffset;
        public override void OnEnable()
        {
            _positionOffset = (PositionOffset)Target;
        }
        
        public override void OnGUI(Rect rect, int index) 
        {
            _positionOffset.MinPositionOffsetY = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                new GUIContent("Min Position Offset Y"), _positionOffset.MinPositionOffsetY);
            rect.y += EditorGUIUtility.singleLineHeight;
            _positionOffset.MaxPositionOffsetY = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                new GUIContent("Max Position Offset Y"), _positionOffset.MaxPositionOffsetY);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif
