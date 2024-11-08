#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem.Components;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings
{
    [SettingsEditor(typeof(PhysicsTransformStackComponent))]
    public class PhysicsTransformComponentStackEditor : ComponentEditor
    {
        private PhysicsTransformStackComponent _physicsTransformStackComponent;
        
        private Scripts.Settings.TransformComponentsSystem.TransformComponentStackEditor _transformComponentEditor = null;

        public override void OnEnable()
        {
            _physicsTransformStackComponent = (PhysicsTransformStackComponent)Target;
            List<Type> types = new List<Type>() {typeof(Align)};
            _transformComponentEditor = new Scripts.Settings.TransformComponentsSystem.TransformComponentStackEditor(new GUIContent("Transform Components Settings"), _physicsTransformStackComponent.Stack, types, true);
        }

        public override void OnGUI() 
        {
            _transformComponentEditor.InternalOnGUI();
        }
    }
}
#endif