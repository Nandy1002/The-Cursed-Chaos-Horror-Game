#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Scripts;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.DeepCopy.Scripts;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ToolsSystem;
using GUIUtility = VladislavTsurikov.Utility.GUIUtility;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.SelectionWindow
{
    public class ClipboardGroup
    {
        private readonly List<BaseComponent> _copiedBaseSettingsList = new List<BaseComponent>();
        
        public readonly Type PrototypeType;
        
        public ClipboardGroup(Type prototypeType)
        {
            PrototypeType = prototypeType;
        }
        
        public static ClipboardGroup GetCurrentClipboardGroup(Type prototypeType, List<ClipboardGroup> clipboardPrototypes)
        {
            foreach (var clipboardPrototype in clipboardPrototypes)
            {
                if (clipboardPrototype.PrototypeType == prototypeType)
                    return clipboardPrototype;
            }

            return null;
        }
        
        public static bool HasDifferentPrototypeTypes(List<Group> groups)
        {
            if (groups.Count == 1)
                return false;
            
            Type prototypeType = null;

            for (int i = 0; i < groups.Count; i++)
            {
                if (i == 0)
                    prototypeType = groups[i].PrototypeType;
                else if(prototypeType != groups[i].PrototypeType)
                {
                    return true;
                }
            }

            return false;
        }
        
        public static void GroupMenu(GenericMenu menu, SelectedVariables selectedVariables, List<ClipboardGroup> clipboardGroups, Type toolType)
        {
            if(selectedVariables.HasOneSelectedGroup())
            {
                if (ToolEditorUtility.GetGroupComponents(selectedVariables.SelectedGroup, toolType).Count == 0)
                {
                    return;
                }
            }
            
            if(selectedVariables.HasOneSelectedGroup())
            {
                menu.AddSeparator("");
            }
            else if (!HasDifferentPrototypeTypes(selectedVariables.SelectedGroupList))
            {
                ClipboardGroup clipboardGroup = GetCurrentClipboardGroup(selectedVariables.SelectedGroupList[0].PrototypeType, clipboardGroups);
                
                if(clipboardGroup._copiedBaseSettingsList.Count != 0)
                {
                    menu.AddSeparator("");
                }
            }
            
            if(selectedVariables.HasOneSelectedGroup())
            {
                ClipboardGroup clipboardGroup = GetCurrentClipboardGroup(selectedVariables.SelectedGroup.PrototypeType, clipboardGroups);
                
                menu.AddItem(new GUIContent("Copy All Settings"), false, GUIUtility.ContextMenuCallback, new Action(() => 
                    clipboardGroup.CopyGroupSettings(new List<Group>() { selectedVariables.SelectedGroup }, toolType)));
            }

            if (!HasDifferentPrototypeTypes(selectedVariables.SelectedGroupList))
            {
                ClipboardGroup clipboardGroup = GetCurrentClipboardGroup(selectedVariables.SelectedGroupList[0].PrototypeType, clipboardGroups);
                
                if(clipboardGroup._copiedBaseSettingsList.Count != 0)
                {
                    menu.AddItem(new GUIContent("Paste All Settings"), false, GUIUtility.ContextMenuCallback, new Action(() => 
                        clipboardGroup.ClipboardGroupAction(new List<Group>(selectedVariables.SelectedGroupList), toolType, true)));

                    foreach (BaseComponent baseSettings in clipboardGroup._copiedBaseSettingsList)
                    {
                        string name = baseSettings.GetType().GetAttribute<NameAttribute>().Name;
                        menu.AddItem(new GUIContent("Paste Settings/" + name), false, GUIUtility.ContextMenuCallback, new Action(() => clipboardGroup.PasteSettings(selectedVariables.SelectedGroupList, baseSettings.GetType(), toolType)));	
                    }
                }
            }
        }
        
        public void CopyGroupSettings(List<Group> groups, Type toolType)
        {
            _copiedBaseSettingsList.Clear();

            ClipboardGroupAction(groups, toolType, false);
        }

        public void ClipboardGroupAction(List<Group> groups, Type toolType, bool paste)
        {
            AddGeneralGroupComponentsAttribute addGeneralGroupComponentsAttribute = toolType.GetAttribute<AddGeneralGroupComponentsAttribute>();
            
            AddGroupComponentsAttribute addGroupComponentsAttribute = toolType.GetAttribute<AddGroupComponentsAttribute>();

            List<Type> clipboardTypes = new List<Type>();
            if(addGeneralGroupComponentsAttribute != null)
                clipboardTypes.AddRange(addGeneralGroupComponentsAttribute.Types);
            if(addGroupComponentsAttribute != null)
                clipboardTypes.AddRange(addGroupComponentsAttribute.Types);
            
            foreach(Group group in groups) 
            {
                foreach (var type in clipboardTypes)
                {
                    ClipboardAction(group, type, toolType, paste);
                }
            }
        }

        public void PasteSettings(List<Group> groupList, Type settingsType, Type toolType)
        {
            foreach (Group group in groupList)
            {
                ClipboardAction(group, settingsType, toolType, true);
            }
        }

        public void ClipboardAction(Group group, Type settingsType, Type toolType, bool paste)
        {
            if(paste)
            {
                var copiedSettings = _copiedBaseSettingsList.Find(obj => obj.GetType() == settingsType);
                
                if(copiedSettings == null)
                    return;
                
                BaseComponent baseComponent = DeepCopier.Copy(copiedSettings);

                SetComponent(group, baseComponent, toolType);
            }
            else
            {
                _copiedBaseSettingsList.Add(DeepCopier.Copy(group.GetSettings(settingsType)));
            }
        }
        
        public void SetComponent(Group group, BaseComponent settings, Type toolType)
        {
            for (int i = 0; i < group.ComponentStack.ComponentList.Count; i++)
            {
                BaseComponent baseComponent = group.ComponentStack.ComponentList[i];
                
                if (baseComponent.GetType() == settings.GetType() && baseComponent.UsedTools.Contains(toolType))
                {
                    group.ComponentStack.ComponentList[i] = settings;
                    group.ComponentStack.IsDirty = true;
                    return;
                }
            }
        }
    }
}
#endif