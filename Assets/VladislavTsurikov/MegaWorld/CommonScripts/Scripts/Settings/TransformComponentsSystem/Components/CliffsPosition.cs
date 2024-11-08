using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem.Components
{
    [Serializable]
    [Name("Cliffs Position")]  
    public class CliffsPosition : TransformComponent
    {
        public float OffsetPosition = 1;

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            Quaternion rotation = Quaternion.identity;

            Vector3 direction = new Vector3(normal.x, 0, normal.z);

            instanceData.Position += direction + new Vector3(OffsetPosition, 0, OffsetPosition);
        }
    }
}