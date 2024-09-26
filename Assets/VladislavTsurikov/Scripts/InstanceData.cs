using UnityEngine;

namespace VladislavTsurikov.Scripts
{
    public class InstanceData
    {
        public Vector3 Position;
        public Vector3 Scale; 
        public Quaternion Rotation;

        public InstanceData(GameObject gameObject)
        {
            Position = gameObject.transform.position;
            Scale = gameObject.transform.localScale; 
            Rotation = gameObject.transform.rotation;
        }
        
        public InstanceData(Vector3 position, Vector3 scaleFactor, Quaternion rotation)
        {
            Position = position;
            Scale = scaleFactor; 
            Rotation = rotation;
        }
    }
}

