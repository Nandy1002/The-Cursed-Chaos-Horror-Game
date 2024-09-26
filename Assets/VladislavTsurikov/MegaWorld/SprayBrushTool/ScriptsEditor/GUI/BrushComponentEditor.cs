#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.PreferencesSettings;

namespace VladislavTsurikov.MegaWorld.SprayBrushTool.ScriptsEditor.GUI
{
    [SettingsEditor(typeof(BrushComponent))]
    public class BrushComponentEditor : ComponentEditor
    {
	    private BrushComponent _component;
	    
        public override void OnEnable()
        {
	        _component = (BrushComponent)Target;
        }

        public override void OnGUI()
        {
	        _component.Spacing = CustomEditorGUILayout.Slider(spacing, _component.Spacing, 0.1f, 5);

	        _component.BrushSize = CustomEditorGUILayout.Slider(brushSize, _component.BrushSize, 0.1f, AdvancedSettings.Instance.EditorSettings.maxBrushSize);
        }

        [NonSerialized]
        private GUIContent brushSize = new GUIContent("Brush Size", "Selected prototypes will only spawn in this range around the center of Brush.");
        
		[NonSerialized]
		private GUIContent spacing = new GUIContent("Spacing", "Controls the distance between brush marks.");
    }
}
#endif
