using System;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem
{
    public static class CreateMegaWorldSettings
	{
        public static void CreateSettings()
        {
            foreach (var type in AllToolTypes.TypeList)
            {
                foreach (Group group in AllAvailableGroups.GetAllGroups())
                {
                    CreateGroupSettings(group, type);

                    foreach (var proto in group.PrototypeList)
                    {
                        CreatePrototypeSettings(proto, type);
                    }
                }
            }
        }

        public static void CreatePrototypeSettings(Prototype proto)
        {
            foreach (var type in AllToolTypes.TypeList)
            {
                CreatePrototypeSettings(proto, type);
            }
        }

        public static void CreateGroupSettings(Group group)
        {
            foreach (var type in AllToolTypes.TypeList)
            {
                CreateGroupSettings(group, type);
            }
        }

        public static void CreatePrototypeSettings(Prototype proto, Type toolType)
        {
            foreach (var attribute in MegaWorldSettingsUtility.GetAddComponentsAttributes<AddGeneralPrototypeComponentsAttribute>(proto.GetType(), toolType))
            {
                proto.ComponentStack.CreateIfMissingToolComponent(attribute.Types, toolType, true);
            }

            foreach (var attribute in MegaWorldSettingsUtility.GetAddComponentsAttributes<AddPrototypeComponentsAttribute>(proto.GetType(), toolType))
            {
                proto.ComponentStack.CreateIfMissingToolComponent(attribute.Types, toolType, false);
            }
        }

        public static void CreateGroupSettings(Group group, Type toolType)
        {
            foreach (var attribute in MegaWorldSettingsUtility.GetAddComponentsAttributes<AddGeneralGroupComponentsAttribute>(group.PrototypeType, toolType))
            {
                group.ComponentStack.CreateIfMissingToolComponent(attribute.Types, toolType, true);
            }
            
            foreach (var attribute in MegaWorldSettingsUtility.GetAddComponentsAttributes<AddGroupComponentsAttribute>(group.PrototypeType, toolType))
            {
                group.ComponentStack.CreateIfMissingToolComponent(attribute.Types, toolType, false);
            }
        }
    }
}