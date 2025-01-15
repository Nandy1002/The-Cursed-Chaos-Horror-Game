#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.PhysicsSettings;
using VladislavTsurikov.PhysicsSimulatorEditor.ScriptsEditor;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings 
{
	[Serializable]
	public static class PhysicsSimulatorSettingsEditor 
	{
		public static bool PhysicsSimulatorSettingsFoldout = true;

		public static void OnGUI(PhysicsSimulatorSettings settings, DisablePhysicsMode disablePhysicsMode, bool accelerationPhysics = true)
		{
			PhysicsSimulatorSettingsFoldout = CustomEditorGUILayout.Foldout(PhysicsSimulatorSettingsFoldout, "Physics Simulator Settings");

			if (!PhysicsSimulatorSettingsFoldout) return;
			
			EditorGUI.indentLevel++;

			string disablePhysicsModeText = disablePhysicsMode == DisablePhysicsMode.GlobalTime ? "Global Time" : "Object Time";

			CustomEditorGUILayout.Label("Disable Physics Mode" + " (" + disablePhysicsModeText + ")");

			EditorGUI.BeginChangeCheck();

			settings.SimulatePhysics = CustomEditorGUILayout.Toggle(new GUIContent("Simulate Physics"), settings.SimulatePhysics);

			if(disablePhysicsMode == DisablePhysicsMode.GlobalTime)
			{
				settings.GlobalTime = CustomEditorGUILayout.FloatField(new GUIContent("Global Time"), settings.GlobalTime);
			}
			else
			{
				settings.ObjectTime = CustomEditorGUILayout.FloatField(new GUIContent("Object Time"), settings.ObjectTime);
			}

			if(accelerationPhysics)
			{
				settings.AccelerationPhysics = CustomEditorGUILayout.IntSlider(new GUIContent("Acceleration Physics"), settings.AccelerationPhysics, 1, 100);
			}

			PositionOffsetSettingsEditor.OnGUI(settings.PositionOffsetSettings);

			if(EditorGUI.EndChangeCheck())
			{
				settings.Save();
			}

			EditorGUI.indentLevel--;
		}
	}
}
#endif