#if UNITY_EDITOR
using System.Collections;
using UnityEngine;

namespace VladislavTsurikov.PhysicsSimulatorEditor.ScriptsEditor
{
    public static class ObjectTimePhysicsSimulator
    {
        private static bool _pastSimulatePhysics;

        public static void SimulatePhysics() 
        {
            DisablePhysicsSupportWithDelay();

            PhysicsSimulator.Simulate();
        }

        private static void DisablePhysicsSupportWithDelay() 
        {            
            if(SimulatedBodyStack.SimulatedBodyList.Count == 0)
            {
                return;
            }

            if(PhysicsSimulatorPath.Settings.SimulatePhysics)
            {
                if(!_pastSimulatePhysics)
                {
                    foreach (SimulatedBody simulatedBody in SimulatedBodyStack.SimulatedBodyList)
                    {
                        EditorCoroutines.ScriptsEditor.EditorCoroutines.StartCoroutine(DisablePhysicsSupportWithDelay(PhysicsSimulatorPath.Settings.ObjectTime, simulatedBody), simulatedBody.GameObject);
                    }
                }

                _pastSimulatePhysics = true;
            }
            else
            {
                if(_pastSimulatePhysics)
                {
                    foreach (SimulatedBody simulatedBody in SimulatedBodyStack.SimulatedBodyList)
                    {
                        EditorCoroutines.ScriptsEditor.EditorCoroutines.StopCoroutine(DisablePhysicsSupportWithDelay(PhysicsSimulatorPath.Settings.ObjectTime, simulatedBody), simulatedBody.GameObject);               
                    }        
                }

                _pastSimulatePhysics = false;      
            }
        }

        public static void StopAllCoroutine() 
        {            
            foreach (SimulatedBody simulatedBody in SimulatedBodyStack.SimulatedBodyList)
            {
                EditorCoroutines.ScriptsEditor.EditorCoroutines.StopCoroutine(DisablePhysicsSupportWithDelay(PhysicsSimulatorPath.Settings.ObjectTime, simulatedBody), simulatedBody.GameObject);               
            } 
        }

        public static IEnumerator DisablePhysicsSupportWithDelay(float waitForSeconds, SimulatedBody simulatedBody) 
        {
            yield return new WaitForSeconds(waitForSeconds);

            SimulatedBodyStack.DisablePhysicsSupport(simulatedBody);
        }
    }
}
#endif