#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.PhysicsSimulatorEditor.ScriptsEditor.Call_Delegate
{
    [InitializeOnLoad]
    public static class CallPositionOffsetDelegate 
    {
        static CallPositionOffsetDelegate()
        {
            SimulatedBodyStack.DisablePhysicsSupportPerformed -= Performed;
            SimulatedBodyStack.DisablePhysicsSupportPerformed += Performed;
        }

        private static void Performed(GameObject go)
        {
            if(go == null) return;

            PhysicsSimulatorPath.Settings.PositionOffsetSettings.ApplyOffset(go);
        }
    }
}
#endif