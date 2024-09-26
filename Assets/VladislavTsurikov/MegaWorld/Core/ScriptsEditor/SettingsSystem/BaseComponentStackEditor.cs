#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Scripts;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.SettingsSystem
{
    public class BaseComponentStackEditor : ComponentStackEditor<BaseComponent, ComponentEditor>
    {
        public BaseComponentStackEditor(ComponentStack<BaseComponent> stack) : base(stack)
        {
        }

        protected override void OnGUI(ComponentEditor editor, int index)
        {
            if (editor.GetType().GetAttribute<DontDrawFoldout>() == null)
            {
                editor.SelectSettingsFoldout = CustomEditorGUILayout.HeaderWithMenu(editor.Target.GetName(), editor.SelectSettingsFoldout,
                    () => Menu((BaseComponent)editor.Target, index)
                );
                
                if(editor.SelectSettingsFoldout)
                {
                    EditorGUI.indentLevel++;

                    editor.OnGUI();

                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                editor.OnGUI();
            }
        }
        
        public void OnGUI(Type type, Type toolType = null)
        {
            Stack.RemoveNullComponent();
            
            if(Stack.IsDirty)
            {
                RefreshEditors(); 
                Stack.IsDirty = false;
            }
            
            for (int i = 0; i < Editors.Count; i++)
            {
                if(Editors[i].Target.GetType() == type)
                {
                    if (toolType != null)
                    {
                        BaseComponent baseComponent = (BaseComponent)Editors[i].Target;

                        foreach (var tool in baseComponent.UsedTools)
                        {
                            if (tool.ToString() == toolType.ToString())
                            {
                                OnGUI(Editors[i], i);
                                break;
                            }
                        }
                    }
                    else
                    {
                        OnGUI(Editors[i], i);
                    }
                }
            }
        }

        public void Reset(Type type, Type toolType)
        {
            for (int i = 0; i < Editors.Count; i++)
            {
                if(Editors[i].Target.GetType() == type)
                {
                    BaseComponent baseComponent = (BaseComponent)Editors[i].Target;
                        
                    if (baseComponent.UsedTools.Contains(toolType))
                    {
                        Reset(baseComponent, i);
                    }
                }
            }
        }

        public void OnGUI(List<Type> drawTypes, Type toolType)
        {
            foreach (var type in drawTypes)
            {
                OnGUI(type, toolType);
            }
        }

        public ComponentEditor GetEditor(Type type, Type toolType = null)
        {
            for (int i = 0; i < Editors.Count; i++)
            {
                if(Editors[i].Target.GetType() == type)
                {
                    if (toolType != null)
                    {
                        BaseComponent baseComponent = (BaseComponent)Editors[i].Target;
                        
                        if (baseComponent.UsedTools.Contains(toolType))
                        {
                            return Editors[i];
                        }
                    }
                    else
                    {
                        return Editors[i];
                    }
                }
            }

            return null;
        }

        private void Menu(BaseComponent component, int index)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Reset"), false, () => Reset(component, index));

            menu.ShowAsContext();
        }

        public override void OnReset(BaseComponent oldComponent, BaseComponent newComponent)
        {
            newComponent.UsedTools = oldComponent.UsedTools;
            newComponent.General = oldComponent.General;
        }
    }
}
#endif