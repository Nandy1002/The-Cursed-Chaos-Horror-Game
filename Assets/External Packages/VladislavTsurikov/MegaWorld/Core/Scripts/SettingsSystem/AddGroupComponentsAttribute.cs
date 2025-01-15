using System;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AddGroupComponentsAttribute : AddComponentsAttribute
    {
        public AddGroupComponentsAttribute(Type[] prototypeTypes, Type[] types) : base(prototypeTypes, types)
        {
        }

        public AddGroupComponentsAttribute(Type prototypeType, Type[] types) : base(prototypeType, types)
        {
        }
    }
}