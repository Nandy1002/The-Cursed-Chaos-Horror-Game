using System;

namespace VladislavTsurikov.ComponentStack.ScriptsEditor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class SettingsEditorAttribute : Attribute
    {
        public readonly Type SettingsType;

        public SettingsEditorAttribute(Type settingsType)
        {
            SettingsType = settingsType;
        }
    }
}