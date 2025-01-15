#if UNITY_EDITOR
using UnityEngine;

namespace VladislavTsurikov.PhysicsSimulatorEditor.ScriptsEditor
{
    public static class PhysicsUtility
    {
        public static void ApplyForce(Rigidbody rigidbody, Vector3 force) 
        {
            if (rigidbody != null) 
            {
                rigidbody.AddForce(force, ForceMode.Impulse);
            }
        }
    }
}
#endif