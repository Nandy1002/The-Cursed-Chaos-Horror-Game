#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Filters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Noise;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.MaskFilters.Filters 
{
    [SettingsEditor(typeof(HeightNoiseFilter))]
    public class HeightNoiseFilterEditor : MaskFilterEditor 
    {
        private HeightNoiseFilter _heightNoiseFilter;

        public override void OnEnable()
        {
            _heightNoiseFilter = (HeightNoiseFilter)Target;
        }

        private NoiseSettingsGUI m_noiseSettingsGUI;
        private NoiseSettingsGUI noiseSettingsGUI
        {
            get
            {
                if(m_noiseSettingsGUI == null)
                {
                    m_noiseSettingsGUI = new NoiseSettingsGUI(_heightNoiseFilter.NoiseSettings);
                }

                return m_noiseSettingsGUI;
            }
        }

        public override void OnGUI(Rect rect, int index) 
        {
            if(index != 0)
            {
                _heightNoiseFilter.BlendMode = (BlendMode)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), 
                    new GUIContent("Blend Mode"), _heightNoiseFilter.BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            CreateNoiseSettingsIfNecessary();

            DrawHeightSettings(ref rect);

            noiseSettingsGUI.OnGUI(rect);
        }

        private void DrawHeightSettings(ref Rect rect)
        {
            _heightNoiseFilter.MinHeight = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Height"), _heightNoiseFilter.MinHeight);

            rect.y += EditorGUIUtility.singleLineHeight;

            _heightNoiseFilter.MaxHeight = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Height"), _heightNoiseFilter.MaxHeight);
                        
			rect.y += EditorGUIUtility.singleLineHeight;     

            _heightNoiseFilter.MinRangeNoise = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Range Noise"), _heightNoiseFilter.MinRangeNoise);
            rect.y += EditorGUIUtility.singleLineHeight;
            _heightNoiseFilter.MaxRangeNoise = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Range Noise"), _heightNoiseFilter.MaxRangeNoise);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index) 
        {
            CreateNoiseSettingsIfNecessary();
            
            float height = EditorGUIUtility.singleLineHeight * 8;
            height += EditorGUIUtility.singleLineHeight;

            if (_heightNoiseFilter.NoiseSettings.ShowNoisePreviewTexture)
            {
                height += 256f + 40f;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }
            if (_heightNoiseFilter.NoiseSettings.ShowNoiseTransformSettings)
            {
                height += EditorGUIUtility.singleLineHeight * 7;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }
            if (_heightNoiseFilter.NoiseSettings.ShowNoiseTypeSettings)
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
            if(_heightNoiseFilter.NoiseSettings == null)
            {
                _heightNoiseFilter.NoiseSettings = new NoiseSettings();
            }
        }
    }
}
#endif