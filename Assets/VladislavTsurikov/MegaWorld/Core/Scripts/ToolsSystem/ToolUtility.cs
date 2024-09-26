using System;
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.OdinSerializer.Utilities.Extensions;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem
{
    public static class ToolUtility
    {
        public static List<Type> GetSupportedPrototypeTypes(Type toolType)
        {
            return toolType.GetAttribute<SupportedPrototypeTypesAttribute>().PrototypeTypes.ToList();
        }
        
        public static bool IsToolSupportSelectedResourcesType(Type toolType, BasicData basicData)
        {
            if(basicData.SelectedVariables.HasOneSelectedGroup())
            {
                if (GetSupportedPrototypeTypes(toolType).Contains(basicData.SelectedVariables.SelectedGroup.PrototypeType))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsToolSupportMultipleTypes(Type toolType, BasicData basicData)
        {
            if(basicData.SelectedVariables.SelectedGroupList.Count > 1)
            {
                if(toolType.GetAttribute<SupportMultipleSelectedGroupsAttribute>() != null)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsToolSupportSelectedData(Type toolType, BasicData basicData)
        {
            if(basicData.SelectedVariables.HasOneSelectedGroup())
            {
                if (GetSupportedPrototypeTypes(toolType)
                    .Contains(basicData.SelectedVariables.SelectedGroup.PrototypeType))
                {
                    return true;
                }
            }
            else
            {
                if(toolType.GetAttribute<SupportMultipleSelectedGroupsAttribute>() != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}