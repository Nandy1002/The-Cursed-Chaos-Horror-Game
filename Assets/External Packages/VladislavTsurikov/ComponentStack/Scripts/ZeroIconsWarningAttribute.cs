using System;

namespace VladislavTsurikov.ComponentStack.Scripts
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ZeroIconsWarningAttribute : Attribute
    {
        public readonly string Text;

        public ZeroIconsWarningAttribute(string text)
        {
            Text = text;
        }
    }
}