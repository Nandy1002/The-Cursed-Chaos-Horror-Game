#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Filters;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.MaskFilters.Filters 
{
    [SettingsEditor(typeof(SmoothFilter))]
    public class SmoothFilterEditor : MaskFilterEditor
    {
        private SmoothFilter _smoothFilter;

        public override void OnEnable()
        {
            _smoothFilter = (SmoothFilter)Target;
        }

        public override void OnGUI(Rect rect, int index) 
        {
            _smoothFilter.SmoothVerticality = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), direction, _smoothFilter.SmoothVerticality, -1.0f, 1.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
            _smoothFilter.SmoothBlurRadius = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), kernelSize, _smoothFilter.SmoothBlurRadius, 0.0f, 10.0f);
        }

        public override float GetElementHeight(int index) 
        {
            return EditorGUIUtility.singleLineHeight * 3;
        }

        public static readonly GUIContent direction = EditorGUIUtility.TrTextContent("Verticality", "Blur only up (1.0), only down (-1.0) or both (0.0)");
        public static readonly GUIContent kernelSize = EditorGUIUtility.TrTextContent("Blur Radius", "Specifies the size of the blur kernel");
    }
}
#endif
