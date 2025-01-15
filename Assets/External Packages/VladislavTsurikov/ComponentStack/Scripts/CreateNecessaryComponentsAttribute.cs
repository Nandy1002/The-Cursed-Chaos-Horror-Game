using System;
using System.Collections.Generic;

namespace VladislavTsurikov.ComponentStack.Scripts
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CreateNecessaryComponentsAttribute : Attribute
    {
        public readonly List<Type> NecessaryTypes;

        public CreateNecessaryComponentsAttribute(Type[] types)
        {
            NecessaryTypes = new List<Type>(types);
        }
    }
}