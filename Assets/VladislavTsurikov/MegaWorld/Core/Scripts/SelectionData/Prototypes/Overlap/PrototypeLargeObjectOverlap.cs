#if INSTANT_RENDERER
using System;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.InstantRenderer.LargeObjectRenderer.Scripts.API;
using VladislavTsurikov.InstantRenderer.LargeObjectRenderer.Scripts.RendererData;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes.Overlap
{
    public class PrototypeLargeObjectOverlap
    {
        public static void OverlapSphere(Vector3 sphereCenter, float sphereRadius, ObjectFilter objectFilter, bool quadTree, bool checkOBBIntersection, Func<PrototypeLargeObject, LargeObjectInstance, bool> func)
        {
            LargeObjectRendererAPI.OverlapSphere(sphereCenter, sphereRadius, objectFilter, quadTree, checkOBBIntersection, go =>
            {
                PrototypeLargeObject proto = GetPrototypeUtility.GetPrototype<PrototypeLargeObject>(go.PrototypeID);

                func.Invoke(proto, go);
                return true;
            });
        }

        public static void OverlapBox(Bounds bounds, ObjectFilter objectFilter, bool quadTree, bool checkOBBIntersection,
            Func<PrototypeLargeObject, LargeObjectInstance, bool> func)
        {
            OverlapBox(bounds.center, bounds.size, Quaternion.identity, objectFilter, quadTree, checkOBBIntersection, func);
        }


        public static void OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, ObjectFilter objectFilter, bool quadTree, bool checkOBBIntersection, Func<PrototypeLargeObject, LargeObjectInstance, bool> func)
        {
            LargeObjectRendererAPI.OverlapBox(boxCenter, boxSize, boxRotation, objectFilter, quadTree, checkOBBIntersection, go =>
            {
                PrototypeLargeObject proto = GetPrototypeUtility.GetPrototype<PrototypeLargeObject>(go.PrototypeID);

                func.Invoke(proto, go);
                return true;
            });
        }
    }
}
#endif