#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem.GlobalToolSettings;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ToolsSystem.GlobalToolSettings
{
    public class GlobalToolComponentStackEditor : ComponentStackEditor<GlobalToolComponent, GlobalToolComponentEditor>
    {
        public GlobalToolComponentStackEditor(ComponentStack<GlobalToolComponent> stack) : base(stack)
        {
        }

        public override void RefreshEditors()
        {
            Editors = new List<GlobalToolComponentEditor>();
            
            foreach (var t in Stack.ComponentList)
            {
                var editor = new GlobalToolComponentEditor();
                editor.InternalInit(t);
                
                Editors.Add(editor);
            }
        }

        public static void OnGUI(Type toolType, Type type)
        {
            foreach (var item in WindowDataPackage.Instance.GlobalToolComponentStackEditor.Editors)
            {
                GlobalToolComponent globalToolComponent = (GlobalToolComponent)item.Target;
                
                if(globalToolComponent == null)
                    continue;
                
                if (globalToolComponent.ToolType == toolType)
                {
                    item.ComponentStackEditor.OnGUI(type);
                }
            }
        }

        public static ComponentEditor GetEditor(Type toolType, Type type)
        {
            foreach (var item in WindowDataPackage.Instance.GlobalToolComponentStackEditor.Editors)
            {
                GlobalToolComponent globalToolComponent = (GlobalToolComponent)item.Target;
                
                if (globalToolComponent.ToolType == toolType)
                {
                    return item.ComponentStackEditor.GetEditor(type);
                }
            }

            return null;
        }
    }
}
#endif