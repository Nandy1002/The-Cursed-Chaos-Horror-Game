#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.PreferencesSettings;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.PreferencesSettings
{
    [Serializable]
    public class RaycastSettingsEditor 
    {
        public bool RaycastSettingsFoldout = true;

        public void OnGUI(RaycastSettings settings)
        {
            RaycastSettingsFoldout = CustomEditorGUILayout.Foldout(RaycastSettingsFoldout, "Raycast Settings");

			if(RaycastSettingsFoldout)
			{
				EditorGUI.indentLevel++;

                settings.RaycastType = (RaycastType)CustomEditorGUILayout.EnumPopup(new GUIContent("Raycast Type", ""), settings.RaycastType);

				settings.Offset = CustomEditorGUILayout.FloatField(new GUIContent("Offset", "If you want to spawn objects under pawns or inside buildings or in other similar cases. You need to decrease the Spawn Check Offset."), settings.Offset);

				EditorGUI.indentLevel--;
			}
        }
    }
}
#endif