#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings
{
    [Serializable]
    public class SimpleFilterSettingsWthoutFalloffEditor
    {
        public bool FilterSettingsFoldout = true;

        public void OnGUI(SimpleFilterComponent component, string text)
        {
            FilterSettingsFoldout = CustomEditorGUILayout.Foldout(FilterSettingsFoldout, text);

            if(FilterSettingsFoldout)
            {
                EditorGUI.indentLevel++;
				
                DrawCheckHeight(component);
                DrawCheckSlope(component);

                EditorGUI.indentLevel--;
            }
        }

        public void DrawCheckHeight(SimpleFilterComponent filterComponent)
        {
            filterComponent.CheckHeight = CustomEditorGUILayout.Toggle(checkHeight, filterComponent.CheckHeight);

            EditorGUI.indentLevel++;

            if(filterComponent.CheckHeight)
            {
                filterComponent.MinHeight = CustomEditorGUILayout.FloatField(new GUIContent("Min Height"), filterComponent.MinHeight);
                filterComponent.MaxHeight = CustomEditorGUILayout.FloatField(new GUIContent("Max Height"), filterComponent.MaxHeight);
            }

            EditorGUI.indentLevel--;
        }

        void DrawCheckSlope(SimpleFilterComponent filterComponent)
        {
            filterComponent.CheckSlope = CustomEditorGUILayout.Toggle(checkSlope, filterComponent.CheckSlope);

            EditorGUI.indentLevel++;

            if(filterComponent.CheckSlope)
            {
                CustomEditorGUILayout.MinMaxSlider(slope, ref filterComponent.MinSlope, ref filterComponent.MaxSlope, 0f, 90);
            }

            EditorGUI.indentLevel--;
        }

        private GUIContent checkHeight = new GUIContent("Check Height");
        private GUIContent checkSlope = new GUIContent("Check Slope");
        private GUIContent slope = new GUIContent("Slope");
    }
}
#endif