#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.ShortcutCombo.ScriptsEditor;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.Window
{
	public partial class MegaWorldWindow
	{
		[MenuItem(MenuUtils.MegaWorldWindow_MenuItem_ItemName, false, 0)]
		private static void OpenMegaWorldWindow()
		{
			if(ToolsWindow.IsOpen)
				ToolsWindow.Instance.Close();
			
			if(SelectionWindow.IsOpen)
				SelectionWindow.Instance.Close();
			
			OpenWindow("Mega World");
		}
		
		[MenuItem(MenuUtils.SeparateWindows_MenuItem_ItemName, false, 1)]
		private static void OpenSeparateWindows()
		{
			OpenWindow("Mega World");
			ToolsWindow.OpenWindow("Tools");
			SelectionWindow.OpenWindow("Selection");
		}
		
		[MenuItem("Window/Vladislav Tsurikov/Mega World/Documentation", false, 1000)]
		public static void Documentation()
		{
			Application.OpenURL("https://docs.google.com/document/d/1o_wtpxailmEGdtEwp5BGIyV8SXklvlJQp9vY2YoTBx4/edit?usp=sharing");
		}
		
		protected override void OnGUI()
        {
	        base.OnGUI();
	        
			WindowDataPackage.Instance.SelectedVariables.DeleteNullValueIfNecessary(WindowDataPackage.Instance.BasicData.GroupList);
			WindowDataPackage.Instance.SelectedVariables.SetAllSelectedParameters(WindowDataPackage.Instance.BasicData.GroupList);
			UpdateSceneViewEvent();

			OnMainGUI();
        }

		private void UpdateSceneViewEvent()
		{
			Event e = Event.current;

			HandleKeyboardEvents(e);
			SceneViewEventHandler.HandleSceneViewEvent(e);
		}

		private void OnMainGUI()
		{
			GUILayout.Space(5);
			
			if(!ToolsWindow.IsOpen)
			{
				WindowDataPackage.Instance.ToolComponentsEditor.InternalOnGUI();
			}

			WindowDataPackage.Instance.ToolComponentsEditor.DrawSelectedToolSettings();

			WindowDataPackage.Instance.Save();
		}
	}
}
#endif