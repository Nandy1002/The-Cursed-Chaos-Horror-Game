#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Scripts;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ToolsSystem;
using GUIUtility = VladislavTsurikov.Utility.GUIUtility;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.TemplatesSystem
{
	public class TemplateStackEditor
    {
		public List<Template> TemplateList;

        public TemplateStackEditor()
        {
			TemplateList = new List<Template>();

            for(int i = 0; i < AllTemplateTypes.TypeList.Count; ++i) 
            {
                Type type = AllTemplateTypes.TypeList[i];

                Create(type);
            }
        }

        public void ShowMenu(GenericMenu menu, Type toolType, Group group, SelectedVariables selectedVariables)
        {
	        if (!ToolEditorUtility.HasTemplate(toolType, group.PrototypeType))
	        {
		        return;
	        }
	        
            menu.AddSeparator("");

            for(int i = 0; i < AllTemplateTypes.TypeList.Count; i++)
            {
                TemplateAttribute templateAttribute = AllTemplateTypes.TypeList[i].GetAttribute<TemplateAttribute>();

                string name = templateAttribute.Name;
				Type[] supportedResourceTypes = templateAttribute.SupportedResourceTypes;

				Template template = TemplateList[i];

                if(templateAttribute.ToolTypes.Contains(toolType))
                {
			        menu.AddItem(new GUIContent("Apply Templates/" + name), false, GUIUtility.ContextMenuCallback, new Action(() => 
			        	selectedVariables.SelectedPrototypeList.ForEach ((proto) => { template.Apply(supportedResourceTypes, group, proto);})));
                }
            }
        }

		private void Create(Type type)
        {
            var editor = (Template)Activator.CreateInstance(type);
            TemplateList.Add(editor);
        }
	}
}
#endif