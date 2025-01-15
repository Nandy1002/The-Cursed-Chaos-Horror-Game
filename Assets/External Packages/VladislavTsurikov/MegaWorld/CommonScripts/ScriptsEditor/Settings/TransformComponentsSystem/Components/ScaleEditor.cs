#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.ComponentStack.ScriptsEditor.ReorderableListSupport;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem.Components;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.TransformComponentsSystem.Components
{
	[SettingsEditor(typeof(Scale))]  
    public class ScaleEditor : ReorderableListComponentEditor
    {
	    private Scale _scale;
	    public override void OnEnable()
	    {
		    _scale = (Scale)Target;
	    }
	    
        public override void OnGUI(Rect rect, int index) 
        {
	        _scale.UniformScale = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Uniform Scale"), _scale.UniformScale);
            rect.y += EditorGUIUtility.singleLineHeight;

            if(_scale.UniformScale)
			{
				float minSameScaleValue = _scale.MinScale.x;
				float maxSameScaleValue = _scale.MaxScale.x;

                GUIStyle alignmentStyleRight = new GUIStyle(UnityEngine.GUI.skin.label);
                alignmentStyleRight.alignment = TextAnchor.MiddleRight;
                alignmentStyleRight.stretchWidth = true;
                GUIStyle alignmentStyleLeft = new GUIStyle(UnityEngine.GUI.skin.label);
                alignmentStyleLeft.alignment = TextAnchor.MiddleLeft;
                alignmentStyleLeft.stretchWidth = true;
                GUIStyle alignmentStyleCenter = new GUIStyle(UnityEngine.GUI.skin.label);
                alignmentStyleCenter.alignment = TextAnchor.MiddleCenter;
                alignmentStyleCenter.stretchWidth = true;
    
                EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Scale"), ref minSameScaleValue, ref maxSameScaleValue, 0f, 5f);
                rect.y += EditorGUIUtility.singleLineHeight * 0.5f;
                Rect slopeLabelRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(slopeLabelRect, "0", alignmentStyleLeft);
                slopeLabelRect = new Rect(rect.x + EditorGUIUtility.labelWidth + (rect.width - EditorGUIUtility.labelWidth) * 0.2f, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.6f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(slopeLabelRect, "2.5", alignmentStyleCenter);
                slopeLabelRect = new Rect(rect.x + EditorGUIUtility.labelWidth + (rect.width - EditorGUIUtility.labelWidth) * 0.8f, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(slopeLabelRect, "5", alignmentStyleRight);
                rect.y += EditorGUIUtility.singleLineHeight;
    
                //Label
                EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), new GUIContent(""));
                //Min Label
                Rect numFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                GUIContent minContent = new GUIContent("");
    
                EditorGUI.LabelField(numFieldRect, minContent, alignmentStyleLeft);
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
    
                minSameScaleValue = EditorGUI.FloatField(numFieldRect, minSameScaleValue);
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                
                EditorGUI.LabelField(numFieldRect, " ");
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
    
                maxSameScaleValue = EditorGUI.FloatField(numFieldRect, maxSameScaleValue);
                numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
    
                GUIContent maxContent = new GUIContent("");
                EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);
    
                rect.y += EditorGUIUtility.singleLineHeight;

                _scale.MinScale = new Vector3(minSameScaleValue, minSameScaleValue, minSameScaleValue);
				_scale.MaxScale = new Vector3(maxSameScaleValue, maxSameScaleValue, maxSameScaleValue);
			}
			else
			{
				_scale.MinScale = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Scale"), _scale.MinScale);
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight;
                _scale.MaxScale = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Scale"), _scale.MaxScale);
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight;
			}
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            if(_scale.UniformScale)
			{
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;                
			}
			else
			{
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
			}

            return height;
        }
    }
}
#endif
