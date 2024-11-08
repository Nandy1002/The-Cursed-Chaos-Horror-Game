using System;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class WindowToolAttribute : Attribute
    {
        public readonly bool DisableToolIfUnityToolActive;

        internal WindowToolAttribute(bool disableToolIfUnityToolActive)
        {
            DisableToolIfUnityToolActive = disableToolIfUnityToolActive;
        }
    }
}