using System;

namespace VladislavTsurikov.ComponentStack.Scripts
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class NameAttribute : Attribute
    {
        public readonly string Name;

        public NameAttribute(string name)
        {
            Name = name;
        }
    }
}