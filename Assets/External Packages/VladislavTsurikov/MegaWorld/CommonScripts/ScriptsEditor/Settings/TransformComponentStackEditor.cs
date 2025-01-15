#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings
{
    [SettingsEditor(typeof(TransformStackComponent))]
    public class TransformComponentStackEditor : ComponentEditor
    {
        private TransformStackComponent _transformStackComponent;
        private Scripts.Settings.TransformComponentsSystem.TransformComponentStackEditor _transformComponentEditor = null;
        
        public override void OnEnable()
        {
            _transformStackComponent = (TransformStackComponent)Target;
            _transformComponentEditor = new Scripts.Settings.TransformComponentsSystem.TransformComponentStackEditor(new GUIContent("Transform Component Stack"), _transformStackComponent.Stack);
        }
        
        public override void OnGUI() 
        {
            _transformComponentEditor.InternalOnGUI();
        }
    }
}
#endif