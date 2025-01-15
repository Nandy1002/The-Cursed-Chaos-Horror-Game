using UnityEngine;

namespace VladislavTsurikov.MegaWorld.Core.Scripts
{
    public class PastTransform
    {
        private Vector3 _position;
        private Vector3 _scale = Vector3.one;
        private Quaternion _rotation;

        public Vector3 Position => _position;
        public Vector3 Scale => _scale;
        public Quaternion Rotation => _rotation;

        public PastTransform(Transform transform)
        {
            _position = transform.position;
            _scale = transform.lossyScale;
            _rotation = transform.rotation;
        }
    }
}
    