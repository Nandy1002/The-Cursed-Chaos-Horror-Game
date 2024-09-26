using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.ColliderSystem.Scripts.Utility;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.PreferencesSettings;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.Utility
{
    public static class RaycastUtility
    {
        public static RayHit Raycast(Ray ray, LayerMask layersMask)
        {
            if(AdvancedSettings.Instance.EditorSettings.raycastSettings.RaycastType == RaycastType.UnityRaycast)
            {
                return UnityRaycast(ray, layersMask);
            }
            else
            {
#if UNITY_EDITOR
                return ColliderUtility.Raycast(ray, layersMask);                
#else
                return UnityRaycast(ray, layersMask);
#endif
            }
        }

        public static RayHit UnityRaycast(Ray ray, LayerMask layersMask)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, AdvancedSettings.Instance.EditorSettings.raycastSettings.MaxRayDistance, layersMask))
            {
                RayHit rayHit = new RayHit(ray, hitInfo.transform.gameObject, hitInfo.normal, hitInfo.point, hitInfo.distance);
                return rayHit;
            }

            return null;
        }

        public static Vector2 GetTextureCoordFromUnityTerrain(Vector3 origin)
        {
            Ray ray = new Ray(origin + new Vector3(0, 10, 0), Vector3.down);

            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, AdvancedSettings.Instance.EditorSettings.raycastSettings.MaxRayDistance, LayerMask.GetMask(LayerMask.LayerToName(Terrain.activeTerrain.gameObject.layer))))
		    {
                return hitInfo.textureCoord;
            }

            return Vector2.zero;
        }
    }
}