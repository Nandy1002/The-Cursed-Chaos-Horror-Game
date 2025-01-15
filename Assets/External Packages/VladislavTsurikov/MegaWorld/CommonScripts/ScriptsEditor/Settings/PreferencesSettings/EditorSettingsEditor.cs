#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using EditorSettings = VladislavTsurikov.MegaWorld.CommonScripts.Scripts.PreferencesSettings.EditorSettings;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.PreferencesSettings
{
    [Serializable]
    public class EditorSettingsEditor 
    {
        public bool editorSettingsFoldout = true;

        public void OnGUI(EditorSettings editorSettings)
        {
            EditorSettingsWindowGUI(editorSettings);
        }

        public void EditorSettingsWindowGUI(EditorSettings editorSettings)
		{
			editorSettingsFoldout = CustomEditorGUILayout.Foldout(editorSettingsFoldout, "Editor Settings");

			if(editorSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				editorSettings.maxBrushSize = Mathf.Max(0.5f, CustomEditorGUILayout.FloatField(new GUIContent("Max Brush Size"), editorSettings.maxBrushSize));
				editorSettings.maxChecks = Mathf.Max(1, CustomEditorGUILayout.IntField(new GUIContent("Max Checks"), editorSettings.maxChecks));
				
				editorSettings.raycastSettings.OnGUI();
				
				EditorGUI.indentLevel--;
			}
		}
    }
}
#endif