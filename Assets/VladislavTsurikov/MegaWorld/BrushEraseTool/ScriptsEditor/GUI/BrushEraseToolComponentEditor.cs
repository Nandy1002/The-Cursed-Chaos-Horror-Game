#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;

namespace VladislavTsurikov.MegaWorld.BrushEraseTool.ScriptsEditor.GUI
{
    [DontDrawFoldout]
    [SettingsEditor(typeof(BrushEraseToolComponent))]
    public class BrushEraseToolComponentEditor : ComponentEditor
    {
        private BrushEraseToolComponent _brushEraseToolComponent;
        
        public override void OnEnable()
        {
            _brushEraseToolComponent = (BrushEraseToolComponent)Target;
        }

        public override void OnGUI() 
        {
            _brushEraseToolComponent.EraseStrength = CustomEditorGUILayout.Slider(new GUIContent("Erase Strength"), _brushEraseToolComponent.EraseStrength, 0, 1);
        }
    }
}
#endif