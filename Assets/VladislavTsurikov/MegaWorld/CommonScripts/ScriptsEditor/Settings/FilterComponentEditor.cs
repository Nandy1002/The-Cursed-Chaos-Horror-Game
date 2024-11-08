#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings
{
    [SettingsEditor(typeof(FilterComponent))]
    public class FilterComponentEditor : ComponentEditor
    {
        private FilterComponent _filterComponent;

        private SimpleFilterComponentEditor _simpleFilterComponentEditor;
        private MaskFilterComponentEditor _maskFilterComponentEditor;
        
        public override void OnEnable()
        {
            _filterComponent = (FilterComponent)Target;

            _simpleFilterComponentEditor = new SimpleFilterComponentEditor();
            _simpleFilterComponentEditor.InternalInit(_filterComponent.SimpleFilterComponent);
            _maskFilterComponentEditor = new MaskFilterComponentEditor();
            _maskFilterComponentEditor.InternalInit(_filterComponent.MaskFilterComponent);
        }
        
        public override void OnGUI()
        {
            _filterComponent.FilterType = (FilterType)CustomEditorGUILayout.EnumPopup(new GUIContent("Filter Type"), _filterComponent.FilterType);
            
            switch (_filterComponent.FilterType)
            {
                case FilterType.SimpleFilter:
                {
                    _simpleFilterComponentEditor.OnGUI();
                    break;
                }
                case FilterType.MaskFilter:
                {
                    CustomEditorGUILayout.HelpBox("\"Mask Filter\" works only with Unity terrain");
                    _maskFilterComponentEditor.OnGUI();
                    break;
                }
            }
        }
    }
}
#endif