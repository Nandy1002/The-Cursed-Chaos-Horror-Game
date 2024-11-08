using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;

namespace VladislavTsurikov.MegaWorld.BrushEraseTool.ScriptsEditor.PrototypeSettings
{
    [Serializable]
    [Name("Additional Erase Settings")]
    public class AdditionalEraseComponent : BaseComponent
    {
        [Range (0, 100)]
        public float SuccessForErase = 100f;
    }
}