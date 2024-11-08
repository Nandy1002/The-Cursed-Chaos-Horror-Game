using System;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem.Components;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem
{
    [Serializable]
    [Name("Transform Component Settings")]
    public class TransformStackComponent : BaseComponent
    {
        public TransformComponentStack Stack = new TransformComponentStack();

        public override void OnCreate()
		{
			Stack.CreateIfMissing(typeof(PositionOffset));
			Stack.CreateIfMissing(typeof(Rotation));
			Stack.CreateIfMissing(typeof(Scale));
		}
    }
}