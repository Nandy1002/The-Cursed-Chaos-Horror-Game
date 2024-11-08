using System;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;

namespace VladislavTsurikov.MegaWorld.BrushEraseTool.ScriptsEditor
{
    [Serializable]
    [Name("Brush Erase Tool Settings")]
    public class BrushEraseToolComponent : BaseComponent
    {   
        public float EraseStrength = 1.0f;
    }   
}
