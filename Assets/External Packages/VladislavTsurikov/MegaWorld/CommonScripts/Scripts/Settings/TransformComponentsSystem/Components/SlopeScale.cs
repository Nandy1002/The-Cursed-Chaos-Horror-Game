using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem.Components
{
    [Serializable]
    [Name("Slope Scale")]  
    public class SlopeScale : TransformComponent
    {
        public bool UniformScaleOffset = true;
        [Range(0.1f, 90f)]
        public float MaxSlope = 30;
        [Min(0.1f)]
        public float MaxUniformScaleOffset = 2;
        public Vector3 MaxScaleOffset = new Vector3(2, 2, 0.5f);

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            float normalAngle = Vector3.Angle(normal, Vector3.up);
            float difference = normalAngle / MaxSlope;

            if(UniformScaleOffset)
            {
                float value = Mathf.Lerp(0, MaxUniformScaleOffset, difference);

                instanceData.Scale += new Vector3(value, value, value);
            }
            else
            {
                float valueX = Mathf.Lerp(0, MaxScaleOffset.x, difference);
                float valueY = Mathf.Lerp(0, MaxScaleOffset.y, difference);
                float valueZ = Mathf.Lerp(0, MaxScaleOffset.z, difference);

                instanceData.Scale += new Vector3(valueX, valueY, valueZ);
            }
        }
    }
}

