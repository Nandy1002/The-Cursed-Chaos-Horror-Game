using System;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using Component = VladislavTsurikov.ComponentStack.Scripts.Component;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem
{
    [Serializable]
    public abstract class ToolWindow : Component
    {
	    public static int EditorHash = "Editor".GetHashCode();
	    
	    public void InternalHandleKeyboardEvents()
        {
            switch (Event.current.type)
            {
                case EventType.Layout:
                case EventType.Repaint:
                    return;
            }

            HandleKeyboardEvents();
        }

        public void InternalDoTool()
        {
#if UNITY_EDITOR
            if(WindowDataPackage.Instance.SelectedVariables.HasOneSelectedGroup())
			{
				if(WindowDataPackage.Instance.ResourcesControllerEditor.IsSyncError(WindowDataPackage.Instance.SelectedVariables.SelectedGroup))
				{
					return;
				}
                else if(WindowDataPackage.Instance.SelectedVariables.SelectedGroup.PrototypeList.Count == 0)
				{
					return;
				}
			}
#endif

            DoTool();
        }

        public virtual void OnDisabled(){}
        public virtual void OnEnable(){}
        public virtual void DoTool() {}
        public virtual void HandleKeyboardEvents(){}
    }
}