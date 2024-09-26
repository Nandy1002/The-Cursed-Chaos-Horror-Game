#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.UnityIntegration;
using VladislavTsurikov.PhysicsSimulatorEditor.ScriptsEditor;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.CallDelegate
{
    [InitializeOnLoad]
    public static class СallPhysicsSimulationDelegate 
    {
        static СallPhysicsSimulationDelegate()
        {
            SimulatedBodyStack.DisablePhysicsSupportPerformed -= Performed;
            SimulatedBodyStack.DisablePhysicsSupportPerformed += Performed;
        }

        private static void Performed(GameObject go)
        {
            if(go == null) return;

            GameObjectCollider.RegisterGameObjectToCurrentScene(go);
        }
    }
}
#endif