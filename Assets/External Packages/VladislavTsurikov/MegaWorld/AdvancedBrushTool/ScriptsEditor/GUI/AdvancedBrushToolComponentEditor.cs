#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;

namespace VladislavTsurikov.MegaWorld.AdvancedBrushTool.ScriptsEditor.GUI
{
    [DontDrawFoldout]
    [SettingsEditor(typeof(AdvancedBrushToolComponent))]
    public class AdvancedBrushToolComponentEditor : ComponentEditor
    {
        private AdvancedBrushToolComponent _advancedBrushToolComponent;
        
        public override void OnEnable()
        {
            _advancedBrushToolComponent = (AdvancedBrushToolComponent)Target;
        }

        public override void OnGUI() 
        {
            if(WindowDataPackage.Instance.SelectedVariables.HasOneSelectedGroup())
            {
                if (WindowDataPackage.Instance.SelectedVariables.SelectedGroup.PrototypeType == typeof(PrototypeTerrainTexture))
                {
                    _advancedBrushToolComponent.TextureTargetStrength = CustomEditorGUILayout.Slider(new GUIContent("Target Strength"), _advancedBrushToolComponent.TextureTargetStrength, 0, 1);
                }
            }
        }
    }
}
#endif