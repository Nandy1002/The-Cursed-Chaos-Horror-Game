#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings
{
    [Serializable]
    [DontDrawFoldout]
    [SettingsEditor(typeof(SuccessComponent))]
    public class SuccessComponentEditor : ComponentEditor
    {
        private SuccessComponent _successComponent;
        
        public override void OnEnable()
        {
            _successComponent = (SuccessComponent)Target;
        }
        
        public override void OnGUI()
        {
            _successComponent.SuccessValue = CustomEditorGUILayout.Slider(success, _successComponent.SuccessValue, 0f, 100f);
        }

		private GUIContent success = new GUIContent("Success (%)");
    }
}
#endif
