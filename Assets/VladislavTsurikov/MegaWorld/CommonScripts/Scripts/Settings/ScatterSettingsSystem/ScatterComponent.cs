using System;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.ScatterSettingsSystem
{
    [Serializable]
    [Name("Scatter Settings")]
    public class ScatterComponent : BaseComponent
    {
        public ScatterStack Stack = new ScatterStack();

        public override void OnCreate()
        {
            Stack.CreateIfMissing(typeof(ScatterAlgorithms.RandomGrid));
        }
    }
}