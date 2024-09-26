using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.OverlapCheckSettings.OverlapChecks;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes.Overlap;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.OverlapCheckSettings
{
    public enum OverlapShape 
    { 
        None, 
        Bounds,
        Sphere,
    }
    
    [Serializable]
    [Name("Overlap Check Settings")]
    public class OverlapCheckComponent : BaseComponent
    {
        public OverlapShape OverlapShape = OverlapShape.Sphere;
        public BoundsCheck BoundsCheck = new BoundsCheck();
        public SphereCheck SphereCheck = new SphereCheck();
        public CollisionCheck CollisionCheck = new CollisionCheck();

        public static bool RunOverlapCheck(Type prototypeType, OverlapCheckComponent overlapCheckComponent, Vector3 extents, InstanceData instanceData)
        {
            if(RunCollisionCheck(overlapCheckComponent, extents, instanceData))
            {
                return false;
            }

            if(overlapCheckComponent.OverlapShape == OverlapShape.None)
            {
                return true;
            }

            if(prototypeType == typeof(PrototypeGameObject))
            {
#if UNITY_EDITOR
                if(!RunOverlapCheckForGameObject(overlapCheckComponent, overlapCheckComponent.GetCheckBounds(instanceData, extents)))
                {
                    return true;
                }
#endif
            }
            else 
            {
                if(!RunOverlapCheckForInstantItem(overlapCheckComponent, overlapCheckComponent.GetCheckBounds(instanceData, extents)))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool RunOverlapCheckForInstantItem(OverlapCheckComponent overlapCheckComponent, Bounds checkBounds)
        {
#if INSTANT_RENDERER
            bool overlaps = false;

            PrototypeLargeObjectOverlap.OverlapBox(checkBounds.center, checkBounds.size, Quaternion.identity, null, true, false, (proto, persistentInstance) =>
            {
                OverlapCheckComponent localOverlapCheckComponent = (OverlapCheckComponent)proto.ComponentStack.GetComponent(typeof(OverlapCheckComponent));

                if(localOverlapCheckComponent.OverlapShape == OverlapShape.None)
                {
                    return true;
                }

                if(OverlapCheck(localOverlapCheckComponent, persistentInstance.Position, persistentInstance.Scale, proto.Extents, checkBounds, overlapCheckComponent))
                {
                    overlaps = true;
                    return false;
                }

                return true;
            });

            return overlaps;
#else
            return false;
#endif
        }

#if UNITY_EDITOR
        private static bool RunOverlapCheckForGameObject(OverlapCheckComponent overlapCheckComponent, Bounds checkBounds)
        {
            bool overlaps = false;

            PrototypeGameObjectOverlap.OverlapBox(checkBounds, (proto, go) =>
            {
                OverlapCheckComponent localOverlapCheckComponent = (OverlapCheckComponent)proto.ComponentStack.GetComponent(typeof(OverlapCheckComponent));

                if(localOverlapCheckComponent.OverlapShape == OverlapShape.None)
                {
                    return true;
                }

                if(OverlapCheck(localOverlapCheckComponent, go.transform.position, go.transform.localScale, proto.Extents, checkBounds, overlapCheckComponent))
                {
                    overlaps = true;
                    return false;
                }

                return true;
            });

            return overlaps;
        }
#endif

        private static bool OverlapCheck(OverlapCheckComponent localOverlapCheckComponent, Vector3 localPosition, Vector3 localScale, Vector3 extents, Bounds checkBounds, OverlapCheckComponent checkOverlapCheckComponent)
        {
            switch (localOverlapCheckComponent.OverlapShape)
            {
                case OverlapShape.Bounds:
                {
                    Bounds localBounds = BoundsCheck.GetBounds(localOverlapCheckComponent.BoundsCheck, localPosition, localScale, extents);

                    return localBounds.Intersects(checkBounds);
                }
                case OverlapShape.Sphere:
                {
                    return checkOverlapCheckComponent.SphereCheck.OverlapCheck(localOverlapCheckComponent.SphereCheck, localPosition, localScale, checkBounds);
                }
            }

            return false;
        }

        public static bool RunCollisionCheck(OverlapCheckComponent overlapCheckComponent, Vector3 prefabExtents, InstanceData instanceData)
        {
            if(overlapCheckComponent.CollisionCheck.collisionCheckType)
            {
                Vector3 extents = Vector3.Scale(prefabExtents * overlapCheckComponent.CollisionCheck.multiplyBoundsSize, instanceData.Scale);

                if(overlapCheckComponent.CollisionCheck.IsBoundHittingWithCollisionsLayers(instanceData.Position, instanceData.Rotation.eulerAngles.y, extents))
                {
                    return true;
                }
            }

            return false;
        }

        public Bounds GetCheckBounds(InstanceData instanceData, Vector3 extents)
        {
            if(OverlapShape == OverlapShape.Bounds)
            {
                return BoundsCheck.GetBounds(BoundsCheck, instanceData.Position, instanceData.Scale, extents);
            }
            else if(OverlapShape == OverlapShape.Sphere)
            {
                return SphereCheck.GetBounds(SphereCheck, instanceData.Position, instanceData.Scale);
            }

            return new Bounds(Vector3.zero, Vector3.zero);
        }

#if UNITY_EDITOR
        public static void VisualizeOverlapForInstantItem(Bounds bounds, bool showSelectedProto = false)
        {
#if INSTANT_RENDERER
            PrototypeLargeObjectOverlap.OverlapBox(bounds, null, false, true, (proto, persistentInstance) =>
            {
                OverlapCheckComponent overlapCheckComponent = (OverlapCheckComponent)proto.ComponentStack.GetComponent(typeof(OverlapCheckComponent));

                DrawOverlapСheckType(persistentInstance.Position, persistentInstance.Scale, proto.Extents, overlapCheckComponent);

                return true;
            });
#endif
        }

        public static void VisualizeOverlapForGameObject(Bounds bounds, bool showSelectedProto = false)
        {
            PrototypeGameObjectOverlap.OverlapBox(bounds, (proto, go) =>
            {
                OverlapCheckComponent overlapCheckComponent = (OverlapCheckComponent)proto.ComponentStack.GetComponent(typeof(OverlapCheckComponent));

                DrawOverlapСheckType(go.transform.position, go.transform.localScale, proto.Extents, overlapCheckComponent);

                return true;
            });
        }
        public static void DrawOverlapСheckType(Vector3 position, Vector3 scale, Vector3 extents, OverlapCheckComponent overlapCheckComponent)
        {
            switch (overlapCheckComponent.OverlapShape)
            {
                case OverlapShape.Sphere:
                {
                    SphereCheck.DrawOverlapСheck(position, scale, overlapCheckComponent.SphereCheck);

                    break;
                }
                case OverlapShape.Bounds:
                {
                    BoundsCheck.DrawIntersectionСheckType(position, scale, extents, overlapCheckComponent.BoundsCheck);
                    
                    break;
                }
            }
        }
#endif
    }
}