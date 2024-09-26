#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.ComponentStack.ScriptsEditor.ReorderableListSupport;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.ScatterSettingsSystem;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.ScatterSettingsSystem
{
    [SettingsEditor(typeof(ScatterComponent))]
    public class ScatterComponentEditor : ComponentEditor
    {
        private ReorderableListComponentStackEditor<Scatter, ReorderableListComponentEditor> _componentStackEditor;

        public override void OnEnable()
        {
            ScatterComponent scatterComponent = (ScatterComponent)Target;
            _componentStackEditor = new ReorderableListComponentStackEditor<Scatter, ReorderableListComponentEditor>(new GUIContent("Scatter Operations"), scatterComponent.Stack);
        }

        public override void OnGUI()
        {
            _componentStackEditor.InternalOnGUI();
        }
    }
}
#endif