using System;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AddGeneralGroupComponentsAttribute : AddComponentsAttribute
    {
        public AddGeneralGroupComponentsAttribute(Type[] prototypeTypes, Type[] types) : base(prototypeTypes, types)
        {
        }

        public AddGeneralGroupComponentsAttribute(Type prototypeType, Type[] types) : base(prototypeType, types)
        {
        }
    }
}