#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.ComponentStack.ScriptsEditor.ReorderableListSupport;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem.Components;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.TransformComponentsSystem.Components
{
    [SettingsEditor(typeof(ScaleFitness))]
    public class ScaleFitnessEditor : ReorderableListComponentEditor
    {
        private ScaleFitness _scaleFitness;
        public override void OnEnable()
        {
            _scaleFitness = (ScaleFitness)Target;
        }
        
        public override void OnGUI(Rect rect, int index) 
        {
            _scaleFitness.OffsetScale = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                new GUIContent("Offset Scale"), _scaleFitness.OffsetScale);
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