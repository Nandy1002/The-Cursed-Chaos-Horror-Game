#if UNITY_EDITOR
using System;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem.Components;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings
{
    [Serializable]
    [Name("Physics Transform Component Settings")]
    public class PhysicsTransformStackComponent : BaseComponent
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
#endif