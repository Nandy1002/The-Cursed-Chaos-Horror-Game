#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.ComponentStack.ScriptsEditor.ReorderableListSupport;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem.Components;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.TransformComponentsSystem.Components
{
    [SettingsEditor(typeof(Rotation))]
    public class RotationEditor : ReorderableListComponentEditor
    {
        private Rotation _rotation;
        public override void OnEnable()
        {
            _rotation = (Rotation)Target;
        }

        public override void OnGUI(Rect rect, int index) 
        {
            _rotation.RandomizeOrientationX = EditorGUI.Slider (new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                new GUIContent("Randomize X (%)"), _rotation.RandomizeOrientationX, 0.0f, 100.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
            _rotation.RandomizeOrientationY = EditorGUI.Slider (new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                new GUIContent("Randomize Y (%)"), _rotation.RandomizeOrientationY, 0.0f, 100.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
            _rotation.RandomizeOrientationZ = EditorGUI.Slider (new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                new GUIContent("Randomize Z (%)"), _rotation.RandomizeOrientationZ, 0.0f, 100.0f);
            rect.y += EditorGUIUtility.singleLineHeight;

        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif
