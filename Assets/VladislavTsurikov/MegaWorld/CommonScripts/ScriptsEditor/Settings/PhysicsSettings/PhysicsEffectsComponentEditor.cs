#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.PhysicsSettings;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.PhysicsSettings
{
    [Serializable]
    [SettingsEditor(typeof(PhysicsEffectsComponent))]
    public class PhysicsEffectsComponentEditor : ComponentEditor
    {
	    private PhysicsEffectsComponent _component;
	    
		public bool forceFoldout = true;
		public bool directionFoldout = true;

		public override void OnEnable()
		{
			_component = (PhysicsEffectsComponent)Target;
		}

		public override void OnGUI()
        {
	        forceFoldout = CustomEditorGUILayout.Foldout(forceFoldout, "Force");

	        if (forceFoldout)
	        {
		        EditorGUI.indentLevel++;

		        _component.ForceRange = CustomEditorGUILayout.Toggle(new GUIContent("Force Range"), _component.ForceRange);
				
		        if(_component.ForceRange)
		        {
			        _component.MinForce = Mathf.Max(0, CustomEditorGUILayout.Slider(new GUIContent("Min Force"), _component.MinForce, 0, 100));
			        _component.MaxForce = Mathf.Max(_component.MinForce, CustomEditorGUILayout.Slider(new GUIContent("Max Force"), _component.MaxForce, 0, 100));
		        }
		        else
		        {
			        _component.MinForce =  Mathf.Max(0, CustomEditorGUILayout.Slider(new GUIContent("Force"), _component.MinForce, 0, 100));
		        }

		        EditorGUI.indentLevel--;
	        }

	        directionFoldout = CustomEditorGUILayout.Foldout(directionFoldout, "Direction");

	        if (directionFoldout)
	        {
		        EditorGUI.indentLevel++;

		        _component.RandomStrength = CustomEditorGUILayout.Slider(new GUIContent("Random Strength"),_component.RandomStrength, 0, 100);

		        EditorGUI.indentLevel--;
	        }
        }
    }
}
#endif