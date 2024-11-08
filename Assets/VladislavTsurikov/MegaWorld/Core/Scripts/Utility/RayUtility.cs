using UnityEngine;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.PreferencesSettings;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.Utility
{
    public static class RayUtility 
    {
        public static Ray GetRayDown(Vector3 point)
        {
            return new Ray(new Vector3(point.x, point.y + AdvancedSettings.Instance.EditorSettings.raycastSettings.Offset, point.z), Vector3.down);
        }

        public static Ray GetRayFromCameraPosition(Vector3 point)
        {
            return new Ray(Camera.current.transform.position, (point - Camera.current.transform.position).normalized);
        }
    }
}