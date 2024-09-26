using System;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ToolsSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DrawBasicDataAttribute : Attribute
    {
        public readonly Type SelectionPrototypeWindowType;
        public readonly Type SelectionGroupWindowType;
        
        public DrawBasicDataAttribute(Type selectionPrototypeWindowType, Type selectionGroupWindowType)
        {
            SelectionPrototypeWindowType = selectionPrototypeWindowType;
            SelectionGroupWindowType = selectionGroupWindowType;
        }

        public DrawBasicDataAttribute(Type selectionPrototypeWindowType)
        {
            SelectionPrototypeWindowType = selectionPrototypeWindowType;
        }
    }
}