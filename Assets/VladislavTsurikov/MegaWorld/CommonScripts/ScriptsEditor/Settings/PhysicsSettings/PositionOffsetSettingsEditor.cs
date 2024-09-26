#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.PhysicsSimulatorEditor.ScriptsEditor;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.PhysicsSettings
{
    public static class PositionOffsetSettingsEditor 
    {
        public static bool positionOffsetFoldout = true;

        public static void OnGUI(PositionOffsetSettings settings)
        {
			positionOffsetFoldout = CustomEditorGUILayout.Foldout(positionOffsetFoldout, "Position Offset Settings");

			if (positionOffsetFoldout)
			{
				EditorGUI.indentLevel++;

				settings.EnableAutoOffset = CustomEditorGUILayout.Toggle(new GUIContent("Enable Auto Offset"), settings.EnableAutoOffset);
                settings.PositionOffsetDown = CustomEditorGUILayout.Slider(new GUIContent("Position Offset Down (%)"), settings.PositionOffsetDown, 0, 100);
                
				EditorGUI.indentLevel--;
			}
        }
    }
}
#endif