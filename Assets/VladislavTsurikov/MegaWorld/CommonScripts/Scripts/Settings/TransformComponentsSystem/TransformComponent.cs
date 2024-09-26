using UnityEngine;
using VladislavTsurikov.Scripts;
using Component = VladislavTsurikov.ComponentStack.Scripts.Component;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem
{
    public abstract class TransformComponent : Component
    {
        public virtual void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal) {}
    }
}
