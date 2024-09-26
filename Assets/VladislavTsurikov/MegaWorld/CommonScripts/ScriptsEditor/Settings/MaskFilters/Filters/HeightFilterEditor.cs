#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Filters;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.MaskFilters.Filters 
{
	[SettingsEditor(typeof(HeightFilter))]
    public class HeightFilterEditor : MaskFilterEditor
    {
        private HeightFilter _heightFilter;

        public override void OnEnable()
        {
            _heightFilter = (HeightFilter)Target;
        }
        
        public override void OnGUI(Rect rect, int index) 
        {
            if(index != 0)
            {
	            _heightFilter.BlendMode = (BlendMode)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Blend Mode"), _heightFilter.BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            _heightFilter.MinHeight = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Height"), _heightFilter.MinHeight);

            rect.y += EditorGUIUtility.singleLineHeight;

            _heightFilter.MaxHeight = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Height"), _heightFilter.MaxHeight);

            rect.y += EditorGUIUtility.singleLineHeight;

            _heightFilter.HeightFalloffType = (FalloffType)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Height Falloff Type"), _heightFilter.HeightFalloffType);

			rect.y += EditorGUIUtility.singleLineHeight;
            
            if(_heightFilter.HeightFalloffType != FalloffType.None)
			{
				_heightFilter.HeightFalloffMinMax = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Height Falloff Min Max"), _heightFilter.HeightFalloffMinMax);
			
				rect.y += EditorGUIUtility.singleLineHeight;
                
                if(_heightFilter.HeightFalloffMinMax)
				{
					_heightFilter.MinAddHeightFalloff = Mathf.Max(0.1f, EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Add Height Falloff"), _heightFilter.MinAddHeightFalloff));
					
                    rect.y += EditorGUIUtility.singleLineHeight;
                    
                    _heightFilter.MaxAddHeightFalloff = Mathf.Max(0.1f, EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Add Height Falloff"), _heightFilter.MaxAddHeightFalloff));
				}
				else
				{
					_heightFilter.AddHeightFalloff = Mathf.Max(0.1f, EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Add Height Falloff"), _heightFilter.AddHeightFalloff));

                    rect.y += EditorGUIUtility.singleLineHeight;
				}
			}
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            if(index != 0)
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
			height += EditorGUIUtility.singleLineHeight;
            
            if(_heightFilter.HeightFalloffType != FalloffType.None)
			{			
				height += EditorGUIUtility.singleLineHeight;
                
                if(_heightFilter.HeightFalloffMinMax)
				{					
                    height += EditorGUIUtility.singleLineHeight;
                    height += EditorGUIUtility.singleLineHeight;
                    
				}
				else
				{
                    height += EditorGUIUtility.singleLineHeight;
				}
			}

            return height;
        }
    }
}
#endif