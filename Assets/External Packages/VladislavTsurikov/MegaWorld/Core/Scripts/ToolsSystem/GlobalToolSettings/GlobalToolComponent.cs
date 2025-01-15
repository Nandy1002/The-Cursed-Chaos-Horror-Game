using System;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem.GlobalToolSettings
{
    [Name("Global Tool Settings")]
    public class GlobalToolComponent : Component
    {
        public Type ToolType;
        
        [OdinSerialize] public BaseComponentStack ComponentStack = new BaseComponentStack(); 
        
        public override void Init(object[] args = null)
        {
            ComponentStack.InternalSetup();
        }
    }
}