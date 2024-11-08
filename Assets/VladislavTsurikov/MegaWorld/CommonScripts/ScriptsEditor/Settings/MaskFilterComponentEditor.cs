#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings
{
    [SettingsEditor(typeof(MaskFilterComponent))]
    public class MaskFilterComponentEditor : ComponentEditor
    {
        private MaskFilterComponent _maskFilterComponent;
        private MaskFilterStackEditor _maskFilterStackEditor = null;
        
        public static bool ChangedGUI;
        
        public override void OnEnable()
        {
            _maskFilterComponent = (MaskFilterComponent)Target;
            _maskFilterStackEditor = new MaskFilterStackEditor(new GUIContent("Mask Filters Settings"), _maskFilterComponent.Stack);
        }

        public override void OnGUI() 
        {
            EditorGUI.BeginChangeCheck();

            _maskFilterStackEditor.InternalOnGUI();
			
            if(EditorGUI.EndChangeCheck())
            {
                ChangedGUI = true;
            }
        }
    }
}
#endif