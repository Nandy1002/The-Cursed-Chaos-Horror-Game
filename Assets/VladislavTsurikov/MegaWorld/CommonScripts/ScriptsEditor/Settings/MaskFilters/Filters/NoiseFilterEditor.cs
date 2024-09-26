#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Filters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Noise;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.MaskFilters.Filters
{
    [SettingsEditor(typeof(NoiseFilter))]
    public class NoiseFilterEditor : MaskFilterEditor
    {
        private NoiseFilter _noiseFilter;

        public override void OnEnable()
        {
            _noiseFilter = (NoiseFilter)Target;
        }
        
        private NoiseSettingsGUI noiseSettingsGUI;
        private NoiseSettingsGUI NoiseSettingsGUI
        {
            get
            {
                if(noiseSettingsGUI == null)
                {
                    noiseSettingsGUI = new NoiseSettingsGUI(_noiseFilter.NoiseSettings);
                }

                return noiseSettingsGUI;
            }
        }

        public override void OnGUI(Rect rect, int index) 
        {
            if(index != 0)
            {
                _noiseFilter.BlendMode = (BlendMode)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Blend Mode"), _noiseFilter.BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            CreateNoiseSettingsIfNecessary();

            NoiseSettingsGUI.OnGUI(rect);
        }

        public override float GetElementHeight(int index) 
        {
            CreateNoiseSettingsIfNecessary();
            
            float height = EditorGUIUtility.singleLineHeight;

            if (_noiseFilter.NoiseSettings.ShowNoisePreviewTexture)
            {
                height += 256f + 40f;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }
            if (_noiseFilter.NoiseSettings.ShowNoiseTransformSettings)
            {
                height += EditorGUIUtility.singleLineHeight * 7;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }
            if (_noiseFilter.NoiseSettings.ShowNoiseTypeSettings)
            {
                height += EditorGUIUtility.singleLineHeight * 15;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
        
        private void CreateNoiseSettingsIfNecessary()
        {
            if(_noiseFilter.NoiseSettings == null)
            {
                _noiseFilter.NoiseSettings = new NoiseSettings();
            }
        }
    }
}
#endif