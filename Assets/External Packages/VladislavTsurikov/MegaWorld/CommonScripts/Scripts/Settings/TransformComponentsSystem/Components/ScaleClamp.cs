using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem.Components
{
    [Serializable]
    [Name("Scale Clamp")]
    public class ScaleClamp : TransformComponent
    {
        public float MaxScale = 2;
        public float MinScale = 0.5f;

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            if(instanceData.Scale.x > MaxScale)
            {
                instanceData.Scale.x = MaxScale;
            }
            else if(instanceData.Scale.x < MinScale)
            {
                instanceData.Scale.x = MinScale;
            }

            if(instanceData.Scale.y > MaxScale)
            {
                instanceData.Scale.y = MaxScale;
            }
            else if(instanceData.Scale.y < MinScale)
            {
                instanceData.Scale.y = MinScale;
            }

            if(instanceData.Scale.z > MaxScale)
            {
                instanceData.Scale.z = MaxScale;
            }
            else if(instanceData.Scale.z < MinScale)
            {
                instanceData.Scale.z = MinScale;
            }
        }
    }
}