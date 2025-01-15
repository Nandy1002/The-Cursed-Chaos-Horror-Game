using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem.Components
{
    [Serializable]
    [SimpleComponent]  
    [Name("Align")]
    public class Align : TransformComponent
    {
        public bool UseNormalWeight = true;
        public bool MinMaxRange = true;
		public float MinWeightToNormal = 0;
		public float MaxWeightToNormal = 0.2f;

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            Quaternion normalRotation = Quaternion.FromToRotation(Vector3.up, normal);

            if(UseNormalWeight == true)
            {
                float normalWeight;

                if(MinMaxRange == true)
                {
                    normalWeight = UnityEngine.Random.Range(MinWeightToNormal, MaxWeightToNormal);
                }
                else
                {
                    normalWeight = MaxWeightToNormal;
                }

                instanceData.Rotation *= Quaternion.Lerp(instanceData.Rotation, normalRotation, normalWeight);
            }
            else
            {
                instanceData.Rotation *= normalRotation;
            }
        }
    }
}

