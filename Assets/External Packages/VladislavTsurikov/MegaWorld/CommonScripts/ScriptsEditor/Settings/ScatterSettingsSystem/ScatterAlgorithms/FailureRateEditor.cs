#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.ComponentStack.ScriptsEditor.ReorderableListSupport;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.ScatterSettingsSystem.ScatterAlgorithms;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.ScatterSettingsSystem.ScatterAlgorithms
{
    [SettingsEditor(typeof(FailureRate))]
    public class FailureRateEditor : ReorderableListComponentEditor
    {
        private FailureRate _failureRate;
        public override void OnEnable()
        {
            _failureRate = (FailureRate)Target;
        }
        
        public override void OnGUI(Rect rect, int index) 
        {
            _failureRate.Value = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Value (%)"), _failureRate.Value, 0f, 100f);
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