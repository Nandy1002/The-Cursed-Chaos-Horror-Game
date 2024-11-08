using System;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AddPrototypeComponentsAttribute : AddComponentsAttribute
    {
        public AddPrototypeComponentsAttribute(Type[] prototypeTypes, Type[] types) : base(prototypeTypes, types)
        {
        }

        public AddPrototypeComponentsAttribute(Type prototypeType, Type[] types) : base(prototypeType, types)
        {
        }
    }
}