#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings
{
	[Serializable]
    [SettingsEditor(typeof(SpawnDetailComponent))]
    public class SpawnDetailComponentEditor : ComponentEditor
    {
	    private SpawnDetailComponent _spawnDetailComponent;

	    public override void OnEnable()
        {
	        _spawnDetailComponent = (SpawnDetailComponent)Target;
        }

        public override void OnGUI()
        {
	        _spawnDetailComponent.UseRandomOpacity = CustomEditorGUILayout.Toggle(_useRandomOpacity, _spawnDetailComponent.UseRandomOpacity);
	        _spawnDetailComponent.Density = CustomEditorGUILayout.IntSlider(_density, _spawnDetailComponent.Density, 0, 10);
	        _spawnDetailComponent.FailureRate = CustomEditorGUILayout.Slider(_failureRate, _spawnDetailComponent.FailureRate, 0f, 100f);
        }

		private GUIContent _density = new GUIContent("Density");
		private GUIContent _useRandomOpacity = new GUIContent("Use Random Opacity");
		private GUIContent _failureRate = new GUIContent("Failure Rate (%)", "The larger this value, the less likely it is to spawn an object.");
    }
}
#endif
