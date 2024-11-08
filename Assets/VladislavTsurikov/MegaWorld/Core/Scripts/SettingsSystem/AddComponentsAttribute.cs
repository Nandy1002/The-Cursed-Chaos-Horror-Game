using System;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AddComponentsAttribute : Attribute
    {
        public readonly Type[] PrototypeTypes;
        public readonly Type[] Types;
        
        public AddComponentsAttribute(Type[] prototypeTypes, Type[] types)
        {
            PrototypeTypes = prototypeTypes;
            Types = types;
        }
        
        public AddComponentsAttribute(Type prototypeType, Type[] types)
        {
            PrototypeTypes = new []{prototypeType};
            Types = types;
        }
        
        public AddComponentsAttribute(Type[] types)
        {
            Types = types;
        }
    }
}