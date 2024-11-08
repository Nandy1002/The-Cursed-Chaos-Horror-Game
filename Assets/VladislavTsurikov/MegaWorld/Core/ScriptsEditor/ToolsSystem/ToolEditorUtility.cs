#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.TemplatesSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem.GlobalToolSettings;
using VladislavTsurikov.OdinSerializer.Utilities.Extensions;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ToolsSystem
{
    public static class ToolEditorUtility
    {
	    public static List<Type> GetGroupComponents(Group group, Type targetType)
        {
	        List<Type> drawTypes = new List<Type>();

	        foreach (var attribute in MegaWorldSettingsUtility.GetAddComponentsAttributes<AddGeneralGroupComponentsAttribute>(group.PrototypeType, targetType))
	        {
		        drawTypes.AddRange(attribute.Types);
	        }

	        foreach (var attribute in MegaWorldSettingsUtility.GetAddComponentsAttributes<AddGroupComponentsAttribute>(group.PrototypeType, targetType))
	        {
		        drawTypes.AddRange(attribute.Types);
	        }

	        return drawTypes;
        }
        
        public static List<Type> GetPrototypeComponents(Prototype proto, Type targetType)
        {
	        List<Type> drawTypes = new List<Type>();
	        
	        foreach (var attribute in MegaWorldSettingsUtility.GetAddComponentsAttributes<AddGeneralPrototypeComponentsAttribute>(proto.GetType(), targetType))
	        {
		        drawTypes.AddRange(attribute.Types);
	        }

	        foreach (var attribute in MegaWorldSettingsUtility.GetAddComponentsAttributes<AddPrototypeComponentsAttribute>(proto.GetType(), targetType))
	        {
		        drawTypes.AddRange(attribute.Types);
	        }

	        return drawTypes;
        }

        public static void DrawResourceController(BasicData basicData, bool drawFoldout = true)
        {
			if(basicData.SelectedVariables.HasOneSelectedGroup())
			{
				WindowDataPackage.Instance.ResourcesControllerEditor.OnGUI(basicData.SelectedVariables.SelectedGroup, drawFoldout);
			}
		}

		public static bool DrawWarningAboutUnsupportedResourceType(BasicData basicData, Type targetType)
        {
            if(basicData.SelectedVariables.HasOneSelectedGroup())
			{
				if(!ToolUtility.IsToolSupportSelectedResourcesType(targetType, basicData))
				{
					List<Type> supportedResourceTypes = ToolUtility.GetSupportedPrototypeTypes(targetType);

					string text = "";

					for (int i = 0; i < supportedResourceTypes.Count; i++)
					{
						string name = supportedResourceTypes[i].Name.Split('/').Last();
						
						if(i == supportedResourceTypes.Count - 1)
						{
							text += name;
						}
						else
						{
							text += name + ", ";
						}
					}

					CustomEditorGUILayout.HelpBox("This tool only works with these Resource Types: " + text); 
					
					return false;
				}

				return true;
			}
			else
			{
				if(!ToolUtility.IsToolSupportMultipleTypes(targetType, basicData))
                {
                    CustomEditorGUILayout.HelpBox("This tool does not support multiple selected types."); 
                    return false;
                }
			}

            return true;
        }

		public static void ResetGlobalToolSettingsStackMenu(Type targetType)
		{
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("Reset"), false, () => ResetGlobalToolSettingsStack(targetType));

			menu.ShowAsContext();
		}
		
		public static void ResetGroupSettingsMenu(Group group, Type targetType)
		{
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("Reset"), false, () => ResetGroupSettingsStack(group, targetType));

			menu.ShowAsContext();
		}
		
		public static void ResetPrototypeSettingsMenu(Prototype proto, Type targetType)
		{
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("Reset"), false, () => ResetPrototypeSettingsStack(proto, targetType));

			menu.ShowAsContext();
		}

		public static void ResetGlobalToolSettingsStack(Type targetType)
		{
			foreach (var item in WindowDataPackage.Instance.GlobalToolComponentStackEditor.Editors)
			{
				GlobalToolComponent globalToolComponent = (GlobalToolComponent)item.Target;
                
				if(globalToolComponent == null)
					continue;
                
				if (globalToolComponent.ToolType == targetType)
				{
					for (int i = 0; i < item.ComponentStackEditor.Editors.Count; i++)
					{
						item.ComponentStackEditor.Reset((BaseComponent)item.ComponentStackEditor.Editors[i].Target, i);
					}
				}
			}
		}

		public static void ResetGroupSettingsStack(Group group, Type targetType)
		{
			foreach (var type in GetGroupComponents(group, targetType))
			{
				group.ComponentStackEditor.Reset(type, targetType);
			}
		}
		
		public static void ResetPrototypeSettingsStack(Prototype proto, Type targetType)
		{
			foreach (var type in GetPrototypeComponents(proto, targetType))
			{
				proto.ComponentStackEditor.Reset(type, targetType);
			}
		}
		
		public static bool HasTemplate(Type toolType, Type prototypeType)
		{
			for(int i = 0; i < AllTemplateTypes.TypeList.Count; i++)
			{
				TemplateAttribute templateAttribute = AllTemplateTypes.TypeList[i].GetAttribute<TemplateAttribute>();

				if(templateAttribute.ToolTypes.Contains(toolType) && templateAttribute.SupportedResourceTypes.Contains(prototypeType))
				{
					return true;
				}
			}

			return false;
		}
    }
}
#endif