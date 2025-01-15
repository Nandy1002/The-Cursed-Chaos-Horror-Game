using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem.Components
{
    [Serializable]
    [SimpleComponent]  
    [Name("Rotation")]
    public class Rotation : TransformComponent
    {
        public float RandomizeOrientationX = 5;
        public float RandomizeOrientationY = 100;
        public float RandomizeOrientationZ = 5;

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            Vector3 randomVector = UnityEngine.Random.insideUnitSphere * 0.5f;
            Quaternion randomRotation = Quaternion.Euler(new Vector3(
                RandomizeOrientationX * 3.6f * randomVector.x,
                RandomizeOrientationY * 3.6f * randomVector.y,
                RandomizeOrientationZ * 3.6f * randomVector.z));

            instanceData.Rotation = randomRotation;
        }
    }
}
