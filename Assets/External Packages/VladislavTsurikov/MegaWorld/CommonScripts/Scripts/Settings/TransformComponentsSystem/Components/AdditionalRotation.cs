using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem.Components
{
    [Serializable]
    [Name("Additional Rotation")]
    public class AdditionalRotation : TransformComponent
    {
        public Vector3 Rotation;

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            instanceData.Rotation *= Quaternion.Euler(Rotation);
        }
    }
}