using System;
using UnityEditor;
using VladislavTsurikov.AttributeUtility.Scripts;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem
{
    [Serializable]
    public class WindowToolStack : ComponentStack<ToolWindow>
    {
#if UNITY_EDITOR
        protected override void Setup()
        {
            EditorApplication.update -= DisableToolsIfNecessary;
            EditorApplication.update += DisableToolsIfNecessary;
            
            if(ComponentList.Count == 0)
            {
                CreateAllAvailableTypes();
            }
        }
        
        public override void OnDisable()
        {
            EditorApplication.update -= DisableToolsIfNecessary;
        }

        public void DisableToolsIfNecessary()
        {
            if (Tools.current != Tool.None)
            {
                ToolWindow toolWindow = GetSelected();

                if (toolWindow != null)
                {
                    if(toolWindow.GetType().GetAttribute<WindowToolAttribute>().DisableToolIfUnityToolActive)
                    {
                        toolWindow.Selected = false;
                    }
                }
            }
        }
#endif

        public void DoSelectedTool()
        {
            WindowDataPackage.Instance.SelectedTool = GetSelected();

            if (WindowDataPackage.Instance.SelectedTool == null)
            {
                return;
            }
            
            foreach (var item in ComponentList)
            {
                if(item.Selected)
                {
                    if(ToolUtility.IsToolSupportSelectedData(item.GetType(), WindowDataPackage.Instance.BasicData))
                    {
                        item.InternalHandleKeyboardEvents();
                        item.InternalDoTool();
                    }

                    return;
                }
            }
        }
    }
}
