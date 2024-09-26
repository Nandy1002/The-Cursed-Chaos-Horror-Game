using UnityEngine;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other
{
    public class SpawnVisualizerPixel
    {
        public Vector3 Position;
        public float Fitness;
        public float Alpha = 1f;
    
        public SpawnVisualizerPixel(Vector3 position, float fitness, float alpha)
        {
            Position = position;
            Fitness = fitness;
            Alpha = alpha;
        }
    }
}
