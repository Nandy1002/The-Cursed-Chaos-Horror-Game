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
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ToolsSystem;
using GUIUtility = VladislavTsurikov.Utility.GUIUtility;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.SelectionWindow
{
    public class ClipboardPrototype
    {
        private readonly List<BaseComponent> _copiedBaseSettingsList = new List<BaseComponent>();
        
        public readonly Type PrototypeType;
        
        public ClipboardPrototype(Type prototypeType)
        {
            PrototypeType = prototypeType;
        }
        
        public static ClipboardPrototype GetCurrentClipboardPrototype(Type prototypeType, List<ClipboardPrototype> clipboardPrototypes)
        {
            foreach (var clipboardPrototype in clipboardPrototypes)
            {
                if (clipboardPrototype.PrototypeType == prototypeType)
                    return clipboardPrototype;
            }

            return null;
        }
        
        public void PrototypeMenu(GenericMenu menu, SelectedVariables selectedVariables, Type toolType)
        {
            if(selectedVariables.HasOneSelectedPrototype())
            {
                if (ToolEditorUtility.GetPrototypeComponents(selectedVariables.SelectedPrototype, toolType).Count == 0)
                {
                    return;
                }
            }
            
            if(selectedVariables.HasOneSelectedPrototype() || _copiedBaseSettingsList.Count != 0)
                menu.AddSeparator("");

            if(selectedVariables.HasOneSelectedPrototype())
            {
                menu.AddItem(new GUIContent("Copy All Settings"), false, GUIUtility.ContextMenuCallback, new Action(() => CopyPrototypeSettings(selectedVariables.GetOneSelectedPrototype().GetType(),
                    new List<Prototype>() { selectedVariables.GetOneSelectedPrototype() }, toolType)));
            }
            
            if(_copiedBaseSettingsList.Count != 0)
            {
                menu.AddItem(new GUIContent("Paste All Settings"), false, GUIUtility.ContextMenuCallback, new Action(() => 
                    ClipboardPrototypeAction(selectedVariables.SelectedGroup.PrototypeType, new List<Prototype>(selectedVariables.SelectedPrototypeList), toolType, true)));

                foreach (BaseComponent baseSettings in _copiedBaseSettingsList)
                {
                    string name = baseSettings.GetType().GetAttribute<NameAttribute>().Name;
                    menu.AddItem(new GUIContent("Paste Settings/" + name), false, GUIUtility.ContextMenuCallback, new Action(() => PasteSettings(selectedVariables.SelectedPrototypeList, baseSettings.GetType(), toolType)));	
                }
            }
        }

        public void CopyPrototypeSettings(Type prototypeType, List<Prototype> prototypes, Type toolType)
        {
            _copiedBaseSettingsList.Clear();

            ClipboardPrototypeAction(prototypeType, prototypes, toolType, false);
        }

        public void ClipboardPrototypeAction(Type prototypeType, List<Prototype> prototypes, Type toolType, bool paste)
        {
            List<Type> clipboardTypes = new List<Type>();

            foreach (var attribute in MegaWorldSettingsUtility.GetAddComponentsAttributes<AddGeneralPrototypeComponentsAttribute>(prototypeType, toolType))
            {
                clipboardTypes.AddRange(attribute.Types);
            }

            foreach (var attribute in MegaWorldSettingsUtility.GetAddComponentsAttributes<AddPrototypeComponentsAttribute>(prototypeType, toolType))
            {
                clipboardTypes.AddRange(attribute.Types);
            }

            foreach(Prototype proto in prototypes) 
            {
                foreach (var type in clipboardTypes)
                {
                    ClipboardAction(proto, type, toolType, paste);
                }
            }
        }

        public void PasteSettings(List<Prototype> protoList, Type settingsType, Type toolType)
        {
            foreach (Prototype proto in protoList)
            {
                ClipboardAction(proto, settingsType, toolType, true);
            }
        }

        public void ClipboardAction(Prototype proto, Type settingsType, Type toolType, bool paste)
        {
            if(paste)
            {
                var copiedSettings = _copiedBaseSettingsList.Find(obj => obj.GetType() == settingsType);
                
                if(copiedSettings == null)
                    return;
                
                BaseComponent baseComponent = DeepCopier.Copy(copiedSettings);

                SetComponent(proto, baseComponent, toolType);
            }
            else
            {
                _copiedBaseSettingsList.Add(DeepCopier.Copy(proto.GetSettings(settingsType)));
            }
        }
        
        public void SetComponent(Prototype proto, BaseComponent settings, Type toolType)
        {
            for (int i = 0; i < proto.ComponentStack.ComponentList.Count; i++)
            {
                BaseComponent baseComponent = proto.ComponentStack.ComponentList[i];

                if (baseComponent.GetType().ToString() == settings.GetType().ToString())
                {
                    foreach (var usedTool in baseComponent.UsedTools)
                    {
                        if (usedTool.ToString() == toolType.ToString())
                        {
                            proto.ComponentStack.ComponentList[i] = settings;
                            proto.ComponentStack.IsDirty = true;
                            return;
                        }
                    }
                }
            }
        }
    }
}
#endif