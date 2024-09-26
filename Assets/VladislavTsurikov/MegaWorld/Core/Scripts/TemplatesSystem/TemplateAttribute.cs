#if UNITY_EDITOR
using System;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.TemplatesSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TemplateAttribute : Attribute
    {
        public readonly string Name;
        public readonly Type[] ToolTypes;
        public readonly Type[] SupportedResourceTypes;

        internal TemplateAttribute(string name, Type[] toolTypes, Type[] supportedResourceTypes)
        {
            ToolTypes = toolTypes;
            Name = name;
            SupportedResourceTypes = supportedResourceTypes;
        }
    }
}
#endif