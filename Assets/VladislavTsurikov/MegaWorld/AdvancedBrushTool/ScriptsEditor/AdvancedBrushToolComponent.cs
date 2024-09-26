using System;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;

namespace VladislavTsurikov.MegaWorld.AdvancedBrushTool.ScriptsEditor
{
    [Serializable]
    [Name("Advanced Brush Tool Settings")]
    public class AdvancedBrushToolComponent : BaseComponent
    {
        public float TextureTargetStrength = 1.0f;

        public bool EnableFailureRateOnMouseDrag = true;
        public float FailureRate = 50f;
    }
}