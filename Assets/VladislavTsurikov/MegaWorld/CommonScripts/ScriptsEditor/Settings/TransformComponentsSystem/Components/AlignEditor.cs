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
    [SettingsEditor(typeof(Align))]  
    public class AlignEditor : ReorderableListComponentEditor
    {
        private Align _align;
        public override void OnEnable()
        {
            _align = (Align)Target;
        }
        
        public override void OnGUI(Rect rect, int index) 
        {
            GUIStyle alignmentStyleRight = new GUIStyle(UnityEngine.GUI.skin.label);
            alignmentStyleRight.alignment = TextAnchor.MiddleRight;
            alignmentStyleRight.stretchWidth = true;
            GUIStyle alignmentStyleLeft = new GUIStyle(UnityEngine.GUI.skin.label);
            alignmentStyleLeft.alignment = TextAnchor.MiddleLeft;
            alignmentStyleLeft.stretchWidth = true;

            _align.UseNormalWeight = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Use Normal Weight"), _align.UseNormalWeight);
            rect.y += EditorGUIUtility.singleLineHeight;
            
            if(_align.UseNormalWeight == true)
            {
                _align.MinMaxRange = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Max Range"), _align.MinMaxRange);
                rect.y += EditorGUIUtility.singleLineHeight;

                if(_align.MinMaxRange == true)
                {
                    EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Weight To Normal"), ref _align.MinWeightToNormal, ref _align.MaxWeightToNormal, 0, 1);
                    rect.y += EditorGUIUtility.singleLineHeight;

                    EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), new GUIContent(""));
                    //Min Label
                    Rect numFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                    GUIContent minContent = new GUIContent("");

                    EditorGUI.LabelField(numFieldRect, minContent, alignmentStyleLeft);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);

                    _align.MinWeightToNormal = EditorGUI.FloatField(numFieldRect, _align.MinWeightToNormal);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);

                    EditorGUI.LabelField(numFieldRect, " ");
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);

                    _align.MaxWeightToNormal = EditorGUI.FloatField(numFieldRect, _align.MaxWeightToNormal);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);

                    GUIContent maxContent = new GUIContent("");
                    EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);
                }
                else
                {
                    _align.MaxWeightToNormal = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Weight To Normal"), _align.MaxWeightToNormal, 0, 1);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }
            }
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            
            if(_align.UseNormalWeight)
            {
                height += EditorGUIUtility.singleLineHeight;

                if(_align.MinMaxRange == true)
                {
                    height += EditorGUIUtility.singleLineHeight;
                    height += EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    height += EditorGUIUtility.singleLineHeight;
                }
            }

            return height;
        }
    }
}
#endif
