#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Filters;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.MaskFilters.Filters
{
    [SettingsEditor(typeof(MaskOperationsFilter))]
    public class MaskOperationsFilterEditor : MaskFilterEditor
    {
        private MaskOperationsFilter _maskOperationsFilter;

        public override void OnEnable()
        {
            _maskOperationsFilter = (MaskOperationsFilter)Target;
        }
        
        public override void OnGUI(Rect rect, int index)
        {
            _maskOperationsFilter.MaskOperations = (MaskOperations)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), _maskOperationsFilter.MaskOperations);

            rect.y += EditorGUIUtility.singleLineHeight;

            switch (_maskOperationsFilter.MaskOperations)
            {
                case MaskOperations.Add:
                {
                    _maskOperationsFilter.Value = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Value"), _maskOperationsFilter.Value, 0f, 1f);
                    break;
                }
                case MaskOperations.Multiply:
                {
                    _maskOperationsFilter.Value = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Value"), _maskOperationsFilter.Value, 0f, 1f);
                    break;
                }
                case MaskOperations.Power:
                {
                    _maskOperationsFilter.Value = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Value"), _maskOperationsFilter.Value, 1f, 10f);
                    break;
                }
                case MaskOperations.Clamp:
                {
                    GUIStyle alignmentStyleRight = new GUIStyle(UnityEngine.GUI.skin.label);
                    alignmentStyleRight.alignment = TextAnchor.MiddleRight;
                    alignmentStyleRight.stretchWidth = true;
                    GUIStyle alignmentStyleLeft = new GUIStyle(UnityEngine.GUI.skin.label);
                    alignmentStyleLeft.alignment = TextAnchor.MiddleLeft;
                    alignmentStyleLeft.stretchWidth = true;

                    EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Range"), ref _maskOperationsFilter.ClampRange.x, ref _maskOperationsFilter.ClampRange.y, 0, 1);

                    rect.y += EditorGUIUtility.singleLineHeight;

                    Rect numFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                    GUIContent minContent = new GUIContent("");
                    EditorGUI.LabelField(numFieldRect, minContent, alignmentStyleLeft);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                    _maskOperationsFilter.ClampRange.x = EditorGUI.FloatField(numFieldRect, _maskOperationsFilter.ClampRange.x);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(numFieldRect, " ");
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                    _maskOperationsFilter.ClampRange.y = EditorGUI.FloatField(numFieldRect, _maskOperationsFilter.ClampRange.y);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                    GUIContent maxContent = new GUIContent("");
                    EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);

                    rect.y += EditorGUIUtility.singleLineHeight;
                    break;
                }
                case MaskOperations.Invert:
                {
                    _maskOperationsFilter.StrengthInvert = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Strength"), _maskOperationsFilter.StrengthInvert, 0f, 1f);

                    break;
                }
                case MaskOperations.Remap:
                {
                    GUIStyle alignmentStyleRight = new GUIStyle(UnityEngine.GUI.skin.label);
                    alignmentStyleRight.alignment = TextAnchor.MiddleRight;
                    alignmentStyleRight.stretchWidth = true;
                    GUIStyle alignmentStyleLeft = new GUIStyle(UnityEngine.GUI.skin.label);
                    alignmentStyleLeft.alignment = TextAnchor.MiddleLeft;
                    alignmentStyleLeft.stretchWidth = true;

                    EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Range"), ref _maskOperationsFilter.RemapRange.x, ref _maskOperationsFilter.RemapRange.y, 0, 1);

                    rect.y += EditorGUIUtility.singleLineHeight;

                    Rect numFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
                    GUIContent minContent = new GUIContent("");
                    EditorGUI.LabelField(numFieldRect, minContent, alignmentStyleLeft);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                    _maskOperationsFilter.RemapRange.x = EditorGUI.FloatField(numFieldRect, _maskOperationsFilter.RemapRange.x);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(numFieldRect, " ");
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                    _maskOperationsFilter.RemapRange.y = EditorGUI.FloatField(numFieldRect, _maskOperationsFilter.RemapRange.y);
                    numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
                    GUIContent maxContent = new GUIContent("");
                    EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);

                    rect.y += EditorGUIUtility.singleLineHeight;
                    break;
                }
            }
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            switch (_maskOperationsFilter.MaskOperations)
            {
                case MaskOperations.Add:
                case MaskOperations.Multiply:
                case MaskOperations.Power:
                case MaskOperations.Invert:
                {
                    height += EditorGUIUtility.singleLineHeight;
                    break;
                }
                case MaskOperations.Clamp:
                case MaskOperations.Remap:
                {
                    height += EditorGUIUtility.singleLineHeight;
                    height += EditorGUIUtility.singleLineHeight;
                    break;
                }
            }

            return height;
        }

        public override string GetAdditionalName()
        {
            return "[" + _maskOperationsFilter.MaskOperations + "]";
        }
    }
}
#endif