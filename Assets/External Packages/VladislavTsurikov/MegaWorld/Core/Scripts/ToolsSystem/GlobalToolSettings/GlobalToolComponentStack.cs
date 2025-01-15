using System;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.OdinSerializer.Utilities.Extensions;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem.GlobalToolSettings
{
    [Serializable]
    public class GlobalToolComponentStack : ComponentStack<GlobalToolComponent>
    {
        public static BaseComponent GetInstance(Type toolType, Type type)
        {
            foreach (var globalToolSettings in WindowDataPackage.Instance.globalToolComponentStack.ComponentList)
            {
                if (globalToolSettings.ToolType == toolType)
                {
                    foreach (var baseSettings in globalToolSettings.ComponentStack.ComponentList)
                    {
                        if (baseSettings.GetType() == type)
                        {
                            return baseSettings;
                        }
                    }
                }
            }
            
            return null;
        }

        public void CreateGlobalToolSettings()
        {
            foreach (var type in AllToolTypes.TypeList)
            {
                AddGlobalToolSettingsAttribute addGlobalToolSettingsAttribute = type.GetAttribute<AddGlobalToolSettingsAttribute>();
                
                if(addGlobalToolSettingsAttribute == null)
                    continue;  

                GlobalToolComponent globalToolComponent = GetGlobalToolSettings(type);
                
                if (globalToolComponent == null)
                {
                    globalToolComponent = Create(typeof(GlobalToolComponent));
                    globalToolComponent.ToolType = type;
                }

                foreach (var globalSettingsType in addGlobalToolSettingsAttribute.Types)
                {
                    BaseComponent baseComponent = globalToolComponent.ComponentStack.CreateIfMissing(globalSettingsType);
                    baseComponent.AddUsedTools(type);
                }
            }
        } 
        
        private GlobalToolComponent GetGlobalToolSettings(Type toolType)
        {
            foreach (var item in ComponentList)
            {
                if (item.ToolType == toolType) 
                    return item;
            }

            return null;
        }
    }
}