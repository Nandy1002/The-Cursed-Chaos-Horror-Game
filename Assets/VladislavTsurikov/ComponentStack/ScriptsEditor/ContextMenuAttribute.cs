using System;

namespace VladislavTsurikov.ComponentStack.ScriptsEditor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ContextMenuAttribute : Attribute
    {
        public readonly string ContextMenu;

        public ContextMenuAttribute(string contextMenu)
        {
            ContextMenu = contextMenu;
        }
    }
}