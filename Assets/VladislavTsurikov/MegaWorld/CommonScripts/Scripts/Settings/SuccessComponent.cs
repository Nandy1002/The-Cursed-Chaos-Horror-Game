using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings
{
    [Serializable]
    [Name("Success")]
    public class SuccessComponent : BaseComponent
    {
        [Range (0, 100)]
        public float SuccessValue = 100f;
    }
}
