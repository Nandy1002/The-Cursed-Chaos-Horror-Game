#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.PreferencesSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.BrushSettings
{
    [Serializable]
    public class BrushJitterSettingsEditor 
    {
        public void OnGUI(BrushComponent brush, BrushJitterSettings jitter)
        {
            brush.BrushSize = CustomEditorGUILayout.Slider(brushSize, brush.BrushSize, 0.1f, AdvancedSettings.Instance.EditorSettings.maxBrushSize);

            jitter.BrushSizeJitter = CustomEditorGUILayout.Slider(brushJitter, jitter.BrushSizeJitter, 0f, 1f);

			CustomEditorGUILayout.Separator();

			jitter.BrushScatter = CustomEditorGUILayout.Slider(brushScatter, jitter.BrushScatter, 0f, 1f);
            jitter.BrushScatterJitter = CustomEditorGUILayout.Slider(brushJitter, jitter.BrushScatterJitter, 0f, 1f);

			CustomEditorGUILayout.Separator();

			if(WindowDataPackage.Instance.SelectedVariables.HasOneSelectedGroup())
			{
				if(WindowDataPackage.Instance.SelectedVariables.SelectedGroup.PrototypeType == typeof(PrototypeTerrainDetail) || 
				   WindowDataPackage.Instance.SelectedVariables.SelectedGroup.PrototypeType == typeof(PrototypeTerrainTexture))
				{
					brush.BrushRotation = CustomEditorGUILayout.Slider(brushRotation, brush.BrushRotation, -180f, 180f);
            		jitter.BrushRotationJitter = CustomEditorGUILayout.Slider(brushJitter, jitter.BrushRotationJitter, 0f, 1f);

					CustomEditorGUILayout.Separator();
				}
			}
        }

		[NonSerialized]
        private GUIContent brushSize = new GUIContent("Brush Size", "Selected prototypes will only spawn in this range around the center of Brush.");
		[NonSerialized]
		private GUIContent brushJitter = new GUIContent("Jitter", "Control brush stroke randomness.");
		[NonSerialized]
		private GUIContent brushScatter = new GUIContent("Brush Scatter", "Randomize brush position by an offset.");
		[NonSerialized]
		private GUIContent brushRotation = new GUIContent("Brush Rotation", "Rotation of the brush.");
    }
}
#endif