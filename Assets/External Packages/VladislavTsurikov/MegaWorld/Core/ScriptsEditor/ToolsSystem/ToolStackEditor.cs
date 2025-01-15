#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Scripts;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem;
using GUIUtility = VladislavTsurikov.Utility.GUIUtility;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ToolsSystem
{
    public class ToolStackEditor : ComponentStackEditor<ToolWindow, ToolWindowEditor>
    {
        private TabStackEditor _tabStackEditor;
        
        public ToolStackEditor(WindowToolStack stack) : base(stack)
        {
            _tabStackEditor = new TabStackEditor(Stack.ComponentList, true, false);
            _tabStackEditor.AddCallback = ShowAddManu;
            _tabStackEditor.AddTabMenuCallback = TabMenu;
            _tabStackEditor.HappenedMoveCallback = HappenedMove;
            _tabStackEditor.TabSelected = DisableUnityTools;
            _tabStackEditor.TabDisabled = TabDisabled;
            _tabStackEditor.TabWidthFromName = true;
        }

        public void DrawSelectedToolSettings()
        {
            if(WindowDataPackage.Instance.SelectedTool == null)
			{
				return;
			}

            for (int i = 0; i < Stack.ComponentList.Count; i++)
            {
                if(Stack.ComponentList[i].Selected)
                {
                    Stack.ComponentList[i].InternalHandleKeyboardEvents();
                    Editors[i].OnGUI();
                    break;
                }
            }
        }

        protected override void OnGUI()
        {
            _tabStackEditor.OnGUI();

            WindowDataPackage.Instance.SelectedTool = Stack.GetSelected();

            if(WindowDataPackage.Instance.SelectedTool == null)
			{
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);
           		EditorGUILayout.LabelField("No Tool Selected");
           		EditorGUILayout.EndVertical();
            }
        }

        private void HappenedMove()
        {
            Stack.IsDirty = true;
        }

        private void DisableUnityTools(ITab tab)
        {
            Selection.objects = new UnityEngine.Object[0];
            Tools.current = Tool.None;
        }

        private void TabDisabled(ITab tab)
        {
            ToolWindow tool = (ToolWindow)tab;
            tool.OnDisabled();
        }

        public void DisableAllTools()
        {
            foreach (var component in Stack.ComponentList)
            {
                component.Selected = false;
                component.OnDisabled();
            }
                
            Selection.objects = new UnityEngine.Object[0];
            Tools.current = Tool.None;
        }

        private GenericMenu TabMenu(int currentTabIndex)
        {
            GenericMenu menu = new GenericMenu();

            if(Stack.ComponentList.Count > 1)
                menu.AddItem(new GUIContent("Delete"), false, GUIUtility.ContextMenuCallback, new Action(() => { Stack.Remove(currentTabIndex); }));
            else
                menu.AddDisabledItem(new GUIContent("Delete"));    

            return menu;
        }

        private void ShowAddManu()
        {
            GenericMenu contextMenu = new GenericMenu();

            foreach (var item in AllSettingsEditorTypes<ToolWindow>.Types)
            {
                Type toolType = item.Key;

                string name = toolType.GetAttribute<NameAttribute>().Name;

                bool exists = Stack.HasComponent(toolType);

                if (!exists)
                {
                    contextMenu.AddItem(new GUIContent(name), false, () => Stack.Create(toolType));
                }
                else
                {
                    contextMenu.AddDisabledItem(new GUIContent(name)); 
                }
            }

            contextMenu.ShowAsContext();
        }
    }
}
#endif