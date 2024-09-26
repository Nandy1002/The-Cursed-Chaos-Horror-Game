using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem
{
    [Serializable]
    public class TransformComponentStack : ComponentStack<TransformComponent>
    {
        public void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            foreach (TransformComponent item in ComponentList)
            {
                if(item.Active)
                {
                    item.SetInstanceData(ref instanceData, fitness, normal);
                }
            }
        }
    }
}
