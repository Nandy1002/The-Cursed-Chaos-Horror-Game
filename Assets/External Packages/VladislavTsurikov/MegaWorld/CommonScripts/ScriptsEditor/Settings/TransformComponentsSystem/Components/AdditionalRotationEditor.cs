#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.ComponentStack.ScriptsEditor.ReorderableListSupport;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem.Components;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.TransformComponentsSystem.Components
{
    [Serializable]
    [SettingsEditor(typeof(AdditionalRotation))]
    public class AdditionalRotationEditor : ReorderableListComponentEditor
    {
        private AdditionalRotation _additionalRotation;
        public override void OnEnable()
        {
            _additionalRotation = (AdditionalRotation)Target;
        }
        
        public override void OnGUI(Rect rect, int index) 
        {
            _additionalRotation.Rotation = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                new GUIContent("Additional Rotation"), _additionalRotation.Rotation);
            rect.y += EditorGUIUtility.singleLineHeight;
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight * 2;

            return height;
        }
    }
}
#endif