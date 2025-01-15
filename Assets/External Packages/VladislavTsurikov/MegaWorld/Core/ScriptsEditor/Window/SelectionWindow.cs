#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ToolsSystem;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.Window
{
	public class SelectionWindow : BaseWindow<SelectionWindow>
	{
		protected override void OnGUI()
        {
	        base.OnGUI();
	        
			WindowDataPackage.Instance.SelectedVariables.DeleteNullValueIfNecessary(WindowDataPackage.Instance.BasicData.GroupList);
			WindowDataPackage.Instance.SelectedVariables.SetAllSelectedParameters(WindowDataPackage.Instance.BasicData.GroupList);

			EditorGUI.indentLevel = 0;

			CustomEditorGUILayout.IsInspector = false;

			OnMainGUI();
        }

		void OnMainGUI()
		{
			GUILayout.Space(5);

			ToolWindowEditor toolWindowEditor = WindowDataPackage.Instance.ToolComponentsEditor.GetSelected();

			if(toolWindowEditor != null)
			{
				WindowDataPackage.Instance.BasicData.OnGUI(toolWindowEditor.DrawBasicData, toolWindowEditor.Target.GetType());
			}
		}
	}
}
#endif