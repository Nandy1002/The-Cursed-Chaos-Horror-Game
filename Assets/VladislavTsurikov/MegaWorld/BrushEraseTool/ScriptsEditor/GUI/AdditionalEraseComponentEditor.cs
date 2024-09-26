#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.BrushEraseTool.ScriptsEditor.PrototypeSettings;

namespace VladislavTsurikov.MegaWorld.BrushEraseTool.ScriptsEditor.GUI
{
    [DontDrawFoldout]
    [SettingsEditor(typeof(AdditionalEraseComponent))]
    public class AdditionalEraseComponentEditor : ComponentEditor
    {
        private AdditionalEraseComponent _additionalEraseComponent;
        
        public override void OnEnable()
        {
            _additionalEraseComponent = (AdditionalEraseComponent)Target;
        }

        public override void OnGUI()
        {
            _additionalEraseComponent.SuccessForErase = CustomEditorGUILayout.Slider(success, _additionalEraseComponent.SuccessForErase, 0f, 100f);

        }

        private GUIContent success = new GUIContent("Success of Erase (%)");
    }
}
#endif