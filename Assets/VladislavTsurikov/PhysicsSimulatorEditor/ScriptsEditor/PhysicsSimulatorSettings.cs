#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.PhysicsSimulatorEditor.ScriptsEditor
{
    public class PhysicsSimulatorSettings : ScriptableObject
    {
        public PositionOffsetSettings PositionOffsetSettings = new PositionOffsetSettings();
        
        public bool SimulatePhysics = true;
        public float GlobalTime = 20f;
        public float ObjectTime = 6f;
        public int AccelerationPhysics = 4;

        public void Save() 
        {
            EditorUtility.SetDirty(this);
        }
    }
}
#endif