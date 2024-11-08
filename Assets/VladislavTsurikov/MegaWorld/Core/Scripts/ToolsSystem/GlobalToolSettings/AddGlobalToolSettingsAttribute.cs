using System;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem.GlobalToolSettings
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AddGlobalToolSettingsAttribute : AddComponentsAttribute
    {
        public AddGlobalToolSettingsAttribute(Type[] types) : base(types)
        {
        }
    }
}