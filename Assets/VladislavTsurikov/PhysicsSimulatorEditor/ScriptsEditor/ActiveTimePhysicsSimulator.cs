#if UNITY_EDITOR
using UnityEngine;

namespace VladislavTsurikov.PhysicsSimulatorEditor.ScriptsEditor
{
    public static class ActiveTimePhysicsSimulator
    {
        private static float s_activeTime = 0f;

        public static void SimulatePhysics() 
        {
            s_activeTime += Time.deltaTime;

            if (s_activeTime >= PhysicsSimulatorPath.Settings.GlobalTime)
            {
                s_activeTime = 0f;
                SimulatedBodyStack.DisableAllPhysicsSupport();
            }
            else
            {
                PhysicsSimulator.Simulate();
            }
        }

        public static void RefreshTime()
        {
            s_activeTime = 0f;
        }
    }
}
#endif
