#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Filters;
using VladislavTsurikov.Utility;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.MaskFilters.Filters 
{
    [SettingsEditor(typeof(ConcavityFilter))]
    public class ConcavityFilterEditor : MaskFilterEditor
    {
        private ConcavityFilter _concavityFilter;

        public override void OnEnable()
        {
            _concavityFilter = (ConcavityFilter)Target;
        }

        public override void OnGUI(Rect rect, int index)
        {
            //Precaculate dimensions
            float epsilonLabelWidth = UnityEngine.GUI.skin.label.CalcSize(epsilonLabel).x;
            float modeLabelWidth = UnityEngine.GUI.skin.label.CalcSize(modeLabel).x;
            float strengthLabelWidth = UnityEngine.GUI.skin.label.CalcSize(strengthLabel).x;
            float curveLabelWidth = UnityEngine.GUI.skin.label.CalcSize(curveLabel).x;
            float labelWidth = Mathf.Max(Mathf.Max(Mathf.Max(modeLabelWidth, epsilonLabelWidth), strengthLabelWidth), curveLabelWidth) + 4.0f;
            labelWidth += 50;

            //Concavity Mode Drop Down
            Rect modeRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, EditorGUIUtility.singleLineHeight);
            ConcavityFilter.ConcavityMode mode = _concavityFilter.ConcavityScalar > 0.0f ? ConcavityFilter.ConcavityMode.Recessed : ConcavityFilter.ConcavityMode.Exposed;
            switch(EditorGUI.EnumPopup(modeRect, mode)) {
                case ConcavityFilter.ConcavityMode.Recessed:
                    _concavityFilter.ConcavityScalar = 1.0f;
                    break;
                case ConcavityFilter.ConcavityMode.Exposed:
                    _concavityFilter.ConcavityScalar = -1.0f;
                    break;
            }

            //Strength Slider
            Rect strengthLabelRect = new Rect(rect.x, modeRect.yMax, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(strengthLabelRect, strengthLabel);
            Rect strengthSliderRect = new Rect(strengthLabelRect.xMax, strengthLabelRect.y, rect.width - labelWidth, strengthLabelRect.height);
            _concavityFilter.ConcavityStrength = EditorGUI.Slider(strengthSliderRect, _concavityFilter.ConcavityStrength, 0.0f, 1.0f);

            //Epsilon (kernel size) Slider
            Rect epsilonLabelRect = new Rect(rect.x, strengthSliderRect.yMax, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(epsilonLabelRect, epsilonLabel);
            Rect epsilonSliderRect = new Rect(epsilonLabelRect.xMax, epsilonLabelRect.y, rect.width - labelWidth, epsilonLabelRect.height);
            _concavityFilter.ConcavityEpsilon = EditorGUI.Slider(epsilonSliderRect, _concavityFilter.ConcavityEpsilon, 1.0f, 20.0f);

            //Value Remap Curve
            Rect curveLabelRect = new Rect(rect.x, epsilonSliderRect.yMax, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(curveLabelRect, curveLabel);
            Rect curveRect = new Rect(curveLabelRect.xMax, curveLabelRect.y, rect.width - labelWidth, curveLabelRect.height);

            EditorGUI.BeginChangeCheck();
            _concavityFilter.ConcavityRemapCurve = EditorGUI.CurveField(curveRect, _concavityFilter.ConcavityRemapCurve);
            if(EditorGUI.EndChangeCheck()) {
                Vector2 range = TextureUtility.AnimationCurveToTexture(_concavityFilter.ConcavityRemapCurve, ref _concavityFilter.ConcavityRemapTex);
            }
        }

        public override float GetElementHeight(int index) 
        {
            return EditorGUIUtility.singleLineHeight * 5;
        }

        private static GUIContent strengthLabel = EditorGUIUtility.TrTextContent("Strength", "Controls the strength of the masking effect.");
        private static GUIContent epsilonLabel = EditorGUIUtility.TrTextContent("Feature Size", "Specifies the scale of Terrain features that affect the mask. This determines the size of features to which to apply the effect.");
        private static GUIContent modeLabel = EditorGUIUtility.TrTextContent("Mode");
        private static GUIContent curveLabel = EditorGUIUtility.TrTextContent("Remap Curve", "Remaps the concavity input before computing the final mask.");
    }
}
#endif