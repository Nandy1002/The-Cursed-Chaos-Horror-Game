#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.PreferencesSettings;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.PreferencesSettings
{
    [Serializable]
    public class BrushHandlesSettingsEditor 
    {
        private bool brushHandlesSettingsFoldout = true;

        public void OnGUI(BrushHandlesSettings brushHandlesSettings)
        {
            BrushHandlesSettings(brushHandlesSettings);
        }

        public void BrushHandlesSettings(BrushHandlesSettings brushHandlesSettings)
		{
			brushHandlesSettingsFoldout = CustomEditorGUILayout.Foldout(brushHandlesSettingsFoldout, "Brush Handles Settings");

			if(brushHandlesSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				brushHandlesSettings.DrawSolidDisc = CustomEditorGUILayout.Toggle(new GUIContent("Draw Solid Disc"), brushHandlesSettings.DrawSolidDisc);
				brushHandlesSettings.CircleColor = CustomEditorGUILayout.ColorField(new GUIContent("Сircle Color"), brushHandlesSettings.CircleColor);       				
				brushHandlesSettings.CirclePixelWidth = CustomEditorGUILayout.Slider(new GUIContent("Сircle Pixel Width"), brushHandlesSettings.CirclePixelWidth, 1f, 5f);

				EditorGUI.indentLevel--;
			}
		}
    }
}
#endif