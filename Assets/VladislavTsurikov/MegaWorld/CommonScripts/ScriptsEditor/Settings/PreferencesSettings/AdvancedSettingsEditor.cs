#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.PreferencesSettings;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.PreferencesSettings
{
    [CustomEditor(typeof(AdvancedSettings))]
    public class AdvancedSettingsEditor : Editor
    {
		public static bool visualisationSettingsFoldout;

        private AdvancedSettings settings;

        private void OnEnable()
        {
            settings = (AdvancedSettings)target;
        }

        public override void OnInspectorGUI()
        {
            CustomEditorGUILayout.IsInspector = true;
            OnGUI(settings);
        }

        public static void OnGUI(AdvancedSettings advancedSettings)
        {
	        EditorGUI.BeginChangeCheck();

            CustomEditorGUILayout.HelpBox("These settings are for more advanced users. In most cases, you do not need to configure anything here.");
            AdvancedSettingsWindowGUI(advancedSettings);

            if(EditorGUI.EndChangeCheck())
			{
                advancedSettings.Save();
			}
        }
		
		public static void AdvancedSettingsWindowGUI(AdvancedSettings advancedSettings)
		{
			advancedSettings.EditorSettings.OnGUI();
			BrushVisualisationSettingsStatic(advancedSettings);
		}

		public static void BrushVisualisationSettingsStatic(AdvancedSettings advancedSettings)
		{
			visualisationSettingsFoldout = CustomEditorGUILayout.Foldout(visualisationSettingsFoldout, "Visualisation Settings");

			if(visualisationSettingsFoldout)
			{
				EditorGUI.indentLevel++;

                advancedSettings.VisualisationSettings.VisualizeOverlapCheckSettings = CustomEditorGUILayout.Toggle(new GUIContent("Visualize Overlap Check Settings"), advancedSettings.VisualisationSettings.VisualizeOverlapCheckSettings);
				advancedSettings.VisualisationSettings.MaskFiltersSettings.OnGUI();
				advancedSettings.VisualisationSettings.SimpleFilterSettings.OnGUI();
                advancedSettings.VisualisationSettings.BrushHandlesSettings.OnGUI();

				EditorGUI.indentLevel--;
			}
		}

        [SettingsProvider]
        public static SettingsProvider PreferencesGUI()
        {
            var provider = new SettingsProvider("Preferences/Mega World", SettingsScope.User)
            {
                label = "Mega World",
                guiHandler = (searchContext) =>
                {
                    OnGUI(AdvancedSettings.Instance);
                },
                keywords = new HashSet<string>(new[] { "Mega World" })
            };

            return provider;
        }
    }
}
#endif