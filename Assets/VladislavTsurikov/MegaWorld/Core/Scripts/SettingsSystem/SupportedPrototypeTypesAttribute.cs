using System;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SupportedPrototypeTypesAttribute : Attribute
    {
        public Type[] PrototypeTypes;
        
        public SupportedPrototypeTypesAttribute(Type[] prototypeTypes)
        {
            PrototypeTypes = prototypeTypes;
        }
    }
}