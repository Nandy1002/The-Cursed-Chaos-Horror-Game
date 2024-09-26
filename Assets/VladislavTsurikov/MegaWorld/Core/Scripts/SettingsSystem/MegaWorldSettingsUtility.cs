using System;
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Scripts;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem
{
    public static class MegaWorldSettingsUtility
    {
        public static IEnumerable<T> GetAddComponentsAttributes<T>(Type prototypeType, Type toolType)
            where T : AddComponentsAttribute
        {
            foreach (var attribute in toolType.GetAttributes<T>())
            {
                if (attribute.PrototypeTypes.Contains(prototypeType))
                    yield return attribute;
            }
        }
    }
}