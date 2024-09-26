#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.Window
{
	public class ToolsWindow : BaseWindow<ToolsWindow>
	{
		protected override void OnGUI()
		{
			base.OnGUI();
			
			EditorGUI.indentLevel = 0;

			CustomEditorGUILayout.IsInspector = false;

			WindowDataPackage.Instance.ToolComponentsEditor.InternalOnGUI();
        }
	}
}
#endif