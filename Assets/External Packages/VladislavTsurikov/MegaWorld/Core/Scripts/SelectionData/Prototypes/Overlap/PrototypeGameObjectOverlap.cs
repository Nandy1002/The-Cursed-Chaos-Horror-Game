#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.ColliderSystem.Scripts.UnityIntegration.Utility;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes.Overlap
{
    public static class PrototypeGameObjectOverlap
    {
        public static void OverlapSphere(Vector3 sphereCenter, float sphereRadius, Func<PrototypeGameObject, GameObject, bool> func)
        {
            ObjectFilter overlapObjectFilter = new ObjectFilter();
            overlapObjectFilter.FindOnlyInstancePrefabs = true;

            List<ColliderObject> overlappedObjects = GameObjectColliderUtility.OverlapSphere(sphereCenter, sphereRadius, overlapObjectFilter, false);

            foreach (ColliderObject colliderObject in overlappedObjects)
            {
                GameObject go = (GameObject)colliderObject.Obj;

                if (go == null)
                {
                    continue;
                }

                PrototypeGameObject proto = GetPrototypeUtility.GetPrototype<PrototypeGameObject>(colliderObject.GetPrefab());

                if(proto == null)
                {
                    continue;
                }

                func.Invoke(proto, go);
            }
        }

        public static void OverlapBox(Bounds bounds, Func<PrototypeGameObject, GameObject, bool> func)
        {
            ObjectFilter overlapObjectFilter = new ObjectFilter();
            overlapObjectFilter.FindOnlyInstancePrefabs = true;

            List<ColliderObject> overlappedObjects = GameObjectColliderUtility.OverlapBox(bounds.center, bounds.size, Quaternion.identity, overlapObjectFilter, false);

            foreach (ColliderObject BVHObject in overlappedObjects)
            {
                GameObject go = (GameObject)BVHObject.Obj;

                if (go == null)
                {
                    continue;
                }

                PrototypeGameObject proto = GetPrototypeUtility.GetPrototype<PrototypeGameObject>(BVHObject.GetPrefab());

                if(proto == null)
                {
                    continue;
                }

                func.Invoke(proto, go);
            }
        }
    }
}
#endif