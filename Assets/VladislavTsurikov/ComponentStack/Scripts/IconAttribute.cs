using System;
using System.Collections.Generic;

namespace VladislavTsurikov.ComponentStack.Scripts
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class IconAttribute : Attribute
    {
        public readonly List<Type> NecessaryTypes = new List<Type>();

        public IconAttribute(Type type)
        {
            NecessaryTypes.Add(type);
        }

        public IconAttribute(Type[] types)
        {
            NecessaryTypes.AddRange(types);
        }
    }
}