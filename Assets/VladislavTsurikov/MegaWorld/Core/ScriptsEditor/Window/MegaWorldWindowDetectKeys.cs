#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.Window
{
    public partial class MegaWorldWindow
    {
        private void HandleKeyboardEvents(Event current)
        {
            if (current.Equals(KeyDeleteEvent()))
            {
                Unspawn.UnspawnAllProto(WindowDataPackage.Instance.SelectedVariables.SelectedGroupList, true);
            }
            
            if(current.keyCode == KeyCode.Escape && current.modifiers == 0)
            {
                if(WindowDataPackage.Instance.SelectedTool != null)
                {
                    Tools.current = Tool.Move;
                    WindowDataPackage.Instance.ToolComponentsEditor.DisableAllTools();
                }

                Repaint();
            }
        }
		
        public static Event KeyDeleteEvent()
        {
            Event retEvent = Event.KeyboardEvent("^" + "backspace");
            return retEvent;
        }
    }
}
#endif