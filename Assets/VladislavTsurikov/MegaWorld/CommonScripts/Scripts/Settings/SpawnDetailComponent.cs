using System;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings
{
    [Serializable]
    [Name("Spawn Detail Settings")]
    public class SpawnDetailComponent : BaseComponent
    {
        public bool UseRandomOpacity = true;
        public int Density = 5;
        public float FailureRate;
    }
}

