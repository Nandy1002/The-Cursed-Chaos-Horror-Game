#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings
{
    [SettingsEditor(typeof(SimpleTransformComponentComponent))]
    public class SimpleTransformComponentStackEditor : ComponentEditor
    {
        private SimpleTransformComponentComponent _simpleTransformComponentComponent;
        private Scripts.Settings.TransformComponentsSystem.TransformComponentStackEditor _transformComponentEditor = null;
        
        public override void OnEnable()
        {
            _simpleTransformComponentComponent = (SimpleTransformComponentComponent)Target;
            _transformComponentEditor = new Scripts.Settings.TransformComponentsSystem.TransformComponentStackEditor(new GUIContent("Transform Component Stack"), _simpleTransformComponentComponent.Stack, true);

        }

        public override void OnGUI() 
        {
            _transformComponentEditor.InternalOnGUI();
        }
    }
}
#endif