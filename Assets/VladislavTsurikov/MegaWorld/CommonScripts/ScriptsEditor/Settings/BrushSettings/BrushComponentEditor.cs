#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.BrushSettings;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.BrushSettings
{
    [SettingsEditor(typeof(BrushComponent))]
    public class BrushComponentEditor : ComponentEditor
    {
	    private BrushComponent _brushComponent;
	    private ProceduralMaskEditor _proceduralMaskEditor;
	    private CustomMasksEditor _customMasksEditor;

	    public BrushJitterSettingsEditor _brushSettingsJitterSettingsEditor = new BrushJitterSettingsEditor();

        public override void OnEnable()
        {
	        _brushComponent = (BrushComponent)Target; 
	        _proceduralMaskEditor = new ProceduralMaskEditor(_brushComponent.ProceduralMask);
	        _customMasksEditor = new CustomMasksEditor(_brushComponent.CustomMasks);
        }

        public override void OnGUI()
        {
	        _brushComponent.SpacingEqualsType = (SpacingEqualsType)CustomEditorGUILayout.EnumPopup(spacingEqualsType, _brushComponent.SpacingEqualsType);

	        if(_brushComponent.SpacingEqualsType == SpacingEqualsType.Custom)
	        {
		        _brushComponent.Spacing = CustomEditorGUILayout.FloatField(spacing, _brushComponent.Spacing);
	        }

	        _brushComponent.MaskType = (MaskType)CustomEditorGUILayout.EnumPopup(maskType, _brushComponent.MaskType);
			
	        switch (_brushComponent.MaskType)
	        {
		        case MaskType.Custom:
		        {
			        _customMasksEditor.OnGUI();

			        break;
		        }
		        case MaskType.Procedural:
		        {
			        _proceduralMaskEditor.OnGUI();

			        break;
		        }
	        }

	        _brushSettingsJitterSettingsEditor.OnGUI(_brushComponent, _brushComponent.BrushJitterSettings);
        }

        [NonSerialized]
		private GUIContent spacingEqualsType = new GUIContent("Spacing Equals", "Allows you to set what size the Spacing will be.");
		[NonSerialized]
		private GUIContent spacing = new GUIContent("Spacing", "Controls the distance between _brushSettings marks.");
		[NonSerialized]
		private GUIContent maskType = new GUIContent("Mask Type", "Allows you to choose which _brushSettings mask will be used.");
    }
}
#endif
