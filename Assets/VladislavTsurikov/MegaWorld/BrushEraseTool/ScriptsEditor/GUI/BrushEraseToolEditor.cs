#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ToolsSystem;

namespace VladislavTsurikov.MegaWorld.BrushEraseTool.ScriptsEditor.GUI
{
    [SettingsEditor(typeof(BrushEraseTool))]
    public class BrushEraseToolEditor : ToolWindowEditor
    {
	    public override void DrawToolButtons()
		{
			if(WindowDataPackage.Instance.SelectedVariables.HasOneSelectedGroup())
			{
				if (WindowDataPackage.Instance.SelectedVariables.SelectedGroup.PrototypeType == typeof(PrototypeGameObject))
				{
					UndoEditor.OnGUI();
				}
			}
		}
    }
}
#endif