using System;

namespace VladislavTsurikov.ComponentStack.ScriptsEditor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DontDrawFoldout : Attribute
    {
        
    }
}