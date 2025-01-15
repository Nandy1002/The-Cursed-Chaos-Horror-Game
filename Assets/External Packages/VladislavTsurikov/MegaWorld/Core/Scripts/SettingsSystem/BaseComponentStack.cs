using System;
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Scripts;
using VladislavTsurikov.ComponentStack.Scripts;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem
{
    public class BaseComponentStack : ComponentStack<BaseComponent>
    {
        public void CreateIfMissingToolComponent(Type[] types, Type toolType, bool generalSettings)
        {
            foreach (Type type in types)
            {
                if (generalSettings)
                {
                    BaseComponent baseComponent = GetGeneralComponent(type);
                
                    if (baseComponent == null)
                    {
                        BaseComponent newBaseComponent = Create(type);
                        newBaseComponent.AddUsedTools(toolType);
                        newBaseComponent.General = true;
                    }
                    else
                    {
                        baseComponent.AddUsedTools(toolType);
                    }
                }
                else
                {
                    BaseComponent baseComponent = GetComponent(type, toolType);
                
                    if (baseComponent == null)
                    {
                        BaseComponent newBaseComponent = Create(type);
                        newBaseComponent.AddUsedTools(toolType);
                    }
                }
            }
        }
        
        public BaseComponent GetGeneralComponent(Type type)
        {
            foreach (var component in ComponentList)
            {
                if(component == null)
                    continue;
                
                if(!component.General)
                    continue;
                
                if(component.GetType() == type)
                {
                    return component;
                }
            }

            return null;
        }

        public BaseComponent GetComponent(Type type, Type toolType)
        {
            foreach (var component in ComponentList)
            {
                if(component == null)
                    continue;
                
                if(component.GetType() == type && component.UsedTools.Contains(toolType))
                {
                    return component;
                }
            }

            return null;
        }

        public override void RemoveUnused()
        {
            List<BaseComponent> removeComponents = new List<BaseComponent>();

            foreach (BaseComponent baseSettings in ComponentList)
            {
                if (baseSettings.UsedTools == null || !HasType(baseSettings))
                {
                    removeComponents.Add(baseSettings);
                }
            }

            ComponentList.RemoveAll(component => removeComponents.Contains(component)); 
        }
        
        private bool HasType(BaseComponent baseComponent)
        {
            baseComponent.UsedTools.RemoveAll(tool => tool == null);
            
            foreach (Type toolType in baseComponent.UsedTools)
            {
                IEnumerable<Attribute> addComponentsAttributes = toolType.GetAttributes(typeof(AddComponentsAttribute)); 

                foreach (var attribute in addComponentsAttributes)
                {
                    AddComponentsAttribute addComponentsAttribute = (AddComponentsAttribute)attribute;
                    if (addComponentsAttribute.Types.Contains(baseComponent.GetType())) 
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}